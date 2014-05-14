using System;
using CoreGraphics;

using Foundation;
using UIKit;

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
			CollectionView.BackgroundColor = UIColor.DarkGray;
		}

		//adjust the status bar in ios7 to use default or light style
		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
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
			
			CGPoint initialPinchPoint = sender.LocationInView (CollectionView);
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