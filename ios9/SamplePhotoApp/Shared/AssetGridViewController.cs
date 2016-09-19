using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using Photos;
using PhotosUI;

namespace SamplePhotoApp
{
	public class Rects
	{
		public IEnumerable<CGRect> Added { get; set; }
		public IEnumerable<CGRect> Removed { get; set; }
	}

	public partial class AssetGridViewController : UICollectionViewController, IPHPhotoLibraryChangeObserver
	{
		// TODO: does it match to id defined in storyboard ?
		const string cellReuseIdentifier = "Cell";

		public PHFetchResult FetchResult { get; set; }
		public PHAssetCollection AssetCollection { get; set; }

		static CGSize thumbnailSize;

		readonly PHCachingImageManager imageManager = new PHCachingImageManager ();
		CGRect previousPreheatRect;

		[Export ("initWithCoder:")]
		public AssetGridViewController (NSCoder coder)
			: base (coder)
		{
		}

		public AssetGridViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ResetCachedAssets ();
			PHPhotoLibrary.SharedPhotoLibrary.RegisterChangeObserver (this);

			// If we get here without a segue, it's because we're visible at app launch,
			// so match the behavior of segue from the default "All Photos" view.
			if (FetchResult == null) {
				var allPhotosOptions = new PHFetchOptions {
					SortDescriptors = new NSSortDescriptor [] { new NSSortDescriptor ("creationDate", true) }
				};
				FetchResult = PHAsset.FetchAssets (allPhotosOptions);
			}
		}

		protected override void Dispose (bool disposing)
		{
			PHPhotoLibrary.SharedPhotoLibrary.UnregisterChangeObserver (this);
			base.Dispose (disposing);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Determine the size of the thumbnails to request from the PHCachingImageManager
			nfloat scale = UIScreen.MainScreen.Scale;
			CGSize cellSize = ((UICollectionViewFlowLayout)CollectionView.CollectionViewLayout).ItemSize;
			thumbnailSize = new CGSize (cellSize.Width * scale, cellSize.Height * scale);

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

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			// Configure the destination AssetViewController.
			var assetViewController = segue.DestinationViewController as AssetViewController;
			if (assetViewController == null)
				throw new InvalidProgramException ("unexpected view controller for segue");

			var indexPath = CollectionView.IndexPathForCell ((UICollectionViewCell)sender);
			assetViewController.Asset = (PHAsset)FetchResult [indexPath.Item];
			assetViewController.AssetCollection = AssetCollection;
		}

		#region UICollectionView

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return FetchResult.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var asset = (PHAsset)FetchResult [indexPath.Item];

			// Dequeue an GridViewCell.
			var cell = (GridViewCell)collectionView.DequeueReusableCell (cellReuseIdentifier, indexPath);

#if __IOS__
			// Add a badge to the cell if the PHAsset represents a Live Photo.
			// Add Badge Image to the cell to denote that the asset is a Live Photo.
			if (asset.MediaSubtypes.HasFlag (PHAssetMediaSubtype.PhotoLive))
				cell.LivePhotoBadgeImage = PHLivePhotoView.GetLivePhotoBadgeImage (PHLivePhotoBadgeOptions.OverContent);
#endif

			// Request an image for the asset from the PHCachingImageManager.
			cell.RepresentedAssetIdentifier = asset.LocalIdentifier;
			imageManager.RequestImageForAsset (asset, thumbnailSize, PHImageContentMode.AspectFill, null, (image, info) => {
				// Set the cell's thumbnail image if it's still showing the same asset.
				if (cell.RepresentedAssetIdentifier == asset.LocalIdentifier)
					cell.ThumbnailImage = image;
			});

