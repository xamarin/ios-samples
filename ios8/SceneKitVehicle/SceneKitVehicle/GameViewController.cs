using System;
using System.Drawing;
using CoreMotion;
using Foundation;
using GameController;
using SceneKit;
using UIKit;
using CoreGraphics;

namespace SceneKitVehicle
{
	public partial class GameViewController : UIViewController
	{
		private SCNNode spotLightNode;
		private SCNNode cameraNode;
		private SCNNode vehicleNode;
		private SCNPhysicsVehicle vehicle;
		private SCNParticleSystem reactor;

		private CMMotionManager motionManager;
		private SCNVector3 accelerometer = SCNVector3.Zero;
		private float orientation;
		private nfloat reactorDefaultBirthRate;
		private float vehicleSteering;

		private float orientationCum = 0;
		private float incrementOrientation = 0.03f;
		private float decrementOrientation = 0.8f;
		private float maxSpeed = 250f;

		private int ticks = 0;
		private int check = 0;
		private int tryVar = 0;

		GameView GameView;

		private bool IsHighEndDevice {
			get {
				return UIScreen.MainScreen.Scale == 2f;
			}
		}

		public GameViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			UIApplication.SharedApplication.SetStatusBarHidden (true, true);

			var scnView = (SCNView)View;
			scnView.BackgroundColor = UIColor.Black;
			scnView.Scene = SetupScene ();
			scnView.Scene.PhysicsWorld.Speed = 4f;

			scnView.OverlayScene = new OverlayScene (scnView.Bounds.Size);
			SetupAccelerometer ();

			scnView.PointOfView = cameraNode;
			scnView.WeakSceneRendererDelegate = this;

			var doubleTap = new UITapGestureRecognizer (HandleDoubleTap);
			doubleTap.NumberOfTapsRequired = 2;
			doubleTap.NumberOfTouchesRequired = 2;
			scnView.GestureRecognizers = new UIGestureRecognizer [] { doubleTap };

			base.ViewDidLoad ();

			GameView = (GameView) View;
		}

		public override void ViewWillDisappear (bool animated)
		{
			motionManager.StopAccelerometerUpdates ();
			motionManager = null;
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Landscape;
		}

		public void AccelerometerDidChange (CMAcceleration acceleration)
		{
			float filteringFactor = 0.5f;
			accelerometer.X = (float)acceleration.X * filteringFactor + accelerometer.X * (1f - filteringFactor);
			accelerometer.Y = (float)acceleration.Y * filteringFactor + accelerometer.Y * (1f - filteringFactor);
			accelerometer.Z = (float)acceleration.Z * filteringFactor + accelerometer.Z * (1f - filteringFactor);

			if (accelerometer.X > 0) {
				orientation = (float)(accelerometer.Y * 1.3f);
			} else {
				orientation = (float)(-accelerometer.Y * 1.3f);
			}
		}

