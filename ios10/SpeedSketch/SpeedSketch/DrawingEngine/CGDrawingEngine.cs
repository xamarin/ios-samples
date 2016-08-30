using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using UIKit;

using static SpeedSketch.CGMathExtensions;

namespace SpeedSketch
{
	public enum StrokeViewDisplayOptions
	{
		Debug,
		Calligraphy,
		Ink
	}

	class EstimatedSample
	{
		public int Index { get; set; }
		public StrokeSample Sample { get; set; }
	}

	class StrokeCGView : UIView
	{
		StrokeViewDisplayOptions displayOptions;
		public StrokeViewDisplayOptions DisplayOptions {
			get {
				return displayOptions;
			}
			set {
				displayOptions = value;
				if (StrokeCollection != null)
					SetNeedsDisplay ();
				foreach (var view in DirtyRectViews)
					view.Hidden = displayOptions != StrokeViewDisplayOptions.Debug;
			}
		}

		StrokeCollection strokeCollection;
		public StrokeCollection StrokeCollection {
			get {
				return strokeCollection;
			}
			set {
				if (strokeCollection != value)
					SetNeedsDisplay ();

				strokeCollection = value;

				if (strokeCollection != null) {
					var len = strokeCollection.Strokes.Count;
					if (len > 0)
						SetNeedsDisplay (strokeCollection.Strokes [len - 1]);
				}
				StrokeToDraw = strokeCollection?.ActiveStroke;

			}
		}

		Stroke strokeToDraw;
		Stroke StrokeToDraw {
			get {
				return strokeToDraw;
			}
			set {
				var oldValue = strokeToDraw;
				strokeToDraw = value;

				if (oldValue != strokeToDraw && oldValue != null)
					SetNeedsDisplay ();
				else if (strokeToDraw != null)
					SetNeedsDisplay (value);

				strokeToDraw = value;
			}
		}

		// Dirty rect calculation and handling.
		List<UIView> DirtyRectViews { get; }
		EstimatedSample lastEstimatedSample;

		public StrokeCGView (CGRect frame)
			: base(frame)
		{
			Layer.DrawsAsynchronously = true;

			Func<UIView> dirtyRectView = () => {
				var view = new UIView (new CGRect (-10, -10, 0, 0));
				view.Layer.BorderColor = UIColor.Red.CGColor;
				view.Layer.BorderWidth = 0.5f;
				view.UserInteractionEnabled = false;
				view.Hidden = true;
				AddSubview (view);
				return view;
			};

			DirtyRectViews = new List<UIView> {
				dirtyRectView (),
				dirtyRectView ()
			};
		}

		public List<CGRect> DirtyRects (Stroke stroke)
		{
			var result = new List<CGRect> ();
			foreach (var range in stroke.UpdatedRanges ()) {
				var lowerBound = range.LowerBound;
				if (lowerBound > 0)
					lowerBound -= 1;

				var les = lastEstimatedSample;
				if (les != null) {
					if (les.Index < lowerBound)
						lowerBound = les.Index;
				}

				var samples = stroke.Samples;

				var upperBound = range.UpperBound;
				if (upperBound < samples.Count)
					upperBound += 1;

				var dirtyRect = DirtyRectForSampleStride (stroke.Samples.Skip (lowerBound).Take (upperBound - lowerBound));
				result.Add (dirtyRect);
			}

			var predictedSamples = stroke.PredictedSamples;
			if (predictedSamples != null && stroke.PredictedSamples.Count > 0) {
				var dirtyRect = DirtyRectForSampleStride (stroke.PredictedSamples);
				result.Add (dirtyRect);
			}

			var previousPredictedSamples = stroke.PreviousPredictedSamples;
			if (previousPredictedSamples != null && previousPredictedSamples.Count > 0) {
				var dirtyRect = DirtyRectForSampleStride (previousPredictedSamples);
				result.Add (dirtyRect);
			}

			return result;
		}

		CGRect DirtyRectForSampleStride (IEnumerable<StrokeSample> sampleStride)
		{
			var rectNull = new CGRect (nfloat.PositiveInfinity, nfloat.PositiveInfinity, 0, 0);
			return sampleStride.Aggregate (rectNull, (acc, s) => acc.UnionWith (s.Location.ToRect ()))
							   .Inset (-20, -20);
		}

		public void SetNeedsDisplay (Stroke stroke)
		{
			DirtyRects (stroke).ForEach (SetNeedsDisplayInRect);
		}

		#region Drawing methods

