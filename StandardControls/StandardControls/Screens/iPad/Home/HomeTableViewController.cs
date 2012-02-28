using System;
using MonoTouch.UIKit;
using Xamarin.Code;
using System.Collections.Generic;

namespace Example_StandardControls.Screens.iPad.Home
{
	public class HomeNavController : UITableViewController
	{
		// declare vars
		List<NavItemGroup> navItems = new List<NavItemGroup>();
		NavItemTableSource tableSource;
		
		public HomeNavController () : base(UITableViewStyle.Grouped)
		{
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// hide the nav bar when this controller appears
			NavigationController.SetNavigationBarHidden (true, true);
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			// show the nav bar when other controllers appear
			NavigationController.SetNavigationBarHidden (false, true);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			
			// create the navigation items
			NavItemGroup navGroup = new NavItemGroup ("Form Controls");
			
			navGroup = new NavItemGroup ("Popups");
			navItems.Add (navGroup);
			//navGroup.Items.Add (new NavItem ("Alert Views", "", typeof(AlertViews.AlertViewsScreen_iPhone)));
			navGroup.Items.Add (new NavItem ("Action Sheets", "", typeof(ActionSheets.ActionSheets_iPad)));

			navGroup = new NavItemGroup ("Pickers");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Simple Date Picker", "", typeof(DatePicker.DatePickerSimple_iPad)));
			//navGroup.Items.Add (new NavItem ("Action Sheet Date Picker", "", typeof(DatePicker.ActionSheetDatePicker_iPad)));

			// create a table source from our nav items
			tableSource = new NavItemTableSource (NavigationController, navItems);
			
			// set the source on the table to our data source
			base.TableView.Source = tableSource;
		}
	}
}

