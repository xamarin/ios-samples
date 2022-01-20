using XamarinShot.Models.GameplayState;

namespace XamarinShot.Models;

public class GameState
{
        public int TeamACatapults { get; set; } = 0;

        public int TeamBCatapults { get; set; } = 0;

        public void Add (Catapult catapult)
        {
                switch (catapult.Team)
                {
                        case Team.TeamA:
                                TeamACatapults += 1;
                                break;
                        case Team.TeamB:
                                TeamBCatapults += 1;
                                break;
                }
        }
}

public interface IGameManagerDelegate
{
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
                       IGameAudioComponentDelegate
{
        // interactions with the scene must be on the main thread
        readonly SCNScene? scene;
        GameLevel level;
        SCNNode levelNode;

        // use this to access the simulation scaled camera
        readonly SCNNode pointOfViewSimulation;

        // these come from ARSCNView currentlys
        readonly SCNNode pointOfView; // can be in sim or render space

        GameBoard? gameBoard;
        GameObject? tableBoxObject;

        // should be the inverse of the level's world transform
        SCNMatrix4 renderToSimulationTransform = SCNMatrix4.Identity;

        readonly IList<GameObject> gameObjects = new List<GameObject> ();      // keep track of all of our entities here
        GameCamera? gameCamera;
        GameLight? gameLight;

        readonly NetworkSession? session;
        readonly SFXCoordinator sfxCoordinator;
        readonly MusicCoordinator musicCoordinator;
        readonly bool useWallClock;

        readonly NSLock catapultsLock = new NSLock ();

        readonly List<GameCommand> gameCommands = new List<GameCommand> ();
        readonly NSLock commandsLock = new NSLock ();

        readonly List<TouchEvent> touchEvents = new List<TouchEvent> ();
        readonly NSLock touchEventsLock = new NSLock ();

        Dictionary<string, List<GameObject>> categories = new Dictionary<string, List<GameObject>> ();  // this object can be used to group like items if their gamedefs include a category

        // Refernces to Metal do not compile for the Simulator
        // TODO:
        //#if !targetEnvironment(simulator)
        //private MetalClothSimulator flagSimulation;
        //#endif

        // Physics
        readonly PhysicsSyncSceneData physicsSyncData = new PhysicsSyncSceneData ();
        readonly GameObjectPool gameObjectPool = new GameObjectPool ();
        readonly InteractionManager interactionManager = new InteractionManager ();
        readonly GameObjectManager gameObjectManager = new GameObjectManager ();

        public GameManager (SCNView sceneView,
                           GameLevel level,
                           NetworkSession? session,
                           AVAudioEnvironmentNode audioEnvironment,
                           MusicCoordinator musicCoordinator) : base ()
        {

                // make our own scene instead of using the incoming one
                scene = sceneView.Scene;
                PhysicsWorld = scene?.PhysicsWorld!;
                PhysicsWorld.Gravity = new SCNVector3 (0f, -10f, 0f);

                //if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR)
                //{
                //this.flagSimulation = new MetalClothSimulator(sceneView.Device);
                //}

                // this is a node, that isn't attached to the ARSCNView
                pointOfView = sceneView.PointOfView!;
                pointOfViewSimulation = pointOfView.Clone ();

                this.level = level;

                this.session = session;
                this.musicCoordinator = musicCoordinator;
                sfxCoordinator = new SFXCoordinator (audioEnvironment);
                useWallClock = UserDefaults.SynchronizeMusicWithWallClock;

                // init entity system
                GameDefinitions = GameObject.LoadGameDefs ("art.scnassets/data/entities_def");

                // load the level if it wasn't already pre-loaded
                this.level.Load ();

                // start with a copy of the level, never change the originals, since we use original to reset
                levelNode = this.level.ActiveLevel!;

                IsNetworked = this.session is not null;
                IsServer = this.session?.IsServer ?? true; // Solo game act like a server

                if (this.session is not null)
                {
                        this.session.Delegate = this;
                }

                PhysicsWorld.ContactDelegate = this; // get notified of collisions
        }

        ~GameManager ()
        {
                Unload ();
                Dispose (false);
        }

        public IGameManagerDelegate? Delegate { get; set; }

        public Player CurrentPlayer { get; private set; } = UserDefaults.Myself;

        public List<Catapult> Catapults { get; private set; } = new List<Catapult> ();

        public SCNPhysicsWorld PhysicsWorld { get; private set; }

        public bool IsNetworked { get; private set; }

        public bool IsServer { get; private set; }

        /// <summary>
        /// Don't execute any code from SCNView renderer until this is true
        /// </summary>
        public bool IsInitialized { get; private set; }

        public SCNMatrix4 RenderToSimulationTransform
        {
                get
                {
                        return renderToSimulationTransform;
                }

                set
                {
                        renderToSimulationTransform = value;
                        sfxCoordinator.RenderToSimulationTransform = renderToSimulationTransform;
                }
        }

        public void Send (BoardSetupAction boardAction)
        {
                session?.Send (new GAction { BoardSetup = boardAction });
        }

        public void Send (BoardSetupAction boardAction, Player player)
        {
                session?.Send (new GAction { BoardSetup = boardAction }, player);
        }

        public void Send (GameActionType gameAction)
        {
                session?.Send (new GAction { GameAction = gameAction });
        }

        #region processing touches

        CameraInfo lastCameraInfo = new CameraInfo (SCNMatrix4.Identity);

        public void HandleTouch (TouchType type)
        {
                if (!UserDefaults.Spectator)
                {
                        touchEventsLock.Lock ();
                        touchEvents.Add (new TouchEvent (type, lastCameraInfo.Ray));
                        touchEventsLock.Unlock ();
                }
        }

        public void UpdateCamera (CameraInfo cameraInfo)
        {
                if (gameCamera is null)
                {
                        // need the real render camera in order to set rendering state
                        var camera = pointOfView;
                        camera.Name = "GameCamera";
                        gameCamera = new GameCamera (camera);
                        _ = InitGameObject (camera);

                        gameCamera.UpdateProperties ();
                }

                // transfer props to the current camera
                gameCamera.TransferProperties ();

                interactionManager.UpdateAll (cameraInfo);
                lastCameraInfo = cameraInfo;
        }

        #endregion

        #region inbound from network

        private void Process (GameCommand command)
        {
                if (command.Action.GameAction is not null)
                {
                        if (command.Player is not null)
                        {
                                interactionManager.Handle (command.Action.GameAction, command.Player);
                        }
                }
                else if (command.Action.BoardSetup is not null)
                {
                        if (command.Player is not null)
                        {
                                Delegate?.OnReceived (this, command.Action.BoardSetup, command.Player);
                        }
                }
                else if (command.Action.Physics is not null)
                {
                        physicsSyncData.Receive (command.Action.Physics);
                }
                else if (command.Action.StartGameMusic is not null)
                {
                        // Start music at the correct place.
                        if (command.Player is not null)
                        {
                                HandleStartGameMusic (command.Action.StartGameMusic, command.Player);
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
                ProcessCommandQueue ();
                ProcessTouches ();
                SyncPhysics ();

                // TODO:
                //#if !targetEnvironment(simulator)
                //this.flagSimulation.Update(this.levelNode);
                //#endif

                gameObjectManager.Update (timeDelta);

                var entities = new List<GameObject> (gameObjects);
                entities.ForEach ((entity) => entity.Update (timeDelta));
        }

        private const int MaxCatapults = 6;

        public CoreGraphics.CGRect MainScreenBounds { get; set; }

        /// <summary>
        /// Keep track of which catapults we can see as candidates for grabbing/highlighting
        /// </summary>
        public void UpdateCatapultVisibility (ISCNSceneRenderer renderer, SCNNode camera)
        {
                catapultsLock.Lock ();
                if (Catapults.Any () && Catapults.Count == MaxCatapults)
                {
                        // track which are visible
                        foreach (var catapult in Catapults)
                        {
                                // projectile part should be available, otherwise this is not highlightable
                                var visGeo = catapult.Projectile?.FindNodeWithGeometry ();
                                if (visGeo is null)
                                {
                                        catapult.IsVisible = false;
                                        continue;
                                }

                                // use bigger geo when already highlighted to avoid highlight from flipping back and fourth
                                if (catapult.IsHighlighted && catapult.HighlightObject is not null)
                                {
                                        visGeo = catapult.HighlightObject;
                                }

                                // this is done in scaled space
                                var isVisible = renderer.IsNodeInsideFrustum (visGeo.FindNodeWithGeometry (), camera);
                                catapult.IsVisible = isVisible;

                                catapult.ProjectedPosition = new SCNVector3 (renderer.ProjectPoint (catapult.Base.WorldPosition));
                                catapult.ProjectedPosition = new SCNVector3 (catapult.ProjectedPosition.X / (float)MainScreenBounds.Width,
                                                                            catapult.ProjectedPosition.Y / (float)MainScreenBounds.Height,
                                                                            catapult.ProjectedPosition.Z);
                        }
                }

                catapultsLock.Unlock ();
        }

        void ProcessCommandQueue ()
        {
                // retrieving the command should happen with the lock held, but executing
                // it should be outside the lock.
                // inner function lets us take advantage of the defer keyword
                // for lock management.

                var command = nextCommand ();
                while (command is not null)
                {
                        Process (command);
                        command = nextCommand ();
                }

                GameCommand? nextCommand ()
                {
                        GameCommand? result = null;
                        commandsLock.Lock ();
                        if (gameCommands.Any ())
                        {
                                result = gameCommands.First ();
                                gameCommands.RemoveAt (0);
                        }

                        commandsLock.Unlock ();
                        return result;
                }
        }

        void ProcessTouches ()
        {
                var touch = nextTouch ();
                while (touch is not null)
                {
                        Process (touch);
                        touch = nextTouch ();
                }

                TouchEvent? nextTouch ()
                {
                        TouchEvent? result = null;

                        touchEventsLock.Lock ();
                        if (touchEvents.Any ())
                        {
                                result = touchEvents.First ();
                                touchEvents.RemoveAt (0);
                        }

                        touchEventsLock.Unlock ();

                        return result;
                }
        }

        void Process (TouchEvent touch)
        {
                interactionManager.HandleTouch (touch.Type, touch.Camera);
        }

        public void QueueAction (GameActionType gameAction)
        {
                commandsLock.Lock ();
                gameCommands.Add (new GameCommand (CurrentPlayer, new GAction { GameAction = gameAction }));
                commandsLock.Unlock ();
        }

        void SyncPhysics ()
        {
                if (IsNetworked && physicsSyncData.IsInitialized)
                {
                        if (IsServer)
                        {
                                if (session!.IsAnyActivePeers)
                                {
                                        var physicsData = physicsSyncData.GenerateData ();
                                        session.Send (new GAction { Physics = physicsData });
                                }
                        } else {
                                physicsSyncData.UpdateFromReceivedData ();
                        }
                }
        }

        public void PlayWinSound ()
        {
                Delegate?.OnManagerDidWinGame (this);
        }

        public void StartGameMusic (IInteraction interaction)
        {
                StartGameMusicEverywhere ();
        }

        public void StartGameMusic (Player player)
        {
                // Begin by handling an empty message. Our timestamp will be added and
                // sent in ping/pong to estimate latency.
                HandleStartGameMusic (new StartGameMusicTime (false, new List<double> ()), player);
        }

        /// <summary>
        /// Status for SceneViewController to query and display UI interaction
        /// </summary>
        public bool CanGrabACatapult (Ray cameraRay)
        {
                if (interactionManager.Interaction (typeof (CatapultInteraction)) is CatapultInteraction catapultInteraction)
                {
                        return catapultInteraction.CanGrabAnyCatapult (cameraRay);
                } else {
                        return false;
                }
        }

        void DisplayWin ()
        {
                if (interactionManager.Interaction (typeof (VictoryInteraction)) is VictoryInteraction victory)
                {
                        victory.ActivateVictory ();
                }
                else
                {
                        throw new Exception ("No Victory Effect");
                }
        }

        public bool IsCurrentPlayerGrabbingACatapult ()
        {
                var grabInteraction = interactionManager.Interaction (typeof (GrabInteraction)) as GrabInteraction;
                return grabInteraction?.GrabbedGrabbable is Catapult;
        }

        /// <summary>
        /// Configures the node from the level to be placed on the provided board.
        /// </summary>
        public void AddLevel (SCNNode node, GameBoard gameBoard)
        {
                this.gameBoard = gameBoard;

                level.PlaceLevel (node, scene!, this.gameBoard.Scale.X);

                // Initialize table box object
                CreateTableTopOcclusionBox ();

                UpdateRenderTransform ();

                if (level?.ActiveLevel is not null)
                {
                        FixLevelsOfDetail (level.ActiveLevel);
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
        void UpdateRenderTransform ()
        {
                if (gameBoard is not null)
                {
                        // Scale level to normalized scale (1 unit wide) for rendering
                        var levelNodeTransform = SimdExtensions.CreateFromScale (level.NormalizedScale);

                        var worldTransform = gameBoard.WorldTransform;
                        worldTransform.Transpose ();
                        RenderToSimulationTransform = SCNMatrix4.Invert (levelNodeTransform) * SCNMatrix4.Invert (worldTransform);
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
                categories = new Dictionary<string, List<GameObject>> ();

                InitializeGameObjectPool ();

                InitializeLevel ();
                InitBehaviors ();

                // Initialize interactions that add objects to the level
                InitializeInteractions ();

                physicsSyncData.Delegate = this;

                // Start advertising game
                if (session is not null && session.IsServer)
                {
                        session.StartAdvertising ();
                }

                Delegate?.OnDidStartGame (this);

                StartGameMusicEverywhere ();

                IsInitialized = true;
        }

        public void ReleaseLevel ()
        {
                // remove all audio players added to AVAudioEngine.
                sfxCoordinator.RemoveAllAudioSamplers ();
                level.Reset ();
        }

        private void InitBehaviors ()
        {
                // after everything is setup, add the behaviors if any
                foreach (var gameObject in gameObjects)
                {
                        // update constraints
                        foreach (var component in gameObject.Components.OfType<IPhysicsBehaviorComponent> ())
                        {
                                component.InitBehavior (levelNode, PhysicsWorld);
                        }
                }
        }

        #endregion

        #region Table Occlusion

        /// <summary>
        /// Create an opaque object representing the table used to occlude falling objects
        /// </summary>
        void CreateTableTopOcclusionBox ()
        {
                var tableBoxNode = scene?.RootNode.FindChildNode ("OcclusionBox", true);
                if (tableBoxNode is null)
                {
                        throw new Exception ("Table node not found");
                }

                // make a table object so we can attach audio component to it
                tableBoxObject = InitGameObject (tableBoxNode);
        }

        #endregion

        #region Initialize Game Functions

        string? TeamName (SCNNode node)
        {
                string? result = null;
                if (!string.IsNullOrEmpty (node.Name))
                {
                        // set to A or B, don't set blocks to teamAA, AB, AC
                        if (node.Name == "_teamA" || node.Name == "_teamB")
                        {
                                result = node.Name;
                        }
                }

                return result;
        }

        /// <summary>
        /// Walk all the nodes looking for actual objects.
        /// </summary>
        void EnumerateHierarchy (SCNNode node, string? teamName = null)
        {
                // If the node has no name or a name does not contain
                // a type identifier, we look at its children.
                string? identifier = null;
                if (!string.IsNullOrEmpty (node.Name) && !string.IsNullOrEmpty (identifier = node.GetTypeIdentifier ()))
                {
                        Configure (node, node.Name, identifier, teamName);
                }
                else
                {
                        var newTeamName = TeamName (node) ?? teamName;
                        foreach (var child in node.ChildNodes)
                        {
                                EnumerateHierarchy (child, newTeamName);
                        }
                }
        }

        void Configure (SCNNode node, string name, string type, string? team)
        {
                // For nodes with types, we create at most one gameObject, configured
                // based on the node type.

                switch (type)
                {
                        case "catapult":
                                // replaces the placeholder node with a working catapult
                                var catapultNode = Catapult.ReplaceCatapultPlaceholder (node);

                                // Create Catapult GameObject
                                var identifier = Catapults.Count;
                                var catapult = new Catapult (catapultNode, sfxCoordinator, identifier, GameDefinitions);
                                gameObjects.Add (catapult);
                                SetupAudioComponent (catapult);

                                catapultNode.Name = name;

                                catapult.Delegate = this;
                                Catapults.Add (catapult);

                                catapult.UpdateProperties ();
                                catapult.AddComponent (new RemoveWhenFallenComponent ());
                                GameState.Add (catapult);

                                physicsSyncData.AddObject (catapult);
                                break;

                        case "ShadowPlane":
                        case "OcclusionBox":
                                // don't add a game object, but don't visit it either
                                return;

                        case "ShadowLight":
                                if (gameLight is null)
                                {
                                        node.Name = "GameLight";
                                        var light = InitGameObject (node);
                                        gameObjects.Add (light);

                                        gameLight = new GameLight (node);
                                        gameLight.UpdateProperties ();
                                }

                                gameLight?.TransferProperties ();
                                return;

                        default:
                                // This handles all other objects, including blocks, reset switches, etc.
                                // All special functionality is defined in entities_def.json file

                                // can't removing these throw off the object index
                                // if not all clients remove these
                                switch (type)
                                {
                                        case "cloud":
                                                if (!UserDefaults.ShowClouds)
                                                {
                                                        node.RemoveFromParentNode ();
                                                        return;
                                                }
                                                break;

                                        case "flag":
                                                if (!UserDefaults.ShowFlags)
                                                {
                                                        node.RemoveFromParentNode ();
                                                        return;
                                                } else {
                                                        //#if !targetEnvironment(simulator)
                                                        //this.flagSimulation.CreateFlagSimulationFromNode(node);
                                                        //#endif
                                                }
                                                break;

                                        case "resetSwitch":
                                                if (!UserDefaults.ShowResetLever)
                                                {
                                                        node.RemoveFromParentNode ();
                                                        return;
                                                }
                                                break;
                                }
                                break;
                }

                var gameObject = InitGameObject (node);

                // hardcoded overrides for physics happens here
                if (!gameObject.UsePredefinedPhysics)
                {
                        // Constrain the angularVelocity until first ball fires.
                        // This is done to stabilize the level.
                        if (gameObject.PhysicsNode?.PhysicsBody is not null)
                        {
                                gameObject.PhysicsNode.PhysicsBody.AngularVelocityFactor = SCNVector3.Zero;
                        }

                        if (gameObject.PhysicsNode?.PhysicsBody is not null)
                        {
                                gameObject.PhysicsNode.PhysicsBody.AngularDamping = 0.03f;
                                gameObject.PhysicsNode.PhysicsBody.Damping = 0.03f;
                                gameObject.PhysicsNode.PhysicsBody.Mass = 3;
                                gameObject.PhysicsNode.PhysicsBody.LinearRestingThreshold = 1f;
                                gameObject.PhysicsNode.PhysicsBody.AngularRestingThreshold = 1f;

                                var collisionBitMask = (CollisionMask)(int)gameObject.PhysicsNode.PhysicsBody.CollisionBitMask;
                                gameObject.PhysicsNode.PhysicsBody.CollisionBitMask = (nuint)(int)(collisionBitMask | CollisionMask.Ball);

                                if (gameObject.Density > 0)
                                {
                                        gameObject.PhysicsNode.CalculateMassFromDensity (name, gameObject.Density);
                                }

                                gameObject.PhysicsNode.PhysicsBody.ResetTransform ();
                                if (gameObject.PhysicsNode.PhysicsBody.AllowsResting)
                                {
                                        gameObject.PhysicsNode.PhysicsBody.SetResting (true);
                                }
                        }

                        // add to network synchronization code
                        if (gameObject.PhysicsNode is not null)
                        {
                                physicsSyncData.AddObject (gameObject);

                                if (gameObject.IsBlockObject)
                                {
                                        gameObjectManager.AddBlockObject (gameObject);
                                }

                                gameObject.AddComponent (new RemoveWhenFallenComponent ());
                        }

                        if (gameObject.Categorize)
                        {
                                if (!categories.ContainsKey (gameObject.Category))
                                {
                                        categories [gameObject.Category] = new List<GameObject> ();
                                }

                                categories [gameObject.Category].Add (gameObject);
                        }
                }
        }

        /// <summary>
        /// Set the world at rest
        /// </summary>
        public void RestWorld ()
        {
                var objects = new List<GameObject> (gameObjects);
                foreach (var gameObject in objects)
                {
                        if (gameObject.PhysicsNode?.PhysicsBody is not null && 
                                gameObject != tableBoxObject &&
                                gameObject.PhysicsNode.PhysicsBody.AllowsResting)
                        {
                                gameObject.PhysicsNode.PhysicsBody.SetResting (true);
                        }
                }
        }

        void PostUpdateHierarchy (SCNNode node)
        {
                if (node.ValueForKey (new NSString ("nameRestore")) is NSString nameRestore)
                {
                        node.Name = nameRestore.ToString ();
                }

                foreach (var child in node.ChildNodes)
                {
                        PostUpdateHierarchy (child);
                }
        }

        void InitializeGameObjectPool ()
        {
                gameObjectPool.ProjectileDelegate = this;
                gameObjectPool.CreatePoolObjects (this);

                // GameObjectPool has a fixed number of items which we need to add to physicsSyncData and gameObjectManager
                foreach (var projectile in gameObjectPool.ProjectilePool)
                {
                        physicsSyncData.AddProjectile (projectile);
                        gameObjectManager.AddProjectile (projectile);
                        SetupAudioComponent (projectile);
                }
        }

        void SetupAudioComponent (GameObject @object)
        {
                if (@object.GetComponent (typeof (GameAudioComponent)) is GameAudioComponent audioComponent)
                {
                        sfxCoordinator.SetupGameAudioComponent (audioComponent);
                        audioComponent.Delegate = this;
                }
        }

        void InitializeLevel ()
        {
                // enumerateHierarchy is recursive and may find catapults at any level
                // putting the lock outside ensures that the win condition won't be evaluated
                // on an incomplete set of catapults.
                catapultsLock.Lock ();

                EnumerateHierarchy (levelNode);

                // do post init functions here
                PostUpdateHierarchy (levelNode);

                catapultsLock.Unlock ();
        }

        void InitializeInteractions ()
        {
                // Grab Interaction
                var grabInteraction = new GrabInteraction (this);
                interactionManager.AddInteraction (grabInteraction);

                // Highlight Interaction
                var highlightInteraction = new HighlightInteraction (this);
                highlightInteraction.GrabInteraction = grabInteraction;
                highlightInteraction.SfxCoordinator = sfxCoordinator;
                interactionManager.AddInteraction (highlightInteraction);

                // Catapult Interaction
                var catapultInteraction = new CatapultInteraction (this);
                catapultInteraction.GrabInteraction = grabInteraction;
                interactionManager.AddInteraction (catapultInteraction);

                // Fill Catapult Interaction with catapults
                if (!Catapults.Any ())
                {
                        throw new Exception ("Catapult not initialized");
                }

                foreach (var catapult in Catapults)
                {
                        catapultInteraction.AddCatapult (catapult);
                }

                // Catapult Disable Interaction
                interactionManager.AddInteraction (new CatapultDisableInteraction (this));

                // Vortex
                var vortex = new VortexInteraction (this);
                vortex.VortexActivationDelegate = catapultInteraction;
                vortex.SfxCoordinator = sfxCoordinator;
                vortex.MusicCoordinator = musicCoordinator;
                interactionManager.AddInteraction (vortex);

                // Lever
                var lever = new LeverInteraction (this);
                var switches = new List<GameObject> ();
                if (categories.TryGetValue ("reset", out List<GameObject>? processedSwitches))
                {
                        switches = processedSwitches;
                }

                lever.Setup (switches, vortex);
                lever.SfxCoordinator = sfxCoordinator;
                interactionManager.AddInteraction (lever);

                // Victory
                interactionManager.AddInteraction (new VictoryInteraction (this));
        }

        #endregion

        #region Physics scaling

        public void CopySimulationCamera ()
        {
                // copy the POV camera to minimize the need to lock, this is right after ARKit updates it in
                // the render thread, and before we scale the actual POV camera for rendering
                pointOfViewSimulation.WorldTransform = pointOfView.WorldTransform;
        }

        public void ScaleCameraToRender ()
        {
                var worldTransform = pointOfView.WorldTransform;
                worldTransform.Transpose ();
                var newWorldTransform = renderToSimulationTransform * worldTransform;
                newWorldTransform.Transpose ();
                pointOfView.WorldTransform = newWorldTransform;
        }

        public void ScaleCameraToSimulation ()
        {
                pointOfView.WorldTransform = pointOfViewSimulation.WorldTransform;
        }

        public SCNMatrix4 RenderSpaceTransformToSimulationSpace (SCNMatrix4 transform)
        {
                return renderToSimulationTransform * transform;
        }

        GameObject InitGameObject (SCNNode node)
        {
                var gameObject = GameObject.Create<GameObject> (node, GameDefinitions);

                gameObjects.Add (gameObject);
                SetupAudioComponent (gameObject);

                return gameObject;
        }

        /// <summary>
        /// After collision we care about is detected, we check for any collision related components and process them
        /// </summary>
        void DidCollision (SCNNode nodeA, SCNNode nodeB, SCNVector3 pos, float impulse)
        {
                // let any collision handling components on nodeA respond to the collision with nodeB

                var entity = nodeA.NearestParentGameObject ();
                if (entity is not null)
                {
                        foreach (var component in entity.Components.OfType<ICollisionHandlerComponent>())
                        {
                                component.DidCollision (this, nodeA, nodeB, pos, impulse);
                        }
                }

                // let any collision handling components in nodeB respond to the collision with nodeA
                entity = nodeB.NearestParentGameObject ();
                if (entity is not null)
                {
                        foreach (var component in entity.Components.OfType<ICollisionHandlerComponent> ())
                        {
                                component.DidCollision (this, nodeA, nodeB, pos, impulse);
                        }
                }

                interactionManager.DidCollision (nodeA, nodeB, pos, impulse);
        }

        public void DidBeginContact (SCNNode nodeA, SCNNode nodeB, SCNVector3 pos, float impulse)
        {
                interactionManager.DidCollision (nodeA, nodeB, pos, impulse);
        }

        public void OnDidApplyConstraints (ISCNSceneRenderer renderer)
        {
                gameObjectManager.OnDidApplyConstraints (renderer);
        }

        /// <summary>
        /// Start the game music on the server device and all connected devices
        /// </summary>
        void StartGameMusicEverywhere ()
        {
                if (IsServer)
                {
                        // Start music locally:
                        var timeData = StartGameMusicNow ();
                        HandleStartGameMusic (timeData, CurrentPlayer);

                        // Start the game music on all connected clients:
                        session?.Send (new GAction { StartGameMusic = timeData });
                }
        }

        StartGameMusicTime StartGameMusicNow ()
        {
                var cal = new NSCalendar (NSCalendarType.Gregorian);
                var dc = cal.Components (NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day, new NSDate ());
                var reference = cal.DateFromComponents (dc); // chose a reference date of the start of today.
                var now = new NSDate ().SecondsSinceReferenceDate - reference.SecondsSinceReferenceDate; ;
                if (useWallClock)
                {
                        return new StartGameMusicTime (true, new List<double> { now });
                } else {
                        return new StartGameMusicTime (true, new List<double> { 0d });
                }
        }

        void HandleStartGameMusic (StartGameMusicTime timeData, Player player)
        {
                if (useWallClock)
                {
                        HandleStartGameMusicWithWallClock (timeData, player);
                } else {
                        HandleStartGameMusicWithLatencyEstimate (timeData, player);
                }
        }

        void HandleStartGameMusicWithWallClock (StartGameMusicTime timeData, Player player)
        {
                if (session is null)
                {
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

                if (timeData.StartNow)
                {
                        if (timeData.Timestamps.Count == 1)
                        {
                                var startWallTime = timeData.Timestamps [0];
                                var position = now - startWallTime;
                                musicCoordinator.PlayMusic ("music_gameplay", position);
                        } else {
                                throw new Exception ("expected to have serverTimestamps.count == 1");
                        }
                } else {
                        if (IsServer)
                        {
                                var position = musicCoordinator.CurrentMusicTime ();
                                var newData = new StartGameMusicTime (true, new List<double> { now - position });
                                session.Send (new GAction { StartGameMusic = newData }, player);
                        }
                }
        }

        void HandleStartGameMusicWithLatencyEstimate (StartGameMusicTime timeData, Player player)
        {
                if (session is null)
                {
                        throw new Exception ("Need a game session");
                }

                // This synchronization method uses an echoed message (like ping) to calculate
                // the time taken to send a message to the other device and back and make an
                // estimate of latency based on the average of a few of these round trips.

                var cal = new NSCalendar (NSCalendarType.Gregorian);
                var dc = cal.Components (NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day, new NSDate ());
                var reference = cal.DateFromComponents (dc); // chose a reference date of the start of today.
                var now = new NSDate ().SecondsSinceReferenceDate - reference.SecondsSinceReferenceDate;

                if (timeData.StartNow)
                {
                        if (timeData.Timestamps.Count == 1)
                        {
                                var position = timeData.Timestamps [0];
                                musicCoordinator.PlayMusic ("music_gameplay", position);
                        } else {
                                throw new Exception ("expected to have serverTimestamps.count == 1");
                        }
                } else {
                        if (IsServer)
                        {
                                var numberOfRoundTripsToEstimateLatency = 4; // must be less than 16 to fit in data structure.
                                                                             // A round trip has a start and an end time, so we want one more than this in the array.
                                if (timeData.Timestamps.Count < numberOfRoundTripsToEstimateLatency + 1)
                                {
                                        var timestamps = timeData.Timestamps;
                                        timestamps.Add (now);
                                        var newData = new StartGameMusicTime (false, timestamps);
                                        session.Send (new GAction { StartGameMusic = newData }, player);
                                } else {
                                        // Estimate the latency as the time taken for a few messages to go across and back
                                        // divided by the number of ping/pongs and assuming the halfway point.
                                        var count = timeData.Timestamps.Count;
                                        var latencyEstimate = 0.5f * (timeData.Timestamps [count - 1] - timeData.Timestamps [0]) / (count - 1f);
                                        var position = musicCoordinator.CurrentMusicTime ();
                                        var newData = new StartGameMusicTime (true, new List<double> { position + latencyEstimate });
                                        session.Send (new GAction { StartGameMusic = newData }, player);
                                }
                        } else {
                                // echo the same message back to the server
                                session.Send (new GAction { StartGameMusic = timeData }, player);
                        }
                }
        }

        public void UpdateSessionLocation (GameTableLocation location)
        {
                session?.UpdateLocation (location);
        }

        #endregion

        #region IGameSessionDelegate

        public void NetworkSessionReceived (NetworkSession session, GameCommand command)
        {
                commandsLock.Lock ();

                // Check if the action received is used to setup the board
                // If so, process it and don't wait for the next update cycle to unqueue the event
                // The GameManager is paused at that time of joining a game
                if (command.Action.BoardSetup is not null)
                {
                        Process (command);
                } else {
                        gameCommands.Add (command);
                }

                commandsLock.Unlock ();
        }

        public void NetworkSessionJoining (NetworkSession session, Player player)
        {
                if (player.Equals (session.Host))
                {
                        Delegate?.OnJoiningHost (this, player);
                } else {
                        Delegate?.OnJoiningPlayer (this, player);
                }
        }

        public void NetworkSessionLeaving (NetworkSession session, Player player)
        {
                if (player == session.Host)
                {
                        Delegate?.OnLeavingHost (this, player);
                } else {
                        Delegate?.OnLeavingPlayer (this, player);
                }
        }

        #endregion

        #region ICatapultDelegate

        public GameState GameState { get; private set; } = new GameState ();

        public void DidBreak (Catapult catapult, bool justKnockedout, bool vortex)
        {
                if (justKnockedout)
                {
                        sfxCoordinator.PlayCatapultBreak (catapult, vortex);
                }

                gameObjectManager.AddBlockObject (catapult);

                GameState.TeamACatapults = Catapults.Count (c => c.Team == Team.TeamA && !c.Disabled);
                GameState.TeamBCatapults = Catapults.Count (c => c.Team == Team.TeamB && !c.Disabled);

                // Sending new gameState
                Delegate?.OnGameStateUpdated (this, GameState);
        }

        public void DidBeginGrab (Catapult catapult)
        {
                // start haptics and sounds too for each catapult
                sfxCoordinator.PlayGrabBall (catapult);
        }

        public void DidMove (Catapult catapult, float stretchDistance, float stretchRate)
        {
                // sounds - balloon squeak
                // haptics - vibrate with more energy depending on pull
                var playHaptic = IsCurrentPlayerGrabbingACatapult ();
                sfxCoordinator.PlayStretch (catapult, stretchDistance, stretchRate, playHaptic);
        }

        public void DidLaunch (Catapult catapult, GameVelocity velocity)
        {
                // sounds - twang of bow or rubber band
                // haptics - big launch vibrate
                sfxCoordinator.StopStretch (catapult);
                var playHaptic = IsCurrentPlayerGrabbingACatapult ();
                sfxCoordinator.PlayLaunch (catapult, velocity, playHaptic);
                if (!UserDefaults.HasOnboarded && playHaptic)
                {
                        UserDefaults.HasOnboarded = true;
                }
        }

        #endregion

        #region IInteractionDelegate

        public IProjectileDelegate ProjectileDelegate => this;

        public List<GameObject> AllBlockObjects => gameObjectManager.BlockObjects;

        public void RemoveTableBoxNodeFromLevel ()
        {
                tableBoxObject?.ObjectRootNode?.RemoveFromParentNode ();
                var shadowPlane = levelNode.FindChildNode ("ShadowPlane", true);
                if (shadowPlane is not null)
                {
                        shadowPlane.RunAction (SCNAction.FadeOut (0.5));
                }
        }

        public void RemoveAllPhysicsBehaviors ()
        {
                PhysicsWorld.RemoveAllBehaviors ();
        }

        public void AddInteraction (IInteraction interaction)
        {
                interactionManager.AddInteraction (interaction);
        }

        public void AddNodeToLevel (SCNNode node)
        {
                levelNode.AddChildNode (node);
        }

        public Projectile SpawnProjectile ()
        {
                var projectile = gameObjectPool.SpawnProjectile ();
                physicsSyncData.ReplaceProjectile (projectile);
                gameObjectManager.ReplaceProjectile (projectile);
                // It would be better to use a preallocated audio sampler here if
                // loading a new one takes too long. But it appears ok for now...
                SetupAudioComponent (projectile);

                return projectile;
        }

        public Projectile CreateProjectile ()
        {
                return gameObjectPool.CreateProjectile (ProjectileType.Cannonball, null);
        }

        public int GameObjectPoolCount ()
        {
                return gameObjectPool.InitialPoolCount;
        }

        public void DispatchActionToServer (GameActionType gameAction)
        {
                if (IsServer)
                {
                        QueueAction (gameAction);
                } else {
                        Send (gameAction); // send to host
                }
        }

        public void DispatchActionToAll (GameActionType gameAction)
        {
                QueueAction (gameAction);
                Send (gameAction);
        }

        public void ServerDispatchActionToAll (GameActionType gameAction)
        {
                if (IsServer)
                {
                        Send (gameAction);
                }
        }

        public void DispatchToPlayer (GameActionType gameAction, Player player)
        {
                if (CurrentPlayer == player)
                {
                        QueueAction (gameAction);
                } else {
                        session?.Send (new GAction { GameAction = gameAction }, player);
                }
        }

        #endregion

        #region IProjectileDelegate

        public void DespawnProjectile (Projectile projectile)
        {
                gameObjectPool.DespawnProjectile (projectile);
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
                Delegate?.OnHasNetworkDelay (this, hasNetworkDelay);
        }

        public Projectile SpawnProjectile (int objectIndex)
        {
                var projectile = gameObjectPool.SpawnProjectile (objectIndex);
                projectile.Delegate = this;

                if (projectile.ObjectRootNode is not null)
                        levelNode.AddChildNode (projectile.ObjectRootNode);
                gameObjectManager.ReplaceProjectile (projectile);

                return projectile;
        }

        public void PlayPhysicsSound (int objectIndex, CollisionEvent soundEvent)
        {
                // Find the correct GameObject and play the collision sound
                foreach (var gameObject in gameObjects.Where (gameObject => gameObject.Index == objectIndex))
                {
                        if (gameObject.GetComponent (typeof (GameAudioComponent)) is GameAudioComponent audioComponent)
                        {
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
                foreach (var block in gameObjectManager.BlockObjects)
                {
                        if (block.PhysicsNode?.PhysicsBody is not null)
                        {
                                block.PhysicsNode.PhysicsBody.AngularVelocityFactor = SCNVector3.One;
                        }
                }
        }

        #endregion

        #region IGameAudioComponentDelegate

        public void CollisionEventOccurred (GameAudioComponent component, CollisionEvent collisionEvent)
        {
                // For the server device, play the sound locally immediately.
                if (IsServer)
                {
                        component.PlayCollisionSound (collisionEvent);

                        // Add to the network sync
                        if (component.Entity is GameObject gameObject)
                        {
                                physicsSyncData.AddSound (gameObject.Index, collisionEvent);
                        } else {
                                throw new Exception ("Component is not attached to GameObject");
                        }
                }
        }

        #endregion

        #region SCNPhysicsContactDelegate

        public override void DidEndContact (SCNPhysicsWorld world, SCNPhysicsContact contact)
        {
                if (contact is not null)
                {
                        DidCollision (contact.NodeA, contact.NodeB, new SCNVector3 (contact.ContactPoint), (float)contact.CollisionImpulse);
                }
        }

        public override void DidBeginContact (SCNPhysicsWorld world, SCNPhysicsContact contact)
        {
                if (contact is not null)
                {
                        DidBeginContact (contact.NodeA, contact.NodeB, new SCNVector3 (contact.ContactPoint), (float)contact.CollisionImpulse);
                }
        }

        #endregion

        #region Destroy

        public void Unload ()
        {
                if (session is not null)
                        session.Delegate = null;
                PhysicsWorld.ContactDelegate = null;
                levelNode.RemoveFromParentNode ();
        }

        protected override void Dispose (bool disposing)
        {
                base.Dispose (disposing);
                if (disposing)
                {
                        Unload ();
                        gameObjects.Clear ();
                        tableBoxObject = null;

                        if (session is not null)
                        {
                                session.Delegate = null;
                                session.Dispose ();
                        }
                        pointOfViewSimulation.Dispose ();

                        commandsLock.Dispose ();
                        catapultsLock.Dispose ();
                        touchEventsLock.Dispose ();

                        physicsSyncData.Delegate = null;
                        gameObjectPool.ProjectileDelegate = null;
                        gameObjectPool.Delegate = null;
                        gameObjectPool.Dispose ();

                        //if(this.flagSimulation is not null)
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
        class TouchEvent
        {
                public TouchEvent (TouchType type, Ray camera)
                {
                        Type = type;
                        Camera = camera;
                }

                public TouchType Type { get; set; }

                public Ray Camera { get; set; }
        }
}
