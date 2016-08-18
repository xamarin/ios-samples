using System;
using UIKit;

using CoreGraphics;
using Foundation;
using CoreAnimation;
using AVFoundation;
using CoreFoundation;
using ObjCRuntime;

namespace AVCamBarcode
{
	public enum ControlCorner
	{
		None,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	[Register ("PreviewView")]
	public class PreviewView : UIView, IUIGestureRecognizerDelegate
	{
		public event EventHandler RegionOfInterestWillChange;
		public event EventHandler RegionOfInterestDidChange;

		readonly CAShapeLayer maskLayer = new CAShapeLayer ();
		readonly CAShapeLayer regionOfInterestOutline = new CAShapeLayer ();

		// White dot on the top left of the region of interest.
		readonly CAShapeLayer topLeftControl = new CAShapeLayer ();

		// White dot on the top right of the region of interest.
		readonly CAShapeLayer topRightControl = new CAShapeLayer ();

		// White dot on the bottom left of the region of interest.
		readonly CAShapeLayer bottomLeftControl = new CAShapeLayer ();

		// White dot on the bottom right of the region of interest.
		readonly CAShapeLayer bottomRightControl = new CAShapeLayer ();

		const float regionOfInterestCornerTouchThreshold = 50;

		// The minimum region of interest's size cannot be smaller than the corner
		// touch threshold as to avoid control selection conflicts when a user tries
		// to resize the region of interest.
		float MinimumRegionOfInterestSize {
			get {
				return regionOfInterestCornerTouchThreshold;
			}
		}

		// Saves a reference to the control corner that the user is using to resize
		// the region of interest in `resizeRegionOfInterestWithGestureRecognizer()`.
		ControlCorner currentControlCorner = ControlCorner.None;

		// This property is set only in `setRegionOfInterestWithProposedRegionOfInterest()`.
		// When a user is resizing the region of interest in `resizeRegionOfInterestWithGestureRecognizer()`,
		// the KVO notification will be triggered when the resizing is finished.
		public CGRect RegionOfInterest { get; set; }

		const float regionOfInterestControlDiameter = 12;
		float RegionOfInterestControlRadius {
			get {
				return regionOfInterestControlDiameter / 2;
			}
		}

		UIPanGestureRecognizer resizeRegionOfInterestGestureRecognizer;
		UIPanGestureRecognizer ResizeRegionOfInterestGestureRecognizer {
			get {
				resizeRegionOfInterestGestureRecognizer = resizeRegionOfInterestGestureRecognizer ?? new UIPanGestureRecognizer (ResizeRegionOfInterestWithGestureRecognizer);
				return resizeRegionOfInterestGestureRecognizer;
			}
		}


		IDisposable runningToken;
		public AVCaptureSession Session {
			get {
				return VideoPreviewLayer.Session;
			}
			set {

				if (value != null)
					runningToken = value.AddObserver ("running", NSKeyValueObservingOptions.New, RunningChanged);
				else
					runningToken?.Dispose ();

				VideoPreviewLayer.Session = value;
			}
		}

		public AVCaptureVideoPreviewLayer VideoPreviewLayer {
			get {
				return Layer as AVCaptureVideoPreviewLayer;
			}
		}

		public bool IsResizingRegionOfInterest {
			get {
				return resizeRegionOfInterestGestureRecognizer.State == UIGestureRecognizerState.Changed;
			}
		}

		static Class layerClass;
		public static Class LayerClass {
			[Export ("layerClass")]
			get {
				return layerClass = layerClass ?? new Class (typeof (AVCaptureVideoPreviewLayer));
			}
		}

		public PreviewView (CGRect frame)
				: base (frame)
		{
			CommonInit ();
		}

		[Export ("initWithCoder:")]
		public PreviewView (NSCoder coder)
			: base (coder)
		{
			CommonInit ();
		}

