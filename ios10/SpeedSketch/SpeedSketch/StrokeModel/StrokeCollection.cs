using System;
using System.Collections;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace SpeedSketch
{
	public class StrokeCollection
	{
		public Stroke ActiveStroke { get; private set; }

		public List<Stroke> Strokes { get; } = new List<Stroke> ();

		void TakeActiveStroke ()
		{
			throw new NotImplementedException ();
		//	if let stroke = activeStroke {
		//		strokes.append (stroke)
	
		//	activeStroke = nil
	
		//}
		}
	}

	public class StrokeSample
	{
		public double Timestamp { get; set; }
		public CGPoint Location { get; set; }

		// Values for debug display.
		public bool Coalesced { get; set; }
		public bool Predicted { get; set; }

		// 3D Touch or Pencil.
		public nfloat Force { get; set; }


		// Pencil only.
		public UITouchProperties EstimatedProperties { get; set; } = new UITouchProperties ();
		public UITouchProperties EstimatedPropertiesExpectingUpdates { get; set; } = new UITouchProperties ();
		public nfloat? Altitude { get; set; }
		public nfloat? Azimuth { get; set; }

		public CGVector AzimuthUnitVector {
			get {
				return new CGVector (1, 1).Apply (CGAffineTransform.MakeRotation (Azimuth.Value));
			}
		}

		public StrokeSample (double timestamp, CGPoint location,
		                     bool coalesced, bool predicted, nfloat? force,
		                     nfloat? azimuth, nfloat? altitude
		                     /*,
		                     List<UITouchProperties> estimatedProperties,
		                     List<UITouchProperties> estimatedPropertiesExpectingUpdates*/)
		{
			throw new NotImplementedException ();
		}


		public nfloat ForceWithDefault ()
		{
			throw new NotImplementedException ();
		}

		public nfloat PerpendicularForce ()
		{
			throw new NotImplementedException ();
		}
	}

	public class StrokeSampleBuilder
	{
		public StrokeSampleBuilder TimeStamp (double timestamp)
		{
			throw new NotImplementedException ();
		}

		public StrokeSampleBuilder Location (CGPoint location)
		{
			throw new NotImplementedException ();
		}

		public StrokeSampleBuilder Coalesced (bool coalesced)
		{
			throw new NotImplementedException ();
		}

		public StrokeSampleBuilder Predicted (bool predicted)
		{
			throw new NotImplementedException ();
		}

		public StrokeSampleBuilder Force (nfloat force)
		{
			throw new NotImplementedException ();
		}

		public StrokeSampleBuilder Azimuth (nfloat azimuth)
		{
			throw new NotImplementedException ();
		}

		public StrokeSampleBuilder Altitude (nfloat altitude)
		{
			throw new NotImplementedException ();
		}

		public StrokeSampleBuilder EstimatedProperties (UITouchProperties props)
		{
			throw new NotImplementedException ();
		}

		public StrokeSample Create ()
		{
			throw new NotImplementedException ();
		}
	}

	public enum StrokeState
	{
		Active,
		Done,
		Cancelled
	}


	public class Stroke : IEnumerable<StrokeSegment>
	{
		public static CGVector CalligraphyFallbackAzimuthUnitVector = new CGVector (1, 1).Normalize ().Value;

		public List<StrokeSample> Samples { get; } = new List<StrokeSample> ();

		public List<StrokeSample> PredictedSamples { get; } = new List<StrokeSample> ();

		public List<StrokeSample> PreviousPredictedSamples { get; } = new List<StrokeSample> ();

		public StrokeState State { get; set; } = StrokeState.Active;
		// var sampleIndicesExpectingUpdates = Set<Int>()

	//var hasUpdatesFromStartTo: Int?
	//var hasUpdatesAtEndFrom: Int?
		public bool ExpectsAltitudeAzimuthBackfill { get; set; }

		public int Add (StrokeSample sample)
		{
			throw new NotImplementedException ();
//        let resultIndex = samples.count
//        if hasUpdatesAtEndFrom == nil {
//            hasUpdatesAtEndFrom = resultIndex
//	}
//	samples.append(sample)
//        if previousPredictedSamples == nil {
//            previousPredictedSamples = predictedSamples
//}
//        if sample.estimatedPropertiesExpectingUpdates != [] {
//            sampleIndicesExpectingUpdates.insert(resultIndex)
//        }
//        predictedSamples.removeAll()
//        return resultIndex
    }


		public void AddPredicted (StrokeSample sample)
		{
			throw new NotImplementedException ();
		}

		public void Update (StrokeSample sample, int at)
		{
			throw new NotImplementedException ();
		}

		public CountableClosedRange [] UpdatedRanges ()
		{
			throw new NotImplementedException ();
			//        guard hasUpdatesFromStartTo != nil || hasUpdatesAtEndFrom != nil else { return []
			//	}
			//        if hasUpdatesFromStartTo == nil {
			//            return [(hasUpdatesAtEndFrom!)...(samples.count - 1)]
			//} else if hasUpdatesAtEndFrom == nil {
			//            return [0...(hasUpdatesFromStartTo!)]
			//        } else {
			//            return [0...(hasUpdatesFromStartTo!), hasUpdatesAtEndFrom!...(samples.count - 1)]
			//        }
		}

		internal void ClearUpdateInfo ()
		{
			throw new NotImplementedException ();
		}

		public IEnumerator<StrokeSegment> GetEnumerator ()
		{
			throw new NotImplementedException ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			throw new NotImplementedException ();
		}
	}

	public class StrokeSegment
	{

		public StrokeSample SampleBefore { get; set; }
		public StrokeSample FromSample { get; set; }
		public StrokeSample ToSample { get; set; }
		public StrokeSample SampleAfter { get; set; }
		public int FromSampleIndex { get; set; }

		public StrokeSegment (StrokeSample sample)
		{
			SampleAfter = sample;
			FromSampleIndex = -2;
		}

		public CGVector FromSampleUnitNormal()
		{
			throw new NotImplementedException ();
        //return interpolatedNormalUnitVector (between: previousSegmentStrokeVector, and: segmentStrokeVector)
		}

		public CGVector ToSampleUnitNormal {
			get {
				throw new NotImplementedException ();
				//return interpolatedNormalUnitVector (between: segmentStrokeVector, and: nextSegmentStrokeVector)
			}

		}

		public bool AdvanceWithSample (StrokeSample sample)
		{
			throw new NotImplementedException ();
		}


}
}
