using System;
using System.Collections.Generic;

using CoreFoundation;
using CoreGraphics;
using Foundation;
using Photos;
using PhotosUI;
using UIKit;

namespace SamplePhotoApp {
	public partial class AssetGridViewController : UICollectionViewController, IPHPhotoLibraryChangeObserver {
		static CGSize assetGridThumbnailSize;

		const string cellReuseIdentifier = "Cell";

		PHCachingImageManager imageManager;
		CGRect previousPreheatRect;

		public PHFetchResult AssetsFetchResults { get; set; }

		public PHAssetCollection AssetCollection { get; set; }

		[Export ("initWithCoder:")]
		public AssetGridViewController (NSCoder coder) : base (coder)
		{
		}

		public AssetGridViewController (IntPtr handle) : base (handle)
		{
		}

		public override void AwakeFromNib ()
		{
			imageManager = new PHCachingImageManager ();
			ResetCachedAssets ();

			PHPhotoLibrary.SharedPhotoLibrary.RegisterChangeObserver (this);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Determine the size of the thumbnails to request from the PHCachingImageManager
			nfloat scale = UIScreen.MainScreen.Scale;
			CGSize cellSize = ((UICollectionViewFlowLayout)CollectionView.CollectionViewLayout).ItemSize;
			assetGridThumbnailSize = new CGSize (cellSize.Width * scale, cellSize.Height * scale);

			// Add button to the navigation bar if the asset collection supports adding content.
			if (AssetCollection == null || AssetCollection.CanPerformEditOperation (PHCollectionEditOperation.AddContent))
				NavigationItem.RightBarButtonItem = AddButton;
			else
				NavigationItem.RightBarButtonItem = null;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			UpdateCachedAssets ();
		}

		public override void Scrolled (UIScrollView scrollView)
		{
			ScrollViewDidScroll ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			// Configure the destination AssetViewController.
			var assetViewController = segue.DestinationViewController as AssetViewController;
			if (assetViewController == null)
				return;

			var indexPath = CollectionView.IndexPathForCell ((UICollectionViewCell)sender);
			assetViewController.Asset = (PHAsset)AssetsFetchResults [indexPath.Item];
			assetViewController.AssetCollection = AssetCollection;
		}

		protected override void Dispose (bool disposing)
		{
			PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver (this);
			base.Dispose (disposing);
		}

		public void PhotoLibraryDidChange (PHChange changeInstance)
		{
			// Check if there are changes to the assets we are showing.
			var collectionChanges = changeInstance.GetFetchResultChangeDetails (AssetsFetchResults);
			if (collectionChanges == null)
				return;

			DispatchQueue.MainQueue.DispatchAsync (() => {
				// Get the new fetch result.
				AssetsFetchResults = collectionChanges.FetchResultAfterChanges;
				UICollectionView collectionView = CollectionView;
				if (collectionChanges.HasIncrementalChanges || !collectionChanges.HasMoves) {
					collectionView.PerformBatchUpdates (() => {
						var removedIndexes = collectionChanges.RemovedIndexes;
						if (removedIndexes != null && removedIndexes.Count > 0)
							collectionView.DeleteItems (removedIndexes.GetIndexPaths (0));

						var insertedIndexes = collectionChanges.InsertedIndexes;
						if (insertedIndexes != null && insertedIndexes.Count > 0)
							collectionView.InsertItems (insertedIndexes.GetIndexPaths (0));

						var changedIndexes = collectionChanges.ChangedIndexes;
						if (changedIndexes != null && changedIndexes.Count > 0)
							collectionView.ReloadItems (changedIndexes.GetIndexPaths (0));
					}, null);
				} else {
					collectionView.ReloadData ();
				}

				ResetCachedAssets ();
			});
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return AssetsFetchResults.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var asset = (PHAsset)AssetsFetchResults [indexPath.Item];
			// Dequeue an GridViewCell.
			var cell = (GridViewCell)collectionView.DequeueReusableCell (cellReuseIdentifier, indexPath);
			cell.RepresentedAssetIdentifier = asset.LocalIdentifier;

			// Add a badge to the cell if the PHAsset represents a Live Photo.
			if (asset.MediaSubtypes == PHAssetMediaSubtype.PhotoLive) {
				// Add Badge Image to the cell to denote that the asset is a Live Photo.
				UIImage badge = PHLivePhotoView.GetLivePhotoBadgeImage (PHLivePhotoBadgeOptions.OverContent);
				cell.LivePhotoBadgeImage = badge;
			}

			// Request an image for the asset from the PHCachingImageManager.
			imageManager.RequestImageForAsset (asset, assetGridThumbnailSize, PHImageContentMode.AspectFill, null, (result, info) => {
				// Set the cell's thumbnail image if it's still showing the same asset.
				if (cell.RepresentedAssetIdentifier == asset.LocalIdentifier)
					cell.ThumbnailImage = result;
			});

			return cell;
		}

		void ScrollViewDidScroll ()
		{
			// Update cached assets for the new visible area.
			UpdateCachedAssets ();
		}

