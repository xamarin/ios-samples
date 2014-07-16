using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using UIKit;

using PassKit;

namespace PassLibrary {

	/// <summary>
	/// Display the list of passes installed on this device, and allowed to be viewed
	/// by virtue of the Entitlements and Provisioning Profile (ie only passes from 
	/// our Team ID).
	/// </summary>
	public class TableSource : UITableViewSource {
		protected PKPass[] tableItems;
		PKPassLibrary library;

		protected string cellIdentifier = "TableCell";
	
		public TableSource (PKPass[] items, PKPassLibrary library)
		{
			tableItems = items;
			this.library = library;
		}
	
		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Length;
		}
		
		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var pass = tableItems[indexPath.Row];
			string passInfo = 
					"Desc:" + pass.LocalizedDescription
					+ "\nOrg:" + pass.OrganizationName
					//+ "\nDebug:" + pass.DebugDescription
					+ "\nID:" + pass.PassTypeIdentifier
					+ "\nDate:" + pass.RelevantDate
					+ "\nWSUrl:" + pass.WebServiceUrl
					+ "\n#" + pass.SerialNumber
					+ "\nPassUrl:" + pass.PassUrl;

			new UIAlertView(pass.LocalizedName + " Selected"
				,passInfo , null, "OK", null).Show();
			tableView.DeselectRow (indexPath, true);
		}
		
		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellIdentifier);

			cell.ImageView.Image = tableItems[indexPath.Row].Icon; 
			cell.TextLabel.Text = tableItems[indexPath.Row].LocalizedDescription;
			cell.DetailTextLabel.Text = tableItems[indexPath.Row].LocalizedName;

			cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;

			return cell;
		}

		/// <summary>
		/// Open for scanning in Passbook
		/// </summary>
		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			var p = tableItems[indexPath.Row];
			var pass = library.GetPass (p.PassTypeIdentifier, p.SerialNumber);

			UIApplication.SharedApplication.OpenUrl (p.PassUrl);

			tableView.DeselectRow (indexPath, true);
		}
	}
}