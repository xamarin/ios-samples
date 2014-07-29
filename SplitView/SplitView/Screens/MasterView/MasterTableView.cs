using System;
using UIKit;
using System.Collections.Generic;

namespace Example_SplitView.Screens.MasterView
{
	public class MasterTableView : UITableViewController
	{
		TableSource tableSource;
		
		public event EventHandler<RowClickedEventArgs> RowClicked;
		
		public MasterTableView ()
		{
		}
		
		public MasterTableView (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// setup our data source
			List<string> items = new List<string>();
			for (int i = 1; i <= 10; i++)
				items.Add (i.ToString ());
			tableSource = new TableSource (items, this);

			// add the data source to the table
			this.TableView.Source = tableSource;
			
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		public class RowClickedEventArgs : EventArgs
		{
			public string Item { get; set; }
			
			public RowClickedEventArgs(string item) : base()
			{ this.Item = item; }
		}
		
		protected class TableSource : UITableViewSource
		{
			public List<string> Items = new List<string> ();
			protected string cellIdentifier = "basicCell";
			protected MasterTableView parentController;
			
			public TableSource(List<string> items, MasterTableView parentController)
			{
				Items = items;
				this.parentController = parentController;
			}
			
			public override nint NumberOfSections (UITableView tableView)
			{ return 1; }
			
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return Items.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
			{
				// declare vars 
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
				// if there are no cells to reuse, create a new one 
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
				// set the item text 
				cell.TextLabel.Text = Items[(int)indexPath.Row];
				
				return cell;
			}
			
			public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
			{
				if (parentController.RowClicked != null)
					parentController.RowClicked (this, new RowClickedEventArgs(Items[(int)indexPath.Row]));
			}
		}
	}
}

