
namespace SpeedySloth.WatchAppExtension {
	using HealthKit;

	public static class WorkoutTypeExtensions {
		public static string DisplayString (this HKWorkoutActivityType workoutActivityType)
		{
			var result = string.Empty;
			switch (workoutActivityType) {
			case HKWorkoutActivityType.Hiking:
				result = "Hiking";
				break;
			case HKWorkoutActivityType.Running:
				result = "Running";
				break;
			case HKWorkoutActivityType.Walking:
				result = "Walking";
				break;
			default:
				break;
			}

			return result;
		}

		public static string DisplayString (this WorkoutType workoutType)
		{
			var result = string.Empty;
			switch (workoutType) {
			case WorkoutType.Hiking:
				result = "Hiking";
				break;
			case WorkoutType.Running:
				result = "Running";
				break;
			case WorkoutType.Walking:
				result = "Walking";
				break;
			default:
				break;
			}

			return result;
		}

		public static HKWorkoutActivityType Map (this WorkoutType workoutType)
		{
			var result = default (HKWorkoutActivityType);
			switch (workoutType) {
			case WorkoutType.Hiking:
				result = HKWorkoutActivityType.Hiking;
				break;
			case WorkoutType.Running:
				result = HKWorkoutActivityType.Running;
				break;
			case WorkoutType.Walking:
				result = HKWorkoutActivityType.Walking;
				break;
			default:
				break;
			}

			return result;
		}
	}
}
