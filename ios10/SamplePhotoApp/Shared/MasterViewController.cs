using System;

using UIKit;
using Photos;
using Foundation;
using CoreFoundation;

namespace SamplePhotoApp
{
	public enum Section
	{
		AllPhotos,
		SmartAlbums,
		UserCollections
	}

	[Register ("MasterViewController")]
	public class MasterViewController : UITableViewController, IPHPhotoLibraryChangeObserver
	{
		const string allPhotosIdentifier = "allPhotos";
		const string collectionIdentifier = "collection";

		const string showAllPhotos = "showAllPhotos";
		const string showCollection = "showCollection";

		// MARK: Properties
		PHFetchResult allPhotos;
		PHFetchResult smartAlbums;
		PHFetchResult userCollections;

		readonly string [] sectionLocalizedTitles = { "", "Smart Albums", "Albums" };

		public MasterViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MasterViewController (NSCoder coder)
			: base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddAlbum);

			// Create a PHFetchResult object for each section in the table view.
			var allPhotosOptions = new PHFetchOptions {
				SortDescriptors = new NSSortDescriptor [] { new NSSortDescriptor("creationDate", true) },
			};

			allPhotos = PHAsset.FetchAssets (allPhotosOptions);
			smartAlbums = PHAssetCollection.FetchAssetCollections (PHAssetCollectionType.SmartAlbum, PHAssetCollectionSubtype.AlbumRegular, null);
			userCollections = PHCollection.FetchTopLevelUserCollections (null);

			PHPhotoLibrary.SharedPhotoLibrary.RegisterChangeObserver(this);
		}

		protected override void Dispose (bool disposing)
		{
			PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver (this);
			base.Dispose (disposing);
		}

		public override void ViewWillAppear (bool animated)
		{
			ClearsSelectionOnViewWillAppear = SplitViewController.Collapsed;
			base.ViewWillAppear (animated);
		}

		void AddAlbum (object sender, EventArgs args)
		{

			var alertController = UIAlertController.Create ("New Album", null, UIAlertControllerStyle.Alert);
			alertController.AddTextField (textField => {
				textField.Placeholder = "Album Name";
			});

			alertController.AddAction (UIAlertAction.Create ("Create", UIAlertActionStyle.Default, action => {
				var textField = alertController.TextFields [0];

				var title = textField.Text;
				if (!string.IsNullOrEmpty (title)) {
					// Create a new album with the title entered.
					PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
						PHAssetCollectionChangeRequest.CreateAssetCollection (title);
					}, (success, error) => {
						if (!success)
							Console.WriteLine ($"error creating album: {error}");
					});
				}
			}));
			PresentViewController (alertController, true, null);
		}

		#region Segues

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var destination = (segue.DestinationViewController as UINavigationController)?.TopViewController as AssetGridViewController;
			if (destination == null)
				throw new InvalidProgramException ("unexpected view controller for segue");

			var cell = (UITableViewCell)sender;
			destination.Title = cell.TextLabel.Text;

			switch (segue.Identifier) {
			case showAllPhotos:
				destination.FetchResult = allPhotos;
				break;

			case showCollection:
				// get the asset collection for the selected row
				var indexPath = TableView.IndexPathForCell (cell);
				NSObject collection = null;
				switch ((Section)indexPath.Section) {
				case Section.SmartAlbums:
					collection = smartAlbums.ObjectAt (indexPath.Row);
					break;

				case Section.UserCollections:
					collection = userCollections.ObjectAt (indexPath.Row);
					break;
				}
				// configure the view controller with the asset collection
				var assetCollection = collection as PHAssetCollection;
				if (assetCollection == null)
					throw new InvalidProgramException ("expected asset collection");

				destination.FetchResult = PHAsset.FetchAssets (assetCollection, null);
				destination.AssetCollection = assetCollection;
				break;
			}
		}

		#endregion

		#region Table View

		public override nint NumberOfSections (UITableView tableView)
		{
			return Enum.GetValues (typeof (Section)).Length;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch ((Section)(int)section) {
			case Section.AllPhotos:
				return 1;
			case Section.SmartAlbums:
				return smartAlbums.Count;
			case Section.UserCollections:
				return userCollections.Count;
			}

			throw new InvalidProgramException ();
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch ((Section)indexPath.Section) {
				case Section.AllPhotos: {
					var cell = tableView.DequeueReusableCell (allPhotosIdentifier, indexPath);
					cell.TextLabel.Text = "All Photos";
					return cell;
				}
				case Section.SmartAlbums: {
					var cell = tableView.DequeueReusableCell (collectionIdentifier, indexPath);
					var collection = (PHCollection)smartAlbums.ObjectAt (indexPath.Row);
					cell.TextLabel.Text = collection.LocalizedTitle;
					return cell;
				}
				case Section.UserCollections: {
					var cell = tableView.DequeueReusableCell (collectionIdentifier, indexPath);
					var collection = (PHCollection)userCollections.ObjectAt (indexPath.Row);
					cell.TextLabel.Text = collection.LocalizedTitle;

					return cell;
				}
			}

			throw new InvalidProgramException ();
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return sectionLocalizedTitles [(int)section];
		}

		#endregion

		#region IPHPhotoLibraryChangeObserver

		public void PhotoLibraryDidChange (PHChange changeInstance)
		{
			// Change notifications may be made on a background queue. Re-dispatch to the
			// main queue before acting on the change as we'll be updating the UI.
			DispatchQueue.MainQueue.DispatchSync (() => {
				// Check each of the three top-level fetches for changes.

				// Update the cached fetch result. 
				var changeDetails = changeInstance.GetFetchResultChangeDetails (allPhotos);
				if (changeDetails != null) {
					allPhotos = changeDetails.FetchResultAfterChanges;
					// (The table row for this one doesn't need updating, it always says "All Photos".)
				}

				// Update the cached fetch results, and reload the table sections to match.
				changeDetails = changeInstance.GetFetchResultChangeDetails (smartAlbums);
				if (changeDetails != null) {
					smartAlbums = changeDetails.FetchResultAfterChanges;
					TableView.ReloadSections (NSIndexSet.FromIndex ((int)Section.SmartAlbums), UITableViewRowAnimation.Automatic);
				}

				changeDetails = changeInstance.GetFetchResultChangeDetails (userCollections);
				if (changeDetails != null) {
					userCollections = changeDetails.FetchResultAfterChanges;
					TableView.ReloadSections (NSIndexSet.FromIndex ((int)Section.UserCollections), UITableViewRowAnimation.Automatic);
				}
			});
		}

		#endregion
	}
}
