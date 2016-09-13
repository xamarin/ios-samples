using System;

using UIKit;
using Foundation;
using AVKit;
using AVFoundation;
using CoreFoundation;

namespace HlsCatalog
{
	public partial class AssetListTableViewController : UITableViewController, IAssetPlaybackDelegate, IAssetListTableViewCellDelegate
	{
		const string PresentPlayerViewControllerSegueIdentifier = "PresentPlayerViewControllerSegueIdentifier";

		AVPlayerViewController playerViewController;

		public AssetListTableViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// General setup for auto sizing UITableViewCells.
			TableView.EstimatedRowHeight = 75;
			TableView.RowHeight = UITableView.AutomaticDimension;

			// Set AssetListTableViewController as the delegate for AssetPlaybackManager to recieve playback information.
			AssetPlaybackManager.SharedManager.Delegate = this;
			AssetListManager.Loaded += AssetLoaded;
		}

		protected override void Dispose (bool disposing)
		{
			AssetListManager.Loaded -= AssetLoaded;
			base.Dispose (disposing);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// The view reappeared as a results of dismissing an AVPlayerViewController. Perform cleanup.
			if (playerViewController != null) {
				AssetPlaybackManager.SharedManager.Asset = null;
				playerViewController.Player = null;
				playerViewController = null;
			}
		}

		#region Table view data source

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return AssetListManager.SharedManager.NumberOfAssets;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (AssetListTableViewCell.CellReusableId, indexPath);

			var asset = AssetListManager.SharedManager.AssetAt (indexPath.Row);
			var assetCell = cell as AssetListTableViewCell;
			if (assetCell != null) {
				assetCell.Asset = asset;
				assetCell.Delegate = this;
			}

			return cell;
		}

		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt (indexPath) as AssetListTableViewCell;
			var asset = cell?.Asset;
			if (asset == null)
				return;

			var downloadState = AssetPersistenceManager.SharedManager.GetDownloadState (asset);
			UIAlertAction alertAction = null;

			switch (downloadState) {
			case DownloadState.NotDownloaded:
				alertAction = UIAlertAction.Create ("Download", UIAlertActionStyle.Default, _ => {
					AssetPersistenceManager.SharedManager.DownloadStream (asset);
				});
				break;

			case DownloadState.Downloading:
				alertAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Default, _ => {
					AssetPersistenceManager.SharedManager.CancelDownload (asset);
				});
				break;

			case DownloadState.Downloaded:
				alertAction = UIAlertAction.Create ("Delete", UIAlertActionStyle.Default, _ => {
					AssetPersistenceManager.SharedManager.DeleteAsset (asset);
				});
				break;
			}

			var alertController = UIAlertController.Create (asset.Name, "Select from the following options:", UIAlertControllerStyle.ActionSheet);
			alertController.AddAction (alertAction);
			alertController.AddAction (UIAlertAction.Create ("Dismiss", UIAlertActionStyle.Cancel, null));

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				var popoverController = alertController.PopoverPresentationController;
				if (popoverController == null)
					return;

				popoverController.SourceView = cell;
				popoverController.SourceRect = cell.Bounds;
			}

			PresentViewController (alertController, true, null);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == PresentPlayerViewControllerSegueIdentifier) {
				var cell = sender as AssetListTableViewCell;
				var pvc = segue.DestinationViewController as AVPlayerViewController;
				if (cell == null || pvc == null)
					return;

				// Grab a reference for the destinationViewController to use in later delegate callbacks from AssetPlaybackManager.
				playerViewController = pvc;

				// Load the new Asset to playback into AssetPlaybackManager.
				AssetPlaybackManager.SharedManager.Asset = cell.Asset;
			}
		}

		#endregion

		void AssetLoaded (object sender, EventArgs e)
		{
			DispatchQueue.MainQueue.DispatchAsync (TableView.ReloadData);
		}

		public void DownloadStateDidChange (AssetListTableViewCell cell, DownloadState newState)
		{
			var indexPath = TableView.IndexPathForCell (cell);
			if (indexPath == null)
				return;

			TableView.ReloadRows (new NSIndexPath [] { indexPath }, UITableViewRowAnimation.Automatic);
		}

		public void PlayerCurrentItemChanged (AssetPlaybackManager streamPlaybackManager, AVPlayer player)
		{
			if (playerViewController == null)
				return;
			if (player.CurrentItem == null)
				return;

			playerViewController.Player = player;
		}

		public void PlayerReadyToPlay (AssetPlaybackManager streamPlaybackManager, AVPlayer player)
		{
			player.Play ();
		}
	}
}
