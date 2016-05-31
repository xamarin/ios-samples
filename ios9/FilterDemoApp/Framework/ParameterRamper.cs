namespace FilterDemoFramework {
	public class ParameterRamper {
		float inverseSlope;
		int samplesRemaining;

		float goal;
		public float Goal {
			get {
				return goal;
			}
		}

		public ParameterRamper (float value)
		{
			Set (value);
		}

		public void Set (float value)
		{
			goal = value;
			inverseSlope = 0.0f;
			samplesRemaining = 0;
		}

		public void StartRamp (float newGoal, int duration)
		{
			if (duration == 0)
				Set (newGoal);
			else {
				inverseSlope = (Get () - newGoal) / duration;
				samplesRemaining = duration;
				goal = newGoal;
			}
		}

		public float Get ()
		{
			return inverseSlope * samplesRemaining + goal;
		}

		public float GetStep ()
		{
			if (samplesRemaining == 0)
				return goal;

			float value = Get ();
			--samplesRemaining;
			return value;
		}
	}
}

