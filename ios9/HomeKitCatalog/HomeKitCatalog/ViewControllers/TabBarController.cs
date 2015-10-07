using System;

using UIKit;
using Foundation;

namespace HomeKitCatalog
{
	/// <summary>
	/// The `TabBarController` maintains the state of the tabs across app launches.
	/// </summary>
	partial class TabBarController : UITabBarController
	{
		const string StartingTabIndexKey = "TabBarController-StartingTabIndexKey";

		public TabBarController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var userDefaults = NSUserDefaults.StandardUserDefaults;
			var startingIndex = (NSNumber)userDefaults [StartingTabIndexKey];

			if (startingIndex != null)
				SelectedIndex = startingIndex.NIntValue;
		}

		public override void ItemSelected (UITabBar tabbar, UITabBarItem item)
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;
			userDefaults [StartingTabIndexKey] = new NSNumber ((nint)Array.IndexOf (tabbar.Items, item));
		}
	}
}