		[Export ("renderer:didSimulatePhysicsAtTime:")]
		public virtual void DidSimulatePhysics (ISCNSceneRenderer renderer, double timeInSeconds)
		{
			float defaultEngineForce = 300.0f;
			float defaultBrakingForce = 3.0f;
			float steeringClamp = 0.6f;
			float cameraDamping = 0.3f;

			GameView scnView = GameView;

			float engineForce = 0;
			float brakingForce = 0;

			var controllers = GCController.Controllers;
			float orientation = this.orientation;

			switch (scnView.TouchesCount) {
			case 1:
				engineForce = defaultEngineForce;
				reactor.BirthRate = reactorDefaultBirthRate;
				break;
			case 2:
				engineForce = -defaultEngineForce;
				reactor.BirthRate = 0;
				break;
			case 3:
				brakingForce = 100;
				reactor.BirthRate = 0;
				break;
			default:
				brakingForce = defaultBrakingForce;
				reactor.BirthRate = 0;
				break;
			}

			if (controllers != null && controllers.Length > 0) {
				GCController controller = controllers [0];
				GCGamepad pad = controller.Gamepad;
				GCControllerDirectionPad dpad = pad.DPad;

				if (dpad.Right.IsPressed) {
					if (orientationCum < 0f)
						orientationCum *= decrementOrientation;

					orientationCum += incrementOrientation;

					if (orientationCum > 1f)
						orientationCum = 1f;
				} else if (dpad.Left.IsPressed) {
					if (orientationCum > 0)
						orientationCum *= decrementOrientation;

					orientationCum -= incrementOrientation;

					if (orientationCum < -1)
						orientationCum = -1;
				} else {
					orientationCum *= decrementOrientation;
				}
			}

			vehicleSteering = -orientation;
			if (orientation == 0)
				vehicleSteering *= 0.9f;
			if (vehicleSteering < -steeringClamp)
				vehicleSteering = -steeringClamp;
			if (vehicleSteering > steeringClamp)
				vehicleSteering = steeringClamp;

			vehicle.SetSteeringAngle (vehicleSteering, 0);
			vehicle.SetSteeringAngle (vehicleSteering, 1);

			vehicle.ApplyEngineForce (engineForce, 2);
			vehicle.ApplyEngineForce (engineForce, 3);

			vehicle.ApplyBrakingForce (brakingForce, 2);
			vehicle.ApplyBrakingForce (brakingForce, 3);

			ReorientCarIfNeeded ();

			SCNNode car = vehicleNode.PresentationNode;
			SCNVector3 carPos = car.Position;
			var targetPos = new SCNVector3 (carPos.X, 30f, carPos.Z + 25f);
			var cameraPos = new SCNVector3 (cameraNode.Position);
			cameraPos = SCNVector3.Add (cameraPos, cameraDamping * (SCNVector3.Subtract (targetPos, cameraPos)));
			cameraNode.Position = cameraPos;

			if (scnView.InCarView) {
				var frontPosition = scnView.PointOfView.PresentationNode.ConvertPositionToNode (new SCNVector3 (0f, 0f, -30f), null);
				spotLightNode.Position = new SCNVector3 (frontPosition.X, 80f, frontPosition.Z);
				spotLightNode.Rotation = new SCNVector4 (1f, 0f, 0f, -(float)Math.PI / 2f);
			} else {
				spotLightNode.Position = new SCNVector3 (carPos.X, 80f, carPos.Z + 30f);
				spotLightNode.Rotation = new SCNVector4 (1f, 0f, 0f, -(float)(Math.PI / 2.8));
			}

			var overlayScene = (OverlayScene)scnView.OverlayScene;
			overlayScene.SpeedNeedle.ZRotation = -(vehicle.SpeedInKilometersPerHour * (float)Math.PI / maxSpeed);
		}

		private void HandleDoubleTap (UITapGestureRecognizer gesture)
		{
			var scnView = (SCNView)View;
			scnView.Scene = SetupScene ();

			scnView.Scene.PhysicsWorld.Speed = 4f;
			scnView.PointOfView = cameraNode;

			((GameView)scnView).TouchesCount = 0;
		}

		private void ReorientCarIfNeeded ()
		{
			var rnd = new Random ();
			SCNNode car = vehicleNode.PresentationNode;
			SCNVector3 carPos = car.Position;

			ticks++;
			if (ticks == 30) {
				SCNMatrix4 t = car.WorldTransform;
				if (t.M22 <= 0.1f) {
					check++;
					if (check == 3) {
						tryVar++;
						if (tryVar == 3) {
							tryVar = 0;
							vehicleNode.Rotation = new SCNVector4 (0f, 0f, 0f, 0f);
							vehicleNode.Position = new SCNVector3 (carPos.X, carPos.Y + 10f, carPos.Z);
							vehicleNode.PhysicsBody.ResetTransform ();
						} else {
							float x = -10f * (rnd.Next () / float.MaxValue - 0.5f);
							float z = -10f * (rnd.Next () / float.MaxValue - 0.5f);
							var pos = new SCNVector3 (x, 0f, z);
							vehicleNode.PhysicsBody.ApplyForce (new SCNVector3 (0f, 300f, 0f), pos, true);
						}

						check = 0;
					}
				} else {
					check = 0;
				}

				ticks = 0;
			}
		}

		private void SetupAccelerometer ()
		{
			motionManager = new CMMotionManager ();

			if (GCController.Controllers != null && GCController.Controllers.Length == 0 && motionManager.AccelerometerAvailable) {
				motionManager.AccelerometerUpdateInterval = 1.0 / 60.0;
				motionManager.StartAccelerometerUpdates (NSOperationQueue.MainQueue, (data, error) => {
					AccelerometerDidChange (data.Acceleration);
				});
			}
		}

