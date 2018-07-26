using System;
using System.Collections.Generic;
using System.Linq;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Photos;
using PhotosUI;
using UIKit;

namespace SamplePhotoApp
{
	public partial class AssetGridViewController : UICollectionViewController, IPHPhotoLibraryChangeObserver
	{
		const string cellReuseIdentifier = "GridViewCell";

		public PHFetchResult FetchResult { get; set; }
		public PHAssetCollection AssetCollection { get; set; }

		readonly PHCachingImageManager imageManager = new PHCachingImageManager ();
		CGSize thumbnailSize;
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
			if (FetchResult == null)
			{
				var allPhotosOptions = new PHFetchOptions
				{
					SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor ("creationDate", true) }
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

			// Add button to the navigation bar if the asset collection supports adding content.
			if (AssetCollection == null || AssetCollection.CanPerformEditOperation (PHCollectionEditOperation.AddContent))
				NavigationItem.RightBarButtonItem = AddButtonItem;
			else
				NavigationItem.RightBarButtonItem = null;

			UpdateItemSize ();
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();
			UpdateItemSize ();
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
			assetViewController.Asset = (PHAsset)FetchResult[indexPath.Item];
			assetViewController.AssetCollection = AssetCollection;
		}

		void UpdateItemSize ()
		{
			var viewWidth = View.Bounds.Width;

			var desiredItemWidth = 100f;
			var columns = Math.Max (Math.Floor (viewWidth / desiredItemWidth), 4f);
			var padding = 1f;
			var itemWidth = Math.Floor ((viewWidth - (columns - 1f) * padding) / columns);
			var itemSize = new CGSize (itemWidth, itemWidth);

			if (Layout is UICollectionViewFlowLayout layout)
			{
				layout.ItemSize = itemSize;
				layout.MinimumInteritemSpacing = padding;
				layout.MinimumLineSpacing = padding;
			}

			// Determine the size of the thumbnails to request from the PHCachingImageManager
			var scale = UIScreen.MainScreen.Scale;
			thumbnailSize = new CGSize (itemSize.Width * scale, itemSize.Height * scale);
		}

		#region UICollectionView

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return FetchResult.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var asset = (PHAsset)FetchResult[indexPath.Item];

			// Dequeue an GridViewCell.
			var cell = (GridViewCell)collectionView.DequeueReusableCell (cellReuseIdentifier, indexPath);

			// Add a badge to the cell if the PHAsset represents a Live Photo.
			if (asset.MediaSubtypes.HasFlag (PHAssetMediaSubtype.PhotoLive))
				cell.LivePhotoBadgeImage = PHLivePhotoView.GetLivePhotoBadgeImage (PHLivePhotoBadgeOptions.OverContent);

			// Request an image for the asset from the PHCachingImageManager.
			cell.RepresentedAssetIdentifier = asset.LocalIdentifier;
			imageManager.RequestImageForAsset (asset, thumbnailSize, PHImageContentMode.AspectFill, null, (image, info) =>
			{
				// The cell may have been recycled by the time this handler gets called;
				// Set the cell's thumbnail image if it's still showing the same asset.
				if (cell.RepresentedAssetIdentifier == asset.LocalIdentifier && image != null)
					cell.ThumbnailImage = image;
			});

			return cell;
		}


		#endregion

		#region UIScrollView

#if __TVOS__
		[Export ("scrollViewDidScroll:")]
		public void Scrolled (UIScrollView scrollView)
#elif __IOS__
		public override void Scrolled (UIScrollView scrollView)
#endif
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
			// Update only if the view is visible.
			bool isViewVisible = IsViewLoaded && View.Window != null;
			if (!isViewVisible)
				return;

			// The preheat window is twice the height of the visible rect.
			var visibleRect = new CGRect (CollectionView.ContentOffset, CollectionView.Bounds.Size);
			var preheatRect = visibleRect.Inset (0f, -0.5f * visibleRect.Height);