		void CommonInit ()
		{
			maskLayer.FillRule = CAShapeLayer.FillRuleEvenOdd;
			maskLayer.FillColor = UIColor.Black.CGColor;
			maskLayer.Opacity = 0.6f;
			Layer.AddSublayer (maskLayer);

			regionOfInterestOutline.Path = UIBezierPath.FromRect (RegionOfInterest).CGPath;
			regionOfInterestOutline.FillColor = UIColor.Clear.CGColor;
			regionOfInterestOutline.StrokeColor = UIColor.Yellow.CGColor;
			Layer.AddSublayer (regionOfInterestOutline);

			topLeftControl.Path = UIBezierPath.FromOval (new CGRect (0, 0, regionOfInterestControlDiameter, regionOfInterestControlDiameter)).CGPath;
			topLeftControl.FillColor = UIColor.White.CGColor;
			Layer.AddSublayer (topLeftControl);

			topRightControl.Path = UIBezierPath.FromOval (new CGRect (0, 0, regionOfInterestControlDiameter, regionOfInterestControlDiameter)).CGPath;
			topRightControl.FillColor = UIColor.White.CGColor;
			Layer.AddSublayer (topRightControl);

			bottomLeftControl.Path = UIBezierPath.FromOval (new CGRect (0, 0, regionOfInterestControlDiameter, regionOfInterestControlDiameter)).CGPath;
			bottomLeftControl.FillColor = UIColor.White.CGColor;
			Layer.AddSublayer (bottomLeftControl);

			bottomRightControl.Path = UIBezierPath.FromOval (new CGRect (0, 0, regionOfInterestControlDiameter, regionOfInterestControlDiameter)).CGPath;
			bottomRightControl.FillColor = UIColor.White.CGColor;
			Layer.AddSublayer (bottomRightControl);

			// TODO: fix comment
			// Add the region of interest gesture recognizer to the region of interest
			// view so that the region of interest can be resized and moved. If you
			// would like to have a fixed region of interest that cannot be resized
			// or moved, do not add the following gesture recognizer. You will simply
			// need to set the region of interest once in
			// `observeValue(forKeyPath:, of:, change:, context:)`.
			ResizeRegionOfInterestGestureRecognizer.Delegate = this;
			AddGestureRecognizer (ResizeRegionOfInterestGestureRecognizer);
		}

		void ResizeRegionOfInterestWithGestureRecognizer (UIPanGestureRecognizer pan)
		{
			var touchLocation = pan.LocationInView (pan.View);
			var oldRegionOfInterest = RegionOfInterest;

			switch (pan.State) {
			case UIGestureRecognizerState.Began:
				// TODO: replace with events
				// WillChangeValue (forKey: "regionOfInterest");
				RegionOfInterestWillChange?.Invoke (this, EventArgs.Empty);

				// When the gesture begins, save the corner that is closes to
				// the resize region of interest gesture recognizer's touch location.
				currentControlCorner = CornerOfRect (oldRegionOfInterest, touchLocation);
				break;


			case UIGestureRecognizerState.Changed:
				var newRegionOfInterest = oldRegionOfInterest;

				switch (currentControlCorner) {
				case ControlCorner.None:
					// Update the new region of interest with the gesture recognizer's translation.
					var translation = pan.TranslationInView (pan.View);
					// Move the region of interest with the gesture recognizer's translation.
					if (RegionOfInterest.Contains (touchLocation)) {
						newRegionOfInterest.X += translation.X;
						newRegionOfInterest.Y += translation.Y;
					}

					// If the touch location goes outside the preview layer,
					// we will only translate the region of interest in the
					// plane that is not out of bounds.
					var normalizedRect = new CGRect (0, 0, 1, 1);
					if (!normalizedRect.Contains (VideoPreviewLayer.PointForCaptureDevicePointOfInterest (touchLocation))) {
						if (touchLocation.X < RegionOfInterest.GetMinX () || touchLocation.X > RegionOfInterest.GetMaxX ()) {
							newRegionOfInterest.Y += translation.Y;
						} else if (touchLocation.Y < RegionOfInterest.GetMinY () || touchLocation.Y > RegionOfInterest.GetMaxY ()) {
							newRegionOfInterest.X += translation.X;
						}
					}

					// Set the translation to be zero so that the new gesture
					// recognizer's translation is in respect to the region of
					// interest's new position.
					pan.SetTranslation (CGPoint.Empty, pan.View);
					break;

				case ControlCorner.TopLeft:
					newRegionOfInterest = new CGRect (touchLocation.X, touchLocation.Y,
													 oldRegionOfInterest.Width + oldRegionOfInterest.X - touchLocation.X,
													 oldRegionOfInterest.Height + oldRegionOfInterest.Y - touchLocation.Y);
					break;

				case ControlCorner.TopRight:
					newRegionOfInterest = new CGRect (newRegionOfInterest.X,
												 touchLocation.Y,
												 touchLocation.X - newRegionOfInterest.X,
												 oldRegionOfInterest.Height + newRegionOfInterest.Y - touchLocation.Y);
					break;


				case ControlCorner.BottomLeft:
					newRegionOfInterest = new CGRect (touchLocation.X, oldRegionOfInterest.Y,
												 oldRegionOfInterest.Width + oldRegionOfInterest.X - touchLocation.X,
												 touchLocation.Y - oldRegionOfInterest.Y);
					break;

				case ControlCorner.BottomRight:
					newRegionOfInterest = new CGRect (oldRegionOfInterest.X, oldRegionOfInterest.Y,
												 touchLocation.X - oldRegionOfInterest.X,
												 touchLocation.Y - oldRegionOfInterest.Y);
					break;
				}

				// Update the region of intresest with a valid CGRect.
				SetRegionOfInterestWithProposedRegionOfInterest (newRegionOfInterest);
				break;

			case UIGestureRecognizerState.Ended:
				// TODO: replace with event
				//DidChangeValue (forKey: "regionOfInterest");
				RegionOfInterestDidChange?.Invoke (this, EventArgs.Empty);

				// Reset the current corner reference to none now that the resize.
				// gesture recognizer has ended.
				currentControlCorner = ControlCorner.None;
				break;

			default:
				return;
			}
		}

