using System;
using UIKit;
using Example_SharedResources.Code;
using System.Collections.Generic;

namespace Example_SharedResources.Screens.iPhone.Home
{
	public class HomeNavController : UITableViewController
	{
		// declare vars
		List<NavItemGroup> navItems = new List<NavItemGroup>();
		NavItemTableSource tableSource;
		UITableView tableView;
		
		public HomeNavController () : base(UITableViewStyle.Grouped)
		{
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			tableView.Frame = View.Frame;
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// hide the nav bar when this controller appears
			this.NavigationController.SetNavigationBarHidden (true, true);
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			// show the nav bar when other controllers appear
			this.NavigationController.SetNavigationBarHidden (false, true);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// declare vars
			NavItemGroup navGroup;
			
			// create the navigation items
			navGroup = new NavItemGroup ("Network");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Activity Indicator", "", typeof (Network.ActivityIndicatorScreen)));

			navGroup = new NavItemGroup ("Battery");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Status", "", typeof (Battery.BatteryStatusScreen)));
			
			navGroup = new NavItemGroup ("Contacts/Address Book");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Contact Picker/View Contact", "", typeof (Contacts.ContactPickerScreen)));
			navGroup.Items.Add (new NavItem ("New/Unknown Contact", "", typeof (Contacts.NewAndUnknownContactScreen)));
			navGroup.Items.Add (new NavItem ("Address Book", "", typeof (Contacts.AddressBookScreen)));
			
			navGroup = new NavItemGroup ("Assets Library");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Photo/Video Album List", "", typeof (AVAssets.AssetGroupEnumerationScreen)));
			
			navGroup = new NavItemGroup ("Photos and Camera Controllers");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Test Overlay", "", typeof (Photos.TestCameraOverlayController)));
			navGroup.Items.Add (new NavItem ("Photo/Camera Picker", "", typeof (Photos.ImagePickerScreen)));
			navGroup.Items.Add (new NavItem ("Custom Camera Overlay", "", typeof (Photos.CustomCameraViewScreen)));
			
			navGroup = new NavItemGroup ("Accelerometer");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("XYZ Data", "", typeof (Accelerometer.XYZDataScreen)));
			navGroup.Items.Add (new NavItem ("Shake Motion", "", typeof (Accelerometer.ShakeScreen)));
			
			navGroup = new NavItemGroup ("File System");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("File system info", "", typeof (FileSystem.FileSystemInfo)));

			// create a table source from our nav items
			tableSource = new NavItemTableSource (this.NavigationController, navItems);
			
			// set the source on the table to our data source
			tableView = new UITableView ();
			tableView.Source = tableSource;
			View.AddSubview (tableView);
		}
	}
}