		private void AddTrainToScene (SCNScene scene, SCNVector3 position)
		{
			var max = SCNVector3.Zero;
			var min = SCNVector3.Zero;
			var trainScene = SCNScene.FromFile ("train_flat", ResourceManager.ResourceFolder, (NSDictionary)null);

			foreach (var node in trainScene.RootNode.ChildNodes) {
				if (node.Geometry != null) {
					node.Position = new SCNVector3 (
						node.Position.X + position.X,
						node.Position.Y + position.Y,
						node.Position.Z + position.Z
					);

					max = SCNVector3.Zero;
					min = SCNVector3.Zero;

					node.GetBoundingBox (ref min, ref max);

					var body = SCNPhysicsBody.CreateDynamicBody ();
					var boxShape = new SCNBox {
						Width = max.X - min.X,
						Height = max.Y - min.Y,
						Length = max.Z - min.Z,
						ChamferRadius = 0f
					};

					body.PhysicsShape = SCNPhysicsShape.Create (boxShape, (NSDictionary)null);
					node.Pivot = SCNMatrix4.CreateTranslation (0f, -min.Y, 0f);
					node.PhysicsBody = body;
					scene.RootNode.AddChildNode (node);
				}
			}

			var smokeHandle = scene.RootNode.FindChildNode ("Smoke", true);
			var smokeParticleSystem = SCNParticleSystem.Create ("smoke", ResourceManager.ResourceFolder);
			smokeParticleSystem.ParticleImage = ResourceManager.GetResourcePath ("smoke.png");
			smokeHandle.AddParticleSystem (smokeParticleSystem);

			var engineCar = scene.RootNode.FindChildNode ("EngineCar", false);
			var wagon1 = scene.RootNode.FindChildNode ("Wagon1", false);
			var wagon2 = scene.RootNode.FindChildNode ("Wagon2", false);

			max = SCNVector3.Zero;
			min = SCNVector3.Zero;
			engineCar.GetBoundingBox (ref min, ref max);

			var wmax = SCNVector3.Zero;
			var wmin = SCNVector3.Zero;
			wagon1.GetBoundingBox (ref wmin, ref wmax);

			var anchorA = new SCNVector3 (max.X, min.Y, 0f);
			var anchorB = new SCNVector3 (wmin.X, wmin.Y, 0f);

			var joint = SCNPhysicsBallSocketJoint.Create (engineCar.PhysicsBody, anchorA, wagon1.PhysicsBody, anchorB);

			scene.PhysicsWorld.AddBehavior (joint);

			joint = SCNPhysicsBallSocketJoint.Create (wagon1.PhysicsBody,
				new SCNVector3 (wmax.X + 0.1f, wmin.Y, 0f),
				wagon2.PhysicsBody,
				new SCNVector3 (wmin.X - 0.1f, wmin.Y, 0f)
			);

			scene.PhysicsWorld.AddBehavior (joint);
		}

		private void AddWoodenBlockToScene (SCNScene scene, NSString imageName, SCNVector3 position)
		{
			var block = SCNNode.Create ();
			block.Position = position;
			block.Geometry = new SCNBox {
				Width = 5f,
				Height = 5f,
				Length = 5f,
				ChamferRadius = 0f
			};

			block.Geometry.FirstMaterial.Diffuse.Contents = imageName;
			block.Geometry.FirstMaterial.Diffuse.MipFilter = SCNFilterMode.Linear;
			block.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
			scene.RootNode.AddChildNode (block);
		}

