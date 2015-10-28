using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class CollectionViewContainerViewController : UICollectionViewController {

		readonly nfloat MinimumEdgePadding = 90f;

		List<DataItem[]> dataItemsByGroup;
		public List<DataItem[]> DataItemsByGroup {
			get {
				if (dataItemsByGroup == null) {
					dataItemsByGroup = new List<DataItem[]> ();

					foreach (Group groupType in Enum.GetValues (typeof(Group)))
						dataItemsByGroup.Add (DataItem.SampleItems.Where (c => c.Group == groupType).ToArray ());
				}

				return dataItemsByGroup;
			}
		}

		[Export ("initWithCoder:")]
		public CollectionViewContainerViewController (NSCoder coder): base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var collectionView = CollectionView;
			var layout = collectionView?.CollectionViewLayout as UICollectionViewFlowLayout;

			if (collectionView == null || layout == null)
				return;
			
			CollectionView.ContentInset = new UIEdgeInsets (
				MinimumEdgePadding - layout.SectionInset.Top,
				CollectionView.ContentInset.Left,
				MinimumEdgePadding - layout.SectionInset.Bottom,
				CollectionView.ContentInset.Right
			);
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return 1;
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return DataItemsByGroup.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return (UICollectionViewCell)CollectionView.DequeueReusableCell (CollectionViewContainerCell.ReuseIdentifier, indexPath);
		}

		public override void WillDisplayCell (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			var currentCell = cell as CollectionViewContainerCell;
			if (currentCell == null)
				throw new Exception ("Expected to display a `CollectionViewContainerCell`.");

			var sectionDataItems = DataItemsByGroup [indexPath.Section];
			currentCell.ConfigureWithDataItems (sectionDataItems);
		}

		public override bool CanFocusItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return false;
		}
	}
}
