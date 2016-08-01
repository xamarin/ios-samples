using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;

using static UIKit.UIViewAutoresizing;
using static UIKit.UIGestureRecognizerState;
using static SpeedSketch.Helpers;

namespace SpeedSketch
{
	public class CanvasMainViewController : UIViewController, IUIScrollViewDelegate, IUIGestureRecognizerDelegate
	{
		readonly List<UIButton> buttons = new List<UIButton> ();
		readonly List<NSObject> observers = new List<NSObject> ();

		StrokeCGView cgView;
		RingControl leftRingControl;

		StrokeGestureRecognizer fingerStrokeRecognizer;
		StrokeGestureRecognizer pencilStrokeRecognizer;

		UIButton clearButton;
		UIButton pencilButton;

		Action [] configurations;

		StrokeCollection strokeCollection = new StrokeCollection ();

		UIScrollView scrollView;
		CanvasContainerView canvasContainerView;

		// Since usage of the Apple Pencil can be very temporary, the best way to
		// actually check for it being in use is to remember the last interaction.
		// Also make sure to provide an escape hatch if you modify your UI for
		// times when the pencil is in use vs. not.

		// Timeout the pencil mode if no pencil has been seen for 5 minutes
		// and the app is brought back in foreground.
		double pencilResetInterval = 5 * 60;

		double? lastSeenPencilInteraction;
		double? LastSeenPencilInteraction {
			get {
				return lastSeenPencilInteraction;
			}
			set {
				lastSeenPencilInteraction = value;
				if (lastSeenPencilInteraction.HasValue && !PencilMode)
					PencilMode = true;
			}
		}