		private void SetupSceneElements (SCNScene scene)
		{
			AddTrainToScene (scene, new SCNVector3 (-5f, 20f, -40f));
			AddWoodenBlockToScene (scene, ResourceManager.GetResourcePath ("WoodCubeA.jpg"), new SCNVector3 (-10f, 15f, 10f));
			AddWoodenBlockToScene (scene, ResourceManager.GetResourcePath ("WoodCubeB.jpg"), new SCNVector3 (-9f, 10f, 10f));
			AddWoodenBlockToScene (scene, ResourceManager.GetResourcePath ("WoodCubeC.jpg"), new SCNVector3 (20f, 15f, -11f));
			AddWoodenBlockToScene (scene, ResourceManager.GetResourcePath ("WoodCubeA.jpg"), new SCNVector3 (25f, 5f, -20f));

			var wallBox = new SCNBox {
				Width = 400f,
				Height = 100f,
				Length = 4f,
				ChamferRadius = 0f
			};

			var wall = SCNNode.FromGeometry (wallBox);
			wall.Geometry.FirstMaterial.Diffuse.Contents = ResourceManager.GetResourcePath ("wall.jpg");
			wall.Geometry.FirstMaterial.Diffuse.ContentsTransform = SCNMatrix4.Mult (
				SCNMatrix4.Scale (24f, 2f, 1f),
				SCNMatrix4.CreateTranslation (0f, 1f, 0f)
			);

			wall.Geometry.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
			wall.Geometry.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Mirror;
			wall.Geometry.FirstMaterial.DoubleSided = false;
			wall.CastsShadow = false;
			wall.Geometry.FirstMaterial.LocksAmbientWithDiffuse = true;

			wall.Position = new SCNVector3 (0f, 50f, -92f);
			wall.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
			scene.RootNode.AddChildNode (wall);

			wall = wall.Clone ();
			wall.Position = new SCNVector3 (202f, 50f, 0f);
			wall.Rotation = new SCNVector4 (0f, 1f, 0f, (float)Math.PI / 2f);
			scene.RootNode.AddChildNode (wall);

			wall = wall.Clone ();
			wall.Position = new SCNVector3 (-202f, 50f, 0f);
			wall.Rotation = new SCNVector4 (0f, 1f, 0f, -(float)Math.PI / 2f);
			scene.RootNode.AddChildNode (wall);

			var planeGeometry = new SCNPlane {
				Width = 400f,
				Height = 100f
			};

			var backWall = SCNNode.FromGeometry (planeGeometry);
			backWall.Geometry.FirstMaterial = wall.Geometry.FirstMaterial;
			backWall.Position = new SCNVector3 (0f, 50f, 200f);
			backWall.Rotation = new SCNVector4 (0f, 1f, 0f, (float)Math.PI);
			backWall.CastsShadow = false;
			backWall.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
			scene.RootNode.AddChildNode (backWall);

			planeGeometry = new SCNPlane {
				Width = 400f,
				Height = 400f
			};

			var ceilNode = SCNNode.FromGeometry (planeGeometry);
			ceilNode.Position = new SCNVector3 (0f, 100f, 0f);
			ceilNode.Rotation = new SCNVector4 (1f, 0f, 0f, (float)Math.PI / 2f);
			ceilNode.Geometry.FirstMaterial.DoubleSided = false;
			ceilNode.CastsShadow = false;
			ceilNode.Geometry.FirstMaterial.LocksAmbientWithDiffuse = true;
			scene.RootNode.AddChildNode (ceilNode);

			var rnd = new Random ();
			for (int i = 0; i < 4; i++) {
				AddWoodenBlockToScene (scene, ResourceManager.GetResourcePath ("WoodCubeA.jpg"),
					new SCNVector3 (rnd.Next (0, 60) - 30f, 20f, rnd.Next (0, 40) - 20f));
				AddWoodenBlockToScene (scene, ResourceManager.GetResourcePath ("WoodCubeB.jpg"),
					new SCNVector3 (rnd.Next (0, 60) - 30f, 20f, rnd.Next (0, 40) - 20f));
				AddWoodenBlockToScene (scene, ResourceManager.GetResourcePath ("WoodCubeC.jpg"),
					new SCNVector3 (rnd.Next (0, 60) - 30f, 20f, rnd.Next (0, 40) - 20f));
			}

			var blockBox = new SCNBox {
				Width = 22f,
				Height = 0.2f,
				Length = 34f,
				ChamferRadius = 0f
			};

			var block = SCNNode.Create ();
			block.Position = new SCNVector3 (20f, 10f, -16f);
			block.Rotation = new SCNVector4 (0f, 1f, 0f, (float)-Math.PI / 4f);
			block.Geometry = blockBox;

			var frontMat = SCNMaterial.Create ();
			frontMat.LocksAmbientWithDiffuse = true;
			frontMat.Diffuse.Contents = ResourceManager.GetResourcePath ("book_front.jpg");
			frontMat.Diffuse.MipFilter = SCNFilterMode.Linear;

			var backMat = SCNMaterial.Create ();
			backMat.LocksAmbientWithDiffuse = true;
			backMat.Diffuse.Contents = ResourceManager.GetResourcePath ("book_back.jpg");
			backMat.Diffuse.MipFilter = SCNFilterMode.Linear;

			block.Geometry.Materials = new SCNMaterial[] { frontMat, backMat };
			block.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
			scene.RootNode.AddChildNode (block);

			var rug = SCNNode.Create ();
			rug.Position = new SCNVector3 (0f, 0.01f, 0f);
			rug.Rotation = new SCNVector4 (1f, 0f, 0f, (float)Math.PI / 2f);
			var path = UIBezierPath.FromRoundedRect (new CGRect (-50f, -30f, 100f, 50f), 2.5f);
			path.Flatness = 0.1f;
			rug.Geometry = SCNShape.Create (path, 0.05f);
			rug.Geometry.FirstMaterial.LocksAmbientWithDiffuse = true;
			rug.Geometry.FirstMaterial.Diffuse.Contents = ResourceManager.GetResourcePath ("carpet.jpg");
			scene.RootNode.AddChildNode (rug);

			var ball = SCNNode.Create ();
			ball.Position = new SCNVector3 (-5f, 5f, -18f);
			ball.Geometry = SCNSphere.Create (5f);
			ball.Geometry.FirstMaterial.LocksAmbientWithDiffuse = true;
			ball.Geometry.FirstMaterial.Diffuse.Contents = ResourceManager.GetResourcePath ("ball.jpg");
			ball.Geometry.FirstMaterial.Diffuse.ContentsTransform = SCNMatrix4.Scale (2f, 1f, 1f);
			ball.Geometry.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Mirror;
			ball.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
			ball.PhysicsBody.Restitution = 0.9f;
			scene.RootNode.AddChildNode (ball);
		}

