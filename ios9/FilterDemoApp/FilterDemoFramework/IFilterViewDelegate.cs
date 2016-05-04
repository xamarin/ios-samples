namespace FilterDemoFramework {
	public interface IFilterViewDelegate  {
		void ResonanceChanged (FilterView filterView, float resonance);

		void FrequencyChanged (FilterView filterView, float frequency);

		void DataChanged (FilterView filterView);
	}
}