		// Note: this is not a particularily efficient way to draw a great stroke path
		// with CoreGraphics.It is just a way to produce an interesting looking result.
		// For a real world example you would reuse and cache CGPaths and draw longer
		// paths instead of an aweful lot of tiny ones, etc.You would also respect the
		// draw rect to cull your draw requests.And you would use bezier paths to
		// interpolate between the points to get a smooother curve.
		void Draw (Stroke stroke)
		{
			var updateRanges = stroke.UpdatedRanges ();
			if (DisplayOptions == StrokeViewDisplayOptions.Debug) {
				for (int index = 0; index < DirtyRectViews.Count; index++) {
					var dirtyRectView = DirtyRectViews [index];
					dirtyRectView.Alpha = 0;
					if (index < updateRanges.Length) {
						dirtyRectView.Alpha = 1;
						var range = updateRanges [index];
						var strokes = stroke.Samples.Skip (range.LowerBound)
											.Take (range.UpperBound - range.LowerBound + 1);
						dirtyRectView.Frame = DirtyRectForSampleStride (strokes);
					}
				}
			}

			lastEstimatedSample = null;
			stroke.ClearUpdateInfo ();
			var sampleCount = stroke.Samples.Count;
			if (sampleCount <= 0)
				return;
			var context = UIGraphics.GetCurrentContext ();
			if (context == null)
				return;
			var strokeColor = UIColor.Black;

			Action lineSettings;
			Action forceEstimatedLineSettings;
			if (displayOptions == StrokeViewDisplayOptions.Debug) {
				lineSettings = () => {
					context.SetLineWidth (0.5f);
					context.SetStrokeColor (UIColor.White.CGColor);
				};
				forceEstimatedLineSettings = () => {
					context.SetLineWidth (0.5f);
					context.SetStrokeColor (UIColor.Blue.CGColor);
				};
			} else {
				lineSettings = () => {
					context.SetLineWidth (0.25f);
					context.SetStrokeColor (strokeColor.CGColor);
				};
				forceEstimatedLineSettings = lineSettings;
			}

			Action azimuthSettings = () => {
				context.SetLineWidth (1.5f);
				context.SetStrokeColor (UIColor.Orange.CGColor);
			};

			Action altitudeSettings = () => {
				context.SetLineWidth (0.5f);
				context.SetStrokeColor (strokeColor.CGColor);
			};
			var forceMultiplier = 2f;
			var forceOffset = 0.1f;

			var fillColorRegular = UIColor.Black.CGColor;
			var fillColorCoalesced = UIColor.LightGray.CGColor;
			var fillColorPredicted = UIColor.Red.CGColor;

			CGVector? lockedAzimuthUnitVector = null;
			var azimuthLockAltitudeThreshold = NMath.PI / 2 * 0.8f; // locking azimuth at 80% altitude

			lineSettings ();

			Func<StrokeSample, nfloat> forceAccessBlock = sample => {
				return sample.ForceWithDefault ();
			};

			if (DisplayOptions == StrokeViewDisplayOptions.Ink) {
				forceAccessBlock = sample => {
					return sample.PerpendicularForce ();
				};
			}

			// Make the force influence less pronounced for the calligraphy pen.
			if (DisplayOptions == StrokeViewDisplayOptions.Calligraphy) {
				var prevGetter = forceAccessBlock;
				forceAccessBlock = sample => {
					return NMath.Max (prevGetter (sample), 1);
				};
				// make force value less pronounced
				forceMultiplier = 1;
				forceOffset = 10;
			}

			var previousGetter = forceAccessBlock;
			forceAccessBlock = sample => {
				return previousGetter (sample) * forceMultiplier + forceOffset;
			};

			StrokeSample heldFromSample = null;
			CGVector? heldFromSampleUnitVector = null;

			Action<StrokeSegment> draw = segment => {
				var toSample = segment.ToSample;
				if (toSample != null) {
					StrokeSample fromSample = heldFromSample ?? segment.FromSample;

					// Skip line segments that are too short.
					var dist = Vector (fromSample.Location, toSample.Location).Quadrance ();
					if (dist < 0.003f) {
						if (heldFromSample == null) {
							heldFromSample = fromSample;
							heldFromSampleUnitVector = segment.FromSampleUnitNormal;
						}
						return;
					}

					if (toSample.Predicted) {
						if (displayOptions == StrokeViewDisplayOptions.Debug)
							context.SetFillColor (fillColorPredicted);
					} else {
						bool coalesced = displayOptions == StrokeViewDisplayOptions.Debug && fromSample.Coalesced;
						context.SetFillColor (coalesced ? fillColorCoalesced : fillColorRegular);
					}

					if (displayOptions == StrokeViewDisplayOptions.Calligraphy) {
						var fromAzimuthUnitVector = Stroke.CalligraphyFallbackAzimuthUnitVector;
						var toAzimuthUnitVector = Stroke.CalligraphyFallbackAzimuthUnitVector;

						if (fromSample.Azimuth.HasValue) {
							if (!lockedAzimuthUnitVector.HasValue)
								lockedAzimuthUnitVector = fromSample.GetAzimuthUnitVector ();
							fromAzimuthUnitVector = fromSample.GetAzimuthUnitVector ();
							toAzimuthUnitVector = toSample.GetAzimuthUnitVector ();

							if (fromSample.Altitude.Value > azimuthLockAltitudeThreshold)
								fromAzimuthUnitVector = lockedAzimuthUnitVector.Value;

							if (toSample.Altitude.Value > azimuthLockAltitudeThreshold)
								toAzimuthUnitVector = lockedAzimuthUnitVector.Value;
							else
								lockedAzimuthUnitVector = toAzimuthUnitVector;
						}

						// Rotate 90 degrees
						var calligraphyTransform = CGAffineTransform.MakeRotation (NMath.PI / 2);
						fromAzimuthUnitVector = fromAzimuthUnitVector.Apply (calligraphyTransform);
						toAzimuthUnitVector = toAzimuthUnitVector.Apply (calligraphyTransform);

						var fromUnitVector = fromAzimuthUnitVector.Mult (forceAccessBlock (fromSample));
						var toUnitVector = toAzimuthUnitVector.Mult (forceAccessBlock (toSample));

						context.BeginPath ();
						context.Move (fromSample.Location.Add (fromUnitVector));
						context.AddLine (toSample.Location.Add (toUnitVector));
						context.AddLine (toSample.Location.Sub (toUnitVector));
						context.AddLine (fromSample.Location.Sub (fromUnitVector));
						context.ClosePath ();

						context.DrawPath (CGPathDrawingMode.FillStroke);
					} else {
						var fromUnitVector = (heldFromSampleUnitVector.HasValue ? heldFromSampleUnitVector.Value : segment.FromSampleUnitNormal).Mult (forceAccessBlock (fromSample));

						var toUnitVector = segment.ToSampleUnitNormal.Mult (forceAccessBlock (toSample));
						var isForceEstimated = fromSample.EstimatedProperties.HasFlag (UITouchProperties.Force)
						                                 || toSample.EstimatedProperties.HasFlag (UITouchProperties.Force);

						if (isForceEstimated) {
							if (lastEstimatedSample == null)
								lastEstimatedSample = new EstimatedSample { Index = segment.FromSampleIndex + 1, Sample = toSample };
							forceEstimatedLineSettings ();
						} else {
							lineSettings ();
						}

						context.BeginPath ();
						context.Move (fromSample.Location.Add (fromUnitVector));
						context.AddLine (toSample.Location.Add (toUnitVector));
						context.AddLine (toSample.Location.Sub (toUnitVector));
						context.AddLine (fromSample.Location.Sub (fromUnitVector));
						context.ClosePath ();
						context.DrawPath (CGPathDrawingMode.FillStroke);
					}

					var isEstimated = fromSample.EstimatedProperties.HasFlag (UITouchProperties.Azimuth);
					if (fromSample.Azimuth.HasValue && (!fromSample.Coalesced || isEstimated) && !fromSample.Predicted && displayOptions == StrokeViewDisplayOptions.Debug) {
						var length = 20f;
						var azimuthUnitVector = fromSample.GetAzimuthUnitVector ();
						var azimuthTarget = fromSample.Location.Add (azimuthUnitVector.Mult (length));

						var altitudeStart = azimuthTarget.Add (azimuthUnitVector.Mult (length / -2));
						var altitudeTarget = altitudeStart.Add ((azimuthUnitVector.Mult (length / 2)).Apply (CGAffineTransform.MakeRotation (fromSample.Altitude.Value)));

						// Draw altitude as black line coming from the center of the azimuth.
						altitudeSettings ();

						context.BeginPath ();
						context.Move (altitudeStart);
						context.AddLine (altitudeTarget);
						context.StrokePath ();

						// Draw azimuth as orange (or blue if estimated) line.
						azimuthSettings ();

						if (isEstimated)
							context.SetStrokeColor (UIColor.Blue.CGColor);
						context.BeginPath ();
						context.Move (fromSample.Location);
						context.AddLine (azimuthTarget);
						context.StrokePath ();
					}

					heldFromSample = null;
					heldFromSampleUnitVector = null;
				}
			};

			if (stroke.Samples.Count == 1) {
				// Construct a face segment to draw for a stroke that is only one point.
				var sample = stroke.Samples [0];

				var tempSampleFrom = new StrokeSample {
					Timestamp = sample.Timestamp,
					Location = sample.Location.Add (new CGVector (-0.5f, 0)),
					Coalesced = false,
					Predicted = false,
					Force = sample.Force,
					Azimuth = sample.Azimuth,
					Altitude = sample.Altitude,
					EstimatedProperties = sample.EstimatedProperties
				};

				var tempSampleTo = new StrokeSample {
					Timestamp = sample.Timestamp,
					Location = sample.Location.Add (new CGVector (0.5f, 0)),
					Coalesced = false,
					Predicted = false,
					Force = sample.Force,
					Azimuth = sample.Azimuth,
					Altitude = sample.Altitude,
					EstimatedProperties = sample.EstimatedProperties
				};

				var segment = new StrokeSegment (tempSampleFrom);
				segment.AdvanceWithSample (tempSampleTo);
				segment.AdvanceWithSample (null);

				draw (segment);
			} else {
				foreach (var segment in stroke)
					draw (segment);
			}
		}

		public override void Draw (CGRect rect)
		{
			UIColor.White.SetFill ();
			UIGraphics.RectFill (rect);

			// Optimization opportunity: Draw the existing collection in a different view, 
			// and only draw each time we add a stroke.
			var collection = StrokeCollection;
			if (collection != null) {
				foreach (var stroke in strokeCollection.Strokes)
					Draw (stroke);
			}

			var toDraw = StrokeToDraw;
			if (toDraw != null)
				Draw (toDraw);
		}

		#endregion
	}
}
