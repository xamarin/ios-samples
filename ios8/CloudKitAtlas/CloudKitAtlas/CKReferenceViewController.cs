using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.CloudKit;

namespace CloudKitAtlas
{
	public partial class CKReferenceViewController : UITableViewController, ICloudViewController
	{
		public CloudManager CloudManager { get; set; }

		private const string ReferenceItemRecordName = "ReferenceItems";
		private List<CKRecord> records;

		public CKReferenceViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			records = new List<CKRecord> ();

			CloudManager.FetchRecords (ReferenceItemRecordName, results => {
				records = results;
				TableView.ReloadData ();
			});
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return records.Count;
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override async void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle != UITableViewCellEditingStyle.Delete)
				return;

			await CloudManager.DeleteAsync (records [indexPath.Row]);

			records.RemoveAt (indexPath.Row);
			tableView.DeleteRows (new [] { indexPath }, UITableViewRowAnimation.Automatic);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var indexPath = TableView.IndexPathForSelectedRow;
			var record = records [indexPath.Row];

			var detail = segue.DestinationViewController as CKReferenceDetailViewController;
			detail.ParentRecordName = record.RecordId.RecordName;
			detail.CloudManager = CloudManager;
		}

		async partial void Add (UIButton sender)
		{
			if (string.IsNullOrEmpty (nameTextField.Text))
				nameTextField.ResignFirstResponder ();
			else {
				var newRecord = new CKRecord (ReferenceItemRecordName);
				newRecord ["name"] = (NSString)nameTextField.Text;
				await CloudManager.SaveAsync (newRecord);

				records.Insert (0, newRecord);
				var indexPath = NSIndexPath.FromRowSection (0, 0);
				TableView.InsertRows (new [] { indexPath }, UITableViewRowAnimation.Automatic);
				nameTextField.ResignFirstResponder ();
				nameTextField.Text = string.Empty;
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = TableView.DequeueReusableCell (new NSString ("cell"), indexPath);
			CKRecord record = records[indexPath.Row];
			cell.TextLabel.Text = (NSString)record["name"];
			return cell;
		}
	}
}
