using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Photos;
using UIKit;

namespace RawExpose {
	// This UICollectionViewController displayes all albums from the Photos library.
	public partial class AlbumCollectionViewController : UICollectionViewController {
		const string lightboxSegueName = "LightboxSegue";

		// Array of albums displayed in this UICollectionView
		readonly List<PHAssetCollection> albums = new List<PHAssetCollection> ();

		public AlbumCollectionViewController (IntPtr handle)
			: base (handle)
		{
		}

		// Requests all album and smart albums collections to be fetched.
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// TODO: https://trello.com/c/ZLOvAFWp
			PHPhotoLibrary.RequestAuthorization (status => {
				if (status != PHAuthorizationStatus.Authorized)
					return;

				// Fetch all albums
				var allAlbums = PHAssetCollection.FetchAssetCollections (PHAssetCollectionType.Album, PHAssetCollectionSubtype.Any, null)
												 .Cast<PHAssetCollection> ();
				albums.AddRange (allAlbums);

				// Fetch all smart albums
				var smartAlbums = PHAssetCollection.FetchAssetCollections (PHAssetCollectionType.SmartAlbum, PHAssetCollectionSubtype.Any, null)
												   .Cast<PHAssetCollection> ();
				albums.AddRange (smartAlbums);

				// Ask the collection view to reload data, so the fetched albums is displayed.
				NSOperationQueue.MainQueue.AddOperation (() => {
					CollectionView?.ReloadData ();
				});
			});
		}

		#region UICollectionViewController

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return albums.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (AlbumCollectionViewCell.ReuseIdentifier, indexPath) as AlbumCollectionViewCell;
			if (cell == null)
				throw new InvalidProgramException ("Unable to dequeue an AlbumCollectionViewCell");

			var collection = albums [indexPath.Row];
			cell.ImageView.Image = null;
			cell.Label.Text = collection.LocalizedTitle;

			var firstAsset = (PHAsset) PHAsset.FetchAssets (collection, new PHFetchOptions ()).firstObject;
			if (firstAsset != null) {
				var options = new PHImageRequestOptions {
					Synchronous = true
				};

				PHImageManager.DefaultManager.RequestImageForAsset (firstAsset, cell.Bounds.Size, PHImageContentMode.AspectFit, options, (requestedImage, _) => {
					cell.ImageView.Image = requestedImage;
				});

			}

			return cell;
		}

		#endregion

		#region Storyboard seque

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			PerformSegue (lightboxSegueName, albums [indexPath.Row]);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var lightboxController = segue.DestinationViewController as LightboxCollectionViewController;
			if (lightboxController == null)
				return;

			var collection = sender as PHAssetCollection;
			if (collection == null)
				return;

			// Tell the Lightbox what collection do display 
			lightboxController.AssetCollection = collection;
		}

		#endregion
	}
}
