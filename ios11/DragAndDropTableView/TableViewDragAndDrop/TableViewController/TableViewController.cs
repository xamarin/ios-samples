using Foundation;
using System;
using UIKit;

namespace TableViewDragAndDrop {
	/// <summary>
	/// Demonstrates how to enable drag and drop for a UITableView instance
	/// </summary>
	public partial class TableViewController : UITableViewController {
		public TableViewController (IntPtr handle) : base (handle)
		{
		}

		Model model = new Model ();

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Specify the table as its own drag source and drop delegate
			this.TableView.DragDelegate = this;
			this.TableView.DropDelegate = this;

			// Impelement delegate and datasource for tableview to operate
			this.TableView.DataSource = this;
			this.TableView.Delegate = this;
		}
	}
}
