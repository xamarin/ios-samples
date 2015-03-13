using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	public class MainMenuTableSource :UITableViewSource
	{
		#region Private Variables
		private MainMenuTableViewController _controller;
		private List<MenuItem> _items = new List<MenuItem>();
		#endregion 

		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}

		/// <summary>
		/// Gets or sets the <see cref="UIKitEnhancements.MenuItem"/> at the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public MenuItem this[int index]
		{
			get
			{
				return _items[index];
			}

			set
			{
				_items[index] = value;
			}
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count {
			get { return _items.Count; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UIKitEnhancements.MainMenuTableSource"/> class.
		/// </summary>
		/// <param name="controller">Controller.</param>
		public MainMenuTableSource (MainMenuTableViewController controller)
		{
			// Initialize
			this._controller = controller;
			LoadData ();
		}
		#endregion

		#region public Methods
		/// <summary>
		/// Loads the data.
		/// </summary>
		public void LoadData() {

			// Clear existing items
			_items.Clear ();

			// Setup list of data items
			_items.Add (new MenuItem ("Alert Controller", "Replaces Action Sheet & Alert View", "AlertSegue"));
			_items.Add (new MenuItem ("Collection View Changes", "New Collection View Features","WebSegue","http://blog.xamarin.com/new-collection-view-features-in-ios-8/"));
			_items.Add (new MenuItem ("Navigation Controller", "New Navigation Controller Options","NavBarSegue"));
			_items.Add (new MenuItem ("Notifications", "Required Notification Settings","NotificationSegue"));
			_items.Add (new MenuItem ("Popover Presentation Controller", "Handles Presentation in a Popover","PopoverSegue"));
			_items.Add (new MenuItem ("Split View Controller", "Now Supported on iPhone","WebSegue","https://developer.apple.com/library/prerelease/ios/documentation/UIKit/Reference/UISplitViewController_class/index.html#//apple_ref/occ/cl/UISplitViewController"));
			_items.Add (new MenuItem ("Visual Effects", "Adding Visual Effects in iOS 8","WebSegue","http://blog.xamarin.com/adding-view-effects-in-ios-8/"));

		}
		#endregion

		#region Override Methods
		/// <Docs>Table view displaying the sections.</Docs>
		/// <returns>Number of sections required to display the data. The default is 1 (a table must have at least one section).</returns>
		/// <para>Declared in [UITableViewDataSource]</para>
		/// <summary>
		/// Numbers the of sections.
		/// </summary>
		/// <param name="tableView">Table view.</param>
		public override nint NumberOfSections (UITableView tableView)
		{
			// Always one section
			return 1;
		}

		/// <Docs>Table view displaying the rows.</Docs>
		/// <summary>
		/// Rowses the in section.
		/// </summary>
		/// <returns>The in section.</returns>
		/// <param name="tableview">Tableview.</param>
		/// <param name="section">Section.</param>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// Return number of items
			return _items.Count;
		}

		/// <Docs>Table view.</Docs>
		/// <summary>
		/// Gets the height for row.
		/// </summary>
		/// <returns>The height for row.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="indexPath">Index path.</param>
		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			// Force height to 44
			return 44;
		}

		/// <summary>
		/// Shoulds the highlight row.
		/// </summary>
		/// <returns><c>true</c>, if highlight row was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="rowIndexPath">Row index path.</param>
		public override bool ShouldHighlightRow (UITableView tableView, NSIndexPath rowIndexPath)
		{
			// Always allow highlighting
			return true;
		}

		/// <Docs>Table view containing the section.</Docs>
		/// <summary>
		/// Called to populate the header for the specified section.
		/// </summary>
		/// <see langword="null"></see>
		/// <returns>The for header.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="section">Section.</param>
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			// Return section title
			return "UIKit Enhancements";
		}

		/// <Docs>Table view containing the section.</Docs>
		/// <summary>
		/// Called to populate the footer for the specified section.
		/// </summary>
		/// <see langword="null"></see>
		/// <returns>The for footer.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="section">Section.</param>
		public override string TitleForFooter (UITableView tableView, nint section)
		{
			// None
			return "";
		}

		/// <Docs>Table view requesting the cell.</Docs>
		/// <summary>
		/// Gets the cell.
		/// </summary>
		/// <returns>The cell.</returns>
		/// <param name="tableView">Table view.</param>
		/// <param name="indexPath">Index path.</param>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// Get new cell
			var cell = tableView.DequeueReusableCell (MainMenuTableCell.Key) as MainMenuTableCell;

			// Populate the cell
			cell.DisplayMenuItem (_items [indexPath.Row]);

			// Return cell
			return cell;
		}

		/// <Docs>Table view containing the row.</Docs>
		/// <summary>
		/// Rows the selected.
		/// </summary>
		/// <param name="tableView">Table view.</param>
		/// <param name="indexPath">Index path.</param>
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// Note item accessory being displayed and 
			// open it's details and services view
			_controller.MenuItem = _items[indexPath.Row];

			// Performing a segue?
			if (_controller.MenuItem.Segue != "") {
				// Are we running on the iPad?
				if (ThisApp.iPadViewController == null) {
					// No, invoke segue against the table controller
					_controller.PerformSegue (_controller.MenuItem.Segue, _controller);
				} else {
					// Yes, invoke segue against the iPad default view
					ThisApp.iPadViewController.MenuItem = _items[indexPath.Row];
					ThisApp.iPadViewController.PerformSegue (_controller.MenuItem.Segue, ThisApp.iPadViewController);
				}
			}

		}
		#endregion
	}
}