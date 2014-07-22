using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Foundation;
using UIKit;

namespace Popovers
{
	class TableSource : UITableViewSource {
		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return 10;
		}	
		
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			string cellIdentifier = "Cell";

			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			
			cell.TextLabel.Text = string.Format ("Row {0}", indexPath.Row);

			return cell;
		}
	}

	public partial class RootViewController : UITableViewController
	{
		[Outlet]
		public DetailViewController DetailViewController { get; set; }


		public RootViewController (IntPtr handle) : base (handle)
		{
		}
		
		//loads the RootViewController.xib file and connects it to this object
		public RootViewController () : base ("RootViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ClearsSelectionOnViewWillAppear = false;
			ContentSizeForViewInPopover = new SizeF (320, 600);
			TableView.Source = new TableSource ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

	}
}
