using Foundation;
using UIKit;

namespace AVMetadataRecordPlay
{

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            
            UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
            return true;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
        }

        public override void WillEnterForeground(UIApplication application)
        {
            UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
        }

        public override void WillTerminate(UIApplication application)
        {
            UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
        }
    }
}