		void ResetCachedAssets ()
		{
			imageManager.StopCaching ();
			previousPreheatRect = CGRect.Empty;
		}

		void UpdateCachedAssets ()
		{
			bool isViewVisible = IsViewLoaded && View.Window != null;
			if (!isViewVisible)
				return;

			// The preheat window is twice the height of the visible rect.
			CGRect preheatRect = CollectionView.Bounds;
			preheatRect = preheatRect.Inset ( 0f, -.5f * preheatRect.Height);

			nfloat delta = NMath.Abs (preheatRect.GetMidY () - previousPreheatRect.GetMidY ());
			if (delta > CollectionView.Bounds.Height / 3.0f) {
				// Compute the assets to start caching and to stop caching.
				var addedIndexPaths = new List<NSIndexPath> ();
				var removedIndexPaths = new List<NSIndexPath> ();

				ComputeDifferenceBetweenRect (previousPreheatRect, preheatRect, removedRect => {
					var indexPaths = CollectionView.GetIndexPaths (removedRect);
					if (indexPaths != null)
						removedIndexPaths.AddRange (indexPaths);
				}, addedRect => {
					var indexPaths = CollectionView.GetIndexPaths (addedRect);
					if (indexPaths != null)
						addedIndexPaths.AddRange (indexPaths);
				});

				var assetsToStartCaching = AssetsAtIndexPaths (addedIndexPaths.ToArray ());
				var assetsToStopCaching = AssetsAtIndexPaths (removedIndexPaths.ToArray ());

				// Update the assets the PHCachingImageManager is caching.
				if (assetsToStartCaching != null)
					imageManager.StartCaching (assetsToStartCaching, assetGridThumbnailSize, PHImageContentMode.AspectFill, null);
				if (assetsToStopCaching != null)
					imageManager.StopCaching (assetsToStopCaching, assetGridThumbnailSize, PHImageContentMode.AspectFill, null);

				// Store the preheat rect to compare against in the future.
				previousPreheatRect = preheatRect;
			}
		}

		static void ComputeDifferenceBetweenRect (CGRect oldRect, CGRect newRect, Action<CGRect> removedHandler, Action<CGRect> addedHandler)
		{
			if (!oldRect.IntersectsWith (newRect)) {
				addedHandler (newRect);
				removedHandler (oldRect);
			} else {
				nfloat oldMaxY = oldRect.GetMaxY ();
				nfloat oldMinY = oldRect.GetMinY ();
				nfloat newMaxY = newRect.GetMaxY ();
				nfloat newMinY = newRect.GetMinY ();

				if (newMaxY > oldMaxY) {
					var rectToAdd = new CGRect (newRect.X, oldMaxY, newRect.Width, newMaxY - oldMaxY);
					addedHandler(rectToAdd);
				}

				if (oldMinY > newMinY) {
					var rectToAdd = new CGRect (newRect.X, newMinY, newRect.Width, oldMinY - newMinY);
					addedHandler(rectToAdd);
				}

				if (newMaxY < oldMaxY) {
					var rectToRemove = new CGRect (newRect.X, newMaxY, newRect.Width, oldMaxY - newMaxY);
					removedHandler(rectToRemove);
				}

				if (oldMinY < newMinY) {
					var rectToRemove = new CGRect (newRect.X, oldMinY, newRect.Width, newMinY - oldMinY);
					removedHandler(rectToRemove);
				}
			}
		}

		PHAsset[] AssetsAtIndexPaths (Array indexPaths)
		{
			if (indexPaths.Length == 0)
				return null;

			var assets = new PHAsset[indexPaths.Length];
			for (int i = 0; i < indexPaths.Length; i++) {
				var asset = (PHAsset)AssetsFetchResults.ObjectAt (i);
				assets [i] = asset;
			}

			return assets;
		}

		partial void AddButtonClickHandler (NSObject sender)
		{
			// Create a random dummy image.
			var rect = new Random ().Next (0, 2) == 0 ?
				new CGRect (0f, 0f, 400f, 300f) : new CGRect (0f, 0f, 300f, 400f);
			UIGraphics.BeginImageContextWithOptions (rect.Size, false, 1f);
			UIColor.FromHSBA (new Random ().Next (0, 100) / 100f, 1f, 1f, 1f).SetFill ();
			UIGraphics.RectFillUsingBlendMode (rect, CGBlendMode.Normal);
			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			// Add it to the photo library
			PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
				PHAssetChangeRequest assetChangeRequest = PHAssetChangeRequest.FromImage (image);

				if (AssetCollection != null) {
					PHAssetCollectionChangeRequest assetCollectionChangeRequest = PHAssetCollectionChangeRequest.ChangeRequest (AssetCollection);
					assetCollectionChangeRequest.AddAssets (new PHObject[] {
						assetChangeRequest.PlaceholderForCreatedAsset
					});
				}
			}, (success, error) => {
				if (!success)
					Console.WriteLine (error.LocalizedDescription);
			});
		}
	}
}
