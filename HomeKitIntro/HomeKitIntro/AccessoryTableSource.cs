using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	/// <summary>
	/// Accessory table source.
	/// </summary>
	public class AccessoryTableSource : UITableViewSource
	{
		#region Private Variables
		private AccessoryTableViewController _controller;
		#endregion 

		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="HomeKitIntro.AccessoryTableSource"/> class.
		/// </summary>
		/// <param name="controller">Controller.</param>
		public AccessoryTableSource (AccessoryTableViewController controller)
		{
			// Initialize
			this._controller = controller;
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
			// Are there any accessories in this home?
			if (ThisApp.HomeManager.PrimaryHome == null) {
				// No
				return 0;
			} else {
				// Yes, return count
				return ThisApp.HomeManager.PrimaryHome.Accessories.Length;
			}
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
			return "Accessories";
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
			var accessoryCell = tableView.DequeueReusableCell (AccessoryCell.Key) as AccessoryCell;

			// Populate the cell
			accessoryCell.DisplayAccessory (ThisApp.HomeManager.PrimaryHome.Accessories [indexPath.Row]);

			// Return cell
			return accessoryCell;
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
			_controller.AccessoryIndex = indexPath.Row;
			_controller.PerformSegue ("ServicesSegue", _controller);

		}
		#endregion
	}
}

