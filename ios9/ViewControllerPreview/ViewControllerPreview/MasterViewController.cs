using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace ViewControllerPreview {
	public partial class MasterViewController : UITableViewController, IUIViewControllerPreviewingDelegate {
		class PreviewDetail
		{
			public string Title { get; set; }

			public double PreferredHeight { get; set; }
		}

		readonly PreviewDetail[] sampleData = {
			new PreviewDetail { Title = "Small", PreferredHeight = 160 },
			new PreviewDetail { Title = "Medium", PreferredHeight = 320 },
			new PreviewDetail { Title = "Large", PreferredHeight = 0 }
		};

		UIAlertController alertController;

		[Export ("initWithCoder:")]
		public MasterViewController (NSCoder coder) : base (coder)
		{
		}

		public MasterViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			
		}

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            //Important: Must call base method
            base.TraitCollectionDidChange(previousTraitCollection);

            if (TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available)
                RegisterForPreviewingWithDelegate (this, View);
            else
                alertController = UIAlertController.Create ("3D Touch Not Available", "Unsupported device.", UIAlertControllerStyle.Alert);
        }

		public override void ViewWillAppear (bool animated)
		{
			ClearsSelectionOnViewWillAppear = SplitViewController.Collapsed;
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (alertController == null)
				return;

			alertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
			PresentViewController (alertController, true, null);
			alertController = null;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier != "showDetail")
				return;

			var indexPath = TableView.IndexPathForSelectedRow;
			var previewDetail = sampleData[indexPath.Row];

			var detailViewController = (DetailViewController)((UINavigationController)segue.DestinationViewController)?.TopViewController;

			// Pass the `title` to the `detailViewController`.
			detailViewController.DetailItemTitle = previewDetail.Title;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return sampleData.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("Cell", indexPath);
			var previewDetail = sampleData [indexPath.Row];
			cell.TextLabel.Text = previewDetail.Title;

			return cell;
		}

		public UIViewController GetViewControllerForPreview (IUIViewControllerPreviewing previewingContext, CGPoint location)
		{
			// Obtain the index path and the cell that was pressed.
			var indexPath = TableView.IndexPathForRowAtPoint (location);

			if (indexPath == null)
				return null;

			var cell = TableView.CellAt (indexPath);

			if (cell == null)
				return null;

			// Create a detail view controller and set its properties.
			var detailViewController = (DetailViewController)Storyboard.InstantiateViewController ("DetailViewController");
			if (detailViewController == null)
				return null;

			var previewDetail = sampleData [indexPath.Row];
			detailViewController.DetailItemTitle = previewDetail.Title;
			detailViewController.PreferredContentSize = new CGSize (0, previewDetail.PreferredHeight);
			previewingContext.SourceRect = cell.Frame;
			return detailViewController;
		}

		public void CommitViewController (IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit)
		{
			ShowViewController (viewControllerToCommit, this);
		}
	}
}
