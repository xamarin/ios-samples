using System;
using System.Linq;
using UIKit;
using MonoTouch.Dialog;
using System.Collections.Generic;
using Xamarin.Code;

namespace Xamarin.Screens.NavTable
{
	public class HomeNavController : UITableViewController
	{
		// declare vars
		protected List<NavItemGroup> navItems = new List<NavItemGroup>();
		protected NavItemTableSource tableSource;
				
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
			NavItemGroup navGroup;
			
			// create the navigation items
			navGroup = new NavItemGroup ("ADO.NET");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Basic ADO.NET", "", typeof(ADONET.BasicOperations)));

			navGroup = new NavItemGroup ("SQLite-Net");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Basic SQLite-Net", "", typeof(SQLiteNet.BasicOperations)));

			navGroup = new NavItemGroup ("Vici CoolStorage");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Basic Vici CoolStorage", "", typeof(ViciCoolStorage.BasicOperations)));
			
			// create a table source from our nav items
			tableSource = new NavItemTableSource (NavigationController, navItems);
			
			// set the source on the table to our data source
			base.TableView.Source = tableSource;
		}
					
	}
}

