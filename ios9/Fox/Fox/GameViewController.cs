using System;
using System.Collections.Generic;

using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using SceneKit;
using SpriteKit;
using UIKit;

namespace Fox {
	public partial class GameViewController : UIViewController, ISCNSceneRendererDelegate, ISCNPhysicsContactDelegate {

		const int NanoSecondsPerSeond = 1000000000;
		const float CharacterSpeedFactor = 2f / 1.3f;
		const float MaxRise = .08f;
		const float MaxJump = 10f;
		const double GravityAcceleration = .18;
		const string ArtFolderRoot = "game.scnassets";

		SCNNode cameraYHandle;
		SCNNode cameraXHandle;

		SCNMaterial grassArea;
		SCNMaterial waterArea;
		SCNNode[] flames;
		SCNNode[] enemies;

		bool gameIsComplete;
		bool isInvincible;
		bool lockCamera;

		SCNAudioPlayer flameThrowerSound;
		SCNAudioSource collectPearlSound;
		SCNAudioSource collectFlowerSound;
		SCNAudioSource hitSound;
		SCNAudioSource pshhhSound;
		SCNAudioSource aahSound;
		SCNAudioSource victoryMusic;

		// Particles
		SCNParticleSystem collectParticles;
		SCNParticleSystem confetti;

		// For automatic camera animation
		SCNNode currentGround;
		SCNNode mainGround;
		Dictionary<SCNNode, SCNVector3> groundToCameraPosition;

		double previousUpdateTime;
		nfloat maxPenetrationDistance;
		float accelerationY;
		bool  positionNeedsAdjustment;
		SCNVector3 replacementPosition;

		public Character Character { get; set; }

		[Export ("initWithCoder:")]
		public GameViewController (NSCoder coder) : base (coder)
		{
		}

		public override bool ShouldAutorotate ()
		{
			return false;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return (toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight) || (toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.LandscapeRight | UIInterfaceOrientationMask.LandscapeLeft;
		}

		void SetupCamera ()
		{
			SCNNode pov = GameView.PointOfView;
			const float altitude = 1f;
			const float distance = 10f;

			cameraYHandle = SCNNode.Create ();
			cameraXHandle = SCNNode.Create ();

			cameraYHandle.Position = new SCNVector3 (0f, altitude, 0f);
			cameraYHandle.AddChildNode (cameraXHandle);
			GameView.Scene.RootNode.AddChildNode (cameraYHandle);

			pov.EulerAngles = new SCNVector3 (0f, 0f, 0f);
			pov.Position = new SCNVector3 (0f, 0f, distance);

			cameraYHandle.Rotation = new SCNVector4 (0f, 1f, 0f, (float)Math.PI / 2f + (float)Math.PI / 4f * 3f);
			cameraXHandle.Rotation = new SCNVector4 (1f, 0f, 0f, -(float)Math.PI / 4f * 0.125f);
			cameraXHandle.AddChildNode (pov);

			// Animate camera on launch and prevent the user from manipulating the camera until the end of the animation
			lockCamera = true;
			SCNTransaction.Begin ();
			SCNTransaction.SetCompletionBlock (() => {
				lockCamera = false;
			});

			var cameraYAnimation = CABasicAnimation.FromKeyPath ("rotation.w");
			cameraYAnimation.From = NSNumber.FromDouble (Math.PI * 2 - cameraYHandle.Rotation.W);
			cameraYAnimation.To = NSNumber.FromDouble (0.0);
			cameraYAnimation.Additive = true;
			cameraYAnimation.BeginTime = CAAnimation.CurrentMediaTime () + 3;
			cameraYAnimation.FillMode = CAFillMode.Both;
			cameraYAnimation.Duration = 5.0;
			cameraYAnimation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			cameraYHandle.AddAnimation (cameraYAnimation, null);

			var cameraXAnimation = CABasicAnimation.FromKeyPath ("rotation.w");
			cameraXAnimation.From = NSNumber.FromDouble (-Math.PI / 2 + cameraXHandle.Rotation.W);	
			cameraXAnimation.To = NSNumber.FromDouble (0.0);
			cameraXAnimation.Additive = true;
			cameraXAnimation.FillMode = CAFillMode.Both;
			cameraXAnimation.Duration = 5.0;
			cameraXAnimation.BeginTime = CAAnimation.CurrentMediaTime () + 3;
			cameraXAnimation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			cameraXHandle.AddAnimation (cameraXAnimation, null);

			SCNTransaction.Commit ();

			var lookAtConstraint = SCNLookAtConstraint.Create (Character.Node.FindChildNode ("Bip001_Head", true));
			lookAtConstraint.InfluenceFactor = 0;
			pov.Constraints = new SCNConstraint[] { lookAtConstraint };
		}

		public void PanCamera (CGSize dir)
		{
			if (lockCamera)
				return;

			const float f = 0.005f;
			// Make sure the camera handles are correctly reset (because automatic camera animations may have put the "rotation" in a weird state
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.0;

			cameraYHandle.RemoveAllActions ();
			cameraXHandle.RemoveAllActions ();

			if (cameraYHandle.Rotation.Y < 0f)
				cameraYHandle.Rotation = new SCNVector4 (0f, 1f, 0f, -cameraYHandle.Rotation.W);

			if (cameraXHandle.Rotation.X < 0f)
				cameraXHandle.Rotation = new SCNVector4 (1f, 0f, 0f, -cameraXHandle.Rotation.W);
			
			SCNTransaction.Commit ();

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.5;
			SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);

			cameraYHandle.Rotation = new SCNVector4 (0f, 1f, 0f, cameraYHandle.Rotation.Y * (cameraYHandle.Rotation.W - (float)dir.Width * f));
			cameraXHandle.Rotation = new SCNVector4 (1f, 0f, 0f, (float)Math.Max (-Math.PI / 2.0, Math.Min (0.13, cameraXHandle.Rotation.W + dir.Height * f)));
			SCNTransaction.Commit ();
		}

