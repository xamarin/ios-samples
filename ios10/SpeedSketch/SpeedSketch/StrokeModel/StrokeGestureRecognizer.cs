using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;
using ObjCRuntime;

using static UIKit.UIGestureRecognizerState;
using static SpeedSketch.Helpers;
using static SpeedSketch.CGMathExtensions;

namespace SpeedSketch
{
	public class StrokeGestureRecognizer : UIGestureRecognizer
	{
		[Register ("__StrokeGestureRecognizer")]
		[Preserve (Conditional = true)]
		class Callback : Token
		{
			readonly Action<StrokeGestureRecognizer> action;

			internal Callback (Action<StrokeGestureRecognizer> action)
			{
				this.action = action;
			}

			[Export ("target:")]
			[Preserve (Conditional = true)]
			public void Activated (StrokeGestureRecognizer sender)
			{
				action (sender);
			}
		}

		class StrokeIndex
		{
			public Stroke Stroke { get; set; }
			public int Index { get; set; }
		}

		bool isForPencil;
		public bool IsForPencil {
			get {
				return isForPencil;
			}
			set {
				AllowedTouchTypes = (isForPencil = value)
					? TouchTypes (UITouchType.Stylus)
					: TouchTypes (UITouchType.Direct);
			}
		}

		// Data.
		readonly Dictionary<NSNumber, StrokeIndex> outstandingUpdateIndexes = new Dictionary<NSNumber, StrokeIndex> ();

		public Stroke Stroke { get; private set; } = new Stroke ();
		public UIView CoordinateSpaceView { get; set; }

		// State.
		UITouch trackedTouch;
		double initialTimestamp;

		NSTimer fingerStartTimer;
		double cancellationTimeInterval = TimeSpan.FromSeconds (0.1).TotalMilliseconds;

		public StrokeGestureRecognizer (Action handler)
			: base (handler)
		{
		}

		public StrokeGestureRecognizer (Action<StrokeGestureRecognizer> handler)
			: base (new Selector ("target:"), new Callback (handler))
		{
		}

		bool Append (HashSet<UITouch> touches, UIEvent uievent)
		{
			var touchToAppend = trackedTouch;
			if (touchToAppend == null)
				return false;

			// Cancel the stroke recognition if we get a second touch during cancellation period.
			foreach (var touch in touches) {
				if (touch != touchToAppend && (touch.Timestamp - initialTimestamp < cancellationTimeInterval)) {
					State = (State == Possible) ? Failed : Cancelled;
					return false;
				}
			}

			// See if those touches contain our tracked touch. If not, ignore gracefully.
			if (!touches.Contains (touchToAppend))
				return false;

			var coalescedTouches = uievent.GetCoalescedTouches (touchToAppend);
			var lastIndex = coalescedTouches.Length - 1;
			for (var index = 0; index <= lastIndex; index++)
				Collect (Stroke, coalescedTouches [index], CoordinateSpaceView, (index != lastIndex), false);

			if (Stroke.State == StrokeState.Active) {
				var predictedTouches = uievent.GetPredictedTouches (touchToAppend);
				foreach (var touch in predictedTouches)
					Collect (Stroke, touch, CoordinateSpaceView, false, true);
			}
			return true;
		}

