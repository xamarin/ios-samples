using System;
using System.Globalization;

using UIKit;
using Foundation;
using CoreGraphics;

namespace LookInside
{
	public class RootViewController : UICollectionViewController, IUICollectionViewDataSource
	{
		static readonly nint kNumberOfViews = 37;
		static readonly nfloat kViewsWide = 5;
		static readonly nfloat kViewMargin = 2;
		static readonly NSString kCellReuseIdentifier = new NSString("CellReuseIdentifier");

		CoolTransitioningDelegate coolTransitioningDelegate;
		OverlayTransitioningDelegate overlayTransitioningDelegate;
		OverlayViewController overlay;

		UISwitch presentSwitch;

		public bool PresentationShouldBeAwesome {
			get {
				return presentSwitch.On;
			}
			private set {
				presentSwitch.On = value;
			}
		}

		public RootViewController ()
			: base(new UICollectionViewFlowLayout())
		{
			ConfigureTitleBar ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CollectionView.RegisterClassForCell(typeof(PhotoCollectionViewCell), kCellReuseIdentifier);
			CollectionView.BackgroundColor = null;
			var layout = (UICollectionViewFlowLayout)Layout;

			layout.MinimumInteritemSpacing = kViewMargin;
			layout.MinimumLineSpacing = kViewMargin;

			ViewWillTransitionToSize (View.Bounds.Size, null);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return kNumberOfViews;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (PhotoCollectionViewCell)collectionView.DequeueReusableCell (kCellReuseIdentifier, indexPath);
			string photoName = indexPath.Item.ToString (CultureInfo.InvariantCulture);
			UIImage photo = UIImage.FromBundle (photoName);

			cell.Image = photo;

			return cell;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			coolTransitioningDelegate = coolTransitioningDelegate ?? new CoolTransitioningDelegate ();
			overlayTransitioningDelegate = overlayTransitioningDelegate ?? new OverlayTransitioningDelegate ();

			overlay = overlay ?? new OverlayViewController ();
			if (PresentationShouldBeAwesome)
				overlay.TransitioningDelegate = coolTransitioningDelegate;
			else
				overlay.TransitioningDelegate = overlayTransitioningDelegate;
			overlay.PhotoView = (PhotoCollectionViewCell)CollectionView.CellForItem (indexPath);

			PresentViewController (overlay, true, null);
		}

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);

			nfloat itemWidth = toSize.Width / kViewsWide;
			itemWidth -= kViewMargin;

			((UICollectionViewFlowLayout)CollectionView.CollectionViewLayout).ItemSize = new CGSize(itemWidth, itemWidth);
			Layout.InvalidateLayout ();
		}

		void ConfigureTitleBar()
		{
			Title = "LookInside Photos";
			EdgesForExtendedLayout = UIRectEdge.Left | UIRectEdge.Bottom | UIRectEdge.Right;

			presentSwitch = new UISwitch ();
			presentSwitch.TintColor = UIColor.Purple;
			presentSwitch.TintColor = UIColor.FromRGBA (1, 0, 0, 0.2f);

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (presentSwitch);
		}
	}
}

