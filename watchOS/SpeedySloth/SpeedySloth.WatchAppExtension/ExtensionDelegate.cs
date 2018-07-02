
namespace SpeedySloth.WatchAppExtension
{
    using Foundation;
    using HealthKit;
    using WatchKit;

    [Register("ExtensionDelegate")]
    public class ExtensionDelegate : WKExtensionDelegate
    {
        public override void HandleWorkoutConfiguration(HKWorkoutConfiguration workoutConfiguration)
        {
            WKInterfaceController.ReloadRootPageControllers(new string[] { nameof(WorkoutInterfaceController) },
                                                            new NSObject[] { workoutConfiguration },
                                                            WKPageOrientation.Vertical,
                                                            0);
        }
    }
}