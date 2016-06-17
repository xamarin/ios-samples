using System;

using CoreAnimation;
using CoreGraphics;
using Foundation;
using SceneKit;
using SpriteKit;
using UIKit;

namespace Fox {
	public enum Direction {
		Up,
		Left,
		Right,
		Down,
		Count
	};

	public partial class GameView : SCNView, IUIGestureRecognizerDelegate {
		const int DPAD_RADIUS = 80;

		CGPoint direction;
		UITouch panningTouch;
		UITouch padTouch;

		CGRect padRect;
		SKSpriteNode[] flowers = new SKSpriteNode[3];
		SKLabelNode pearlLabel;
		SKNode overlayGroup;
		int pearlCount;
		int flowerCount;
		double defaultFov;

		bool directionCacheValid;
		SCNVector3 directionCache;

		public int CollectedFlowers { get; set; }

		public int CollectedPearls { get; set; }

		public SCNVector3 CurrentDirection {
			get {
				if (!directionCacheValid) {
					directionCache = ComputeDirection ();
					directionCacheValid = true;
				}

				return directionCache;
			}
			set {
				directionCache = value;
			}
		}

		[Export ("initWithCoder:")]
		public GameView (NSCoder coder) : base (coder)
		{
		}

		public override bool GestureRecognizerShouldBegin (UIGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.GetType () == typeof (UIPinchGestureRecognizer) && padTouch != null)
				return false;

			return true;
		}

		public void Setup ()
		{
			nfloat w = Bounds.Width;
			nfloat h = Bounds.Height;

			if (w < h) {
				nfloat wTmp = w;
				w = h;
				h = wTmp;
			}

			// Setup the game overlays using SpriteKit
			SKScene skScene = SKScene.FromSize (new CGSize (w, h));
			skScene.ScaleMode = SKSceneScaleMode.ResizeFill;

			overlayGroup = SKNode.Create ();
			skScene.AddChild (overlayGroup);
			overlayGroup.Position = new CGPoint (0f, h);

			// The Max icon
			SKSpriteNode sprite = SKSpriteNode.FromImageNamed ("Images/MaxIcon.png");
			sprite.Position = new CGPoint (50f, -50f);

			overlayGroup.AddChild (sprite);
			sprite.XScale = sprite.YScale = 0.5f;

			for (int i = 0; i < 3; i++) {
				flowers [i] = SKSpriteNode.FromImageNamed ("Images/FlowerEmpty.png");
				flowers [i].Position = new CGPoint (110f + i * 40f, -50f);
				flowers [i].XScale = flowers [i].YScale = 0.25f;
				overlayGroup.AddChild (flowers[i]);
			}

			// The peal icon and count.
			sprite = SKSpriteNode.FromImageNamed ("Images/ItemsPearl.png");
			sprite.Position = new CGPoint (110f, -100f);
			sprite.XScale = sprite.YScale = 0.5f;
			overlayGroup.AddChild (sprite);

			pearlLabel = SKLabelNode.FromFont ("Chalkduster");
			pearlLabel.Text = "x0";
			pearlLabel.Position = new CGPoint (152f, -113f);
			overlayGroup.AddChild (pearlLabel);

			// The D-Pad
			sprite = SKSpriteNode.FromImageNamed ("Images/dpad.png");
			sprite.Position = new CGPoint (100f, 100f);
			sprite.XScale = sprite.YScale = 0.5f;
			skScene.AddChild (sprite);
			padRect = new CGRect (
				(sprite.Position.Y - DPAD_RADIUS) / w,
				1f - (sprite.Position.Y + DPAD_RADIUS) / h,
				2f * DPAD_RADIUS / w,
				2f * DPAD_RADIUS / h
			);

			// Assign the SpriteKit overlay to the SceneKit view.
			OverlayScene = skScene;

			// Setup the pinch gesture
			defaultFov = PointOfView.Camera.XFov;

			var pinch = new UIPinchGestureRecognizer {
				Delegate = this,
				CancelsTouchesInView = false
			};
			pinch.AddTarget (() => PinchWithGestureRecognizer (pinch));

			AddGestureRecognizer (pinch);
		}

		public bool DidCollectAFlower ()
		{
			if (flowerCount < 3)
				flowers [flowerCount].Texture = SKTexture.FromImageNamed ("Images/FlowerFull.png");
			flowerCount++;
			return flowerCount == 3;
		}