		private SCNNode SetupVehicle (SCNScene scene)
		{
			var carScene = SCNScene.FromFile ("rc_car", ResourceManager.ResourceFolder, (NSDictionary)null);
			var chassisNode = carScene.RootNode.FindChildNode ("rccarBody", false);

			chassisNode.Position = new SCNVector3 (0f, 10f, 30f);
			chassisNode.Rotation = new SCNVector4 (0f, 1f, 0f, (float)Math.PI);

			var body = SCNPhysicsBody.CreateDynamicBody ();
			body.AllowsResting = false;
			body.Mass = 80f;
			body.Restitution = 0.1f;
			body.Friction = 0.5f;
			body.RollingFriction = 0f;

			chassisNode.PhysicsBody = body;

			var frontCameraNode = SCNNode.Create ();
			frontCameraNode.Position = new SCNVector3 (0f, 3.5f, 2.5f);
			frontCameraNode.Rotation = new SCNVector4 (0f, 1f, 0f, (float)Math.PI);
			frontCameraNode.Camera = SCNCamera.Create ();
			frontCameraNode.Camera.XFov = 75f;
			frontCameraNode.Camera.ZFar = 500f;

			chassisNode.AddChildNode (frontCameraNode);

			scene.RootNode.AddChildNode (chassisNode);

			var pipeNode = chassisNode.FindChildNode ("pipe", true);
			reactor = SCNParticleSystem.Create ("reactor", ResourceManager.ResourceFolder);
			reactor.ParticleImage = ResourceManager.GetResourcePath ("spark.png");
			reactorDefaultBirthRate = reactor.BirthRate;
			reactor.BirthRate = 0;
			pipeNode.AddParticleSystem (reactor);

			SCNNode wheel0Node = chassisNode.FindChildNode ("wheelLocator_FL", true);
			SCNNode wheel1Node = chassisNode.FindChildNode ("wheelLocator_FR", true);
			SCNNode wheel2Node = chassisNode.FindChildNode ("wheelLocator_RL", true);
			SCNNode wheel3Node = chassisNode.FindChildNode ("wheelLocator_RR", true);

			var wheel0 = SCNPhysicsVehicleWheel.Create (wheel0Node);
			var wheel1 = SCNPhysicsVehicleWheel.Create (wheel1Node);
			var wheel2 = SCNPhysicsVehicleWheel.Create (wheel2Node);
			var wheel3 = SCNPhysicsVehicleWheel.Create (wheel3Node);

			var min = SCNVector3.Zero;
			var max = SCNVector3.Zero;

			wheel0Node.GetBoundingBox (ref min, ref max);
			float wheelHalfWidth = 0.5f * (max.X - min.X);

			wheel0.ConnectionPosition = SCNVector3.Add (wheel0Node.ConvertPositionToNode (SCNVector3.Zero, chassisNode), new SCNVector3 (wheelHalfWidth, 0f, 0f));
			wheel1.ConnectionPosition = SCNVector3.Subtract (wheel1Node.ConvertPositionToNode (SCNVector3.Zero, chassisNode), new SCNVector3 (wheelHalfWidth, 0f, 0f));
			wheel2.ConnectionPosition = SCNVector3.Add (wheel2Node.ConvertPositionToNode (SCNVector3.Zero, chassisNode), new SCNVector3 (wheelHalfWidth, 0f, 0f));
			wheel3.ConnectionPosition = SCNVector3.Subtract (wheel3Node.ConvertPositionToNode (SCNVector3.Zero, chassisNode), new SCNVector3 (wheelHalfWidth, 0f, 0f));

			var vehicle = SCNPhysicsVehicle.Create (chassisNode.PhysicsBody,
				              new SCNPhysicsVehicleWheel[] { wheel0, wheel1, wheel2, wheel3 });
			scene.PhysicsWorld.AddBehavior (vehicle);
			this.vehicle = vehicle;

			return chassisNode;
		}