		void Collect (Stroke stroke, UITouch touch, UIView view, bool coalesced, bool predicted)
		{
			if (view == null)
				throw new ArgumentNullException ();

			// Only collect samples that actually moved in 2D space.
			var location = touch.GetPreciseLocation (view);
			var previousSample = stroke.Samples.LastOrDefault ();
			if (Distance (previousSample?.Location, location) < 0.003)
				return;

			var sample = new StrokeSample {
				Timestamp = touch.Timestamp,
				Location = location,
				Coalesced = coalesced,
				Predicted = predicted
			};
			bool collectForce = touch.Type == UITouchType.Stylus || view.TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available;
			if (collectForce)
				sample.Force = touch.Force;

			if (touch.Type == UITouchType.Stylus) {
				var estimatedProperties = touch.EstimatedProperties;
				sample.EstimatedProperties = estimatedProperties;
				sample.EstimatedPropertiesExpectingUpdates = touch.EstimatedPropertiesExpectingUpdates;
				sample.Altitude = touch.AltitudeAngle;
				sample.Azimuth = touch.GetAzimuthAngle (view);

				if (stroke.Samples.Count == 0 && estimatedProperties.HasFlag (UITouchProperties.Azimuth)) {
					stroke.ExpectsAltitudeAzimuthBackfill = true;
				} else if (stroke.ExpectsAltitudeAzimuthBackfill &&
						   !estimatedProperties.HasFlag (UITouchProperties.Azimuth)) {
					for (int index = 0; index < stroke.Samples.Count; index++) {
						var priorSample = stroke.Samples [index];
						var updatedSample = priorSample;

						if (updatedSample.EstimatedProperties.HasFlag (UITouchProperties.Altitude)) {
							updatedSample.EstimatedProperties &= ~UITouchProperties.Altitude;
							updatedSample.Altitude = sample.Altitude;
						}
						if (updatedSample.EstimatedProperties.HasFlag (UITouchProperties.Azimuth)) {
							updatedSample.EstimatedProperties &= ~UITouchProperties.Azimuth;
							updatedSample.Azimuth = sample.Azimuth;
						}
						stroke.Update (updatedSample, index);
					}
					stroke.ExpectsAltitudeAzimuthBackfill = false;
				}
			}

			if (predicted) {
				stroke.AddPredicted (sample);
			} else {
				var index = stroke.Add (sample);
				if (touch.EstimatedPropertiesExpectingUpdates != 0) {
					outstandingUpdateIndexes [touch.EstimationUpdateIndex] = new StrokeIndex {
						Stroke = stroke,
						Index = index
					};
				}
			}
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			if (trackedTouch == null) {
				trackedTouch = (UITouch)touches.FirstOrDefault ();
				initialTimestamp = trackedTouch.Timestamp;

				if (!IsForPencil)
					BeginIfNeeded (null);
					fingerStartTimer = NSTimer.CreateScheduledTimer (cancellationTimeInterval, BeginIfNeeded);
			}
			if (Append (Touches(touches), evt)) {
				if (IsForPencil)
					State = Began;
			}
		}

		// If not for pencil we give other gestures (pan, pinch) a chance by delaying our begin just a little.
		void BeginIfNeeded (NSTimer timer)
		{
			if (State == Possible)
				State = Began;
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			if (Append (Touches(touches), evt)) {
				if (State == Began)
					State = Changed;
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			if (Append (Touches (touches), evt)) {
				Stroke.State = StrokeState.Done;
				State = Ended;
			}
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			if (Append (Touches (touches), evt)) {
				Stroke.State = StrokeState.Cancelled;
				State = Failed;
			}
		}

		public override void TouchesEstimatedPropertiesUpdated (NSSet touches)
		{
			foreach (var touch in touches.Cast<UITouch> ()) {
				StrokeIndex val;
				if (outstandingUpdateIndexes.TryGetValue (touch.EstimationUpdateIndex.Int32Value, out val)) {
					var stroke = val.Stroke;
					var sampleIndex = val.Index;
					var sample = stroke.Samples [sampleIndex];

					var expectedUpdates = sample.EstimatedPropertiesExpectingUpdates;
					// Only force is reported this way as of iOS 10.0
					if (expectedUpdates.HasFlag (UITouchProperties.Force)) {
						sample.Force = touch.Force;

						// Only remove the estimate flag if the new value isn't estimated as well.
						if (!touch.EstimatedProperties.HasFlag (UITouchProperties.Force))
							sample.EstimatedProperties &= ~UITouchProperties.Force;
					}
					sample.EstimatedPropertiesExpectingUpdates = touch.EstimatedPropertiesExpectingUpdates;

					if (touch.EstimatedPropertiesExpectingUpdates == 0)
						outstandingUpdateIndexes.Remove (sampleIndex);
					stroke.Update (sample, sampleIndex);
				}
			}
		}

		public override void Reset ()
		{
			Stroke = new Stroke ();
			trackedTouch = null;

			var timer = fingerStartTimer;
			if (timer != null) {
				timer.Invalidate ();
				fingerStartTimer = null;
			}

			base.Reset ();
		}

		HashSet<UITouch> Touches (NSSet touches)
		{
			return new HashSet<UITouch> (touches.Cast<UITouch> ());
		}
	}
}
