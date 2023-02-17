using Foundation;
using MonoTouch.Dialog;
using UIKit;

namespace KannadaKeyboardCompanion {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		UIWindow window;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			var web = new WebElement ();
			web.HtmlFile = "instructions";

			var root = new RootElement ("Kannada Keyboard") {
				new Section{
					new UIViewElement("Instruction", web.View, false)
				}
			};

			var dv = new DialogViewController (root) {
				Autorotate = true
			};
			var navigation = new UINavigationController ();
			navigation.PushViewController (dv, true);

			window = new UIWindow (UIScreen.MainScreen.Bounds);

			window.RootViewController = navigation;
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}

