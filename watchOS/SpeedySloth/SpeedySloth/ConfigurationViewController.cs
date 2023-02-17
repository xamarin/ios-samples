
namespace SpeedySloth {
	using Foundation;
	using HealthKit;
	using SpeedySloth.WatchAppExtension;
	using System;
	using System.Collections.Generic;
	using UIKit;

	public partial class ConfigurationViewController : UIViewController, IUIPickerViewDelegate, IUIPickerViewDataSource {
		private readonly List<WorkoutType> activityTypes = new List<WorkoutType> { WorkoutType.Walking, WorkoutType.Running, WorkoutType.Hiking };

		private readonly List<LocationType> locationTypes = new List<LocationType> { LocationType.Indoor, LocationType.Outdoor };

		public ConfigurationViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.ActivityTypePicker.Delegate = this;
			this.ActivityTypePicker.DataSource = this;

			this.LocationTypePicker.Delegate = this;
			this.LocationTypePicker.DataSource = this;
		}

		#region UIPickerViewDataSource

		public nint GetComponentCount (UIPickerView pickerView)
		{
			return 1;
		}

		public nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			var result = 0;
			if (pickerView == this.ActivityTypePicker) {
				result = this.activityTypes.Count;
			} else if (pickerView == this.LocationTypePicker) {
				result = this.locationTypes.Count;
			}

			return result;
		}

		#endregion

		#region UIPickerViewDelegate

		[Export ("pickerView:titleForRow:forComponent:")]
		public string GetTitle (UIPickerView pickerView, nint row, nint component)
		{
			string result = null;
			if (pickerView == this.ActivityTypePicker) {
				result = this.activityTypes [(int) row].DisplayString ();
			} else if (pickerView == this.LocationTypePicker) {
				result = this.locationTypes [(int) row].DisplayString ();
			}

			return result;
		}

		#endregion

		[Action ("UnwindToConfigurationWithSegue:")]
		public void UnwindToConfigurationWithSegue (UIStoryboardSegue segue) { }

		#region Segues

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "presentWorkoutSegue") {
				this.PrepareForPresentWorkoutSegue (segue);
			}
		}

		private void PrepareForPresentWorkoutSegue (UIStoryboardSegue segue)
		{
			var selectedActivityType = this.activityTypes [(int) this.ActivityTypePicker.SelectedRowInComponent (0)];
			var selectedLocationType = this.locationTypes [(int) this.LocationTypePicker.SelectedRowInComponent (0)];

			var workoutViewController = segue.DestinationViewController as WorkoutViewController;
			if (workoutViewController != null) {
				workoutViewController.Configuration = new HKWorkoutConfiguration {
					ActivityType = selectedActivityType.Map (),
					LocationType = selectedLocationType.Map (),
				};
			}
		}

		#endregion
	}
}
