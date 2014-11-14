using System;
using CoreGraphics;

using UIKit;
using Foundation;

namespace PinchIt
{
	public class ViewController : UICollectionViewController
	{
		static NSString cellClass = new NSString ("Cell");

		public ViewController (UICollectionViewFlowLayout layout) : base (layout)
		{
		}

		public override void ViewDidLoad ()
		{
			CollectionView.RegisterClassForCell (typeof (Cell), cellClass);

			var pinchRecognizer = new UIPinchGestureRecognizer (handlePinchGesture);
			handlePinchGesture (pinchRecognizer);

			CollectionView.AddGestureRecognizer (pinchRecognizer);
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 63 : 12;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return (UICollectionViewCell) collectionView.DequeueReusableCell (cellClass, indexPath);
		}

		public void handlePinchGesture (UIPinchGestureRecognizer sender)
		{
			PinchLayout pinchLayout = (PinchLayout) CollectionView.CollectionViewLayout;

			switch (sender.State) {
			case UIGestureRecognizerState.Began:
				CGPoint initialPinchPoint = sender.LocationInView (CollectionView);
				pinchLayout.pinchedCellPath = CollectionView.IndexPathForItemAtPoint (initialPinchPoint);
				break;
			case UIGestureRecognizerState.Changed:
				pinchLayout.setPinchedCellScale ((float)sender.Scale);
				pinchLayout.setPinchedCellCenter (sender.LocationInView (CollectionView));
				break;
			default:
				CollectionView.PerformBatchUpdates (delegate {
					pinchLayout.pinchedCellPath = null;
					pinchLayout.setPinchedCellScale (1.0f);
				}, null);
				break;
			}
		}
	}
}

