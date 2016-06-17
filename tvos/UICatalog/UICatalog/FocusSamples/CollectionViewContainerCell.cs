using System;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class CollectionViewContainerCell : UICollectionViewCell, IUICollectionViewDataSource, IUICollectionViewDelegate {

		DataItem[] dataItems;
		DataItemCellComposer cellComposer = new DataItemCellComposer ();

		[Export ("reuseIdentifier")]
		public static new string ReuseIdentifier => "CollectionViewContainerCell";

		public override UIView PreferredFocusedView {
			get {
				return CollectionView;
			}
		}

		public void ConfigureWithDataItems (DataItem[] dataItems)
		{
			this.dataItems = dataItems;
			CollectionView.ReloadData ();
		}

		[Export ("initWithCoder:")]
		public CollectionViewContainerCell (NSCoder coder): base (coder)
		{
		}

		public nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return 1;
		}

		[Export ("numberOfSectionsInCollectionView:")]
		public nint NumberOfSections (UICollectionView collectionView)
		{
			return dataItems.Length;
		}

		public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return (UICollectionViewCell)CollectionView.DequeueReusableCell (DataItemCollectionViewCell.ReuseIdentifier, indexPath);
		}

		[Export ("collectionView:willDisplayCell:forItemAtIndexPath:")]
		public void WillDisplayCell (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			var currentCell = cell as DataItemCollectionViewCell;
			if (currentCell == null)
				throw new Exception ("Expected to display a DataItemCollectionViewCell");

			var item = dataItems [indexPath.Section];

			// Configure the cell.
			cellComposer.ComposeCell (currentCell, item);
		}
	}
}
