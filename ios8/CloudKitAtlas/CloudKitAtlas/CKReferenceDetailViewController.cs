using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.CloudKit;

namespace CloudKitAtlas
{
	public partial class CKReferenceDetailViewController : UITableViewController, ICloudViewController
	{
		public CloudManager CloudManager { get; set; }

		public string ParentRecordName { get; set; }

		private const string ReferenceSubItemRecordType = "ReferenceSubItems";
		private List<CKRecord> records;

		public CKReferenceDetailViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			records = new List<CKRecord> ();

			CloudManager.QueryForRecords (ParentRecordName, results => {
				records = results;
				TableView.ReloadData ();
			});
		}

		async partial void Add (UIButton sender)
		{
			if (string.IsNullOrEmpty (nameTextField.Text))
				nameTextField.ResignFirstResponder ();
			else {
				var newRecord = new CKRecord (ReferenceSubItemRecordType);
				var parentRecordId = new CKRecordID (ParentRecordName);

				newRecord ["name"] = (NSString)nameTextField.Text;
				newRecord ["parent"] = new CKReference (parentRecordId, CKReferenceAction.DeleteSelf);

				await CloudManager.SaveAsync (newRecord);

				records.Insert (0, newRecord);
				var indexPath = NSIndexPath.FromRowSection (0, 0);
				TableView.InsertRows (new [] { indexPath }, UITableViewRowAnimation.Automatic);
				nameTextField.ResignFirstResponder ();
				nameTextField.Text = string.Empty;
			}
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return records.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = TableView.DequeueReusableCell ((NSString)"cell", indexPath);
			var record = records [indexPath.Row];
			cell.TextLabel.Text = record ["name"].ToString ();
			return cell;
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
			tableView.DeleteRows (new [] { indexPath }, UITableViewRowAnimation.Fade);
		}
	}
}