		ControlCorner CornerOfRect (CGRect rect, CGPoint point)
		{
			var closestDistance = nfloat.MaxValue;
			var closestCorner = ControlCorner.None;
			Tuple<ControlCorner, CGPoint> [] corners = {
				Tuple (ControlCorner.TopLeft, rect.Location),
				Tuple (ControlCorner.TopRight, new CGPoint (rect.GetMaxX(), rect.GetMinY())),
				Tuple (ControlCorner.BottomLeft, new CGPoint (rect.GetMinX (), rect.GetMaxY())),
				Tuple (ControlCorner.BottomRight, new CGPoint (rect.GetMaxX (), rect.GetMaxY()))
			};

			// corner, cornerPoint
			foreach (var tpl in corners) {
				var corner = tpl.Item1;
				var cornerPoint = tpl.Item2;

				var dX = point.X - cornerPoint.X;
				var dY = point.Y - cornerPoint.Y;
				var distance = NMath.Sqrt ((dX * dX) + (dY * dY));

				if (distance < closestDistance) {
					closestDistance = distance;
					closestCorner = corner;
				}
			}

			if (closestDistance > regionOfInterestCornerTouchThreshold) {
				closestCorner = ControlCorner.None;
			}

			return closestCorner;
		}

		static Tuple<ControlCorner, CGPoint> Tuple (ControlCorner corner, CGPoint point)
		{
			return new Tuple<ControlCorner, CGPoint> (corner, point);
		}

		// Updates the region of interest with a proposed region of interest ensuring
		// the new region of interest is within the bounds of the video preview. When
		// a new region of interest is set, the region of interest is redrawn.
		public void SetRegionOfInterestWithProposedRegionOfInterest (CGRect proposedRegionOfInterest)
		{
			// We standardize to ensure we have positive widths and heights with an origin at the top left.
			var videoPreviewRect = VideoPreviewLayer.MapToLayerCoordinates (new CGRect (0, 0, 1, 1)).Standardize ();

			// Intersect the video preview view with the view's frame to only get
			// the visible portions of the video preview view.
			var visibleVideoPreviewRect = CGRect.Intersect (videoPreviewRect, Frame);
			var oldRegionOfInterest = RegionOfInterest;
			var newRegionOfInterest = proposedRegionOfInterest.Standardize ();

			// Move the region of interest in bounds.
			if (currentControlCorner == ControlCorner.None) {
				nfloat xOffset = 0;
				nfloat yOffset = 0;

				if (!visibleVideoPreviewRect.Contains (newRegionOfInterest.Location)) {
					xOffset = NMath.Max (visibleVideoPreviewRect.GetMinX () - newRegionOfInterest.GetMinX (), 0);
					yOffset = NMath.Max (visibleVideoPreviewRect.GetMinY () - newRegionOfInterest.GetMinY (), 0);
				}

				if (!visibleVideoPreviewRect.Contains (new CGPoint (visibleVideoPreviewRect.GetMaxX (), visibleVideoPreviewRect.GetMaxY ()))) {
					xOffset = NMath.Min (visibleVideoPreviewRect.GetMaxX () - newRegionOfInterest.GetMaxX (), xOffset);
					yOffset = NMath.Min (visibleVideoPreviewRect.GetMaxY () - newRegionOfInterest.GetMaxY (), yOffset);
				}

				newRegionOfInterest.Offset (xOffset, yOffset);
			}

			// Clamp the size when the region of interest is being resized.
			visibleVideoPreviewRect.Intersect(newRegionOfInterest);
			newRegionOfInterest = visibleVideoPreviewRect;

			// Fix a minimum width of the region of interest.
			if (proposedRegionOfInterest.Size.Width < MinimumRegionOfInterestSize) {
				switch (currentControlCorner) {
				case ControlCorner.TopLeft:
				case ControlCorner.BottomLeft:
					newRegionOfInterest.X = oldRegionOfInterest.Location.X + oldRegionOfInterest.Size.Width - MinimumRegionOfInterestSize;
					newRegionOfInterest.Width = MinimumRegionOfInterestSize;
					break;

				case ControlCorner.TopRight:
					newRegionOfInterest.X = oldRegionOfInterest.Location.X;
					newRegionOfInterest.Width = MinimumRegionOfInterestSize;
					break;

				default:
					newRegionOfInterest.Location = oldRegionOfInterest.Location;
					newRegionOfInterest.Width = MinimumRegionOfInterestSize;
					break;
				}
			}

			// Fix a minimum height of the region of interest.
			if (proposedRegionOfInterest.Height < MinimumRegionOfInterestSize) {
				switch (currentControlCorner) {
				case ControlCorner.TopLeft:
				case ControlCorner.TopRight:
					newRegionOfInterest.Y = oldRegionOfInterest.Y + oldRegionOfInterest.Height - MinimumRegionOfInterestSize;
					newRegionOfInterest.Height = MinimumRegionOfInterestSize;
					break;

				case ControlCorner.BottomLeft:
					newRegionOfInterest.Y = oldRegionOfInterest.Y;
					newRegionOfInterest.Height = MinimumRegionOfInterestSize;
					break;


				default:
					newRegionOfInterest.Location = oldRegionOfInterest.Location;
					newRegionOfInterest.Height = MinimumRegionOfInterestSize;
					break;
				}
			}

			RegionOfInterest = newRegionOfInterest;
			SetNeedsLayout ();
		}

