using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace UIKitEnhancements
{
	public class SearchResultsTableSource :UITableViewSource
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
		public SearchResultsTableSource (MainMenuTableViewController controller)
		{
			// Initialize
			this._controller = controller;
		}
		#endregion

		#region public Methods
		/// <summary>
		/// Loads the data.
		/// </summary>
		public void Search(string searchText) {

			// Clear existing items
			_items.Clear ();

			// Look for the search text in the parent list
			for (int i = 0; i < _controller.DataSource.Count; ++i) {
				// Grab the current item
				var item = _controller.DataSource [i];

				// Does the item contain the search text?
				if (item.Title.Contains (searchText)) {
					// Yes, add it to the collection
					_items.Add (item);
				}
			}
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
			return "Search Results";
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
			//Decant reusable cell
			UITableViewCell searchCell = tableView.DequeueReusableCell("SEARCH"); 
			if (searchCell == null )
			{
				//Not found, assemble new cell
				searchCell = new UITableViewCell (UITableViewCellStyle.Default, "SEARCH");
			}

			// Configure cell
			searchCell.TextLabel.Text = _items [indexPath.Row].Title;

			// Return search cell
			return searchCell;

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