		void SetupCollisionNodes (SCNNode node)
		{
			if (node.Geometry != null) {
				Console.WriteLine (node.Name);
				node.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
				node.PhysicsBody.CategoryBitMask = (nuint)(int)Bitmask.Collision;

				var options = new SCNPhysicsShapeOptions {
					ShapeType = SCNPhysicsShapeType.ConcavePolyhedron
				};
				node.PhysicsBody.PhysicsShape = SCNPhysicsShape.Create (node, options);

				// Get grass area to play the right sound steps
				if (node.Geometry.FirstMaterial.Name == "grass-area") {
					if (grassArea != null)
						node.Geometry.FirstMaterial = grassArea;
					else
						grassArea = node.Geometry.FirstMaterial;
				}

				// Get the water area
				if (node.Geometry.FirstMaterial.Name == "water")
					waterArea = node.Geometry.FirstMaterial;

				// Temporary workaround because concave shape created from geometry instead of node fails
				SCNNode child = SCNNode.Create ();
				node.AddChildNode (child);
				child.Hidden = true;
				child.Geometry = node.Geometry;
				node.Geometry = null;
				node.Hidden = false;

				if (node.Name == "water")
					node.PhysicsBody.CategoryBitMask = (nuint)(int)Bitmask.Water;
			}

			foreach (SCNNode child in node.ChildNodes) {
				if (!child.Hidden)
					SetupCollisionNodes (child);
			}
		}

		void SetupSounds ()
		{
			// Get an arbitrary node to attach the sounds to
			SCNNode node = GameView.Scene.RootNode;
			// The wind sound
			SCNAudioSource source = SCNAudioSource.FromFile ("game.scnassets/sounds/wind.m4a");
			source.Volume = .3f;
			SCNAudioPlayer player = SCNAudioPlayer.FromSource (source);
			source.Loops = true;
			source.ShouldStream = true;
			source.Positional = false;
			node.AddAudioPlayer (player);

			// fire
			source = SCNAudioSource.FromFile ("game.scnassets/sounds/flamethrower.mp3");
			source.Loops = true;
			source.Volume = 0;
			source.Positional = false;
			flameThrowerSound = SCNAudioPlayer.FromSource (source);
			node.AddAudioPlayer (flameThrowerSound);

			// hit
			hitSound = SCNAudioSource.FromFile ("game.scnassets/sounds/ouch_firehit.mp3");
			hitSound.Volume = 2f;
			hitSound.Load ();

			pshhhSound = SCNAudioSource.FromFile ("game.scnassets/sounds/fire_extinction.mp3");
			pshhhSound.Volume = 2f;
			pshhhSound.Load ();

			aahSound = SCNAudioSource.FromFile ("game.scnassets/sounds/aah_extinction.mp3");
			aahSound.Volume = 2f;
			aahSound.Load ();

			// collectable
			collectPearlSound = SCNAudioSource.FromFile ("game.scnassets/sounds/collect1.mp3");
			collectPearlSound.Volume = 0.5f;
			collectPearlSound.Load ();

			collectFlowerSound = SCNAudioSource.FromFile ("game.scnassets/sounds/collect1.mp3");
			collectFlowerSound.Load ();

			// victory
			victoryMusic = SCNAudioSource.FromFile ("game.scnassets/sounds/Music_victory.mp3");
			victoryMusic.Volume = 0.5f;
		}