			return cell;
		}


		#endregion

		#region UIScrollView

		public override void Scrolled (UIScrollView scrollView)
		{
			UpdateCachedAssets ();
		}

		#endregion

		#region Asset Caching

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
			preheatRect = preheatRect.Inset (0, -preheatRect.Height / 2);

			// Update only if the visible area is significantly different from the last preheated area.
			nfloat delta = NMath.Abs (preheatRect.GetMidY () - previousPreheatRect.GetMidY ());
			if (delta <= CollectionView.Bounds.Height / 3)
				return;

			// Compute the assets to start caching and to stop caching.
			var addedIndexPaths = new List<NSIndexPath> ();
			var removedIndexPaths = new List<NSIndexPath> ();

			var rects = ComputeDifferenceBetweenRect (previousPreheatRect, preheatRect);
			var addedAssets = rects.Added
								   .SelectMany (rect => CollectionView.GetIndexPaths (rect))
								   .Select (indexPath => FetchResult.ObjectAt (indexPath.Item))
								   .Cast<PHAsset> ()
								   .ToArray ();

			var removedAssets = rects.Removed
									 .SelectMany (rect => CollectionView.GetIndexPaths (rect))
									 .Select (indexPath => FetchResult.ObjectAt (indexPath.Item))
									 .Cast<PHAsset> ()
									 .ToArray ();

			// Update the assets the PHCachingImageManager is caching.
			imageManager.StartCaching (addedAssets, thumbnailSize, PHImageContentMode.AspectFill, null);
			imageManager.StopCaching (removedAssets, thumbnailSize, PHImageContentMode.AspectFill, null);

			// Store the preheat rect to compare against in the future.
			previousPreheatRect = preheatRect;
		}

		static Rects ComputeDifferenceBetweenRect (CGRect oldRect, CGRect newRect)
		{
			if (!oldRect.IntersectsWith (newRect)) {
				return new Rects {
					Added = new CGRect [] { newRect },
					Removed = new CGRect [] { oldRect }
				};
			}

			var oldMaxY = oldRect.GetMaxY ();
			var oldMinY = oldRect.GetMinY ();
			var newMaxY = newRect.GetMaxY ();
			var newMinY = newRect.GetMinY ();

			var added = new List<CGRect> ();
			var removed = new List<CGRect> ();

			if (newMaxY > oldMaxY)
				added.Add (new CGRect (newRect.X, oldMaxY, newRect.Width, newMaxY - oldMaxY));

			if (oldMinY > newMinY)
				added.Add (new CGRect (newRect.X, newMinY, newRect.Width, oldMinY - newMinY));

			if (newMaxY < oldMaxY)
				removed.Add (new CGRect (newRect.X, newMaxY, newRect.Width, oldMaxY - newMaxY));

			if (oldMinY < newMinY)
				removed.Add (new CGRect (newRect.X, oldMinY, newRect.Width, newMinY - oldMinY));

			return new Rects {
				Added = added,
				Removed = removed
			};
		}

		#endregion

		#region UI Actions

		partial void AddButtonClickHandler (NSObject sender)
		{
			var rnd = new Random ();
				
			// Create a random dummy image.
			var size = (rnd.Next (0, 2) == 0)
				? new CGSize (400f, 300f)
				: new CGSize (300f, 400f);

			var renderer = new UIGraphicsImageRenderer (size);
			var image = renderer.CreateImage (context => {
				UIColor.FromHSBA ((float)rnd.NextDouble (), 1, 1, 1).SetFill ();
				context.FillRect (context.Format.Bounds);
			});

			// Add it to the photo library
			PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() => {
				PHAssetChangeRequest creationRequest = PHAssetChangeRequest.FromImage (image);

				if (AssetCollection != null) {
					var addAssetRequest = PHAssetCollectionChangeRequest.ChangeRequest (AssetCollection);
					addAssetRequest.AddAssets (new PHObject [] {
						creationRequest.PlaceholderForCreatedAsset
					});
				}
			}, (success, error) => {
				if (!success)
					Console.WriteLine (error.LocalizedDescription);
			});
		}

		#endregion

		#region IPHPhotoLibraryChangeObserver

		public void PhotoLibraryDidChange (PHChange changeInstance)
		{
			var changes = changeInstance.GetFetchResultChangeDetails (FetchResult);
			if (changes == null)
				return;

			DispatchQueue.MainQueue.DispatchSync (() => {
				// Hang on to the new fetch result.
				FetchResult = changes.FetchResultAfterChanges;

				if (changes.HasIncrementalChanges) {
					// If we have incremental diffs, animate them in the collection view.
					CollectionView.PerformBatchUpdates (() => {
						// For indexes to make sense, updates must be in this order:
						// delete, insert, reload, move
						var removed = changes.RemovedIndexes;
						if (removed.Count > 0)
							CollectionView.DeleteItems (ToNSIndexPaths (removed));

						var inserted = changes.InsertedIndexes;
						if (inserted.Count > 0)
							CollectionView.InsertItems (ToNSIndexPaths (inserted));

						var changed = changes.ChangedIndexes;
						if (changed.Count > 0)
							CollectionView.ReloadItems (ToNSIndexPaths (changed));

						changes.EnumerateMoves ((fromIndex, toIndex) => {
							var start = NSIndexPath.FromItemSection ((nint)fromIndex, 0);
							var end = NSIndexPath.FromItemSection ((nint)toIndex, 0);
							CollectionView.MoveItem (start, end);
						});
					}, null);

				} else {
					// Reload the collection view if incremental diffs are not available.
					CollectionView.ReloadData ();
				}

				ResetCachedAssets ();
			});
		}

		static NSIndexPath [] ToNSIndexPaths (NSIndexSet indexSet)
		{
			var cnt = indexSet.Count;
			var result = new NSIndexPath [(int)cnt];
			indexSet.EnumerateIndexes ((nuint idx, ref bool stop) => {
				stop = false;
				result [idx] = NSIndexPath.FromItemSection ((nint)idx, 0);
			});
			return result;
		}

		#endregion

	}
}
