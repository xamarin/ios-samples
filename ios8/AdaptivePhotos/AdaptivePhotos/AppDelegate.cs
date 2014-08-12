using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Reflection;

namespace AdaptivePhotos
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		private UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var url = NSBundle.MainBundle.PathForResource ("User", "plist");
			var userDictionary = NSDictionary.FromFile (url);
			var user = AAPLUser.UserWithDictionary (userDictionary);

			var controller = new CustomSplitViewController ();
			controller.Delegate = new SplitViewControllerDelegate ();

			var master = new AAPLListTableViewController (user);
			var masterNav = new CustomNavigationController (master);
			var detail = new AAPLEmptyViewController ();

			controller.ViewControllers = new UIViewController[] { masterNav, detail };
			controller.PreferredDisplayMode = UISplitViewControllerDisplayMode.AllVisible;

			var traitController = new AAPLTraitOverrideViewController () {
				ViewController = controller
			};

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = traitController;
			window.MakeKeyAndVisible ();
			
			return true;
		}

		public class SplitViewControllerDelegate : UISplitViewControllerDelegate
		{
			public override bool CollapseSecondViewController (UISplitViewController splitViewController,
			                                                   UIViewController secondaryViewController, UIViewController primaryViewController)
			{
				AAPLPhoto photo = ((CustomViewController)secondaryViewController).Aapl_containedPhoto (null);
				if (photo == null) {
					return true;
				}

				if (primaryViewController.GetType () == typeof(CustomNavigationController)) {
					var viewControllers = new List<UIViewController> ();
					foreach (var controller in ((UINavigationController)primaryViewController).ViewControllers) {
						var type = controller.GetType ();
						MethodInfo method = type.GetMethod ("Aapl_containsPhoto");

						if ((bool)method.Invoke (controller, new object[] { null })) {
							viewControllers.Add (controller);
						}
					}

					((UINavigationController)primaryViewController).ViewControllers = viewControllers.ToArray<UIViewController> ();
				}

				return false;
			}

			public override UIViewController SeparateSecondaryViewController (UISplitViewController splitViewController,
																			  UIViewController primaryViewController)
			{
				if (primaryViewController.GetType () == typeof(CustomNavigationController)) {
					foreach (var controller in ((CustomNavigationController)primaryViewController).ViewControllers) {
						var type = controller.GetType ();
						MethodInfo method = type.GetMethod ("Aapl_containedPhoto");

						if (method.Invoke (controller, new object[] { null }) != null) {
							return null;
						}
					}
				}

				return new AAPLEmptyViewController ();
			}
		}
	}
}