		public void DidCollectAPearl ()
		{
			pearlCount++;
			if (pearlCount == 10)
				pearlLabel.Position = new CGPoint (158f, pearlLabel.Position.Y);

			pearlLabel.Text = string.Format ("x{0}", pearlCount);
		}

		bool TouchIsInRect (UITouch touch, CGRect rect)
		{
			rect = CGAffineTransform.CGRectApplyAffineTransform (rect, CGAffineTransform.MakeScale (Bounds.Width, Bounds.Height));
			return rect.Contains (touch.LocationInView (this));
		}

		public override void TouchesBegan (NSSet touches, UIEvent touchEvent)
		{
			foreach (UITouch touch in touches) {
				if (TouchIsInRect (touch, padRect))
					padTouch = padTouch ?? touch;
				else if (panningTouch == null)
					panningTouch = (UITouch)touches.AnyObject;

				if (padTouch != null && panningTouch != null)
					break;
			}
			base.TouchesBegan (touches, touchEvent);
		}

		public override void TouchesMoved (NSSet touches, UIEvent touchEvent)
		{
			directionCacheValid = false;

			if (panningTouch != null) {
				CGPoint p0 = panningTouch.PreviousLocationInView (this);
				CGPoint p1 = panningTouch.LocationInView (this);

				Controller.PanCamera (new CGSize (p1.X - p0.X, p0.Y - p1.Y));
			}

			if (padTouch != null) {
				CGPoint p0 = padTouch.PreviousLocationInView (this);
				CGPoint p1 = padTouch.LocationInView (this);

				const float speed = 1f / 10f;
				const float limit = 1f;

				direction.X += (p1.X - p0.X) * speed;
				direction.Y += (p1.Y - p0.Y) * speed;

				if (direction.X > limit)
					direction.X = limit;

				if (direction.X < -limit)
					direction.X = -limit;

				if (direction.Y > limit)
					direction.Y = limit;

				if (direction.Y < -limit)
					direction.Y = -limit;

				DirectionDidChange ();
			}

			base.TouchesMoved (touches, touchEvent);
		}

		public override void TouchesEnded (NSSet touches, UIEvent touchEvent)
		{
			if (panningTouch != null && touches.Contains (panningTouch))
				panningTouch = null;

			if (padTouch == null)
				return;

			if (touches.Contains (padTouch) || !touchEvent.TouchesForView (this).Contains (padTouch)) {
				padTouch = null;
				direction = new CGPoint (0, 0);
				DirectionDidChange ();
			}
		}

		void PinchWithGestureRecognizer (UIPinchGestureRecognizer recognizer)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
			double fov = defaultFov;
			nfloat constraintFactor = 0;

			if (recognizer.State == UIGestureRecognizerState.Ended || recognizer.State == UIGestureRecognizerState.Cancelled) {
				SCNTransaction.AnimationDuration = 0.5;
			} else {
				SCNTransaction.AnimationDuration = 0.1;
				if (recognizer.Scale > 1) {
					nfloat scale = 1f + (recognizer.Scale - 1f) * 0.75f; //make pinch smoother
					fov *= 1 / scale; //zoom on pinch
					constraintFactor = NMath.Min (1f, (scale - 1f) * 0.75f); //focus on character when pinching
				}
			}

			PointOfView.Camera.XFov = fov;
			PointOfView.Constraints [0].InfluenceFactor = constraintFactor;

			SCNTransaction.Commit ();
		}

		void DirectionDidChange ()
		{
			directionCacheValid = false;
		}

		CGPoint DirectionFromPressedKeys ()
		{
			return direction;
		}

		SCNVector3 ComputeDirection ()
		{
			CGPoint p = DirectionFromPressedKeys ();
			var dir = new SCNVector3 ((float)p.X, 0f, (float)p.Y);
			var p0 = new SCNVector3 (0f, 0f, 0f);

			dir = PointOfView.PresentationNode.ConvertPositionToNode (dir, null);
			p0 = PointOfView.PresentationNode.ConvertPositionToNode (p0, null);

			dir = new SCNVector3 (dir.X - p0.X, 0f, dir.Z - p0.Z);

			if (dir.X != 0 || dir.Z != 0)
				dir.Normalize ();

			return dir;
		}
	}
}