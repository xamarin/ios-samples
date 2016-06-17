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
			
		// ---------------------------------------------------------------------------------------
		// TODO: I shouldn't need override these two methods, but without them the Datasource and 
		// Delegate aren't being called. This wasn't required before and is invalid behavior.
		// See: https://bugzilla.xamarin.com/show_bug.cgi?id=37448
		// ---------------------------------------------------------------------------------------
		public override nint NumberOfSections (UITableView tableView)
		{
			return DataSource.NumberOfSections(tableView);
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return DataSource.RowsInSection(tableView, section);
		}
		// ---------------------------------------------------------------------------------------
		#endregion
	}
}
