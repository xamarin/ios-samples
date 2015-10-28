using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace UICatalog {
	public partial class CollectionViewController : UICollectionViewController {

		readonly DataItem[] items = DataItem.SampleItems;
		readonly DataItemCellComposer cellComposer = new DataItemCellComposer ();

		[Export ("initWithCoder:")]
		public CollectionViewController (NSCoder coder): base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			if (CollectionView == null)
				return;
			
			CollectionView.MaskView = new GradientMaskView (new CGRect (CGPoint.Empty, CollectionView.Bounds.Size));
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			var maskView = CollectionView.MaskView as GradientMaskView;
			if (maskView == null)
				return;

			maskView.MaskPosition = new CGPoint (maskView.MaskPosition.X, TopLayoutGuide.Length * 0.8f);

			var maximumMaskStart = maskView.MaskPosition.Y + TopLayoutGuide.Length * 0.5f;
			var verticalScrollPosition = NMath.Max (0, CollectionView.ContentOffset.Y + CollectionView.ContentInset.Top);
			maskView.MaskPosition = new CGPoint (
				NMath.Min (maximumMaskStart, maskView.MaskPosition.Y + verticalScrollPosition),
				maskView.MaskPosition.Y
			);
			maskView.Frame = new CGRect (new CGPoint (0, CollectionView.ContentOffset.Y), CollectionView.Bounds.Size);
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return items.Length;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return (UICollectionViewCell)CollectionView.DequeueReusableCell (DataItemCollectionViewCell.ReuseIdentifier, indexPath);
		}

		public override void WillDisplayCell (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			var currentcell = cell as DataItemCollectionViewCell;

			if (currentcell == null)
				throw new Exception ("Expected to display a `DataItemCollectionViewCell`.");

			var item = items [indexPath.Row];
			cellComposer.ComposeCell (currentcell, item);
		}
	}
}
