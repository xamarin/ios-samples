using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.Reflection;

namespace Xamarin.Code
{
	
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class NavItemTableSource : UITableViewSource
	{
		protected List<NavItemGroup> navItems;
		string cellIdentifier = "NavTableCellView";
		UINavigationController navigationController;
		
		public NavItemTableSource (UINavigationController navigationController, List<NavItemGroup> items)
		{
			navItems = items;
			this.navigationController = navigationController;
		}

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override nint NumberOfSections (UITableView tableView)
		{
			return navItems.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return navItems[(int)section].Items.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return navItems[(int)section].Name;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return navItems[(int)section].Footer;
		}

		/// <summary>
		/// Called by the TableView to actually build each cell. 
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// declare vars
			NavItem navItem = navItems[indexPath.Section].Items[indexPath.Row];
			UIImage navIcon = null;
			
			var cell = tableView.DequeueReusableCell (this.cellIdentifier);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, this.cellIdentifier);
				cell.Tag = Environment.TickCount;
			}
			
			// set the cell properties
			cell.TextLabel.Text = navItems[indexPath.Section].Items[indexPath.Row].Name;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			
			// return the cell
			return cell;
		}

		
		/// <summary>
		/// Is called when a row is selected
		/// </summary>
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// get a reference to the nav item
			NavItem navItem = navItems[indexPath.Section].Items[indexPath.Row];
			
			// if the nav item has a proper controller, push it on to the NavigationController
			// NOTE: we could also raise an event here, to loosely couple this, but isn't neccessary,
			// because we'll only ever use this this way
			if (navItem.Controller != null) {
				navigationController.PushViewController (navItem.Controller, true);
				// show the nav bar (we don't show it on the home page)
				navigationController.NavigationBarHidden = false;
			}
			else
			{
				if (navItem.ControllerType != null) {
					//
					ConstructorInfo ctor = null;
					
					// if the nav item has constructor aguments
					if (navItem.ControllerConstructorArgs.Length > 0) {
						// look for the constructor
						ctor = navItem.ControllerType.GetConstructor (navItem.ControllerConstructorTypes);
					}
					else {
						// search for the default constructor
						ctor = navItem.ControllerType.GetConstructor (System.Type.EmptyTypes);
					}
					
					// if we found the constructor
					if (ctor != null)
					{
						//
						UIViewController instance = null;
						
						if (navItem.ControllerConstructorArgs.Length > 0) {
							// instance the view controller
							instance = ctor.Invoke (navItem.ControllerConstructorArgs) as UIViewController;
						}
						else {
							// instance the view controller
							instance = ctor.Invoke (null) as UIViewController;
						}
						
						if (instance != null) {
							// save the object
							navItem.Controller = instance;
							
							// push the view controller onto the stack
							navigationController.PushViewController (navItem.Controller, true);
						}
						else {
							Console.WriteLine ("instance of view controller not created");
						}
					}
					else
					{
						Console.WriteLine ("constructor not found");
					}
				}
			}
		}
	}
}

