//#define NO_IMAGES
using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CollectionViewTransition {

	public class APLCollectionViewController : UICollectionViewController {

		public const int MAX_COUNT = 20;

		public APLCollectionViewController (UICollectionViewLayout layout) : base (layout)
		{
			CollectionView.RegisterClassForCell (typeof (APLCollectionViewCell), APLCollectionViewCell.Key);
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return MAX_COUNT;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (APLCollectionViewCell.Key, indexPath) as APLCollectionViewCell;
#if NO_IMAGES
			float hue = indexPath.Item / (float) MAX_COUNT;
			UIColor cellColor = UIColor.FromHSB (hue, 1.0f, 1.0f);
			cell.ContentView.BackgroundColor = cellColor;
#else
			cell.ImageView.Image = UIImage.FromFile ("Images/sa" + indexPath.Item + ".jpg");
#endif
			return cell;
		}

		public override UICollectionViewTransitionLayout TransitionLayout (UICollectionView collectionView, UICollectionViewLayout fromLayout, UICollectionViewLayout toLayout)
		{
			return new APLTransitionLayout (fromLayout, toLayout);
		}

		public virtual UICollectionViewController NextViewControllerAtPoint (CGPoint p)
		{
			return null;
		}
	}
}