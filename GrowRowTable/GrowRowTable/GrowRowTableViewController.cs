using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GrowRowTable
{
	public partial class GrowRowTableViewController : UITableViewController
	{
		public GrowRowTableDataSource DataSource {
			get { return TableView.DataSource as GrowRowTableDataSource; }
		}

		public GrowRowTableDelegate TableDelegate {
			get { return TableView.Delegate as GrowRowTableDelegate; }
		}

		public GrowRowTableViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Initialize table
			TableView.DataSource = new GrowRowTableDataSource();
			TableView.Delegate = new GrowRowTableDelegate ();
			TableView.EstimatedRowHeight = 40f;
			TableView.ReloadData ();
		}
			

	}
}
