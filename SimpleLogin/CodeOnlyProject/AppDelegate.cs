using Foundation;
using UIKit;

namespace CodeOnlyDemo
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            var controller = new CustomViewController();

            window.RootViewController = controller;

            // make the window visible
            window.MakeKeyAndVisible();
			
            return true;
        }
    }
}

