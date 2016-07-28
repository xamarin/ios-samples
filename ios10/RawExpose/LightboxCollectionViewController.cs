using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Photos;
using UIKit;

namespace RawExpose
{
	public partial class LightboxCollectionViewController : UICollectionViewController
	{
		const string imageSegueName = "ImageSegue";

		readonly List<PHAsset> assets = new List<PHAsset> ();

		public PHAssetCollection AssetCollection { get; internal set; }

		public LightboxCollectionViewController (IntPtr handle)
			: base (handle)
		{
		}

		// Requests all image of the assetCollections to be fetched.
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var assetCollection = AssetCollection;
			if (assetCollection == null)
				return;

			var items = PHAsset.FetchKeyAssets (assetCollection, new PHFetchOptions ())
							   .Cast<PHAsset> ();
			assets.AddRange (items);
		}

		#region UICollectionViewController delegate methods

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return assets.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (LightboxCollectionViewCell.ReuseIdentifier, indexPath) as LightboxCollectionViewCell;
			if (cell == null)
				throw new InvalidProgramException ("Unable to dequeue a LightboxCollectionViewCell");

			cell.ImageView.Image = null;

			var options = new PHImageRequestOptions {
				Synchronous = true
			};

			var asset = assets [indexPath.Row];
			PHImageManager.DefaultManager.RequestImageForAsset (asset, cell.Bounds.Size, PHImageContentMode.AspectFit, options, (requestedImage, _) => {
				cell.ImageView.Image = requestedImage;
			});

			return cell;
		}

		#endregion

		#region Storyboard seque

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			PerformSegue (imageSegueName, assets [indexPath.Row]);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var imageViewController = segue.DestinationViewController as ImageViewController;
			if (imageViewController == null)
				return;

			var asset = sender as PHAsset;
			if (asset == null)
				return;

			imageViewController.Asset = asset;
		}

		#endregion
	}
}
