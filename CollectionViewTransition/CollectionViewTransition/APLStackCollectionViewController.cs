using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CollectionViewTransition {

	public class APLStackCollectionViewController : APLCollectionViewController {

		public APLStackCollectionViewController (UICollectionViewLayout layout) : base (layout)
		{
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			NavigationController.PushViewController (NextViewControllerAtPoint (PointF.Empty), true);
		}

		public override UICollectionViewController NextViewControllerAtPoint (PointF p)
		{
			UICollectionViewFlowLayout grid = new UICollectionViewFlowLayout () {
				ItemSize = new SizeF (75.0f, 75.0f),
				SectionInset = new UIEdgeInsets (10.0f, 10.0f, 10.0f, 10.0f)
			};
			return new APLGridCollectionViewController (grid) {
				Title = "Grid Layout",
				UseLayoutToLayoutNavigationTransitions = true
			};
		}
	}
}