		#region KVO

		void RunningChanged (NSObservedChange change)
		{
			var running = ((NSNumber)change.NewValue).BoolValue;
			if (!running)
				return;

			DispatchQueue.MainQueue.DispatchAsync (() => {
				// If the region of interest view's region of interest has not
				// been initialized yet, let's set an inital region of interest
				// that is 80% of the shortest side by 25% of the longest side
				// and centered in the root view.
				if (RegionOfInterest.IsEmpty) {
					var width = NMath.Min (Frame.Width, Frame.Height) * 0.8f;
					var height = NMath.Max (Frame.Width, Frame.Height) * 0.25f;

					var newRegionOfInterest = Frame.Inset (Frame.GetMidX () - width / 2, Frame.GetMidY () - height / 2);
					SetRegionOfInterestWithProposedRegionOfInterest (newRegionOfInterest);
				}

				if (running)
					SetRegionOfInterestWithProposedRegionOfInterest (RegionOfInterest);
			});
		}

		#endregion

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			// Disable CoreAnimation actions so that the positions of the sublayers immediately move to their new position.
			CATransaction.Begin ();
			CATransaction.DisableActions = true;

			// Create the path for the mask layer. We use the even odd fill rule so that the region of interest does not have a fill color.
			var path = UIBezierPath.FromRect (new CGRect (0, 0, Frame.Width, Frame.Height));
			path.AppendPath (UIBezierPath.FromRect (RegionOfInterest));
			path.UsesEvenOddFillRule = true;
			maskLayer.Path = path.CGPath;

			regionOfInterestOutline.Path = CGPath.FromRect (RegionOfInterest);

			topLeftControl.Position = new CGPoint (RegionOfInterest.X - RegionOfInterestControlRadius, RegionOfInterest.Y - RegionOfInterestControlRadius);
			topRightControl.Position = new CGPoint (RegionOfInterest.X + RegionOfInterest.Width - RegionOfInterestControlRadius, RegionOfInterest.Y - RegionOfInterestControlRadius);
			bottomLeftControl.Position = new CGPoint (RegionOfInterest.X - RegionOfInterestControlRadius, RegionOfInterest.Y + RegionOfInterest.Height - RegionOfInterestControlRadius);
			bottomRightControl.Position = new CGPoint (RegionOfInterest.X + RegionOfInterest.Width - RegionOfInterestControlRadius, RegionOfInterest.Y + RegionOfInterest.Height - RegionOfInterestControlRadius);

			CATransaction.Commit ();
		}

		#region IUIGestureRecognizerDelegate

		[Export ("gestureRecognizer:shouldReceiveTouch:")]
		public bool ShouldReceiveTouch (UIGestureRecognizer gestureRecognizer, UITouch touch)
		{
			// Ignore drags outside of the region of interest (plus some padding).
			if (gestureRecognizer == resizeRegionOfInterestGestureRecognizer) {
				var touchLocation = touch.LocationInView (gestureRecognizer.View);
				var paddedRegionOfInterest = RegionOfInterest.Inset (-regionOfInterestCornerTouchThreshold, -regionOfInterestCornerTouchThreshold);
				if (!paddedRegionOfInterest.Contains (touchLocation))
					return false;
			}

			return true;
		}


		[Export ("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
		public bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
		{
			// Allow multiple gesture recognizers to be recognized simultaneously if and only if the touch location is not within the touch threshold.
			if (gestureRecognizer == resizeRegionOfInterestGestureRecognizer) {
				var touchLocation = gestureRecognizer.LocationInView (gestureRecognizer.View);
				var closestCorner = CornerOfRect (RegionOfInterest, touchLocation);
				return closestCorner == ControlCorner.None;
			}

			return false;
		}

		#endregion
	}
}
