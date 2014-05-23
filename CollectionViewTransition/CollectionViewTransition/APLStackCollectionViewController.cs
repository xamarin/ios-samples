using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CollectionViewTransition {

	public class APLStackCollectionViewController : APLCollectionViewController {

		public APLStackCollectionViewController (UICollectionViewLayout layout) : base (layout)
		{
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			NavigationController.PushViewController (NextViewControllerAtPoint (CGPoint.Empty), true);
		}

		public override UICollectionViewController NextViewControllerAtPoint (CGPoint p)
		{
			UICollectionViewFlowLayout grid = new UICollectionViewFlowLayout () {
				ItemSize = new CGSize (75.0f, 75.0f),
				SectionInset = new UIEdgeInsets (10.0f, 10.0f, 10.0f, 10.0f)
			};
			return new APLGridCollectionViewController (grid) {
				Title = "Grid Layout",
				UseLayoutToLayoutNavigationTransitions = true
			};
		}
	}
}