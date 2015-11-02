using System;
using System.Linq;

using CoreFoundation;
using Foundation;
using Photos;
using UIKit;

namespace SamplePhotoApp {
	public partial class RootListViewController : UITableViewController, IPHPhotoLibraryChangeObserver {

		const string allPhotosReuseIdentifier = "AllPhotosCell";
		const string collectionCellReuseIdentifier = "CollectionCell";

		const string allPhotosSegue = "showAllPhotos";
		const string collectionSegue = "showCollection";

		PHFetchResult[] sectionFetchResults;
		string[] sectionLocalizedTitles;

		[Export ("initWithCoder:")]
		public RootListViewController (NSCoder coder) : base (coder)
		{
		}

		public RootListViewController (IntPtr handle) : base (handle)
		{
		}

		public override void AwakeFromNib ()
		{
			// Create a PHFetchResult object for each section in the table view.
			var allPhotosOptions = new PHFetchOptions ();
			allPhotosOptions.SortDescriptors = new [] {
				new NSSortDescriptor ("creationDate", true)
			};

			PHFetchResult allPhotos = PHAsset.FetchAssets (allPhotosOptions);
			PHFetchResult smartAlbums = PHAssetCollection.FetchAssetCollections (PHAssetCollectionType.SmartAlbum,
				PHAssetCollectionSubtype.AlbumRegular, null);

			PHFetchResult topLevelUserCollections = PHCollection.FetchTopLevelUserCollections (null);

			// Store the PHFetchResult objects and localized titles for each section.
			sectionFetchResults = new [] {
				allPhotos, smartAlbums, topLevelUserCollections
			};

			sectionLocalizedTitles = new [] {
				string.Empty, "Smart Albums", "Albums", string.Empty
			};

			PHPhotoLibrary.SharedPhotoLibrary.RegisterChangeObserver (this);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (!(segue.DestinationViewController is AssetGridViewController) || !(sender is UITableViewCell))
				return;

			var assetGridViewController = (AssetGridViewController)segue.DestinationViewController;
			var cell = (UITableViewCell)sender;

			// Set the title of the AssetGridViewController.
			assetGridViewController.Title = cell.TextLabel.Text;

			// Get the PHFetchResult for the selected section.
			NSIndexPath indexPath = TableView.IndexPathForCell (cell);
			PHFetchResult fetchResult = sectionFetchResults [indexPath.Section];

			if (segue.Identifier == allPhotosSegue) {
				assetGridViewController.AssetsFetchResults = fetchResult;
			} else if (segue.Identifier == collectionSegue) {
				// Get the PHAssetCollection for the selected row.
				var collection = fetchResult [indexPath.Row] as PHAssetCollection;
				if (collection == null)
					return;
				var assetsFetchResult = PHAsset.FetchAssets (collection, null);

				assetGridViewController.AssetsFetchResults = assetsFetchResult;
				assetGridViewController.AssetCollection = collection;
			}
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return sectionFetchResults.Length;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			nint numberOfRows;

			if (section == 0) {
				// The "All Photos" section only ever has a single row.
				numberOfRows = 1;
			} else {
				PHFetchResult fetchResult = sectionFetchResults[section];
				numberOfRows = fetchResult.Count;
			}

			return numberOfRows;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell;
			if (indexPath.Section == 0) {
				cell = TableView.DequeueReusableCell (allPhotosReuseIdentifier, indexPath);
				cell.TextLabel.Text = "All Photos";
			} else {
				PHFetchResult fetchResult = sectionFetchResults [indexPath.Section];
				var collections = fetchResult.Cast<PHCollection> ().ToArray ();
				var collection = collections [indexPath.Row];

				cell = TableView.DequeueReusableCell (collectionCellReuseIdentifier, indexPath);
				cell.TextLabel.Text = collection.LocalizedTitle;
			}

			return cell;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return sectionLocalizedTitles [section];
		}

		protected override void Dispose (bool disposing)
		{
			PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver (this);
			base.Dispose (disposing);
		}

		public void PhotoLibraryDidChange (PHChange changeInstance)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				// Loop through the section fetch results, replacing any fetch results that have been updated.
				for (int i = 0; i < sectionFetchResults.Length; i++) {
					PHFetchResultChangeDetails changeDetails = changeInstance.GetFetchResultChangeDetails (sectionFetchResults [i]);

					if(changeDetails != null) {
						sectionFetchResults [i] = changeDetails.FetchResultAfterChanges;
					}
				}

				TableView.ReloadData ();
			});
		}

		partial void AddButtonClickHandler (NSObject sender)
		{
			// Prompt user from new album title.
			var alertController = UIAlertController.Create ("New Album", null, UIAlertControllerStyle.Alert);
			alertController.AddTextField (textField => {
				textField.Placeholder = "Album Name";
			});

			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

			alertController.AddAction (UIAlertAction.Create ("Create", UIAlertActionStyle.Default, action => {
				UITextField textField = alertController.TextFields.First ();
				string title = textField.Text;
				if (string.IsNullOrEmpty (title))
					return;

				// Create a new album with the title entered.
				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => 
					PHAssetCollectionChangeRequest.CreateAssetCollection (title), (success, error) => {
						if (!success)
							Console.WriteLine (error.LocalizedDescription);
				});
			}));

			PresentViewController (alertController, true, null);
		}
	}
}
