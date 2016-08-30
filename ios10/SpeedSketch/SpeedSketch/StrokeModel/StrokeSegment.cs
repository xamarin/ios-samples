using CoreGraphics;

using static SpeedSketch.CGMathExtensions;

namespace SpeedSketch
{
	public class StrokeSegment
	{
		public StrokeSample SampleBefore { get; private set; }
		public StrokeSample FromSample { get; private set; }
		public StrokeSample ToSample { get; private set; }
		public StrokeSample SampleAfter { get; private set; }
		public int FromSampleIndex { get; set; }

		CGVector PreviousSegmentStrokeVector {
			get {
				var start = SampleBefore?.Location;
				var end = FromSample.Location;
				return start.HasValue ? Vector(start.Value, end) : SegmentStrokeVector;
			}
		}

		CGVector NextSegmentStrokeVector {
			get {
				var start = ToSample.Location;
				var end = SampleAfter?.Location;
				return end.HasValue ? Vector(start, end.Value) : SegmentStrokeVector;
			}
		}

		CGVector SegmentStrokeVector {
			get {
				return Vector (FromSample.Location, ToSample.Location);
			}
		}

		public StrokeSegment (StrokeSample sample)
		{
			SampleAfter = sample;
			FromSampleIndex = -2;
		}

		public CGVector FromSampleUnitNormal {
			get {
				return InterpolatedNormalUnitVector (PreviousSegmentStrokeVector, SegmentStrokeVector);
			}
		}

		public CGVector ToSampleUnitNormal {
			get {
				return InterpolatedNormalUnitVector (SegmentStrokeVector, NextSegmentStrokeVector);
			}
		}

		public void AdvanceWithSample (StrokeSample incomingSample)
		{
			var sampleAfter = SampleAfter;
			if (sampleAfter != null) {
				SampleBefore = FromSample;
				FromSample = ToSample;
				ToSample = SampleAfter;
				SampleAfter = incomingSample;
				FromSampleIndex += 1;
			}
		}

		static CGVector InterpolatedNormalUnitVector (CGVector vector1, CGVector vector2)
		{
			var n1 = vector1.Normal ();
			var n2 = vector2.Normal ();

			var result = (n1.Add (n2))?.Normalize ();
			if (result.HasValue)
				return result.Value;

			// This means they resulted in a 0,0 vector, 
			// in this case one of the incoming vectors is a good result.
			result = vector1.Normalize ();
			if (result.HasValue)
				return result.Value;

			result = vector2.Normalize ();
			if (result.HasValue)
				return result.Value;

			// This case should not happen.
			return new CGVector (1, 0);
		}
	}
}
