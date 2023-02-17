
namespace SpeedySloth.WatchAppExtension {
	using Foundation;
	using HealthKit;
	using System;
	using WatchKit;

	public partial class SummaryInterfaceController : WKInterfaceController {
		private HKWorkout workout;

		protected SummaryInterfaceController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			this.workout = context as HKWorkout;
			this.SetTitle ("Summary");
		}

		public override void WillActivate ()
		{
			base.WillActivate ();

			if (this.workout != null) {
				var workoutTitle = this.workout.WorkoutActivityType.DisplayString ();
				if (this.workout.Metadata.IndoorWorkout.HasValue) {
					var locationType = this.workout.Metadata.IndoorWorkout.Value ? LocationType.Indoor : LocationType.Outdoor;
					var formatString = NSBundle.MainBundle.GetLocalizedString ("LOCATION_TYPE_%@_WORKOUT_TYPE_%@");
					workoutTitle = string.Format (formatString, locationType.DisplayString (), workoutTitle);
				}

				this.WorkoutLabel.SetText (workoutTitle);
				this.CaloriesLabel.SetText (Utilities.Format (totalEnergyBurned: this.workout.TotalEnergyBurned));
				this.DistanceLabel.SetText (Utilities.Format (totalDistance: this.workout.TotalDistance));
				this.DurationLabel.SetText (Utilities.FormatDuration (this.workout.Duration));
			}
		}

		partial void TapDoneTapped ()
		{
			WKInterfaceController.ReloadRootPageControllers (new string [] { nameof (ConfigurationInterfaceController) },
															null,
															WKPageOrientation.Vertical,
															0);
		}
	}
}
