using System;
using System.Collections.Generic;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

using static TouchCanvas.CGRectHelpers;

namespace TouchCanvas {
	public partial class CanvasView : UIView {
		bool isPredictionEnabled = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;

		bool needsFullRedraw = true;

		// List containing all line objects that need to be drawn in Draw(CGRect rect)
		readonly List<Line> lines = new List<Line> ();

		// List containing all line objects that have been completely drawn into the frozenContext.
		readonly List<Line> finishedLines = new List<Line> ();

		// Holds a map of UITouch objects to Line objects whose touch has not ended yet.
		readonly Dictionary<UITouch, Line> activeLines = new Dictionary<UITouch, Line> ();

		// Holds a map of UITouch objects to Line objects whose touch has ended but still has points awaiting updates.
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

		// An CGImage containing the last representation of lines no longer receiving updates.
		CGImage frozenImage;

		CGBitmapContext frozenContext;
		public CGBitmapContext FrozenContext {
			get {
				if (frozenContext == null) {
					var scale = Window.Screen.Scale;
					var size = Bounds.Size;

					size.Width *= scale;
					size.Height *= scale;
					var colorSpace = CGColorSpace.CreateDeviceRGB ();

					frozenContext = new CGBitmapContext (null, (nint) size.Width, (nint) size.Height, 8, 0, colorSpace, CGImageAlphaInfo.PremultipliedLast);
					frozenContext.SetLineCap (CGLineCap.Round);
					frozenContext.ConcatCTM (CGAffineTransform.MakeScale (scale, scale));
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
				foreach (var line in finishedLines)
					line.DrawCommitedPointsInContext (FrozenContext, IsDebuggingEnabled, UsePreciseLocations);

				needsFullRedraw = false;
			}

			frozenImage = frozenImage ?? FrozenContext.ToImage ();
			if (frozenImage != null)
				context.DrawImage (Bounds, frozenImage);

			foreach (var line in lines)
				line.DrawInContext (context, IsDebuggingEnabled, UsePreciseLocations);
		}

		void SetFrozenImageNeedsUpdate ()
		{
			frozenImage?.Dispose ();
			frozenImage = null;
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
			var updateRect = CGRectNull ();

			foreach (var touch in touches.Cast<UITouch> ()) {
				Line line;

				// Retrieve a line from activeLines. If no line exists, create one.
				if (!activeLines.TryGetValue (touch, out line))
					line = AddActiveLineForTouch (touch);

				// Remove prior predicted points and update the updateRect based on the removals. The touches
				// used to create these points are predictions provided to offer additional data. They are stale
				// by the time of the next event for this touch.
				updateRect = updateRect.UnionWith (line.RemovePointsWithType (PointType.Predicted));

				// Incorporate coalesced touch data. The data in the last touch in the returned array will match
				// the data of the touch supplied to GetCoalescedTouches
				var coalescedTouches = evt.GetCoalescedTouches (touch) ?? new UITouch [0];
				var coalescedRect = AddPointsOfType (PointType.Coalesced, coalescedTouches, line);
				updateRect = updateRect.UnionWith (coalescedRect);

				// Incorporate predicted touch data. This sample draws predicted touches differently; however, 
				// you may want to use them as inputs to smoothing algorithms rather than directly drawing them. 
				// Points derived from predicted touches should be removed from the line at the next event for this touch.
				if (isPredictionEnabled) {
					var predictedTouches = evt.GetPredictedTouches (touch) ?? new UITouch [0];
					var predictedRect = AddPointsOfType (PointType.Predicted, predictedTouches, line);
					updateRect = updateRect.UnionWith (predictedRect);
				}
			}
			SetNeedsDisplayInRect (updateRect);
		}

		Line AddActiveLineForTouch (UITouch touch)
		{
			var newLine = new Line ();
			activeLines.Add (touch, newLine);
			lines.Add (newLine);
			return newLine;
		}

		CGRect AddPointsOfType (PointType type, UITouch [] touches, Line line)
		{
			var accumulatedRect = CGRectNull ();

			for (int i = 0; i < touches.Length; i++) {
				var touch = touches [i];
				// The visualization displays non-`.Stylus` touches differently.
				if (touch.Type != UITouchType.Stylus)
					type |= PointType.Finger;

				// Touches with estimated properties require updates; add this information to the `PointType`.
				if (touch.EstimatedProperties != 0)
					type |= PointType.NeedsUpdate;

				// The last touch in a set of .Coalesced touches is the originating touch. Track it differently.
				bool isLast = (i == touches.Length - 1);
				if (type.HasFlag (PointType.Coalesced) && isLast) {
					type &= ~PointType.Coalesced;
					type |= PointType.Standard;
				}

				var touchRect = line.AddPointOfType (type, touch);
				accumulatedRect = accumulatedRect.UnionWith (touchRect);
				CommitLine (line);
			}

			return accumulatedRect;
		}

		public void EndTouches (NSSet touches, bool cancel)
		{
			var updateRect = CGRectNull ();

			foreach (var touch in touches.Cast<UITouch> ()) {
				// Skip over touches that do not correspond to an active line.
				Line line;
				if (!activeLines.TryGetValue (touch, out line))
					continue;

				// If this is a touch cancellation, cancel the associated line.
				if (cancel)
					updateRect = updateRect.UnionWith (line.Cancel ());

				// If the line is complete (no points needing updates) or updating isn't enabled, move the line to the frozenImage.
				if (line.IsComplete)
					FinishLine (line);
				else
					pendingLines.Add (touch, line);

				// This touch is ending, remove the line corresponding to it from `activeLines`.
				activeLines.Remove (touch);
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

				CGRect rect;
				if (possibleLine.UpdateWithTouch (touch, out rect))
					SetNeedsDisplayInRect (rect);

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
			// Have the line draw any segments between points no longer being updated into the frozenContext and remove them from the line.
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