		private SCNScene SetupScene ()
		{
			var scene = SCNScene.Create ();

			SetupInvironment (scene);
			SetupSceneElements (scene);
			vehicleNode = SetupVehicle (scene);

			cameraNode = SCNNode.Create ();
			cameraNode.Camera = SCNCamera.Create ();
			cameraNode.Camera.ZFar = 500f;
			cameraNode.Position = new SCNVector3 (0f, 60f, 50f);
			cameraNode.Rotation = new SCNVector4 (1f, 0f, 0f, -(float)Math.PI / 4f * 0.75f);
			scene.RootNode.AddChildNode (cameraNode);

			return scene;
		}

		private void SetupInvironment (SCNScene scene)
		{
			var ambientLight = SCNNode.Create ();
			ambientLight.Light = new SCNLight {
				LightType = SCNLightType.Ambient,
				Color = UIColor.FromWhiteAlpha (0.3f, 1f)
			};
			scene.RootNode.AddChildNode (ambientLight);

			var lightNode = new SCNNode {
				Position = new SCNVector3 (0f, 80f, 30f),
				Rotation = new SCNVector4 (1f, 0f, 0f, (float)(-Math.PI / 2.8))
			};

			lightNode.Light = new SCNLight {
				LightType = SCNLightType.Spot,
				Color = UIColor.FromWhiteAlpha (0.8f, 1f),
				SpotInnerAngle = 0f,
				SpotOuterAngle = 50f,
				ShadowColor = UIColor.Black,
				ZFar = 500f,
				ZNear = 50f
			};

			if (IsHighEndDevice)
				ambientLight.Light.CastsShadow = true;

			scene.RootNode.AddChildNode (lightNode);

			spotLightNode = lightNode;

			var floor = SCNNode.Create ();
			floor.Geometry = new SCNFloor ();
			floor.Geometry.FirstMaterial.Diffuse.Contents = ResourceManager.GetResourcePath ("wood.png");
			floor.Geometry.FirstMaterial.Diffuse.ContentsTransform = SCNMatrix4.Scale (2f, 2f, 1f);
			floor.Geometry.FirstMaterial.LocksAmbientWithDiffuse = true;

			if (IsHighEndDevice)
				((SCNFloor)floor.Geometry).ReflectionFalloffEnd = 10f;

			var staticBody = SCNPhysicsBody.CreateStaticBody ();
			floor.PhysicsBody = staticBody;
			scene.RootNode.AddChildNode (floor);
		}
	}
}
