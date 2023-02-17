
namespace XamarinShot.Models {
	using AVFoundation;
	using Foundation;
	using SceneKit;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using XamarinShot.Models.Enums;
	using XamarinShot.Models.GameplayState;
	using XamarinShot.Utils;

	public class GameState {
		public int TeamACatapults { get; set; } = 0;

		public int TeamBCatapults { get; set; } = 0;

		public void Add (Catapult catapult)
		{
			switch (catapult.Team) {
			case Team.TeamA:
				this.TeamACatapults += 1;
				break;
			case Team.TeamB:
				this.TeamBCatapults += 1;
				break;
			}
		}
	}

	public interface IGameManagerDelegate {
		void OnReceived (GameManager manager, BoardSetupAction received, Player from);
		void OnJoiningPlayer (GameManager manager, Player player);
		void OnLeavingPlayer (GameManager manager, Player player);
		void OnJoiningHost (GameManager manager, Player host);
		void OnLeavingHost (GameManager manager, Player host);
		void OnDidStartGame (GameManager manager);
		void OnManagerDidWinGame (GameManager manager);
		void OnHasNetworkDelay (GameManager manager, bool hasNetworkDelay);
		void OnGameStateUpdated (GameManager manager, GameState gameState);
	}

	public class GameManager : SCNPhysicsContactDelegate,
							   INetworkSessionDelegate,
							   ICatapultDelegate,
							   IInteractionDelegate,
							   IProjectileDelegate,
							   IPhysicsSyncSceneDataDelegate,
							   IGameObjectPoolDelegate,
							   IGameAudioComponentDelegate {
		// interactions with the scene must be on the main thread
		private readonly SCNScene scene;
		private GameLevel level;
		private SCNNode levelNode;

		// use this to access the simulation scaled camera
		private readonly SCNNode pointOfViewSimulation;

		// these come from ARSCNView currentlys
		private readonly SCNNode pointOfView; // can be in sim or render space

		private GameBoard gameBoard;
		private GameObject tableBoxObject;

		// should be the inverse of the level's world transform
		private SCNMatrix4 renderToSimulationTransform = SCNMatrix4.Identity;

		private readonly IList<GameObject> gameObjects = new List<GameObject> ();      // keep track of all of our entities here
		private GameCamera gameCamera;
		private GameLight gameLight;

		private readonly NetworkSession session;
		private readonly SFXCoordinator sfxCoordinator;
		private readonly MusicCoordinator musicCoordinator;
		private readonly bool useWallClock;

		private readonly NSLock catapultsLock = new NSLock ();

		private readonly List<GameCommand> gameCommands = new List<GameCommand> ();
		private readonly NSLock commandsLock = new NSLock ();

		private readonly List<TouchEvent> touchEvents = new List<TouchEvent> ();
		private readonly NSLock touchEventsLock = new NSLock ();

		private Dictionary<string, List<GameObject>> categories = new Dictionary<string, List<GameObject>> ();  // this object can be used to group like items if their gamedefs include a category

		// Refernces to Metal do not compile for the Simulator
		// TODO:
		//#if !targetEnvironment(simulator)
		//private MetalClothSimulator flagSimulation;
		//#endif

		// Physics
		private readonly PhysicsSyncSceneData physicsSyncData = new PhysicsSyncSceneData ();
		private readonly GameObjectPool gameObjectPool = new GameObjectPool ();
		private readonly InteractionManager interactionManager = new InteractionManager ();
		private readonly GameObjectManager gameObjectManager = new GameObjectManager ();

		public GameManager (SCNView sceneView,
						   GameLevel level,
						   NetworkSession session,
						   AVAudioEnvironmentNode audioEnvironment,
						   MusicCoordinator musicCoordinator) : base ()
		{

			// make our own scene instead of using the incoming one
			this.scene = sceneView.Scene;
			this.PhysicsWorld = this.scene.PhysicsWorld;
			this.PhysicsWorld.Gravity = new SCNVector3 (0f, -10f, 0f);

			//if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR)
			//{
			//this.flagSimulation = new MetalClothSimulator(sceneView.Device);
			//}

			// this is a node, that isn't attached to the ARSCNView
			this.pointOfView = sceneView.PointOfView;
			this.pointOfViewSimulation = this.pointOfView.Clone ();

			this.level = level;

			this.session = session;
			this.musicCoordinator = musicCoordinator;
			this.sfxCoordinator = new SFXCoordinator (audioEnvironment);
			this.useWallClock = UserDefaults.SynchronizeMusicWithWallClock;

			// init entity system
			this.GameDefinitions = GameObject.LoadGameDefs ("art.scnassets/data/entities_def");

			// load the level if it wasn't already pre-loaded
			this.level.Load ();

			// start with a copy of the level, never change the originals, since we use original to reset
			this.levelNode = this.level.ActiveLevel;

			this.IsNetworked = this.session != null;
			this.IsServer = this.session?.IsServer ?? true; // Solo game act like a server

			if (this.session != null) {
				this.session.Delegate = this;
			}

			this.PhysicsWorld.ContactDelegate = this; // get notified of collisions
		}

		~GameManager ()
		{
			this.Unload ();
			this.Dispose (false);
		}

		public IGameManagerDelegate Delegate { get; set; }

		public Player CurrentPlayer { get; private set; } = UserDefaults.Myself;

		public List<Catapult> Catapults { get; private set; } = new List<Catapult> ();

		public SCNPhysicsWorld PhysicsWorld { get; private set; }

		public bool IsNetworked { get; private set; }

		public bool IsServer { get; private set; }

		/// <summary>
		/// Don't execute any code from SCNView renderer until this is true
		/// </summary>
		public bool IsInitialized { get; private set; }

		public SCNMatrix4 RenderToSimulationTransform {
			get {
				return this.renderToSimulationTransform;
			}

			set {
				this.renderToSimulationTransform = value;
				this.sfxCoordinator.RenderToSimulationTransform = this.renderToSimulationTransform;
			}
		}

		public void Send (BoardSetupAction boardAction)
		{
			this.session?.Send (new GAction { BoardSetup = boardAction });
		}

		public void Send (BoardSetupAction boardAction, Player player)
		{
			this.session?.Send (new GAction { BoardSetup = boardAction }, player);
		}

		public void Send (GameActionType gameAction)
		{
			this.session?.Send (new GAction { GameAction = gameAction });
		}

		#region processing touches

		private CameraInfo lastCameraInfo = new CameraInfo (SCNMatrix4.Identity);

		public void HandleTouch (TouchType type)
		{
			if (!UserDefaults.Spectator) {
				this.touchEventsLock.Lock ();
				this.touchEvents.Add (new TouchEvent (type, this.lastCameraInfo.Ray));
				this.touchEventsLock.Unlock ();
			}
		}

		public void UpdateCamera (CameraInfo cameraInfo)
		{
			if (this.gameCamera == null) {
				// need the real render camera in order to set rendering state
				var camera = this.pointOfView;
				camera.Name = "GameCamera";
				this.gameCamera = new GameCamera (camera);
				_ = this.InitGameObject (camera);

				this.gameCamera.UpdateProperties ();
			}

			// transfer props to the current camera
			this.gameCamera.TransferProperties ();

			this.interactionManager.UpdateAll (cameraInfo);
			this.lastCameraInfo = cameraInfo;
		}

		#endregion

		#region inbound from network

		private void Process (GameCommand command)
		{
			if (command.Action.GameAction != null) {
				if (command.Player != null) {
					this.interactionManager.Handle (command.Action.GameAction, command.Player);
				}
			} else if (command.Action.BoardSetup != null) {
				if (command.Player != null) {
					this.Delegate?.OnReceived (this, command.Action.BoardSetup, command.Player);
				}
			} else if (command.Action.Physics != null) {
				this.physicsSyncData.Receive (command.Action.Physics);
			} else if (command.Action.StartGameMusic != null) {
				// Start music at the correct place.
				if (command.Player != null) {
					this.HandleStartGameMusic (command.Action.StartGameMusic, command.Player);
				}
			}
		}

		#endregion

		#region update

		/// <summary>
		/// Called from rendering loop once per frame
		/// </summary>
		public void Update (double timeDelta)
		{
			this.ProcessCommandQueue ();
			this.ProcessTouches ();
			this.SyncPhysics ();

			// TODO:
			//#if !targetEnvironment(simulator)
			//this.flagSimulation.Update(this.levelNode);
			//#endif

			this.gameObjectManager.Update (timeDelta);

			var entities = new List<GameObject> (this.gameObjects);
			entities.ForEach ((entity) => entity.Update (timeDelta));
		}

		private const int MaxCatapults = 6;

		public CoreGraphics.CGRect MainScreenBounds { get; set; }

		/// <summary>
		/// Keep track of which catapults we can see as candidates for grabbing/highlighting
		/// </summary>
		public void UpdateCatapultVisibility (ISCNSceneRenderer renderer, SCNNode camera)
		{
			this.catapultsLock.Lock ();
			if (this.Catapults.Any () && this.Catapults.Count == MaxCatapults) {
				// track which are visible
				foreach (var catapult in this.Catapults) {
					// projectile part should be available, otherwise this is not highlightable
					var visGeo = catapult.Projectile?.FindNodeWithGeometry ();
					if (visGeo == null) {
						catapult.IsVisible = false;
						continue;
					}

					// use bigger geo when already highlighted to avoid highlight from flipping back and fourth
					if (catapult.IsHighlighted && catapult.HighlightObject != null) {
						visGeo = catapult.HighlightObject;
					}

					// this is done in scaled space
					var isVisible = renderer.IsNodeInsideFrustum (visGeo.FindNodeWithGeometry (), camera);
					catapult.IsVisible = isVisible;

					catapult.ProjectedPosition = new SCNVector3 (renderer.ProjectPoint (catapult.Base.WorldPosition));
					catapult.ProjectedPosition = new SCNVector3 (catapult.ProjectedPosition.X / (float) this.MainScreenBounds.Width,
																catapult.ProjectedPosition.Y / (float) this.MainScreenBounds.Height,
																catapult.ProjectedPosition.Z);
				}
			}

			this.catapultsLock.Unlock ();
		}

		private void ProcessCommandQueue ()
		{
			// retrieving the command should happen with the lock held, but executing
			// it should be outside the lock.
			// inner function lets us take advantage of the defer keyword
			// for lock management.

			var command = nextCommand ();
			while (command != null) {
				this.Process (command);
				command = nextCommand ();
			}

			GameCommand nextCommand ()
			{
				GameCommand result = null;
				this.commandsLock.Lock ();
				if (this.gameCommands.Any ()) {
					result = this.gameCommands.First ();
					this.gameCommands.RemoveAt (0);
				}

				this.commandsLock.Unlock ();
				return result;
			}
		}

		private void ProcessTouches ()
		{
			var touch = nextTouch ();
			while (touch != null) {
				this.Process (touch);
				touch = nextTouch ();
			}

			TouchEvent nextTouch ()
			{
				TouchEvent result = null;

				this.touchEventsLock.Lock ();
				if (this.touchEvents.Any ()) {
					result = this.touchEvents.First ();
					this.touchEvents.RemoveAt (0);
				}

				this.touchEventsLock.Unlock ();

				return result;
			}
		}

		private void Process (TouchEvent touch)
		{
			this.interactionManager.HandleTouch (touch.Type, touch.Camera);
		}

		public void QueueAction (GameActionType gameAction)
		{
			this.commandsLock.Lock ();
			this.gameCommands.Add (new GameCommand (this.CurrentPlayer, new GAction { GameAction = gameAction }));
			this.commandsLock.Unlock ();
		}

		private void SyncPhysics ()
		{
			if (this.IsNetworked && this.physicsSyncData.IsInitialized) {
				if (this.IsServer) {
					if (this.session.IsAnyActivePeers) {
						var physicsData = this.physicsSyncData.GenerateData ();
						this.session.Send (new GAction { Physics = physicsData });
					}
				} else {
					this.physicsSyncData.UpdateFromReceivedData ();
				}
			}
		}

		public void PlayWinSound ()
		{
			this.Delegate?.OnManagerDidWinGame (this);
		}

		public void StartGameMusic (IInteraction interaction)
		{
			this.StartGameMusicEverywhere ();
		}

		public void StartGameMusic (Player player)
		{
			// Begin by handling an empty message. Our timestamp will be added and
			// sent in ping/pong to estimate latency.
			this.HandleStartGameMusic (new StartGameMusicTime (false, new List<double> ()), player);
		}

		/// <summary>
		/// Status for SceneViewController to query and display UI interaction
		/// </summary>
		public bool CanGrabACatapult (Ray cameraRay)
		{
			if (this.interactionManager.Interaction (typeof (CatapultInteraction)) is CatapultInteraction catapultInteraction) {
				return catapultInteraction.CanGrabAnyCatapult (cameraRay);
			} else {
				return false;
			}
		}

		private void DisplayWin ()
		{
			if (this.interactionManager.Interaction (typeof (VictoryInteraction)) is VictoryInteraction victory) {
				victory.ActivateVictory ();
			} else {
				throw new Exception ("No Victory Effect");
			}
		}

		public bool IsCurrentPlayerGrabbingACatapult ()
		{
			var grabInteraction = this.interactionManager.Interaction (typeof (GrabInteraction)) as GrabInteraction;
			return grabInteraction?.GrabbedGrabbable is Catapult;
		}

		/// <summary>
		/// Configures the node from the level to be placed on the provided board.
		/// </summary>
		public void AddLevel (SCNNode node, GameBoard gameBoard)
		{
			this.gameBoard = gameBoard;

			this.level.PlaceLevel (node, this.scene, this.gameBoard.Scale.X);

			// Initialize table box object
			this.CreateTableTopOcclusionBox ();

			this.UpdateRenderTransform ();

			if (this.level?.ActiveLevel != null) {
				this.FixLevelsOfDetail (this.level.ActiveLevel);
			}
		}

		public void FixLevelsOfDetail (SCNNode node)
		{
			// set screenSpacePercent to 0 for high-poly lod always,
			// or to much greater than 1 for low-poly lod always
			var screenSpacePercent = 0.15f;
			var screenSpaceRadius = SCNNodeExtensions.ComputeScreenSpaceRadius (screenSpacePercent);

			// The lod system doesn't account for camera being scaled
			// so do it ourselves.  Here we remove the scale.
			screenSpaceRadius /= level.LodScale;

			var showLOD = UserDefaults.ShowLOD;
			node.FixLevelsOfDetail (screenSpaceRadius, showLOD);
		}

		// call this if the level moves from AR changes or user moving/scaling it
		private void UpdateRenderTransform ()
		{
			if (this.gameBoard != null) {
				// Scale level to normalized scale (1 unit wide) for rendering
				var levelNodeTransform = SimdExtensions.CreateFromScale (this.level.NormalizedScale);

				var worldTransform = this.gameBoard.WorldTransform;
				worldTransform.Transpose ();
				this.RenderToSimulationTransform = SCNMatrix4.Invert (levelNodeTransform) * SCNMatrix4.Invert (worldTransform);
			}
		}

		/// <summary>
		/// Initializes all the objects and interactions for the game, and prepares to process user input.
		/// </summary>
		public void Start ()
		{
			// Now we initialize all the game objects and interactions for the game.

			// reset the index that we assign to GameObjects.
			// test to make sure no GameObjects are built prior
			// also be careful that the server increments the counter for new nodes
			GameObject.ResetIndexCounter ();
			this.categories = new Dictionary<string, List<GameObject>> ();

			this.InitializeGameObjectPool ();

			this.InitializeLevel ();
			this.InitBehaviors ();

			// Initialize interactions that add objects to the level
			this.InitializeInteractions ();

			this.physicsSyncData.Delegate = this;

			// Start advertising game
			if (this.session != null && this.session.IsServer) {
				this.session.StartAdvertising ();
			}

			this.Delegate?.OnDidStartGame (this);

			this.StartGameMusicEverywhere ();

			this.IsInitialized = true;
		}

		public void ReleaseLevel ()
		{
			// remove all audio players added to AVAudioEngine.
			this.sfxCoordinator.RemoveAllAudioSamplers ();
			this.level.Reset ();
		}

		private void InitBehaviors ()
		{
			// after everything is setup, add the behaviors if any
			foreach (var gameObject in this.gameObjects) {
				// update constraints
				foreach (var component in gameObject.Components.Where (component => component is IPhysicsBehaviorComponent)) {
					(component as IPhysicsBehaviorComponent).InitBehavior (this.levelNode, this.PhysicsWorld);
				}
			}
		}

		#endregion

		#region Table Occlusion

		/// <summary>
		/// Create an opaque object representing the table used to occlude falling objects
		/// </summary>
		private void CreateTableTopOcclusionBox ()
		{
			var tableBoxNode = this.scene.RootNode.FindChildNode ("OcclusionBox", true);
			if (tableBoxNode == null) {
				throw new Exception ("Table node not found");
			}

			// make a table object so we can attach audio component to it
			this.tableBoxObject = this.InitGameObject (tableBoxNode);
		}

		#endregion

		#region Initialize Game Functions

		private string TeamName (SCNNode node)
		{
			string result = null;
			if (!string.IsNullOrEmpty (node.Name)) {
				// set to A or B, don't set blocks to teamAA, AB, AC
				if (node.Name == "_teamA" || node.Name == "_teamB") {
					result = node.Name;
				}
			}

			return result;
		}

		/// <summary>
		/// Walk all the nodes looking for actual objects.
		/// </summary>
		private void EnumerateHierarchy (SCNNode node, string teamName = null)
		{
			// If the node has no name or a name does not contain
			// a type identifier, we look at its children.
			string identifier = null;
			if (!string.IsNullOrEmpty (node.Name) && !string.IsNullOrEmpty (identifier = node.GetTypeIdentifier ())) {
				this.Configure (node, node.Name, identifier, teamName);
			} else {
				var newTeamName = this.TeamName (node) ?? teamName;
				foreach (var child in node.ChildNodes) {
					this.EnumerateHierarchy (child, newTeamName);
				}
			}
		}

		private void Configure (SCNNode node, string name, string type, string team)
		{
			// For nodes with types, we create at most one gameObject, configured
			// based on the node type.

			switch (type) {
			case "catapult":
				// replaces the placeholder node with a working catapult
				var catapultNode = Catapult.ReplaceCatapultPlaceholder (node);

				// Create Catapult GameObject
				var identifier = this.Catapults.Count;
				var catapult = new Catapult (catapultNode, this.sfxCoordinator, identifier, this.GameDefinitions);
				this.gameObjects.Add (catapult);
				this.SetupAudioComponent (catapult);

				catapultNode.Name = name;

				catapult.Delegate = this;
				this.Catapults.Add (catapult);

				catapult.UpdateProperties ();
				catapult.AddComponent (new RemoveWhenFallenComponent ());
				this.GameState.Add (catapult);

				this.physicsSyncData.AddObject (catapult);
				break;

			case "ShadowPlane":
			case "OcclusionBox":
				// don't add a game object, but don't visit it either
				return;

			case "ShadowLight":
				if (this.gameLight == null) {
					node.Name = "GameLight";
					var light = this.InitGameObject (node);
					this.gameObjects.Add (light);

					this.gameLight = new GameLight (node);
					this.gameLight.UpdateProperties ();
				}

				this.gameLight?.TransferProperties ();
				return;

			default:
				// This handles all other objects, including blocks, reset switches, etc.
				// All special functionality is defined in entities_def.json file

				// can't removing these throw off the object index
				// if not all clients remove these
				switch (type) {
				case "cloud":
					if (!UserDefaults.ShowClouds) {
						node.RemoveFromParentNode ();
						return;
					}
					break;

				case "flag":
					if (!UserDefaults.ShowFlags) {
						node.RemoveFromParentNode ();
						return;
					} else {
						//#if !targetEnvironment(simulator)
						//this.flagSimulation.CreateFlagSimulationFromNode(node);
						//#endif
					}
					break;

				case "resetSwitch":
					if (!UserDefaults.ShowResetLever) {
						node.RemoveFromParentNode ();
						return;
					}
					break;
				}
				break;
			}

			var gameObject = this.InitGameObject (node);

			// hardcoded overrides for physics happens here
			if (!gameObject.UsePredefinedPhysics) {
				// Constrain the angularVelocity until first ball fires.
				// This is done to stabilize the level.
				if (gameObject.PhysicsNode?.PhysicsBody != null) {
					gameObject.PhysicsNode.PhysicsBody.AngularVelocityFactor = SCNVector3.Zero;
				}

				if (gameObject.PhysicsNode?.PhysicsBody != null) {
					gameObject.PhysicsNode.PhysicsBody.AngularDamping = 0.03f;
					gameObject.PhysicsNode.PhysicsBody.Damping = 0.03f;
					gameObject.PhysicsNode.PhysicsBody.Mass = 3;
					gameObject.PhysicsNode.PhysicsBody.LinearRestingThreshold = 1f;
					gameObject.PhysicsNode.PhysicsBody.AngularRestingThreshold = 1f;

					var collisionBitMask = (CollisionMask) (int) gameObject.PhysicsNode.PhysicsBody.CollisionBitMask;
					gameObject.PhysicsNode.PhysicsBody.CollisionBitMask = (nuint) (int) (collisionBitMask | CollisionMask.Ball);

					if (gameObject.Density > 0) {
						gameObject.PhysicsNode.CalculateMassFromDensity (name, gameObject.Density);
					}

					gameObject.PhysicsNode.PhysicsBody.ResetTransform ();
					if (gameObject.PhysicsNode.PhysicsBody.AllowsResting) {
						gameObject.PhysicsNode.PhysicsBody.SetResting (true);
					}
				}

				// add to network synchronization code
				if (gameObject.PhysicsNode != null) {
					this.physicsSyncData.AddObject (gameObject);

					if (gameObject.IsBlockObject) {
						this.gameObjectManager.AddBlockObject (gameObject);
					}

					gameObject.AddComponent (new RemoveWhenFallenComponent ());
				}

				if (gameObject.Categorize) {
					if (!this.categories.ContainsKey (gameObject.Category)) {
						this.categories [gameObject.Category] = new List<GameObject> ();
					}

					this.categories [gameObject.Category].Add (gameObject);
				}
			}
		}

		/// <summary>
		/// Set the world at rest
		/// </summary>
		public void RestWorld ()
		{
			var objects = new List<GameObject> (this.gameObjects);
			foreach (var gameObject in objects) {
				if (gameObject.PhysicsNode?.PhysicsBody != null &&
				   gameObject != this.tableBoxObject &&
				   gameObject.PhysicsNode.PhysicsBody.AllowsResting) {
					gameObject.PhysicsNode.PhysicsBody.SetResting (true);
				}
			}
		}

		private void PostUpdateHierarchy (SCNNode node)
		{
			if (node.ValueForKey (new NSString ("nameRestore")) is NSString nameRestore) {
				node.Name = nameRestore.ToString ();
			}

			foreach (var child in node.ChildNodes) {
				this.PostUpdateHierarchy (child);
			}
		}

		private void InitializeGameObjectPool ()
		{
			this.gameObjectPool.ProjectileDelegate = this;
			this.gameObjectPool.CreatePoolObjects (this);

			// GameObjectPool has a fixed number of items which we need to add to physicsSyncData and gameObjectManager
			foreach (var projectile in this.gameObjectPool.ProjectilePool) {
				this.physicsSyncData.AddProjectile (projectile);
				this.gameObjectManager.AddProjectile (projectile);
				this.SetupAudioComponent (projectile);
			}
		}

		private void SetupAudioComponent (GameObject @object)
		{
			if (@object.GetComponent (typeof (GameAudioComponent)) is GameAudioComponent audioComponent) {
				this.sfxCoordinator.SetupGameAudioComponent (audioComponent);
				audioComponent.Delegate = this;
			}
		}

		private void InitializeLevel ()
		{
			// enumerateHierarchy is recursive and may find catapults at any level
			// putting the lock outside ensures that the win condition won't be evaluated
			// on an incomplete set of catapults.
			this.catapultsLock.Lock ();

			this.EnumerateHierarchy (this.levelNode);

			// do post init functions here
			this.PostUpdateHierarchy (this.levelNode);

			this.catapultsLock.Unlock ();
		}

		private void InitializeInteractions ()
		{
			// Grab Interaction
			var grabInteraction = new GrabInteraction (this);
			this.interactionManager.AddInteraction (grabInteraction);

			// Highlight Interaction
			var highlightInteraction = new HighlightInteraction (this);
			highlightInteraction.GrabInteraction = grabInteraction;
			highlightInteraction.SfxCoordinator = this.sfxCoordinator;
			this.interactionManager.AddInteraction (highlightInteraction);

			// Catapult Interaction
			var catapultInteraction = new CatapultInteraction (this);
			catapultInteraction.GrabInteraction = grabInteraction;
			this.interactionManager.AddInteraction (catapultInteraction);

			// Fill Catapult Interaction with catapults
			if (!this.Catapults.Any ()) {
				throw new Exception ("Catapult not initialized");
			}

			foreach (var catapult in this.Catapults) {
				catapultInteraction.AddCatapult (catapult);
			}

			// Catapult Disable Interaction
			this.interactionManager.AddInteraction (new CatapultDisableInteraction (this));

			// Vortex
			var vortex = new VortexInteraction (this);
			vortex.VortexActivationDelegate = catapultInteraction;
			vortex.SfxCoordinator = this.sfxCoordinator;
			vortex.MusicCoordinator = this.musicCoordinator;
			this.interactionManager.AddInteraction (vortex);

			// Lever
			var lever = new LeverInteraction (this);
			var switches = new List<GameObject> ();
			if (this.categories.TryGetValue ("reset", out List<GameObject> processedSwitches)) {
				switches = processedSwitches;
			}

			lever.Setup (switches, vortex);
			lever.SfxCoordinator = this.sfxCoordinator;
			this.interactionManager.AddInteraction (lever);

			// Victory
			this.interactionManager.AddInteraction (new VictoryInteraction (this));
		}

		#endregion

		#region Physics scaling

		public void CopySimulationCamera ()
		{
			// copy the POV camera to minimize the need to lock, this is right after ARKit updates it in
			// the render thread, and before we scale the actual POV camera for rendering
			this.pointOfViewSimulation.WorldTransform = this.pointOfView.WorldTransform;
		}

		public void ScaleCameraToRender ()
		{
			var worldTransform = this.pointOfView.WorldTransform;
			worldTransform.Transpose ();
			var newWorldTransform = this.renderToSimulationTransform * worldTransform;
			newWorldTransform.Transpose ();
			this.pointOfView.WorldTransform = newWorldTransform;
		}

		public void ScaleCameraToSimulation ()
		{
			this.pointOfView.WorldTransform = this.pointOfViewSimulation.WorldTransform;
		}

		public SCNMatrix4 RenderSpaceTransformToSimulationSpace (SCNMatrix4 transform)
		{
			return this.renderToSimulationTransform * transform;
		}

		private GameObject InitGameObject (SCNNode node)
		{
			var gameObject = GameObject.Create<GameObject> (node, this.GameDefinitions);

			this.gameObjects.Add (gameObject);
			this.SetupAudioComponent (gameObject);

			return gameObject;
		}

		/// <summary>
		/// After collision we care about is detected, we check for any collision related components and process them
		/// </summary>
		private void DidCollision (SCNNode nodeA, SCNNode nodeB, SCNVector3 pos, float impulse)
		{
			// let any collision handling components on nodeA respond to the collision with nodeB

			var entity = nodeA.NearestParentGameObject ();
			if (entity != null) {
				foreach (var component in entity.Components.Where (component => component is ICollisionHandlerComponent)) {
					(component as ICollisionHandlerComponent).DidCollision (this, nodeA, nodeB, pos, impulse);
				}
			}

			// let any collision handling components in nodeB respond to the collision with nodeA
			entity = nodeB.NearestParentGameObject ();
			if (entity != null) {
				foreach (var component in entity.Components.Where (component => component is ICollisionHandlerComponent)) {
					(component as ICollisionHandlerComponent).DidCollision (this, nodeA, nodeB, pos, impulse);
				}
			}

			this.interactionManager.DidCollision (nodeA, nodeB, pos, impulse);
		}

		public void DidBeginContact (SCNNode nodeA, SCNNode nodeB, SCNVector3 pos, float impulse)
		{
			this.interactionManager.DidCollision (nodeA, nodeB, pos, impulse);
		}

		public void OnDidApplyConstraints (ISCNSceneRenderer renderer)
		{
			this.gameObjectManager.OnDidApplyConstraints (renderer);
		}

		/// <summary>
		/// Start the game music on the server device and all connected devices
		/// </summary>
		private void StartGameMusicEverywhere ()
		{
			if (this.IsServer) {
				// Start music locally:
				var timeData = this.StartGameMusicNow ();
				this.HandleStartGameMusic (timeData, this.CurrentPlayer);

				// Start the game music on all connected clients:
				this.session?.Send (new GAction { StartGameMusic = timeData });
			}
		}

		private StartGameMusicTime StartGameMusicNow ()
		{
			var cal = new NSCalendar (NSCalendarType.Gregorian);
			var dc = cal.Components (NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day, new NSDate ());
			var reference = cal.DateFromComponents (dc); // chose a reference date of the start of today.
			var now = new NSDate ().SecondsSinceReferenceDate - reference.SecondsSinceReferenceDate; ;
			if (this.useWallClock) {
				return new StartGameMusicTime (true, new List<double> { now });
			} else {
				return new StartGameMusicTime (true, new List<double> { 0d });
			}
		}

		private void HandleStartGameMusic (StartGameMusicTime timeData, Player player)
		{
			if (this.useWallClock) {
				this.HandleStartGameMusicWithWallClock (timeData, player);
			} else {
				this.HandleStartGameMusicWithLatencyEstimate (timeData, player);
			}
		}

		private void HandleStartGameMusicWithWallClock (StartGameMusicTime timeData, Player player)
		{
			if (this.session == null) {
				throw new Exception ("Need a game session");
			}

			// This synchronization method uses the wall clock of the two devices. It
			// relies on them both having a very accurate clock, which really may not be
			// the case.
			//
			// Choose a time reference closer to the present so that milliseconds since
			// this reference can be expressed in UInt32.

			var cal = new NSCalendar (NSCalendarType.Gregorian);
			var dc = cal.Components (NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day, new NSDate ());
			var reference = cal.DateFromComponents (dc); // chose a reference date of the start of today.
			var now = new NSDate ().SecondsSinceReferenceDate - reference.SecondsSinceReferenceDate;

			if (timeData.StartNow) {
				if (timeData.Timestamps.Count == 1) {
					var startWallTime = timeData.Timestamps [0];
					var position = now - startWallTime;
					this.musicCoordinator.PlayMusic ("music_gameplay", position);
				} else {
					throw new Exception ("expected to have serverTimestamps.count == 1");
				}
			} else {
				if (this.IsServer) {
					var position = this.musicCoordinator.CurrentMusicTime ();
					var newData = new StartGameMusicTime (true, new List<double> { now - position });
					this.session.Send (new GAction { StartGameMusic = newData }, player);
				}
			}
		}

		private void HandleStartGameMusicWithLatencyEstimate (StartGameMusicTime timeData, Player player)
		{
			if (this.session == null) {
				throw new Exception ("Need a game session");
			}

			// This synchronization method uses an echoed message (like ping) to calculate
			// the time taken to send a message to the other device and back and make an
			// estimate of latency based on the average of a few of these round trips.

			var cal = new NSCalendar (NSCalendarType.Gregorian);
			var dc = cal.Components (NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day, new NSDate ());
			var reference = cal.DateFromComponents (dc); // chose a reference date of the start of today.
			var now = new NSDate ().SecondsSinceReferenceDate - reference.SecondsSinceReferenceDate;

			if (timeData.StartNow) {
				if (timeData.Timestamps.Count == 1) {
					var position = timeData.Timestamps [0];
					this.musicCoordinator.PlayMusic ("music_gameplay", position);
				} else {
					throw new Exception ("expected to have serverTimestamps.count == 1");
				}
			} else {
				if (this.IsServer) {
					var numberOfRoundTripsToEstimateLatency = 4; // must be less than 16 to fit in data structure.
																 // A round trip has a start and an end time, so we want one more than this in the array.
					if (timeData.Timestamps.Count < numberOfRoundTripsToEstimateLatency + 1) {
						var timestamps = timeData.Timestamps;
						timestamps.Add (now);
						var newData = new StartGameMusicTime (false, timestamps);
						this.session.Send (new GAction { StartGameMusic = newData }, player);
					} else {
						// Estimate the latency as the time taken for a few messages to go across and back
						// divided by the number of ping/pongs and assuming the halfway point.
						var count = timeData.Timestamps.Count;
						var latencyEstimate = 0.5f * (timeData.Timestamps [count - 1] - timeData.Timestamps [0]) / (count - 1f);
						var position = this.musicCoordinator.CurrentMusicTime ();
						var newData = new StartGameMusicTime (true, new List<double> { position + latencyEstimate });
						this.session.Send (new GAction { StartGameMusic = newData }, player);
					}
				} else {
					// echo the same message back to the server
					this.session.Send (new GAction { StartGameMusic = timeData }, player);
				}
			}
		}

		public void UpdateSessionLocation (GameTableLocation location)
		{
			this.session?.UpdateLocation (location);
		}

		#endregion

		#region IGameSessionDelegate

		public void NetworkSessionReceived (NetworkSession session, GameCommand command)
		{
			this.commandsLock.Lock ();

			// Check if the action received is used to setup the board
			// If so, process it and don't wait for the next update cycle to unqueue the event
			// The GameManager is paused at that time of joining a game
			if (command.Action.BoardSetup != null) {
				this.Process (command);
			} else {
				this.gameCommands.Add (command);
			}

			this.commandsLock.Unlock ();
		}

		public void NetworkSessionJoining (NetworkSession session, Player player)
		{
			if (player.Equals (this.session.Host)) {
				this.Delegate?.OnJoiningHost (this, player);
			} else {
				this.Delegate?.OnJoiningPlayer (this, player);
			}
		}

		public void NetworkSessionLeaving (NetworkSession session, Player player)
		{
			if (player == this.session.Host) {
				this.Delegate?.OnLeavingHost (this, player);
			} else {
				this.Delegate?.OnLeavingPlayer (this, player);
			}
		}

		#endregion

		#region ICatapultDelegate

		public GameState GameState { get; private set; } = new GameState ();

		public void DidBreak (Catapult catapult, bool justKnockedout, bool vortex)
		{
			if (justKnockedout) {
				this.sfxCoordinator.PlayCatapultBreak (catapult, vortex);
			}

			this.gameObjectManager.AddBlockObject (catapult);

			this.GameState.TeamACatapults = this.Catapults.Count (c => c.Team == Team.TeamA && !c.Disabled);
			this.GameState.TeamBCatapults = this.Catapults.Count (c => c.Team == Team.TeamB && !c.Disabled);

			// Sending new gameState
			this.Delegate?.OnGameStateUpdated (this, this.GameState);
		}

		public void DidBeginGrab (Catapult catapult)
		{
			// start haptics and sounds too for each catapult
			this.sfxCoordinator.PlayGrabBall (catapult);
		}

		public void DidMove (Catapult catapult, float stretchDistance, float stretchRate)
		{
			// sounds - balloon squeak
			// haptics - vibrate with more energy depending on pull
			var playHaptic = this.IsCurrentPlayerGrabbingACatapult ();
			this.sfxCoordinator.PlayStretch (catapult, stretchDistance, stretchRate, playHaptic);
		}

		public void DidLaunch (Catapult catapult, GameVelocity velocity)
		{
			// sounds - twang of bow or rubber band
			// haptics - big launch vibrate
			this.sfxCoordinator.StopStretch (catapult);
			var playHaptic = this.IsCurrentPlayerGrabbingACatapult ();
			this.sfxCoordinator.PlayLaunch (catapult, velocity, playHaptic);
			if (!UserDefaults.HasOnboarded && playHaptic) {
				UserDefaults.HasOnboarded = true;
			}
		}

		#endregion

		#region IInteractionDelegate

		public IProjectileDelegate ProjectileDelegate => this;

		public List<GameObject> AllBlockObjects => this.gameObjectManager.BlockObjects;

		public void RemoveTableBoxNodeFromLevel ()
		{
			this.tableBoxObject?.ObjectRootNode?.RemoveFromParentNode ();
			var shadowPlane = this.levelNode.FindChildNode ("ShadowPlane", true);
			if (shadowPlane != null) {
				shadowPlane.RunAction (SCNAction.FadeOut (0.5));
			}
		}

		public void RemoveAllPhysicsBehaviors ()
		{
			this.PhysicsWorld.RemoveAllBehaviors ();
		}

		public void AddInteraction (IInteraction interaction)
		{
			this.interactionManager.AddInteraction (interaction);
		}

		public void AddNodeToLevel (SCNNode node)
		{
			this.levelNode.AddChildNode (node);
		}

		public Projectile SpawnProjectile ()
		{
			var projectile = this.gameObjectPool.SpawnProjectile ();
			this.physicsSyncData.ReplaceProjectile (projectile);
			this.gameObjectManager.ReplaceProjectile (projectile);
			// It would be better to use a preallocated audio sampler here if
			// loading a new one takes too long. But it appears ok for now...
			this.SetupAudioComponent (projectile);

			return projectile;
		}

		public Projectile CreateProjectile ()
		{
			return this.gameObjectPool.CreateProjectile (ProjectileType.Cannonball, null);
		}

		public int GameObjectPoolCount ()
		{
			return this.gameObjectPool.InitialPoolCount;
		}

		public void DispatchActionToServer (GameActionType gameAction)
		{
			if (this.IsServer) {
				this.QueueAction (gameAction);
			} else {
				this.Send (gameAction); // send to host
			}
		}

		public void DispatchActionToAll (GameActionType gameAction)
		{
			this.QueueAction (gameAction);
			this.Send (gameAction);
		}

		public void ServerDispatchActionToAll (GameActionType gameAction)
		{
			if (this.IsServer) {
				this.Send (gameAction);
			}
		}

		public void DispatchToPlayer (GameActionType gameAction, Player player)
		{
			if (this.CurrentPlayer == player) {
				this.QueueAction (gameAction);
			} else {
				this.session?.Send (new GAction { GameAction = gameAction }, player);
			}
		}

		#endregion

		#region IProjectileDelegate

		public void DespawnProjectile (Projectile projectile)
		{
			this.gameObjectPool.DespawnProjectile (projectile);
		}

		public void AddParticles (SCNNode particlesNode, SCNVector3 worldPosition)
		{
			levelNode.AddChildNode (particlesNode);
			particlesNode.WorldPosition = worldPosition;
		}

		#endregion

		#region PhysicsSyncSceneDataDelegate

		public void HasNetworkDelayStatusChanged (bool hasNetworkDelay)
		{
			this.Delegate?.OnHasNetworkDelay (this, hasNetworkDelay);
		}

		public Projectile SpawnProjectile (int objectIndex)
		{
			var projectile = this.gameObjectPool.SpawnProjectile (objectIndex);
			projectile.Delegate = this;

			this.levelNode.AddChildNode (projectile.ObjectRootNode);
			this.gameObjectManager.ReplaceProjectile (projectile);

			return projectile;
		}

		public void PlayPhysicsSound (int objectIndex, CollisionEvent soundEvent)
		{
			// Find the correct GameObject and play the collision sound
			foreach (var gameObject in this.gameObjects.Where (gameObject => gameObject.Index == objectIndex)) {
				if (gameObject.GetComponent (typeof (GameAudioComponent)) is GameAudioComponent audioComponent) {
					audioComponent.PlayCollisionSound (soundEvent);
				}

				break;
			}
		}

		#endregion

		#region IGameObjectPoolDelegate

		public Dictionary<string, object> GameDefinitions { get; } = new Dictionary<string, object> ();

		public void OnSpawnedProjectile ()
		{
			// Release all physics contraints
			foreach (var block in this.gameObjectManager.BlockObjects) {
				if (block.PhysicsNode?.PhysicsBody != null) {
					block.PhysicsNode.PhysicsBody.AngularVelocityFactor = SCNVector3.One;
				}
			}
		}

		#endregion

		#region IGameAudioComponentDelegate

		public void CollisionEventOccurred (GameAudioComponent component, CollisionEvent collisionEvent)
		{
			// For the server device, play the sound locally immediately.
			if (this.IsServer) {
				component.PlayCollisionSound (collisionEvent);

				// Add to the network sync
				if (component.Entity is GameObject gameObject) {
					this.physicsSyncData.AddSound (gameObject.Index, collisionEvent);
				} else {
					throw new Exception ("Component is not attached to GameObject");
				}
			}
		}

		#endregion

		#region SCNPhysicsContactDelegate

		public override void DidEndContact (SCNPhysicsWorld world, SCNPhysicsContact contact)
		{
			if (contact != null) {
				this.DidCollision (contact.NodeA, contact.NodeB, new SCNVector3 (contact.ContactPoint), (float) contact.CollisionImpulse);
			}
		}

		public override void DidBeginContact (SCNPhysicsWorld world, SCNPhysicsContact contact)
		{
			if (contact != null) {
				this.DidBeginContact (contact.NodeA, contact.NodeB, new SCNVector3 (contact.ContactPoint), (float) contact.CollisionImpulse);
			}
		}

		#endregion

		#region Destroy

		public void Unload ()
		{
			this.session.Delegate = null;
			this.PhysicsWorld.ContactDelegate = null;
			this.levelNode.RemoveFromParentNode ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing) {
				this.Unload ();
				this.gameObjects.Clear ();
				this.tableBoxObject = null;

				this.session.Delegate = null;
				this.session.Dispose ();
				this.pointOfViewSimulation.Dispose ();

				this.commandsLock.Dispose ();
				this.catapultsLock.Dispose ();
				this.touchEventsLock.Dispose ();

				this.physicsSyncData.Delegate = null;
				this.gameObjectPool.ProjectileDelegate = null;
				this.gameObjectPool.Delegate = null;
				this.gameObjectPool.Dispose ();

				//if(this.flagSimulation != null)
				//{
				//    this.flagSimulation.Dispose();
				//    this.flagSimulation = null;
				//}
			}
		}

		#endregion

		/// <summary>
		/// Actions coming from the main thread/UI layer
		/// </summary>
		class TouchEvent {
			public TouchEvent (TouchType type, Ray camera)
			{
				this.Type = type;
				this.Camera = camera;
			}

			public TouchType Type { get; set; }

			public Ray Camera { get; set; }
		}
	}
}
