using Foundation;
using System;
using UIKit;

namespace NavigationBar {
	/// <summary>
	/// Demonstrates applying a large title to the UINavigationBar.
	/// </summary>
	public partial class LargeTitleViewController : UITableViewController {
		// Our data source is an array of city names, populated from Cities.json.
		private CitiesDataSource dataSource = new CitiesDataSource ();

		public LargeTitleViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			base.TableView.DataSource = this.dataSource;

			if (UIDevice.CurrentDevice.CheckSystemVersion (11, 0)) {
				base.NavigationController.NavigationBar.PrefersLargeTitles = true;
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "pushSeque") {
				// This segue is pushing a detailed view controller.
				segue.DestinationViewController.Title = dataSource [base.TableView.IndexPathForSelectedRow.Row];
				if (UIDevice.CurrentDevice.CheckSystemVersion (11, 0)) {
					segue.DestinationViewController.NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Never;
				}
			}
		}
	}
}
