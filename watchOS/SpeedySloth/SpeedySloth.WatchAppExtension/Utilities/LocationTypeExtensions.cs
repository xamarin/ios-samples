
namespace SpeedySloth.WatchAppExtension {
	using HealthKit;

	public static class LocationTypeExtensions {
		public static string DisplayString (this LocationType locationType)
		{
			var result = string.Empty;
			switch (locationType) {
			case LocationType.Indoor:
				result = "Indoor";
				break;
			case LocationType.Outdoor:
				result = "Outdoor";
				break;
			}

			return result;
		}

		public static HKWorkoutSessionLocationType Map (this LocationType locationType)
		{
			var result = default (HKWorkoutSessionLocationType);
			switch (locationType) {
			case LocationType.Indoor:
				result = HKWorkoutSessionLocationType.Indoor;
				break;
			case LocationType.Outdoor:
				result = HKWorkoutSessionLocationType.Outdoor;
				break;
			}

			return result;
		}
	}
}
