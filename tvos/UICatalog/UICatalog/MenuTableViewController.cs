using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace UICatalog {
	public class MenuTableViewController : UITableViewController {
		const double performSegueDelay = 0.1;

		string lastPerformedSegueIdentifier;
		readonly NSOperationQueue delayedSeguesOperationQueue = new NSOperationQueue ();

		public virtual List<string []> SegueIdentifierMap { get; }

		public override void ViewDidLoad ()
		{
			TableView.RemembersLastFocusedIndexPath = true;
			TableView.LayoutMargins = new UIEdgeInsets (TableView.LayoutMargins.Top, 90f, TableView.LayoutMargins.Bottom, 20f);
		}

		public override bool ShouldPerformSegue (string segueIdentifier, NSObject sender)
		{
			if (!SegueIdentifierMap.Any (c => c.Contains (segueIdentifier)))
				return true;

			return segueIdentifier != lastPerformedSegueIdentifier;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var menuSplitViewController = SplitViewController as MenuSplitViewController;
			if (menuSplitViewController == null)
				return;
			
			menuSplitViewController.UpdateFocusToDetailViewController ();
		}

		public override void DidUpdateFocus (UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
		{
			var nextFocusedView = context.NextFocusedView;

			if (!nextFocusedView.IsDescendantOfView (TableView))
				return;

			NSIndexPath indexPath = ((UITableViewFocusUpdateContext)context).NextFocusedIndexPath;
			if (indexPath == null)
				return;

			// Cancel any previously queued segues.
			delayedSeguesOperationQueue.CancelAllOperations ();

			// Create an `NSBlockOperation` to perform the detail segue after a delay.
			var performSegueOperation = new NSBlockOperation ();
			var segueIdentifier = SegueIdentifierMap[indexPath.Section][indexPath.Row];

			performSegueOperation.AddExecutionBlock (() => {
				NSThread.SleepFor (performSegueDelay);
				if (performSegueOperation.IsCancelled && segueIdentifier == lastPerformedSegueIdentifier)
					return;

				NSOperationQueue.MainQueue.AddOperation (() => {
					PerformSegue (segueIdentifier, nextFocusedView);
					lastPerformedSegueIdentifier = segueIdentifier;
					TableView.SelectRow (indexPath, true, UITableViewScrollPosition.None);
				});
			});

			delayedSeguesOperationQueue.AddOperation (performSegueOperation);
		}
	}
}

