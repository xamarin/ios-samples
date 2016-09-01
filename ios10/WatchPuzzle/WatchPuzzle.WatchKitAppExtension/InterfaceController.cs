using System;

using UIKit;
using WatchKit;
using Foundation;
using SceneKit;
using SpriteKit;
using OpenTK;
using CoreGraphics;

namespace WatchPuzzle.WatchKitAppExtension
{
	public static class GameColors
	{
		public static readonly UIColor DefaultFont = new UIColor (31f / 255, 226f / 255, 63f / 255, 1);
		public static readonly UIColor Warning = UIColor.Orange;
		public static readonly UIColor Danger = UIColor.Red;
	}

	// A struct containing all the `SCNNode`s used in the game.
	public struct GameNodes
	{
		public readonly SCNNode Object;
		public readonly SCNMaterial ObjectMaterial;
		public readonly SCNNode Confetti;
		public readonly SCNCamera Camera;
		public readonly SKLabelNode CountdownLabel;
		public readonly SKLabelNode CongratulationsLabel;

		// Queries the root node for the expected nodes.
		internal GameNodes (SCNNode sceneRoot)
		{
			Object = sceneRoot.FindChildNode ("teapot", true);
			if (Object == null)
				throw new InvalidProgramException ();

			ObjectMaterial = Object.Geometry?.FirstMaterial;
			if (ObjectMaterial == null)
				throw new InvalidProgramException ();

			Confetti = sceneRoot.FindChildNode ("particles", true);
			if (Confetti == null)
				throw new InvalidProgramException ();

			Camera = sceneRoot.FindChildNode ("camera", true).Camera;
			if (Camera == null)
				throw new InvalidProgramException ();

			CountdownLabel = new SKLabelNode ("Chalkduster") {
				HorizontalAlignmentMode = SKLabelHorizontalAlignmentMode.Center
			};

			CongratulationsLabel = new SKLabelNode ("Chalkduster") {
				FontColor = GameColors.DefaultFont,
				Text = "You Win!",
				FontSize = 45
			};
		}
	}

	public partial class InterfaceController : WKInterfaceController
	{
		[Outlet ("sceneInterface")]
		WKInterfaceSCNScene sceneInterface { get; set; }

		GameNodes? gameNodes;
		bool gameStarted;
		SCNMatrix4 initialObject3DRotation = SCNMatrix4.Identity;
		Vector3 initialSphereLocation = new Vector3 ();
		int countdown;
		NSTimer textUpdateTimer;
		NSTimer particleRemovalTimer;

		protected InterfaceController (IntPtr handle) : base (handle)
		{
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);
			SetupGame ();
		}

		public override void WillActivate ()
		{
			// Start the game if not already started.
			if (!gameStarted)
				StartGame ();

			base.WillActivate ();
		}

		[Action ("handleTapWithSender:")]
		void HandleTap (NSObject sender)
		{
			var tapGesture = sender as WKTapGestureRecognizer;
			if (sender == null)
				return;

			// Restart the game on single tap only if presenting congratulation screen.
			if (tapGesture.NumberOfTapsRequired == 1 && !gameStarted)
				StartGame ();
		}

		#region Gesture reconginzer handling

		// Handle rotation of the 3D object by computing rotations of a virtual
		// trackball using the pan gesture touch locations.
		// On state ended, end the game if the object has the right orientation.

		[Action ("handlePanWithPanGesture:")]
		void HandlePan (WKPanGestureRecognizer panGesture)
		{
			if (!gameNodes.HasValue || !gameStarted)
				return;

			var gNodes = gameNodes.Value;
			var location = panGesture.LocationInObject;
			var bounds = panGesture.ObjectBounds;

			// Compute the projection of the interface point to the virtual trackball.
			var sphereLocation = SphereProjection (location, bounds);

			switch (panGesture.State) {
			case WKGestureRecognizerState.Began:
				// Record initial states.
				initialSphereLocation = sphereLocation;
				initialObject3DRotation = gNodes.Object.Transform;
				break;

			case WKGestureRecognizerState.Cancelled:
			case WKGestureRecognizerState.Ended:
			case WKGestureRecognizerState.Changed:
				// Compute the rotation and apply to the object.
				var currentRotation = RotationFromPoint (initialSphereLocation, sphereLocation);
				gNodes.Object.Transform = SCNMatrix4.Mult (initialObject3DRotation, currentRotation);
				break;


			default:
				Console.WriteLine ($"Unhandled gesture state: {panGesture.State}");
				throw new InvalidProgramException ();
			}

			// End the game if the object has the initial orientation.
			if (panGesture.State == WKGestureRecognizerState.Ended)
				EndGameOnCorrectOrientation ();
		}

