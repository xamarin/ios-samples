
namespace XamarinShot.Models {
	public static class GameTime {
		//! The start time of the app.
		// Note: Uninitialized time/startTime are set to -1.0 so it can be checked in a lazy initialization
		private static double StartTime = -1d;

		private static double LevelStartTime = -1d;

		//! The time given by the renderer's updateAtTime
		public static double Time { get; private set; } = -1d;

		//! The time since the app started
		public static double TimeSinceStart { get; private set; } = 0d;

		//! The time changed since last frame
		public static double DeltaTime { get; private set; } = 0d;

		//! The frame count since the app started
		public static int FrameCount { get; private set; } = 0;

		public static double TimeSinceLevelStart => GameTime.Time - LevelStartTime;

		public static void SetLevelStartTime ()
		{
			LevelStartTime = GameTime.Time;
		}

		public static void UpdateAtTime (double time)
		{
			if (StartTime == -1d) {
				StartTime = time;
				Time = time;
				return;
			}

			DeltaTime = time - Time;
			TimeSinceStart = time - StartTime;
			Time = time;
			FrameCount += 1;
		}
	}
}
