using System;
using MonoTouch.UIKit;

namespace Example_ContentControls.Screens.iPhone.Tabs
{
	public class TabBarController : UITabBarController
	{
		// screens
		UINavigationController browsersTabNavController;
		Browsers.BrowsersHome browsersHome;
		Search.SearchScreen searchScreen;
		UINavigationController mapsTabNavController;
		Maps.MapsHome mapsHome;
		UINavigationController customizeNavBarNavController;
		CustomizingNavBar.CustomizingNavBarScreen customizingNavBarScreen;
		
		/// <summary>
		/// 
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		
			// browsers tab
			// in this case, we create a navigation controller and then add our screen 
			// to that
			browsersTabNavController = new UINavigationController();
			browsersTabNavController.TabBarItem = new UITabBarItem();
			browsersTabNavController.TabBarItem.Title = "Browsers";
			browsersHome = new Browsers.BrowsersHome();
			browsersTabNavController.PushViewController(browsersHome, false);
			
			// maps tab
			mapsTabNavController = new UINavigationController();
			mapsTabNavController.TabBarItem = new UITabBarItem();
			mapsTabNavController.TabBarItem.Title = "Maps";
			mapsHome = new Maps.MapsHome();
			mapsTabNavController.PushViewController(mapsHome, false);
			
			// search
			searchScreen = new Search.SearchScreen();
			searchScreen.TabBarItem = new UITabBarItem(UITabBarSystemItem.Search, 1);
		
			// custom nav bar
			customizeNavBarNavController = new UINavigationController();
			customizeNavBarNavController.TabBarItem = new UITabBarItem();
			customizeNavBarNavController.TabBarItem.Title = "Nav";
			customizingNavBarScreen = new CustomizingNavBar.CustomizingNavBarScreen();
			customizeNavBarNavController.PushViewController(customizingNavBarScreen, false);
			
			// set a badge, just for fun
			customizeNavBarNavController.TabBarItem.BadgeValue = "3";
			
			
			// create our array of controllers
			var viewControllers = new UIViewController[] {
				browsersTabNavController,
				mapsTabNavController,
				searchScreen,
				customizeNavBarNavController,
				new ExtraScreens.CustomizableTabScreen() { Number = "1" },
				new ExtraScreens.CustomizableTabScreen() { Number = "2" },
				new ExtraScreens.CustomizableTabScreen() { Number = "3" },
				new ExtraScreens.CustomizableTabScreen() { Number = "4" },
				new ExtraScreens.CustomizableTabScreen() { Number = "5" }
			};
			
			// create an array of customizable controllers from just the 
			// ones we want to customize. 
			var customizableControllers = new UIViewController[] {
				viewControllers[2],
				viewControllers[3],
				viewControllers[4],
				viewControllers[5],
				viewControllers[6]
			};
			
			// attach the view controllers
			this.ViewControllers = viewControllers;
			
			// tell the tab bar which controllers are allowed to customize. if we 
			// don't set this, it assumes all controllers are customizable.
			CustomizableViewControllers = customizableControllers;
			
			// set our selected item
			SelectedViewController = browsersTabNavController;
			
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

	}
}

