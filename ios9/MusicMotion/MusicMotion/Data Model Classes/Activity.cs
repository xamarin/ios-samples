using System;

using CoreMotion;

namespace MusicMotion {
	public class Activity {

		const float MilesPerMeter = 0.000621371192f;

		DateTime startDate;
		DateTime endDate;
		double timeInterval;

		public int NumberOfSteps { get; private set; }

		public int Distance { get; private set; }

		public int FloorsAscended { get; private set; }

		public int FloorsDescended { get; private set; }

		public CMMotionActivity MotionActivity { get; private set; }

		public string StartDateDescription =>
			startDate.ToString ("d MMM yyyy HH:mm:ss");

		public string EndDateDescription =>
			endDate.ToString ("d MMM yyyy HH:mm:ss");

		public string ActivityDuration =>
			TimeSpan.FromSeconds (timeInterval).ToString ("h'h 'm'm 's's'");

		public string DistanceInMiles =>
			(Distance == 0) ? "N/A" : string.Format ("0.#######", Distance * Activity.MilesPerMeter);

		public string CalculatedPace {
			get {
				if (Distance == 0)
					return "N/A";
				
				var miles = Distance * Activity.MilesPerMeter;
				var paceInSecondsPerMile = timeInterval / miles;
				return TimeSpan.FromSeconds (paceInSecondsPerMile).ToString ("h'h 'm'm 's's'");
			}
		}

		public string ActivityType {
			get {
				if (MotionActivity.Walking)
					return "Walking";
				if (MotionActivity.Running)
					return "Running";
				if (MotionActivity.Automotive)
					return "Automotive";
				if (MotionActivity.Cycling)
					return "Cycling";
				if (MotionActivity.Stationary)
					return "Stationary";

				return "Unknown";
			}
		}

		public Activity (CMMotionActivity activity, DateTime startDate, DateTime endDate, CMPedometerData pedometerData)
		{
			MotionActivity = activity;
			this.startDate = startDate;
			this.endDate = endDate;
			timeInterval = (endDate - startDate).TotalSeconds;

			if (!activity.Walking && !activity.Running)
				return;

			NumberOfSteps = pedometerData.NumberOfSteps.Int32Value;
			Distance = pedometerData.Distance.Int32Value;
			FloorsAscended = pedometerData.FloorsAscended.Int32Value;
			FloorsDescended = pedometerData.FloorsDescended.Int32Value;
		}
	}
}