		#endregion

		#region Game flow

		// Setup overlays and lookup scene objects.
		void SetupGame ()
		{
			var sceneRoot = sceneInterface.Scene?.RootNode;
			if (sceneRoot == null)
				throw new InvalidProgramException ();

			gameNodes = new GameNodes (sceneRoot);
			var gNodes = gameNodes.Value;

			gNodes.Object.Transform = SCNMatrix4.Identity;
			gNodes.ObjectMaterial.Transparency = 0;
			gNodes.Confetti.Hidden = true;

			var skScene = new SKScene (ContentFrame.Size) {
				ScaleMode = SKSceneScaleMode.ResizeFill
			};
			skScene.AddChild (gNodes.CountdownLabel);

			sceneInterface.OverlayScene = skScene;
		}

		void StartGame ()
		{
			if (!gameNodes.HasValue)
				throw new InvalidProgramException ("Nodes not set");

			var startSequence = SCNAction.Sequence (new SCNAction [] {
					// Wait for 1 second.
					SCNAction.Wait(1),
					SCNAction.Group(new SCNAction[]{
						SCNAction.FadeIn(0.3),

						// Start the game.
						SCNAction.Run(node => {
							if(!gameNodes.HasValue)
								return;
							var gNodes = gameNodes.Value;

							var rnd = new Random();

							// Compute a random orientation for the object3D.
							var theta = (float)(Math.PI * rnd.NextDouble());
							var phi = (float) (Math.Acos(2 * rnd.NextDouble() - 1) / NMath.PI);

							var axis = new Vector3 {
								X = (float)(Math.Cos(theta)*Math.Sin(phi)),
								Y = (float)(Math.Sin(theta) * Math.Sin(phi)),
								Z = (float)Math.Cos(theta)
							};
							var angle = (float)(2 * Math.PI * rnd.NextDouble());

							SCNTransaction.Begin ();
							SCNTransaction.AnimationDuration = 0.3;
							SCNTransaction.SetCompletionBlock(() => gameStarted = true);

							gNodes.ObjectMaterial.Transparency = 1;
							gNodes.Object.Transform = SCNMatrix4.CreateFromAxisAngle(axis, angle);

							SCNTransaction.Commit ();
						})
					})
				});
			gameNodes.Value.Object.RunAction (startSequence);

			// Load and set the background image.
			var backgroundImage = UIImage.FromBundle ("art.scnassets/background.png");
			sceneInterface.Scene.Background.Contents = backgroundImage;

			// Hide particles, set camera projection to orthographic.
			particleRemovalTimer?.Invalidate ();
			gameNodes.Value.CongratulationsLabel.RemoveFromParent ();
			gameNodes.Value.Confetti.Hidden = true;
			gameNodes.Value.Camera.UsesOrthographicProjection = true;


			// Reset the countdown.
			countdown = 30;
			gameNodes.Value.CountdownLabel.Text = countdown.ToString ();
			gameNodes.Value.CountdownLabel.FontColor = GameColors.DefaultFont;
			gameNodes.Value.CountdownLabel.Position = new CGPoint (ContentFrame.Width / 2, ContentFrame.Height - 30);

			textUpdateTimer?.Invalidate ();
			textUpdateTimer = NSTimer.CreateRepeatingScheduledTimer (1, UpdateText);
		}

		// Update countdown timer.
		void UpdateText (NSTimer timer)
		{
			var gNodes = gameNodes.Value;
			gNodes.CountdownLabel.Text = countdown.ToString ();
			sceneInterface.Playing = true;
			sceneInterface.Playing = false;
			countdown -= 1;

			if (countdown < 0) {
				gNodes.CountdownLabel.FontColor = GameColors.Danger;
				textUpdateTimer?.Invalidate ();
				return;
			}

			if (countdown < 10) {
				gNodes.CountdownLabel.FontColor = GameColors.Warning;
			}
		}

		// End the game by showing the congratulation screen after fading the object to white.
		void EndGame ()
		{
			var gNodes = gameNodes.Value;

			textUpdateTimer?.Invalidate ();
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.5;
			SCNTransaction.SetCompletionBlock (() => {
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.3;
				SCNTransaction.SetCompletionBlock (() => {
					ShowCongratulation ();
					gNodes.ObjectMaterial.Emission.Contents = UIColor.Black;
					gameStarted = false;
				});
				SCNTransaction.Commit ();

			});

			gNodes.Object.Transform = SCNMatrix4.Identity;
			gNodes.ObjectMaterial.Emission.Contents = UIColor.White;
			gNodes.ObjectMaterial.Transparency = 0;

			SCNTransaction.Commit ();
		}

