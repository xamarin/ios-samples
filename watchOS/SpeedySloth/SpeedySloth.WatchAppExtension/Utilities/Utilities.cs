
namespace SpeedySloth.WatchAppExtension {
	using Foundation;
	using HealthKit;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class Utilities {
		public static string Format (HKQuantity totalDistance = null, HKQuantity totalEnergyBurned = null)
		{
			var result = string.Empty;
			if (totalDistance != null) {
				result = Format (totalDistance: totalDistance.GetDoubleValue (HKUnit.Meter));
			} else if (totalEnergyBurned != null) {
				result = Format (totalEnergyBurned: totalEnergyBurned.GetDoubleValue (HKUnit.Kilocalorie));
			}

			return result;
		}

		public static string Format (double totalDistance = -1d, double totalEnergyBurned = -1d)
		{
			var result = string.Empty;
			if (totalDistance != -1d) {
				result = FormatDistance (totalDistance);
			} else if (totalEnergyBurned != -1d) {
				result = FormatEnergy (totalEnergyBurned);
			}

			return result;
		}

		private static string FormatDistance (double distance)
		{
			var metersString = NSBundle.MainBundle.GetLocalizedString ("Meters");
			var formatString = NSBundle.MainBundle.GetLocalizedString ("VALUE_%@_UNIT_%@");

			return string.Format (formatString, distance, metersString);
		}

		private static string FormatEnergy (double energyBurned)
		{
			var caloriesString = NSBundle.MainBundle.GetLocalizedString ("Calories");
			var formatString = NSBundle.MainBundle.GetLocalizedString ("VALUE_%@_UNIT_%@");

			return string.Format (formatString, energyBurned, caloriesString);
		}

		public static string FormatDuration (double duration)
		{
			var durationFormatter = new NSDateComponentsFormatter {
				UnitsStyle = NSDateComponentsFormatterUnitsStyle.Positional,
				ZeroFormattingBehavior = NSDateComponentsFormatterZeroFormattingBehavior.Pad,
				AllowedUnits = NSCalendarUnit.Second | NSCalendarUnit.Minute | NSCalendarUnit.Hour,
			};

			return durationFormatter.StringFromTimeInterval (duration) ?? string.Empty;
		}

		public static double ComputeDurationOfWorkout (List<HKWorkoutEvent> workoutEvents, NSDate startDate, NSDate endDate)
		{
			var duration = 0d;

			if (startDate != null) {
				if (workoutEvents != null) {
					var lastDate = startDate;

					var events = workoutEvents.Where (@event => @event.Type == HKWorkoutEventType.Pause || @event.Type == HKWorkoutEventType.Resume).ToList ();
					foreach (var @event in events) {
						switch (@event.Type) {
						case HKWorkoutEventType.Pause:
							duration += ((DateTime) @event.DateInterval.StartDate - ((DateTime) lastDate)).TotalSeconds;
							break;
						case HKWorkoutEventType.Resume:
							lastDate = @event.DateInterval.StartDate;
							break;
						}
					}

					if (events.LastOrDefault ()?.Type != HKWorkoutEventType.Pause) {
						var end = endDate ?? new NSDate ();
						duration += ((DateTime) end - ((DateTime) lastDate)).TotalSeconds;
					}
				}
			}

			return duration;
		}
	}
}
