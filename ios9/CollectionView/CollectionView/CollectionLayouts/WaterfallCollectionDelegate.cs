using Foundation;
using UIKit;

namespace CollectionView {
	/// <summary>
	/// Waterfall collection delegate.
	/// </summary>
	/// <remarks>
	/// Origionally created by Nicholas Tau on 6/30/14.
	/// Copyright (c) 2014 Nicholas Tau. All rights reserved.
	/// Ported from http://nshint.io/blog/2015/07/16/uicollectionviews-now-have-easy-reordering/ to
	/// Xamarin.iOS by Kevin Mullins.
	/// </remarks>
	public class WaterfallCollectionDelegate : UICollectionViewDelegate {
		#region Computed Properties
		public WaterfallCollectionView CollectionView { get; set;}
		#endregion

		#region Constructors
		public WaterfallCollectionDelegate (WaterfallCollectionView collectionView)
		{
			// Initialize
			CollectionView = collectionView;
		}
		#endregion

		#region Overrides Methods
		public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Always allow for highlighting
			return true;
		}

		public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Get cell and change to green background
			var cell = collectionView.CellForItem (indexPath);
			cell.ContentView.BackgroundColor = UIColor.FromRGB (183, 208, 57);
		}

		public override void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Get cell and return to blue background
			var cell = collectionView.CellForItem (indexPath);
			cell.ContentView.BackgroundColor = UIColor.FromRGB (164, 205, 255);
		}
		#endregion
	}
}

