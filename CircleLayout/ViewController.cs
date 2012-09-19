using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CircleLayout
{
	public partial class ViewController : UICollectionViewController
	{
		static NSString cellClass = new NSString ("Cell");
		int cellCount = 20;

		public ViewController (UICollectionViewLayout layout) : base (layout)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			CollectionView.RegisterClassForCell (typeof(Cell), cellClass);
			CollectionView.AddGestureRecognizer (new UITapGestureRecognizer (HandleTapGesture));
			CollectionView.ReloadData ();
			CollectionView.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return cellCount;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return (UICollectionViewCell) collectionView.DequeueReusableCell (cellClass, indexPath);
		}

		void HandleTapGesture (UITapGestureRecognizer sender)
		{
			if (sender.State != UIGestureRecognizerState.Ended)
				return;
			
			PointF initialPinchPoint = sender.LocationInView (CollectionView);
			NSIndexPath tappedCellPath = CollectionView.IndexPathForItemAtPoint (initialPinchPoint);
			
			if (tappedCellPath != null) {
				cellCount--;
				
				CollectionView.PerformBatchUpdates (delegate {
						CollectionView.DeleteItems (new NSIndexPath [] { tappedCellPath });
					}, null);
			} else {
				cellCount++;
				
				CollectionView.PerformBatchUpdates (delegate {
						CollectionView.InsertItems (new NSIndexPath[] {
								NSIndexPath.FromItemSection (0, 0)
							});
					}, null);
			}
		}
	}
}