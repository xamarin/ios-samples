using System;
using System.Collections.Generic;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

namespace TouchCanvas {
	public partial class CanvasView : UIView {
		bool isPredictionEnabled = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
		const bool isTouchUpdatingEnabled = true;

		CGImage frozenImage;
		readonly List<Line> lines = new List<Line> ();

		readonly Dictionary<UITouch, Line> activeLines = new Dictionary<UITouch, Line> ();
		readonly Dictionary<UITouch, Line> pendingLines = new Dictionary<UITouch, Line> ();

		public bool IsDebuggingEnabled { get; set; }

		public bool UsePreciseLocations { get; set; }

		CGBitmapContext frozenContext;
		public CGBitmapContext FrozenContext {
			get {
				if (frozenContext == null) {
					var scale = Window.Screen.Scale;
					var size = Bounds.Size;

					size.Width *= scale;
					size.Height *= scale;
					var colorSpace = CGColorSpace.CreateDeviceRGB ();

					frozenContext = new CGBitmapContext (null, (nint)size.Width, (nint)size.Height, 8, 0, colorSpace, CGImageAlphaInfo.PremultipliedLast);
					frozenContext.SetLineCap (CGLineCap.Round);
					var transform = CGAffineTransform.MakeScale (scale, scale);
					frozenContext.ConcatCTM (transform);
				}

				return frozenContext;
			}
		}

		[Export ("initWithCoder:")]
		public CanvasView (NSCoder coder) : base (coder)
		{
		}

		[Export ("initWithFrame:")]
		public CanvasView (CGRect frame) : base (frame)
		{
		}

		public CanvasView (IntPtr handle) : base (handle)
		{
		}

		public override void Draw (CGRect rect)
		{
			var context = UIGraphics.GetCurrentContext ();
			context.SetLineCap (CGLineCap.Round);
			frozenImage = frozenImage ?? FrozenContext.ToImage ();

			if (frozenImage != null)
				context.DrawImage (Bounds, frozenImage);

			foreach (var line in lines)
				line.DrawInContext (context, IsDebuggingEnabled, UsePreciseLocations);
		}

		public void UpdateEstimatedPropertiesForTouches (NSSet touches)
		{
			var updateRect = CGRect.Empty;

			foreach (var touch in touches.Cast<UITouch> ()) {
				bool isPending = false;

				// Look to retrieve a line from `activeLines`. If no line exists, look it up in `pendingLines`.
				// If no line is related to the touch, return as there is no additional work to do.
				Line possibleLine;
				if (!activeLines.TryGetValue (touch, out possibleLine) && !(isPending = pendingLines.TryGetValue (touch, out possibleLine)))
					return;

				var updateEstimatedRect = possibleLine.UpdatePointLocation (touch.LocationInView (this), touch.GetPreciseLocation (this), touch.Force, touch.Timestamp);
				updateRect.UnionWith (updateEstimatedRect);

				if (isPending && possibleLine.IsComplete) {
					FinishLine (possibleLine);
					pendingLines.Remove (touch);
				} else {
					CommitLine (possibleLine);
				}

				SetNeedsDisplayInRect (updateRect);
			}
		}

		public void Clear ()
		{
			activeLines.Clear ();
			pendingLines.Clear ();
			lines.Clear ();
			frozenImage = null;
			FrozenContext.ClearRect (Bounds);
			SetNeedsDisplay ();
		}

		public void DrawTouches (NSSet touches, UIEvent evt)
		{
			var updateRect = CGRect.Empty;

			foreach (var touch in touches.Cast<UITouch> ()) {
				Line line;

				// Retrieve a line from `activeLines`. If no line exists, create one.
				if (!activeLines.TryGetValue (touch, out line))
					line = AddActiveLineForTouch (touch);
				updateRect.UnionWith (line.RemovePointsWithType (PointType.Predicted));

				var coalescedTouches = evt.GetCoalescedTouches (touch) ?? new UITouch[0];
				var coalescedRect = AddPointsOfType (PointType.Coalesced, coalescedTouches, line, updateRect);
				updateRect.UnionWith (coalescedRect);

				if (isPredictionEnabled) {
					var predictedTouches = evt.GetPredictedTouches (touch) ?? new UITouch[0];
					var predictedRect = AddPointsOfType (PointType.Predicted, predictedTouches, line, updateRect);
					updateRect.UnionWith (predictedRect);
				}
			}

			SetNeedsDisplay ();
		}

		Line AddActiveLineForTouch (UITouch touch)
		{
			var newLine = new Line ();
			activeLines.Add (touch, newLine);
			lines.Add (newLine);
			return newLine;
		}

		CGRect AddPointsOfType (PointType type, UITouch[] touches, Line line, CGRect rect)
		{
			var accumulatedRect = CGRect.Empty;

			for (int i = 0; i < touches.Length; i++) {
				var touch = touches [i];
				bool isStylus = touch.Type == UITouchType.Stylus;

				// The visualization displays non-`.Stylus` touches differently.
				if (!isStylus)
					type = PointType.Finger;

				if (isTouchUpdatingEnabled && (touch.EstimatedProperties != 0))
					type = PointType.NeedsUpdate;

				bool isLast = i == touches.Length - 1;
				if (type == PointType.Coalesced && isLast)
					type = PointType.Standard;

				var force = (isStylus || touch.Force > 0) ? touch.Force : 1f;
				var touchRect = line.AddPointAtLocation (touch.LocationInView (this), touch.GetPreciseLocation (this), force, touch.Timestamp, type);
				accumulatedRect.UnionWith (touchRect);
				CommitLine (line);
			}

			return rect.UnionWith (accumulatedRect);
		}

		public void EndTouches (NSSet touches, bool cancel)
		{
			var updateRect = CGRect.Empty;

			foreach (var touch in touches.Cast<UITouch> ()) {
				// Skip over touches that do not correspond to an active line.
				Line line;
				if (!activeLines.TryGetValue (touch, out line))
					continue;

				if (cancel)
					updateRect.UnionWith (line.Cancel ());

				if (line.IsComplete || !isTouchUpdatingEnabled) {
					FinishLine (line);
					pendingLines.Remove (touch);
				} else {
					CommitLine (line);
				}
			}

			SetNeedsDisplayInRect (updateRect);
		}

		void FinishLine (Line line)
		{
			line.DrawInContext (FrozenContext, IsDebuggingEnabled, UsePreciseLocations);
			frozenImage = null;
			lines.Remove (line);
		}

		void CommitLine (Line line)
		{
			line.DrawFixedPointsInContext (FrozenContext, IsDebuggingEnabled, UsePreciseLocations);
			frozenImage = null;
		}
	}
}