		#endregion

		#region Utils

		Vector3 SphereProjection (CGPoint location, CGRect bounds)
		{
			var screenLocation = ScreenProjection (location, bounds);
			return SphereProjection (screenLocation);
		}

		// Compute projection from object interface to virtual screen on the range [-1, 1].
		CGPoint ScreenProjection (CGPoint location, CGRect bounds)
		{
			var w = bounds.Width;
			var h = bounds.Height;
			var aspectRatioCorrection = (h - w) / 2;
			var screenCoord = new CGPoint (location.X / w * 2 - 1,
										   (h - location.Y - aspectRatioCorrection) / w * 2 - 1);
			screenCoord.X = NMath.Min (1, NMath.Max (-1, screenCoord.X));
			screenCoord.Y = NMath.Min (1, NMath.Max (-1, screenCoord.Y));
			return screenCoord;
		}

		// Compute projection of virtual screen point to unit sphere.
		Vector3 SphereProjection (CGPoint location)
		{
			var sphereCoord = new Vector3 ();
			var squaredLenght = location.X * location.X + location.Y * location.Y;

			if (squaredLenght <= 1) {
				sphereCoord.X = (float)location.X;
				sphereCoord.Y = (float)location.Y;
				sphereCoord.Z = (float)NMath.Sqrt (1 - squaredLenght);
			} else {
				var n = 1f / NMath.Sqrt (squaredLenght);
				sphereCoord.X = (float)(n * location.X);
				sphereCoord.Y = (float)(n * location.Y);
				sphereCoord.Z = 0;
			}

			return sphereCoord;
		}

		// Compute the rotation matrix from one point to another on a unit sphere.
		SCNMatrix4 RotationFromPoint (Vector3 start, Vector3 end)
		{
			var axis = Vector3.Cross (start, end);
			var angle = NMath.Atan2 (axis.Length, Vector3.Dot (start, end));

			return SCNMatrix4.CreateFromAxisAngle (axis, (float)angle);
		}

		// End the game if the object has its initial orientation with a 10 degree tolerance.
		void EndGameOnCorrectOrientation ()
		{
			if (!gameNodes.HasValue || !gameStarted)
				return;

			var gNodes = gameNodes.Value;

			var transform = gNodes.Object.Transform;
			var unitX = new SCNVector4 (1, 0, 0, 0);
			var unitY = new SCNVector4 (0, 1, 0, 0);

			var tX = SCNVector4.Transform (unitX, transform);
			var tY = SCNVector4.Transform (unitY, transform);

			float toleranceDegree = 10;
			var max_cos_angle = Math.Cos (toleranceDegree * Math.PI / 180);
			var cos_angleX = SCNVector4.Dot (unitX, tX);
			var cos_angleY = SCNVector4.Dot (unitY, tY);

			if (cos_angleX >= max_cos_angle && cos_angleY >= max_cos_angle)
				EndGame ();
		}

		// Show the congratulation screen.
		void ShowCongratulation ()
		{
			var gNodes = gameNodes.Value;
			gNodes.Camera.UsesOrthographicProjection = false;

			sceneInterface.Scene.Background.Contents = UIColor.Black;

			gNodes.Confetti.Hidden = false;
			particleRemovalTimer?.Invalidate ();
			particleRemovalTimer = NSTimer.CreateScheduledTimer (30, RemoveParticles);

			gNodes.CongratulationsLabel.RemoveFromParent ();
			gNodes.CongratulationsLabel.Position = new CGPoint (ContentFrame.Width / 2, ContentFrame.Height / 2);
			gNodes.CongratulationsLabel.XScale = 0;
			gNodes.CongratulationsLabel.YScale = 0;
			gNodes.CongratulationsLabel.Alpha = 0;

			gNodes.CongratulationsLabel.RunAction (SKAction.Group (new SKAction []{
					SKAction.FadeInWithDuration (0.25),
					SKAction.Sequence (new SKAction []{
						SKAction.ScaleTo (0.7f, 0.25),
						SKAction.ScaleTo (0.8f, 0.2)
					})
				}));

			sceneInterface.OverlayScene.AddChild (gNodes.CongratulationsLabel);
		}

		void RemoveParticles (NSTimer timer)
		{
			gameNodes.Value.Confetti.Hidden = false;
		}

		#endregion

	}
}
