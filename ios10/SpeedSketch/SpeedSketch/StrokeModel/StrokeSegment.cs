using CoreGraphics;

namespace SpeedSketch
{
	public class StrokeSegment
	{
		public StrokeSample SampleBefore { get; set; }
		public StrokeSample FromSample { get; set; }
		public StrokeSample ToSample { get; set; }
		public StrokeSample SampleAfter { get; set; }
		public int FromSampleIndex { get; set; }

		CGVector PreviousSegmentStrokeVector {
			get {
				var sb = SampleBefore;
				if (sb != null)
					return FromSample.Location.Sub (sb.Location);
				return SegmentStrokeVector;
			}
		}

		CGVector SegmentStrokeVector {
			get {
				return ToSample.Location.Sub (FromSample.Location);
			}
		}

		CGVector NextSegmentStrokeVector {
			get {
				var sampleAfter = SampleAfter;
				if (sampleAfter != null)
					return sampleAfter.Location.Sub (ToSample.Location);
				return SegmentStrokeVector;
			}
		}

		public StrokeSegment (StrokeSample sample)
		{
			SampleAfter = sample;
			FromSampleIndex = -2;
		}

		public CGVector FromSampleUnitNormal ()
		{
			return InterpolatedNormalUnitVector (PreviousSegmentStrokeVector, SegmentStrokeVector);
		}

		public CGVector ToSampleUnitNormal {
			get {
				return InterpolatedNormalUnitVector (SegmentStrokeVector, NextSegmentStrokeVector);
			}
		}

		public bool AdvanceWithSample (StrokeSample incomingSample)
		{
			var sampleAfter = SampleAfter;
			if (sampleAfter != null) {
				SampleBefore = FromSample;
				FromSample = ToSample;
				ToSample = SampleAfter;
				SampleAfter = incomingSample;
				FromSampleIndex += 1;
				return true;
			}
			return false;
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