		bool pencilMode;
		bool PencilMode {
			get {
				return pencilMode;
			}
			set {
				if (pencilMode = value) {
					scrollView.PanGestureRecognizer.MinimumNumberOfTouches = 1;
					pencilButton.Hidden = false;
					var view = fingerStrokeRecognizer.View;
					if (view != null)
						view.RemoveGestureRecognizer (fingerStrokeRecognizer);
				} else {
					scrollView.PanGestureRecognizer.MinimumNumberOfTouches = 2;
					pencilButton.Hidden = true;
					if (fingerStrokeRecognizer.View == null)
						scrollView.AddGestureRecognizer (fingerStrokeRecognizer);
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var bounds = View.Bounds;
			var screenBounds = UIScreen.MainScreen.Bounds;
			var maxScreenDimension = NMath.Max (screenBounds.Width, screenBounds.Height);

			UIViewAutoresizing flexibleDimensions = FlexibleWidth | FlexibleHeight;

			scrollView = new UIScrollView (bounds) {
				AutoresizingMask = flexibleDimensions
			};
			View.AddSubview (scrollView);

			var frame = new CGRect (CGPoint.Empty, new CGSize (maxScreenDimension, maxScreenDimension));
			cgView = new StrokeCGView (frame) {
				AutoresizingMask = flexibleDimensions
			};

			View.BackgroundColor = UIColor.White;

			canvasContainerView = CanvasContainerView.FromCanvasSize (cgView.Frame.Size);
			canvasContainerView.DocumentView = cgView;
			scrollView.ContentSize = canvasContainerView.Frame.Size;
			scrollView.ContentOffset = new CGPoint ((canvasContainerView.Frame.Width - scrollView.Bounds.Width) / 2,
													(canvasContainerView.Frame.Height - scrollView.Bounds.Height) / 2);
			scrollView.AddSubview (canvasContainerView);
			scrollView.BackgroundColor = canvasContainerView.BackgroundColor;
			scrollView.MaximumZoomScale = 3;
			scrollView.MinimumZoomScale = 0.5f;
			scrollView.PanGestureRecognizer.AllowedTouchTypes = TouchTypes (UITouchType.Direct);
			scrollView.PinchGestureRecognizer.AllowedTouchTypes = TouchTypes (UITouchType.Direct);

			scrollView.Delegate = this;

			// We put our UI elements on top of the scroll view, so we don't want any of the
			// delay or cancel machinery in place.
			scrollView.DelaysContentTouches = false;

			fingerStrokeRecognizer = new StrokeGestureRecognizer (StrokeUpdated) {
				Delegate = this,
				CancelsTouchesInView = false,
				IsForPencil = false,
				CoordinateSpaceView = cgView
			};
			scrollView.AddGestureRecognizer (fingerStrokeRecognizer);

			pencilStrokeRecognizer = new StrokeGestureRecognizer (StrokeUpdated) {
				Delegate = this,
				CancelsTouchesInView = false,
				CoordinateSpaceView = cgView,
				IsForPencil = true
			};
			scrollView.AddGestureRecognizer (pencilStrokeRecognizer);

			SetupConfigurations ();

			var onPhone = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;

			var ringDiameter = onPhone ? 66f : 74f;
			var ringImageInset = onPhone ? 12f : 14f;
			var borderWidth = 1f;
			var ringOutset = ringDiameter / 2 - (NMath.Floor (NMath.Sqrt ((ringDiameter * ringDiameter) / 8) - borderWidth));
			var ringFrame = new CGRect (-ringOutset, View.Bounds.Height - ringDiameter + ringOutset, ringDiameter, ringDiameter);
			var ringControl = new RingControl (ringFrame, configurations.Length);
			ringControl.AutoresizingMask = FlexibleRightMargin | FlexibleTopMargin;
			View.AddSubview (ringControl);
			leftRingControl = ringControl;
			string [] imageNames = { "Calligraphy", "Ink", "Debug" };
			for (int index = 0; index < leftRingControl.RingViews.Count; index++) {
				var ringView = leftRingControl.RingViews [index];
				ringView.ActionClosure = configurations [index];
				var imageView = new UIImageView (ringView.Bounds.Inset (ringImageInset, ringImageInset));
				imageView.Image = UIImage.FromBundle (imageNames [index]);
				imageView.AutoresizingMask = FlexibleLeftMargin | FlexibleRightMargin | FlexibleTopMargin | FlexibleBottomMargin;
				ringView.AddSubview (imageView);
			}

			clearButton = AddButton ("clear", ClearButtonAction);
			SetupPencilUI ();
		}

		UIButton AddButton (string title, EventHandler handler)
		{
			var bounds = View.Bounds;
			var lastButton = buttons.LastOrDefault ();
			var maxX = (lastButton != null) ? lastButton.Frame.GetMinX () : bounds.GetMaxX ();

			var button = new UIButton (UIButtonType.Custom) {
				AutoresizingMask = FlexibleLeftMargin | FlexibleBottomMargin
			};
			button.TouchUpInside += handler;
			button.SetTitleColor (UIColor.Orange, UIControlState.Normal);
			button.SetTitleColor (UIColor.LightGray, UIControlState.Highlighted);
			button.SetTitle (title, UIControlState.Normal);
			button.SizeToFit ();
			var frame = button.Frame.Inset (-20, -4);
			frame.Location = new CGPoint (maxX - frame.Width - 5, bounds.GetMinY () - 5);
			button.Frame = frame;
			var buttonLayer = button.Layer;
			buttonLayer.CornerRadius = 5;
			button.BackgroundColor = UIColor.FromWhiteAlpha (1, 0.4f);
			View.AddSubview (button);
			buttons.Add (button);

			return button;
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			scrollView.FlashScrollIndicators ();
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		void SetupConfigurations ()
		{
			configurations = new Action [] {
				() => cgView.DisplayOptions = StrokeViewDisplayOptions.Calligraphy,
				() => cgView.DisplayOptions = StrokeViewDisplayOptions.Ink,
				() => cgView.DisplayOptions = StrokeViewDisplayOptions.Debug
			};
			configurations [0] ();
		}

		void ReceivedAllUpdatesForStroke (Stroke stroke)
		{
			cgView.SetNeedsDisplay (stroke);
			stroke.ClearUpdateInfo ();
		}

		void ClearButtonAction (object sender, EventArgs e)
		{
			cgView.StrokeCollection = strokeCollection = new StrokeCollection ();
		}

		void StrokeUpdated (StrokeGestureRecognizer strokeGesture)
		{
			if (strokeGesture == pencilStrokeRecognizer)
				lastSeenPencilInteraction = DateTime.Now.Ticks;

			var state = strokeGesture.State;

			Stroke stroke = null;
			if (state != Cancelled) {
				stroke = strokeGesture.Stroke;
				if (state == Began || (state == Ended && strokeCollection.ActiveStroke == null)) {
					strokeCollection.ActiveStroke = stroke;
					leftRingControl.CancelInteraction ();
				}
			} else {
				strokeCollection.ActiveStroke = null;
			}

			if (stroke != null) {
				if (state == Ended) {
					if (strokeGesture == pencilStrokeRecognizer) {
						// Make sure we get the final stroke update if needed.
						stroke.ReceivedAllNeededUpdatesBlock = () => {
							ReceivedAllUpdatesForStroke (stroke);
						};
					}
					strokeCollection.TakeActiveStroke ();
				}
			}
			cgView.StrokeCollection = strokeCollection;
		}

		#region Pencil Recognition and UI Adjustments

		void SetupPencilUI ()
		{
			pencilButton = AddButton ("pencil", StopPencilButtonAction);
			pencilButton.TitleLabel.TextAlignment = UITextAlignment.Left;
			var bounds = pencilButton.Bounds;
			var dimension = bounds.Height - 16;
			pencilButton.ContentEdgeInsets = new UIEdgeInsets (0, dimension, 0, 0);

			var x = bounds.GetMinX () + 3;
			var y = bounds.GetMinY () + (bounds.Height - dimension) - 7;
			var closeImg = UIImage.FromBundle ("Close");
			var imageView = (closeImg != null) ? new UIImageView (closeImg) : new UIImageView ();
			imageView.Frame = new CGRect (x, y, dimension, dimension);
			imageView.Alpha = 0.7f;
			pencilButton.AddSubview (imageView);
			PencilMode = false;

			NSObject observer = UIApplication.Notifications.ObserveWillEnterForeground ((sender, e) => {
				if (PencilMode
				    && (!LastSeenPencilInteraction.HasValue || (DateTime.Now.Ticks - LastSeenPencilInteraction.Value) > pencilResetInterval))
					StopPencilButtonAction (this, EventArgs.Empty);
			});
			observers.Add (observer);
		}

		protected override void Dispose (bool disposing)
		{
			observers.ForEach (o => o.Dispose ());
			base.Dispose (disposing);
		}

		void StopPencilButtonAction (object sender, EventArgs e)
		{
			lastSeenPencilInteraction = null;
			PencilMode = false;
		}

		#endregion

		#region IUIGestureRecognizerDelegate

		// Since our gesture recognizer is beginning immediately, we do the hit test ambiguation here
		// instead of adding failure requirements to the gesture for minimizing the delay
		// to the first action sent and therefore the first lines drawn.
		[Export ("gestureRecognizer:shouldReceiveTouch:")]
		public bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
		{
			if (leftRingControl.HitTest (touch.LocationInView (leftRingControl), null) != null)
				return false;

			foreach(var button in buttons) {
				if (button.HitTest (touch.LocationInView (clearButton), null) != null)
					return false;
			}

			return true;
		}

		// We want the pencil to recognize simultaniously with all others.
		[Export ("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
		public bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
		{
			if (gestureRecognizer == pencilStrokeRecognizer)
				return otherGestureRecognizer != fingerStrokeRecognizer;
			return false;
		}

		#endregion

		#region IUIScrollViewDelegate

		[Export ("viewForZoomingInScrollView:")]
		public UIView ViewForZoomingInScrollView (UIScrollView scrollView)
		{
			return canvasContainerView;
		}

		[Export ("scrollViewDidEndZooming:withView:atScale:")]
		public void ZoomingEnded (UIScrollView scrollView, UIView withView, nfloat scale)
		{
			var desiredScale = TraitCollection.DisplayScale;
			var existingScale = cgView.ContentScaleFactor;

			if (scale >= 2)
				desiredScale *= 2;

			if (NMath.Abs (desiredScale - existingScale) > 0.00001) {
				cgView.ContentScaleFactor = desiredScale;
				cgView.SetNeedsDisplay ();
			}
		}

		#endregion

	}
}
