using System;
using System.Linq;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class SearchResultsViewController : UICollectionViewController, IUISearchResultsUpdating {

		public static readonly string StoryboardIdentifier = "SearchResultsViewController";

		DataItemCellComposer cellComposer = new DataItemCellComposer ();
		DataItem[] allDataItems = DataItem.SampleItems;
		DataItem[] filteredDataItems = DataItem.SampleItems;

		string filterString;
		string FilterString {
			get {
				return filterString;
			}
			set {
				if (filterString == value)
					return;
				
				filterString = value;
				filteredDataItems = string.IsNullOrEmpty (filterString) ?
					allDataItems : allDataItems.Where (c => c.Title.Contains (filterString)).ToArray ();

				CollectionView?.ReloadData ();
			}
		}

		[Export ("initWithCoder:")]
		public SearchResultsViewController (NSCoder coder): base (coder)
		{
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return filteredDataItems.Length;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (DataItemCollectionViewCell.ReuseIdentifier, indexPath);
			return (UICollectionViewCell)cell;
		}

		public override void WillDisplayCell (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			var currentCell = cell as DataItemCollectionViewCell;
			if (currentCell == null)
				throw new Exception ("Expected to display a `DataItemCollectionViewCell`.");

			var item = filteredDataItems[indexPath.Row];
			cellComposer.ComposeCell (currentCell, item);
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			DismissViewController (true, null);
		}

		public void UpdateSearchResultsForSearchController (UISearchController searchController)
		{
			FilterString = searchController.SearchBar.Text ?? string.Empty;
		}
	}
}
