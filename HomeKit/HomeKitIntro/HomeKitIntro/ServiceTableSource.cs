using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using HomeKit;

namespace HomeKitIntro
{
	public class ServiceTableSource : UITableViewSource
	{
		#region Private Variables
		private ServiceTableViewController _controller;
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
		/// Initializes a new instance of the <see cref="HomeKitIntro.ServiceTableSource"/> class.
		/// </summary>
		/// <param name="controller">Controller.</param>
		public ServiceTableSource (ServiceTableViewController controller)
		{
			// Initialize
			_controller = controller;
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
			// Always two section
			return 2;
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
			// Take action based on the current section
			switch (section) {
			case 0:
				// Always one secton for details
				return 1;
			case 1:
				// Return the number of services
				return _controller.Accessory.Services.Length;
			}

			// Default to zero on error
			return 0;
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
			// Force height to 60
			return 60;
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
			switch (rowIndexPath.Section) {
			case 0:
				return false;
			case 1:
				return true;
			}

			return false;
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
			// Take action based on section
			switch (section) {
			case 0:
				return "Accessory";
			case 1:
				return "Services";
			}

			return "";
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
			var accessoryCell = tableView.DequeueReusableCell (ServiceTableCell.Key) as ServiceTableCell;

			// Take action based on the section
			switch (indexPath.Section) {
			case 0:
				// Display Accessory Details
				accessoryCell.DisplayInformation (string.Format ("{0} Reachable", _controller.Accessory.Name), _controller.Accessory.Reachable.ToString());
				accessoryCell.Accessory = UITableViewCellAccessory.None;
				break;
			case 1:
				// Display Accessory service
				var service = _controller.Accessory.Services [indexPath.Row];
				accessoryCell.DisplayService (service.Name, service.ServiceType);
				accessoryCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				break;
			}

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
			// Take action based on the section
			switch (indexPath.Section) {
			case 1:
				// Display the service characteristics
				_controller.ServiceIndex = indexPath.Row;
				_controller.PerformSegue ("CharacteristicSegue", _controller);
				break;
			}

		}
		#endregion
	}
}

