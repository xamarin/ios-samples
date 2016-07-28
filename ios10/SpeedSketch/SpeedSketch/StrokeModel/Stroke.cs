using System;
using System.Collections;
using System.Collections.Generic;

using CoreGraphics;

namespace SpeedSketch
{
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

		public List<StrokeSample> PreviousPredictedSamples { get; private set; } = new List<StrokeSample> ();

		public StrokeState State { get; set; } = StrokeState.Active;
		HashSet<int> sampleIndicesExpectingUpdates = new HashSet<int> ();

		int? hasUpdatesFromStartTo;
		int? hasUpdatesAtEndFrom;

		public bool ExpectsAltitudeAzimuthBackfill { get; set; }

		Action receivedAllNeededUpdatesBlock;

		public int Add (StrokeSample sample)
		{
			var resultIndex = Samples.Count;
			if (!hasUpdatesAtEndFrom.HasValue)
				hasUpdatesAtEndFrom = resultIndex;

			Samples.Add (sample);

			if (PreviousPredictedSamples == null)
				PreviousPredictedSamples = new List<StrokeSample> (PredictedSamples);

			if (sample.EstimatedPropertiesExpectingUpdates != 0)
				sampleIndicesExpectingUpdates.Add (resultIndex);

			PredictedSamples.Clear ();
			return resultIndex;
		}

		public void Update (StrokeSample sample, int index)
		{
			if (index == 0)
				hasUpdatesFromStartTo = 0;
			else if (hasUpdatesFromStartTo.HasValue && index == hasUpdatesFromStartTo.Value + 1)
				hasUpdatesFromStartTo = index;
			else if (!hasUpdatesAtEndFrom.HasValue || hasUpdatesAtEndFrom.Value > index)
				hasUpdatesAtEndFrom = index;

			Samples [index] = sample;

			sampleIndicesExpectingUpdates.Remove (index);
			if (sampleIndicesExpectingUpdates.Count == 0) {
				receivedAllNeededUpdatesBlock?.Invoke ();
				receivedAllNeededUpdatesBlock = null;
			}
		}

		public void AddPredicted (StrokeSample sample)
		{
			PredictedSamples.Add (sample);
		}

		public void ClearUpdateInfo ()
		{
			hasUpdatesFromStartTo = null;
			hasUpdatesAtEndFrom = null;
			PreviousPredictedSamples = null;
		}

		public CountableClosedRange [] UpdatedRanges ()
		{
			if (!hasUpdatesFromStartTo.HasValue && !hasUpdatesAtEndFrom.HasValue)
				return new CountableClosedRange [0];

			if (!hasUpdatesFromStartTo.HasValue)
				return new CountableClosedRange [] { new CountableClosedRange (hasUpdatesAtEndFrom.Value, Samples.Count - 1) };

			if (!hasUpdatesAtEndFrom.HasValue)
				return new CountableClosedRange [] { new CountableClosedRange (0, hasUpdatesFromStartTo.Value) };

			return new CountableClosedRange [] {
				new CountableClosedRange(0,hasUpdatesFromStartTo.Value),
				new CountableClosedRange(hasUpdatesAtEndFrom.Value, Samples.Count - 1)
			};
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
}
