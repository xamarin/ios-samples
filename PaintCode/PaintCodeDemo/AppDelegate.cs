using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace PaintCode
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UINavigationController navCtrlr;
		UITabBarController tabBarController;
		UIViewController news, blue, glossy, lineart;		

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			navCtrlr = new UINavigationController ();
			navCtrlr.NavigationBar.Translucent = false;
			
			blue = new BlueButtonViewController ();
			glossy = new GlossyButtonViewController ();
			lineart = new DrawingViewController ();
			
			news = new NewsDialogViewController ();
//			news.View.Frame = new CoreGraphics.CGRect (0
//						, UIApplication.SharedApplication.StatusBarFrame.Height
//						, UIScreen.MainScreen.ApplicationFrame.Width
//						, UIScreen.MainScreen.ApplicationFrame.Height);

			navCtrlr.PushViewController (news, false);

			
			navCtrlr.TabBarItem = new UITabBarItem ("Calendar", UIImage.FromBundle ("Images/about.png"), 0);
			blue.TabBarItem = new UITabBarItem ("Blue Button", UIImage.FromBundle ("Images/about.png"), 0);
			glossy.TabBarItem = new UITabBarItem ("Glossy Button", UIImage.FromBundle ("Images/about.png"), 0);
			lineart.TabBarItem = new UITabBarItem ("Line Art", UIImage.FromBundle ("Images/about.png"), 0);
			
			tabBarController = new UITabBarController ();
			tabBarController.ViewControllers = new UIViewController [] {
				navCtrlr,
				blue,
				glossy,
				lineart
			};
			
			
			window.RootViewController = tabBarController;

			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

