using System;
using CoreNFC;
using UIKit;
using Foundation;
using CoreFoundation;
using System.Collections.Generic;

namespace NFCTagReader {
	public partial class MessagesTableViewController : UITableViewController, INFCNdefReaderSessionDelegate {

		public MessagesTableViewController (IntPtr p) : base (p)
		{

		}

		public MessagesTableViewController () : base ("MessagesTableViewController", null)
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

		List<NFCNdefMessage> DetectedMessages = new List<NFCNdefMessage> { };
		NFCNdefReaderSession Session;
		string CellIdentifier = "reuseIdentifier";


		partial void Scan (UIBarButtonItem sender)
		{
			Session = new NFCNdefReaderSession (this, null, true);
			if (Session != null) {
				Session.BeginSession ();
			}
		}

		#endregion


		#region NFCNDEFReaderSessionDelegate

		public void DidDetect (NFCNdefReaderSession session, NFCNdefMessage [] messages)
		{
			foreach (NFCNdefMessage msg in messages) {
				DetectedMessages.Add (msg);
			}
			DispatchQueue.MainQueue.DispatchAsync (() => {
				this.TableView.ReloadData ();
			});
		}


		public void DidInvalidate (NFCNdefReaderSession session, NSError error)
		{

			var readerError = (NFCReaderError) (long) error.Code;

			if (readerError != NFCReaderError.ReaderSessionInvalidationErrorFirstNDEFTagRead &&
				readerError != NFCReaderError.ReaderSessionInvalidationErrorUserCanceled) {
				BeginInvokeOnMainThread (() => {
					var alertController = UIAlertController.Create ("Session Invalidated", error.LocalizedDescription, UIAlertControllerStyle.Alert);
					alertController.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));
					PresentViewController (alertController, true, null);
				});
			}




		}

		#endregion

		#region DataSource


		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return DetectedMessages.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);
			string item = $"{DetectedMessages [indexPath.Row].Records.Length.ToString ()} payload(s)";

			//---- if there are no cells to reuse, create a new one
			if (cell == null) { cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier); }

			cell.TextLabel.Text = item;

			return cell;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{

			var indexPath = TableView.IndexPathForSelectedRow;
			if (indexPath != null) {
				var payloadsTableViewController = (PayloadsTableViewController) segue.DestinationViewController;
				payloadsTableViewController.Payloads = DetectedMessages [indexPath.Row].Records;

			}

		}

		#endregion

	}
}

