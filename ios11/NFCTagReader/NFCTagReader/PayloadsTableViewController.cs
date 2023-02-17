using System;
using CoreNFC;
using UIKit;
using Foundation;
using CoreFoundation;
using System.Collections.Generic;

namespace NFCTagReader {
	public partial class PayloadsTableViewController : UITableViewController {
		public PayloadsTableViewController () : base ("PayloadsTableViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		#region Properties

		public NFCNdefPayload [] Payloads = new NFCNdefPayload [] { };
		string CellIdentifier = "reuseIdentifier";

		#endregion


		#region DataSource

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Payloads.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);

			var payload = Payloads [indexPath.Row];

			//---- if there are no cells to reuse, create a new one
			if (cell == null) { cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier); }

			switch (payload.TypeNameFormat) {
			case NFCTypeNameFormat.NFCWellKnown:
				var type = new NSString (payload.Type, NSStringEncoding.UTF8);
				if (type != null) {
					cell.TextLabel.Text = $"NFC Well Known type: {type}";
				} else {
					cell.TextLabel.Text = "Invalid data";
				}
				break;
			case NFCTypeNameFormat.AbsoluteUri:
				var text = new NSString (payload.Payload, NSStringEncoding.UTF8);
				if (text != null) {
					cell.TextLabel.Text = text;
				} else {
					cell.TextLabel.Text = "Invalid data";
				}
				break;
			case NFCTypeNameFormat.Media:
				var mediaType = new NSString (payload.Type, NSStringEncoding.UTF8);
				if (mediaType != null) {
					cell.TextLabel.Text = $"Media type: {mediaType}";
				} else {
					cell.TextLabel.Text = "Invalid data";
				}
				break;
			case NFCTypeNameFormat.NFCExternal:
				cell.TextLabel.Text = "NFC External type";
				break;
			case NFCTypeNameFormat.Unknown:
				cell.TextLabel.Text = "Unknown type";
				break;
			case NFCTypeNameFormat.Unchanged:
				cell.TextLabel.Text = "Unchanged type";
				break;
			default:
				cell.TextLabel.Text = "Invalid data";
				break;
			}


			return cell;
		}

		#endregion
	}
}

