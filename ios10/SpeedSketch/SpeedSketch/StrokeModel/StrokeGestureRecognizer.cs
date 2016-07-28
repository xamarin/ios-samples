using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using static UIKit.UIGestureRecognizerState;

namespace SpeedSketch
{
	public class StrokeGestureRecognizer : UIGestureRecognizer
	{
		class StrokeIndex
		{
			public Stroke Stroke { get; set; }
			public int Index { get; set; }
		}

		// Configuration.
		bool collectsCoalescedTouches = true;
		bool usesPredictedSamples = true;

		public bool isForPencil;
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
		readonly Dictionary<int, StrokeIndex> outstandingUpdateIndexes = new Dictionary<int, StrokeIndex> ();
		Stroke stroke = new Stroke ();
		UIView coordinateSpaceView;

		// State.
		UITouch trackedTouch;
		double initialTimestamp;
		bool collectForce;

		NSTimer fingerStartTimer;
		double cancellationTimeInterval = TimeSpan.FromSeconds (0.1).TotalMilliseconds;

		public StrokeGestureRecognizer ()
		{
		}

		public bool Append (HashSet<UITouch> touches, UIEvent uievent)
		{
			var touchToAppend = trackedTouch;
			if (touchToAppend != null) {
				// Cancel the stroke recognition if we get a second touch during cancellation period.
				foreach (var touch in touches) {
					if (touch != touchToAppend && (touch.Timestamp - initialTimestamp < cancellationTimeInterval))
						State = (State == Possible) ? Failed : Cancelled;
					return false;
				}

				// See if those touches contain our tracked touch. If not, ignore gracefully.
				if (touches.Contains (touchToAppend)) {
					Action<Stroke, UITouch, UIView, bool, bool> collector = (stroke, touch, view, coalesced, predicted) => {

						// Only collect samples that actually moved in 2D space.
						var location = touch.GetPreciseLocation (view);
						var previousSample = stroke.Samples.LastOrDefault ();
						if (previousSample != null) {
							if (previousSample.Location.Sub (location).Quadrance () < 0.003)
								return;
						}

						var builder = new StrokeSampleBuilder ()
							.TimeStamp (touch.Timestamp)
							.Location (location)
							.Coalesced (coalesced)
							.Predicted (predicted);
						if (collectForce)
							builder.Force (touch.Force);
						var sample = builder.Create ();

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
								outstandingUpdateIndexes [(int)touch.EstimationUpdateIndex] = new StrokeIndex {
									Stroke = stroke,
									Index = index
								};
							}
						}
					};

					var v = coordinateSpaceView;
					if (v == null)
						throw new InvalidProgramException ();

					if (collectsCoalescedTouches) {
						if (uievent != null) {
							var coalescedTouches = uievent.GetCoalescedTouches (touchToAppend);
							var lastIndex = coalescedTouches.Length - 1;

							for (var index = 0; index < lastIndex; index++)
								collector (stroke, coalescedTouches [index], v, true, false);
							collector (stroke, coalescedTouches [lastIndex], v, false, false);
						}
					} else {
						collector (stroke, touchToAppend, v, false, false);
					}

					if (usesPredictedSamples && stroke.State == StrokeState.Active) {
						var predictedTouches = uievent.GetPredictedTouches (touchToAppend);
						if (predictedTouches != null) {
							foreach (var touch in predictedTouches) {
								collector (stroke, touch, v, false, true);
							}
						}
					}
					return true;
				}
			}
			return false;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			if (trackedTouch == null) {
				trackedTouch = (UITouch)touches.FirstOrDefault ();
				initialTimestamp = trackedTouch.Timestamp;
				collectForce = trackedTouch.Type == UITouchType.Stylus || View.TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available;
				if (!isForPencil)
					fingerStartTimer = NSTimer.CreateScheduledTimer (cancellationTimeInterval, BeginIfNeeded);
			}
			if (Append (Touches(touches), evt)) {
				if (isForPencil)
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
				stroke.State = StrokeState.Done;
				State = Ended;
			}
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			if (Append (Touches (touches), evt)) {
				stroke.State = StrokeState.Cancelled;
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
			stroke = new Stroke ();
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

		NSNumber [] TouchTypes (UITouchType type)
		{
			return new NSNumber [] { new NSNumber ((long)type) };
		}
	}
}