		void SetupMusic ()
		{
			// Get an arbitrary node to attach the sounds to.
			SCNNode node = GameView.Scene.RootNode;
			SCNAudioSource source = SCNAudioSource.FromFile ("game.scnassets/sounds/music.m4a");
			source.Loops = true;
			source.Volume = 0.25f;
			source.ShouldStream = true;
			source.Positional = false;

			SCNAudioPlayer player = SCNAudioPlayer.FromSource (source);
			node.AddAudioPlayer (player);
		}

		void SetupAutomaticCameraPositions ()
		{
			SCNNode root = GameView.Scene.RootNode;
			mainGround = root.FindChildNode ("bloc05_collisionMesh_02", true);

			groundToCameraPosition = new Dictionary<SCNNode, SCNVector3> ();

			groundToCameraPosition.Add (root.FindChildNode ("bloc04_collisionMesh_02", true), new SCNVector3 (-0.188683f, 4.719608f, 0f));
			groundToCameraPosition.Add (root.FindChildNode ("bloc03_collisionMesh", true), new SCNVector3 (-0.435909f, 6.297167f, 0f));
			groundToCameraPosition.Add (root.FindChildNode ("bloc07_collisionMesh", true), new SCNVector3 (-0.333663f, 7.868592f, 0f));
			groundToCameraPosition.Add (root.FindChildNode ("bloc08_collisionMesh", true), new SCNVector3 (-0.575011f, 8.739003f, 0f));
			groundToCameraPosition.Add (root.FindChildNode ("bloc06_collisionMesh", true), new SCNVector3 (-1.095519f, 9.425292f, 0f));
			groundToCameraPosition.Add (root.FindChildNode ("bloc05_collisionMesh_02", true), new SCNVector3 (-0.072051f, 8.202264f, 0f));
			groundToCameraPosition.Add (root.FindChildNode ("bloc05_collisionMesh_01", true), new SCNVector3 (-0.072051f, 8.202264f, 0));
		}

		public override void AwakeFromNib ()
		{
			GameView = (GameView)View;
			// Create a new scene.
			SCNScene scene = SCNScene.FromFile ("game.scnassets/level.scn");

			// Set the scene to the view and loops for the animation of the bamboos
			GameView.Scene = scene;
			GameView.Playing = true;
			GameView.Loops = true;

			Character = new Character ();

			SetupCamera ();
			SetupSounds ();
			SetupMusic ();

			// setup particles
			collectParticles = SCNParticleSystem.Create ("collect.scnp", "game.scnassets");
			collectParticles.ParticleImage = PathForArtResource ("textures/glow01.png");
			collectParticles.Loops = false;

			confetti = SCNParticleSystem.Create ("confetti.scnp", "game.scnassets");
			confetti.ParticleImage = PathForArtResource ("textures/confetti.png");

			// Add the character to the scene.
			scene.RootNode.AddChildNode (Character.Node);

			// Place it
			SCNNode sp = scene.RootNode.FindChildNode ("startingPoint", true);
			Character.Node.Transform = sp.Transform;

			// Setup physics masks and physics shape
			SCNNode[] collisionNodes = scene.RootNode.FindNodes ((SCNNode node, out bool stop) => {
				bool? collidable = node.Name?.Contains ("collision");
				stop = false;
				return collidable.HasValue && collidable.Value;
			});

			foreach (SCNNode node in collisionNodes) {
				node.Hidden = false;
				SetupCollisionNodes (node);
			}

			// Setup physics masks and physics shape
			flames = scene.RootNode.FindNodes ((SCNNode node, out bool stop) => {
				stop = false;
				if (node.Name == "flame") {
					node.PhysicsBody.CategoryBitMask = (nuint)(int)Bitmask.Enemy;
					return true;
				}

				return false;
			});

			enemies = scene.RootNode.FindNodes ((SCNNode node, out bool stop) => {
				stop = false;
				return node.Name == "enemy";
			});

			// Setup delegates
			GameView.Scene.PhysicsWorld.ContactDelegate = this;
			GameView.SceneRendererDelegate = this;

			// setup view overlays
			GameView.Setup ();
			SetupAutomaticCameraPositions ();
		}

