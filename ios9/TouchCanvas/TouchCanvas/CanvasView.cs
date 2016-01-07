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

		bool needsFullRedraw = true;
		CGImage frozenImage;
		readonly List<Line> lines = new List<Line> ();
		readonly List<Line> finishedLines = new List<Line> ();

		readonly Dictionary<UITouch, Line> activeLines = new Dictionary<UITouch, Line> ();
		readonly Dictionary<UITouch, Line> pendingLines = new Dictionary<UITouch, Line> ();

		bool isDebuggingEnabled;
		public bool IsDebuggingEnabled {
			get {
				return isDebuggingEnabled;
			}
			set {
				isDebuggingEnabled = value;
				needsFullRedraw = true;
				SetNeedsDisplay ();
			}
		}

		bool usePreciseLocations;
		public bool UsePreciseLocations {
			get {
				return usePreciseLocations;
			}
			set {
				usePreciseLocations = value;
				needsFullRedraw = true;
				SetNeedsDisplay ();
			}
		}

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

			if (needsFullRedraw) {
				SetFrozenImageNeedsUpdate ();
				FrozenContext.ClearRect (Bounds);
				foreach (var line in finishedLines.Union (lines)) {
					line.DrawCommitedPointsInContext (context, IsDebuggingEnabled, UsePreciseLocations);
				}

				needsFullRedraw = false;
			}

			frozenImage = frozenImage ?? FrozenContext.ToImage ();
			context.DrawImage (Bounds, frozenImage);

			foreach (var line in lines)
				line.DrawInContext (context, IsDebuggingEnabled, UsePreciseLocations);
		}

		public void Clear ()
		{
			activeLines.Clear ();
			pendingLines.Clear ();
			lines.Clear ();
			finishedLines.Clear ();
			needsFullRedraw = true;
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
				updateRect = updateRect.UnionWith (line.RemovePointsWithType (PointType.Predicted));

				var coalescedTouches = evt.GetCoalescedTouches (touch) ?? new UITouch[0];
				var coalescedRect = AddPointsOfType (PointType.Coalesced, coalescedTouches, line, updateRect);
				updateRect = updateRect.UnionWith (coalescedRect);

				if (isPredictionEnabled) {
					var predictedTouches = evt.GetPredictedTouches (touch) ?? new UITouch[0];
					var predictedRect = AddPointsOfType (PointType.Predicted, predictedTouches, line, updateRect);
					updateRect = updateRect.UnionWith (predictedRect);
				}
			}
			SetNeedsDisplay ();
//			SetNeedsDisplayInRect (updateRect);
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
					type = type.Add (PointType.Finger);

				if (isTouchUpdatingEnabled && (touch.EstimatedProperties != 0))
					type = type.Add (PointType.NeedsUpdate);

				bool isLast = i == touches.Length - 1;
				if (type.Has (PointType.Coalesced) && isLast) {
					type = type.Remove (PointType.Coalesced);
					type = type.Add (PointType.Standard);
				}

				var touchRect = line.AddPointOfType (type, touch);
				accumulatedRect = accumulatedRect.UnionWith (touchRect);
				CommitLine (line);
			}

			return rect.UnionWith (accumulatedRect);
		}

		void SetFrozenImageNeedsUpdate ()
		{
			frozenImage = null;
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
					updateRect = updateRect.UnionWith (line.Cancel ());

				if (line.IsComplete) {
					FinishLine (line);
				} else {
					pendingLines.Add (touch, line);
				}
			}

			SetNeedsDisplayInRect (updateRect);
		}

		public void UpdateEstimatedPropertiesForTouches (NSSet touches)
		{
			foreach (var touch in touches.Cast<UITouch> ()) {
				bool isPending = false;

				// Look to retrieve a line from `activeLines`. If no line exists, look it up in `pendingLines`.
				Line possibleLine;
				if (!activeLines.TryGetValue (touch, out possibleLine))
					isPending = pendingLines.TryGetValue (touch, out possibleLine);

				// If no line is related to the touch, return as there is no additional work to do.
				if (possibleLine == null)
					return;

				var updateResult = possibleLine.UpdateWithTouch (touch);
				if (updateResult.Key)
					SetNeedsDisplayInRect (updateResult.Value);

				// If this update updated the last point requiring an update, move the line to the `frozenImage`.
				if (isPending && possibleLine.IsComplete) {
					FinishLine (possibleLine);
					pendingLines.Remove (touch);
				} else {
					CommitLine (possibleLine);
				}
			}
		}

		void CommitLine (Line line)
		{
			line.DrawFixedPointsInContext (FrozenContext, IsDebuggingEnabled, UsePreciseLocations);
			SetFrozenImageNeedsUpdate ();
		}

		void FinishLine (Line line)
		{
			line.DrawFixedPointsInContext (FrozenContext, IsDebuggingEnabled, UsePreciseLocations, true);
			SetFrozenImageNeedsUpdate ();
			lines.Remove (line);
			finishedLines.Add (line);
		}
	}
}
