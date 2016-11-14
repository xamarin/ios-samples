using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GrowRowTable
{
	public partial class GrowRowTableViewController : UITableViewController
	{
		#region Computed Properties
		public GrowRowTableDataSource DataSource {
			get { return TableView.DataSource as GrowRowTableDataSource; }
		}

		public GrowRowTableDelegate TableDelegate {
			get { return TableView.Delegate as GrowRowTableDelegate; }
		}
		#endregion

		#region Constructors
		public GrowRowTableViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Initialize table
			TableView.DataSource = new GrowRowTableDataSource(this);
			TableView.Delegate = new GrowRowTableDelegate (this);
			TableView.EstimatedRowHeight = 40f;
			TableView.ReloadData ();
		}
			

		#endregion
	}
}