		void UpdateCameraWithCurrentGround (SCNNode node)
		{
			if (gameIsComplete)
				return;

			if (currentGround == null) {
				currentGround = node;
				return;
			}

			if (node != null && node != currentGround) {
				currentGround = node;

				if (groundToCameraPosition.ContainsKey (node)) {
					var position = groundToCameraPosition [node];

					if (node == mainGround && Character.Node.Position.X < 2.5)
						position = new SCNVector3 (-0.098175f, 3.926991f, 0f);

					SCNAction actionY = SCNAction.RotateTo (0f, position.Y, 0f, 3.0, true);
					actionY.TimingMode = SCNActionTimingMode.EaseInEaseOut;
					SCNAction actionX = SCNAction.RotateTo (position.X, 0f, 0f, 3.0, true);
					actionX.TimingMode = SCNActionTimingMode.EaseInEaseOut;

					cameraXHandle.RunAction (actionY);
					cameraXHandle.RunAction (actionX);
				}
			}
		}

		[Export ("renderer:updateAtTime:")]
		public virtual void Update (ISCNSceneRenderer renderer, double timeInSeconds)
		{
			// delta time since last update
			if (Math.Abs (previousUpdateTime) < float.Epsilon)
				previousUpdateTime = timeInSeconds;

			double deltaTime = Math.Min (Math.Max (1.0 / 60.0, timeInSeconds - previousUpdateTime), 1f);
			previousUpdateTime = timeInSeconds;

			// Reset some states every frame
			maxPenetrationDistance = 0;
			positionNeedsAdjustment = false;

			SCNVector3 direction = GameView.CurrentDirection;
			SCNVector3 initialPosition = Character.Node.Position;

			// Move
			if (Math.Abs (direction.X) > float.Epsilon && Math.Abs (direction.Z) > float.Epsilon) {
				var characterSpeed = (float)deltaTime * CharacterSpeedFactor * .84f;
				Character.Node.Position = new SCNVector3 (
					initialPosition.X + direction.X * characterSpeed,
					initialPosition.Y + direction.Y * characterSpeed,
					initialPosition.Z + direction.Z * characterSpeed
				);

				// update orientation
				double angle = Math.Atan2 (direction.X, direction.Z);
				Character.Direction = (float)angle;
				Character.Walking = true;
			} else {
				Character.Walking = false;
			}

			var p0 = Character.Node.Position;
			var p1 = Character.Node.Position;

			p0.Y -= MaxJump;
			p1.Y += MaxRise;

			var options = new SCNPhysicsTest {
				CollisionBitMask = (nuint)(int)(Bitmask.Collision | Bitmask.Water),
				SearchMode = SCNPhysicsSearchMode.Closest
			};

			SCNHitTestResult[] results = GameView.Scene.PhysicsWorld.RayTestWithSegmentFromPoint (p1, p0, options);
			float groundY = -10;

			if (results.Length > 0) {
				
				SCNHitTestResult result = results [0];
				groundY = result.WorldCoordinates.Y;
				UpdateCameraWithCurrentGround (result.Node);
				SCNMaterial groundMaterial = result.Node.ChildNodes [0].Geometry.FirstMaterial;
				if (grassArea == groundMaterial) {
					Character.CurrentFloorMaterial = FloorMaterial.Grass;
				} else if (waterArea == groundMaterial) {
					if (Character.Burning) {
						Character.Pshhhh ();
						Character.Node.RunAction (SCNAction.Sequence (new [] {
							SCNAction.PlayAudioSource (pshhhSound, true),
							SCNAction.PlayAudioSource (aahSound, false)
						}));
					}

					Character.CurrentFloorMaterial = FloorMaterial.Water;

					options = new SCNPhysicsTest {
						CollisionBitMask = (nuint)(int)Bitmask.Collision,
						SearchMode = SCNPhysicsSearchMode.Closest
					};

					results = GameView.Scene.PhysicsWorld.RayTestWithSegmentFromPoint (p1, p0, options);
					result = results [0];
					groundY = result.WorldCoordinates.Y;
				} else {
					Character.CurrentFloorMaterial = FloorMaterial.Rock;
				}
			}

//			var nextPosition = Character.Node.Position;
//			const double threshold = 1e-5;
//
//			if (groundY < nextPosition.Y - threshold) {
//				// approximation of acceleration for a delta time
//				accelerationY += (float)(deltaTime * GravityAcceleration);
//				if (groundY < nextPosition.Y - 0.2)
//					Character.CurrentFloorMaterial = FloorMaterial.Air;
//			} else {
//				accelerationY = 0;
//			}
//
//			nextPosition.Y -= accelerationY;
//
//			// reset acceleration if we touch the ground
//			if (groundY > nextPosition.Y) {
//				accelerationY = 0;
//				nextPosition.Y = groundY;
//			}

			// Flames are static physics bodies, but they are moved by an action - So we need to tell the physics engine that the transforms did change.
			foreach (SCNNode flame in flames)
				flame.PhysicsBody.ResetTransform ();

			// Adjust the volume of the enemy based on the distance with the character.
			float distanceToClosestEnemy = float.MaxValue;
			SCNVector3 pos3 = Character.Node.Position;
			foreach (SCNNode enemy in enemies) {
				// distance to enemy
				SCNMatrix4 enemyMat = enemy.WorldTransform;
				var enemyPosition = new SCNVector3 (enemyMat.M41, enemyMat.M42, enemyMat.M43);
				float distance = SCNVector3.Subtract (pos3, enemyPosition).Length;
				distanceToClosestEnemy = Math.Min (distanceToClosestEnemy, distance);
			}

			// Adjust sounds volumes based on distance with the enemy.
			if (!gameIsComplete) {
				double fireVolume = 0.3 * Math.Max (0.0, Math.Min (1.0, 1.0 - (distanceToClosestEnemy - 1.2) / 1.6));
				var  mixerNode = flameThrowerSound.AudioNode as AVAudioMixerNode;
				if (mixerNode != null)
					mixerNode.Volume = (float)fireVolume;
			}
		}

