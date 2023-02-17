using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas {
	public partial class SubmenuTableViewController : UITableViewController {
		public string GroupTitle { get; set; }
		public CodeSample [] CodeSamples { get; set; }

		public SubmenuTableViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public SubmenuTableViewController (NSCoder coder)
			: base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var groupTitle = GroupTitle;
			if (!string.IsNullOrWhiteSpace (groupTitle))
				NavigationItem.Title = groupTitle;

			NavigationItem.HidesBackButton = NavigationController.ViewControllers [0].NavigationItem.HidesBackButton;
		}

		#region Table view data source

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return CodeSamples.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var codeSample = CodeSamples [indexPath.Row];
			var cell = (SubmenuTableViewCell) tableView.DequeueReusableCell ("SubmenuItem", indexPath);
			cell.SubmenuLabel.Text = codeSample.Title;

			return cell;
		}

		#endregion

		#region Navigation

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "ShowCodeSampleFromSubmenu") {
				var codeSampleViewController = (CodeSampleViewController) segue.DestinationViewController;
				var selectedCell = sender as SubmenuTableViewCell;
				if (selectedCell != null) {
					var indexPath = TableView.IndexPathForCell (selectedCell);
					codeSampleViewController.SelectedCodeSample = CodeSamples [indexPath.Row];
				}
			}
		}

		#endregion
	}
}
