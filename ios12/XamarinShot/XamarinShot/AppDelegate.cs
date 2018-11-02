
namespace XamarinShot
{
    using Foundation;
    using System.Linq;
    using XamarinShot.Utils;
    using UIKit;

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var dictionary = NSDictionary.FromObjectsAndKeys(UserDefaults.ApplicationDefaults.Values.Select(value => value).ToArray(), 
                                                             UserDefaults.ApplicationDefaults.Keys.Select(key => new NSString(key)).ToArray());
            NSUserDefaults.StandardUserDefaults.RegisterDefaults(dictionary);

            return true;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return false;
        }
    }
}