using System;
using System.Collections.Generic;

using CoreGraphics;
using Foundation;
using UIKit;

namespace SimpleBackgroundFetch
{
	public partial class RootViewController : UITableViewController
	{
		DataSource dataSource;

		public RootViewController (IntPtr handle) : base (handle)
		{
			Title = NSBundle.MainBundle.LocalizedString ("Master", "Master");
			TableView.Source = dataSource = new DataSource (this);
		}

		public void InsertNewObjectForFetch (Action<UIBackgroundFetchResult> completionhandler)
		{
			AddNewItem (null, EventArgs.Empty);
			completionhandler (UIBackgroundFetchResult.NewData);
		}

		void AddNewItem (object sender, EventArgs args)
		{
			dataSource.Objects.Insert (0, DateTime.Now);

			using (var indexPath = NSIndexPath.FromRowSection (0, 0))
				TableView.InsertRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			NavigationItem.RightBarButtonItem = addButton;
		}

		class DataSource : UITableViewSource
		{
			static readonly NSString CellIdentifier = new NSString ("DataSourceCell");
			List<object> objects = new List<object> ();
			RootViewController controller;

			public DataSource (RootViewController controller)
			{
				this.controller = controller;
			}

			public IList<object> Objects {
				get {
					return objects;
				}
			}

			// Customize the number of sections in the table view.
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return objects.Count;
			}

			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier);
				cell.TextLabel.Text = objects [indexPath.Row].ToString ();
				return cell;
			}

			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the specified item to be editable.
				return true;
			}

			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) {
					// Delete the row from the data source.
					objects.RemoveAt (indexPath.Row);
					controller.TableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				}
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetail") {
				var indexPath = TableView.IndexPathForSelectedRow;
				var item = dataSource.Objects [indexPath.Row];

				((DetailViewController)segue.DestinationViewController).SetDetailItem (item);
			}
		}
	}
}