		[Export ("physicsWorld:didBeginContact:")]
		public virtual void DidBeginContact (SCNPhysicsWorld world, SCNPhysicsContact contact)
		{
			if (contact.NodeA.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.Collision)
				CharacterNodeHitWallWithContact (contact.NodeB, contact);

			if (contact.NodeB.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.Collision)
				CharacterNodeHitWallWithContact (contact.NodeA, contact);

			if (contact.NodeA.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.Collectable)
				CollectPearl (contact.NodeA);

			if (contact.NodeB.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.Collectable)
				CollectPearl (contact.NodeB);

			if (contact.NodeA.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.SuperCollectable)
				CollectFlower (contact.NodeA);

			if (contact.NodeB.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.SuperCollectable)
				CollectFlower (contact.NodeB);

			if (contact.NodeA.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.Enemy)
				WasHit ();

			if (contact.NodeB.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.Enemy)
				WasHit ();
		}

		[Export ("physicsWorld:didUpdateContact:")]
		public virtual void DidUpdateContact (SCNPhysicsWorld world, SCNPhysicsContact contact)
		{
			if (contact.NodeA?.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.Collision)
				CharacterNodeHitWallWithContact (contact.NodeB, contact);

			if (contact.NodeB?.PhysicsBody.CategoryBitMask == (nuint)(int)Bitmask.Collision)
				CharacterNodeHitWallWithContact (contact.NodeA, contact);
		}

		[Export ("renderer:didSimulatePhysicsAtTime:")]
		public virtual void DidSimulatePhysics (ISCNSceneRenderer renderer, double timeInSeconds)
		{
			// If we hit a wall, position needs to be adjusted
			if (positionNeedsAdjustment)
				Character.Node.Position = replacementPosition;
		}

		void WasHit ()
		{
			if (isInvincible)
				return;

			isInvincible = true;
			Character.Hit ();

			Character.Node.RunAction (SCNAction.Sequence (new [] {
				SCNAction.PlayAudioSource (hitSound, false),
				SCNAction.RepeatAction (SCNAction.Sequence (new [] {
					SCNAction.FadeOpacityTo (.01f, 0.1),
					SCNAction.FadeOpacityTo (1f, 0.1)
				}), 7),
				SCNAction.Run (_ => isInvincible = false)
			}));
		}

