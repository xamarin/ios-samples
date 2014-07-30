using System;

using Foundation;
using UIKit;

namespace TextKitDemo
{
	public partial class CollectionViewController : UICollectionViewController
	{
		NSString key = new NSString ("collectionViewCell");

		public CollectionViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.BarTintColor = UIColor.White;
			NavigationItem.BackBarButtonItem = new UIBarButtonItem ("", UIBarButtonItemStyle.Plain, null, null);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			foreach (CollectionViewCell cell in CollectionView.VisibleCells) {
				var indexPath = CollectionView.IndexPathForCell (cell);
				cell.FormatCell (DemoModel.GetDemo(indexPath));
			}
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			CollectionViewCell cell = (CollectionViewCell) CollectionView.DequeueReusableCell (key, indexPath);

			if (cell == null)
				return null;

			cell.FormatCell (DemoModel.GetDemo (indexPath));
			return cell;
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return DemoModel.Demos.Count;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			DemoModel demo = DemoModel.GetDemo (indexPath);

			var newViewController = (TextViewController) Storyboard.InstantiateViewController (demo.ViewControllerIdentifier);
			newViewController.Title = demo.Title;
			newViewController.model = demo;
			NavigationController.PushViewController (newViewController, true);

			return;
		}
	}
}