			// Update only if the visible area is significantly different from the last preheated area.
			nfloat delta = NMath.Abs (preheatRect.GetMidY () - previousPreheatRect.GetMidY ());
			if (delta <= CollectionView.Bounds.Height / 3f)
				return;

			// Compute the assets to start caching and to stop caching.
			var rects = ComputeDifferenceBetweenRect (previousPreheatRect, preheatRect);
			var addedAssets = rects.added
								   .SelectMany (rect => CollectionView.GetIndexPaths (rect))
								   .Select (indexPath => FetchResult.ObjectAt (indexPath.Item))
								   .Cast<PHAsset> ()
								   .ToArray ();

			var removedAssets = rects.removed
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

		(IEnumerable<CGRect> added, IEnumerable<CGRect> removed) ComputeDifferenceBetweenRect (CGRect oldRect, CGRect newRect)
		{
			if (!oldRect.IntersectsWith (newRect))
			{
				return (new CGRect[] { newRect }, new CGRect[] { oldRect });
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

			return (added, removed);
		}

		#endregion

		#region UI Actions

		partial void AddAsset (NSObject sender)
		{
			var rnd = new Random ();

			// Create a random dummy image.
			var size = (rnd.Next (0, 2) == 0)
				? new CGSize (400f, 300f)
				: new CGSize (300f, 400f);

			var renderer = new UIGraphicsImageRenderer (size);
			var image = renderer.CreateImage (context =>
			{
				UIColor.FromHSBA ((float)rnd.NextDouble (), 1, 1, 1).SetFill ();
				context.FillRect (context.Format.Bounds);
			});

			// Add it to the photo library
			PHPhotoLibrary.SharedPhotoLibrary.PerformChanges (() =>
			{
				PHAssetChangeRequest creationRequest = PHAssetChangeRequest.FromImage (image);

				if (AssetCollection != null)
				{
					var addAssetRequest = PHAssetCollectionChangeRequest.ChangeRequest (AssetCollection);
					addAssetRequest.AddAssets (new PHObject[] {
						creationRequest.PlaceholderForCreatedAsset
					});
				}
			}, (success, error) =>
			{
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

			DispatchQueue.MainQueue.DispatchSync (() =>
			{
				// Hang on to the new fetch result.
				FetchResult = changes.FetchResultAfterChanges;

				if (changes.HasIncrementalChanges)
				{
					// If we have incremental diffs, animate them in the collection view.
					CollectionView.PerformBatchUpdates (() =>
					{
						// For indexes to make sense, updates must be in this order:
						// delete, insert, reload, move
						var removed = changes.RemovedIndexes;
						if (removed != null && removed.Count > 0)
							CollectionView.DeleteItems (ToNSIndexPaths (removed));

						var inserted = changes.InsertedIndexes;
						if (inserted != null && inserted.Count > 0)
							CollectionView.InsertItems (ToNSIndexPaths (inserted));

						var changed = changes.ChangedIndexes;
						if (changed != null && changed.Count > 0)
							CollectionView.ReloadItems (ToNSIndexPaths (changed));

						changes.EnumerateMoves ((fromIndex, toIndex) =>
						{
							var start = NSIndexPath.FromItemSection ((nint)fromIndex, 0);
							var end = NSIndexPath.FromItemSection ((nint)toIndex, 0);
							CollectionView.MoveItem (start, end);
						});
					}, null);

				}
				else
				{
					// Reload the collection view if incremental diffs are not available.
					CollectionView.ReloadData ();
				}

				ResetCachedAssets ();
			});
		}

		static NSIndexPath[] ToNSIndexPaths (NSIndexSet indexSet)
		{
			var cnt = indexSet.Count;
			var result = new NSIndexPath[(int)cnt];
			int i = 0;
			indexSet.EnumerateIndexes ((nuint idx, ref bool stop) =>
			{
				stop = false;
				result[i++] = NSIndexPath.FromItemSection ((nint)idx, 0);
			});
			return result;
		}

		#endregion
	}
}
