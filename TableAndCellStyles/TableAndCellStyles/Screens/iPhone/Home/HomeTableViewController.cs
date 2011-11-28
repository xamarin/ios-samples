using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Example_TableAndCellStyles.Code.NavigationTable;
using Example_TableAndCellStyles.Screens.iPhone;

namespace Example_TableAndCellStyles.Screens.iPhone.Home
{
	public class HomeNavController : UITableViewController
	{
		// declare vars
		List<NavItemGroup> navItems = new List<NavItemGroup> ();
		NavItemTableSource tableSource;
		
		public HomeNavController () : base(UITableViewStyle.Grouped)
		{
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
			
			
			// create the navigation items
			NavItemGroup navGroup = new NavItemGroup ("Table Styles");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Grouped", "", typeof(TableStyles.SimpleTableScreen), new object[] { UITableViewStyle.Grouped }));
			navGroup.Items.Add (new NavItem ("Plain", "", typeof(TableStyles.SimpleTableScreen), new object[] { UITableViewStyle.Plain }));
			navGroup.Items.Add (new NavItem ("Indexed Grouped", "", typeof(TableStyles.TableWithIndexScreen), new object[] { UITableViewStyle.Grouped }));
			navGroup.Items.Add (new NavItem ("Indexed Plain", "", typeof(TableStyles.TableWithIndexScreen), new object[] { UITableViewStyle.Plain }));

			navGroup = new NavItemGroup ("Cell Styles");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Default", "", typeof(CellStyles.TableScreen)
				, new object[] { UITableViewStyle.Plain, UITableViewCellStyle.Default, UITableViewCellAccessory.None }));
			navGroup.Items.Add (new NavItem ("Subtitle", "", typeof(CellStyles.TableScreen)
				, new object[] { UITableViewStyle.Plain, UITableViewCellStyle.Subtitle, UITableViewCellAccessory.None }));
			navGroup.Items.Add (new NavItem ("Value1 (Right-Aligned Subtitle)", "", typeof(CellStyles.TableScreen)
				, new object[] { UITableViewStyle.Plain, UITableViewCellStyle.Value1, UITableViewCellAccessory.None }));
			navGroup.Items.Add (new NavItem ("Value2 (Contact Style)", "", typeof(CellStyles.TableScreen)
				, new object[] { UITableViewStyle.Plain, UITableViewCellStyle.Value2, UITableViewCellAccessory.None }));

			navGroup = new NavItemGroup ("Accessory Styles");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Checkmark", "", typeof(CellStyles.TableScreen)
				, new object[] { UITableViewStyle.Plain, UITableViewCellStyle.Default, UITableViewCellAccessory.Checkmark }));
			navGroup.Items.Add (new NavItem ("DetailDisclosureButton", "", typeof(CellStyles.TableScreen)
				, new object[] { UITableViewStyle.Plain, UITableViewCellStyle.Default, UITableViewCellAccessory.DetailDisclosureButton }));
			navGroup.Items.Add (new NavItem ("DisclosureIndicator", "", typeof(CellStyles.TableScreen)
				, new object[] { UITableViewStyle.Plain, UITableViewCellStyle.Default, UITableViewCellAccessory.DisclosureIndicator }));

			navGroup = new NavItemGroup ("Custom Cells");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Custom Cell from XIB", "", typeof(CustomCells.CustomCell1TableScreen)));
			navGroup.Items.Add (new NavItem ("Custom Cell in code", "", typeof(CustomCells.CustomCell12TableScreen)));
			
			// create a table source from our nav items
			tableSource = new NavItemTableSource (this.NavigationController, navItems);
			
			// set the source on the table to our data source
			base.TableView.Source = tableSource;
		}
	}
}