		void CharacterNodeHitWallWithContact (SCNNode capsule, SCNPhysicsContact contact)
		{
			if (capsule.ParentNode != Character.Node)
				return;

			if (maxPenetrationDistance > contact.PenetrationDistance)
				return;

			maxPenetrationDistance = contact.PenetrationDistance;

			var charPosition = Character.Node.Position;
			SCNVector3 n = contact.ContactNormal;
			SCNVector3.Multiply (ref n, (float)contact.PenetrationDistance, out n);
			n.Y = 0;

			SCNVector3.Add (ref charPosition, ref n, out replacementPosition);
			positionNeedsAdjustment = true;
		}

		void ShowEndScreen ()
		{
			gameIsComplete = true;

			// Add confettis
			SCNMatrix4 particlePosition = SCNMatrix4.CreateTranslation (0f, 8f, 0f);
			GameView.Scene.AddParticleSystem (confetti, particlePosition);

			// Congratulation title
			SKSpriteNode congrat = SKSpriteNode.FromImageNamed ("Images/congratulations.png");
			congrat.Position = new CGPoint (GameView.Bounds.Width / 2, GameView.Bounds.Height / 2);
			SKScene overlay = GameView.OverlayScene;
			congrat.XScale = congrat.YScale = 0;
			congrat.Alpha = 0;
			congrat.RunAction (SKAction.Group (new [] {
				SKAction.FadeInWithDuration (0.25),
				SKAction.Sequence (new [] {
					SKAction.ScaleTo (.55f, 0.25),
					SKAction.ScaleTo (.3f, 0.1),
				})
			}));

			// Panda Image
			SKSpriteNode congratPanda = SKSpriteNode.FromImageNamed ("Images/congratulations_pandaMax.png");
			congratPanda.Position = new CGPoint (GameView.Bounds.Width / 2f, GameView.Bounds.Height / 2f - 90f);
			congratPanda.AnchorPoint = new CGPoint (.5f, 0f);
			congratPanda.XScale = congratPanda.YScale = 0f;
			congratPanda.Alpha = 0;

			congratPanda.RunAction (SKAction.Sequence (new [] {
				SKAction.WaitForDuration (.5f),
				SKAction.Sequence (new [] {
					SKAction.ScaleTo (.5f, 0.25),
					SKAction.ScaleTo (.4f, 0.1)
				})
			}));

			overlay.AddChild (congratPanda);
			overlay.AddChild (congrat);

			// Stop music
			GameView.Scene.RootNode.RemoveAllAudioPlayers ();

			// Play the congrat sound.
			GameView.Scene.RootNode.AddAudioPlayer (SCNAudioPlayer.FromSource (victoryMusic));

			// Animate the camera forever
			DispatchQueue.MainQueue.DispatchAfter (new DispatchTime (DispatchTime.Now, 1 * NanoSecondsPerSeond), () => {
				cameraYHandle.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (0f, -1f, 0f, 3.0)));
				cameraXHandle.RunAction (SCNAction.RotateTo (-(float)Math.PI / 4f, 0f, 0f, 5.0));
			});
		}

		void CollectPearl (SCNNode node)
		{
			if (node.ParentNode == null)
				return;

			SCNNode soundEmitter = SCNNode.Create ();
			soundEmitter.Position = node.Position;
			node.ParentNode.AddChildNode (soundEmitter);
			soundEmitter.RunAction (SCNAction.Sequence (new [] {
				SCNAction.PlayAudioSource (collectPearlSound, true),
				SCNAction.RemoveFromParentNode ()
			}));

			node.RemoveFromParentNode ();
			GameView.DidCollectAPearl ();
		}

		void CollectFlower (SCNNode node)
		{
			if (node.ParentNode == null)
				return;

			SCNNode soundEmitter = SCNNode.Create ();
			soundEmitter.Position = node.Position;
			node.ParentNode.AddChildNode (soundEmitter);
			soundEmitter.RunAction (SCNAction.Sequence (new [] {
				SCNAction.PlayAudioSource (collectFlowerSound, true),
				SCNAction.RemoveFromParentNode ()
			}));

			node.RemoveFromParentNode ();

			// Check if game is complete.
			bool gameComplete = GameView.DidCollectAFlower ();

			// Edit some particles.
			SCNMatrix4 particlePosition = soundEmitter.WorldTransform;
			particlePosition.M42 += 0.1f;
			GameView.Scene.AddParticleSystem (collectParticles, particlePosition);

			if (gameComplete)
				ShowEndScreen ();
		}

		NSString PathForArtResource (string resourceName)
		{
			return (NSString)string.Format ("{0}/{1}", ArtFolderRoot, resourceName);
		}
	}
}