
using System;

using Foundation;
using UIKit;

namespace StateRestoration
{
	public partial class CollectionViewController : UICollectionViewController
	{
		const string DetailedViewControllerID = "DetailView";
		const string CellID = "cellID";

		public DataSource DataSource { get; set; }

		public CollectionViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ClearsSelectionOnViewWillAppear = false;
		}

		public void AdjustContentInsetForLegacy ()
		{
			CollectionView.ContentInset = new UIEdgeInsets (64f, 0f, 0f, 0f);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			var selectedItems = CollectionView.GetIndexPathsForSelectedItems ();
			if (selectedItems.Length == 0)
				return;

			UIView.Animate (0.3f, 0.0f, UIViewAnimationOptions.CurveLinear, () => {
				CollectionView.DeselectItem (selectedItems [0], true);
			}, null);
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return DataSource.GetItemsCount (section);
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (CollectionCell)collectionView.DequeueReusableCell ((NSString)CellID, indexPath);

			string identifier = DataSource.GetIdentifier (indexPath);
			cell.Label.Text = DataSource.GetTitle (identifier);
			cell.ImageView.Image = DataSource.GetThumbnail (identifier);

			return cell;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "showDetail")
				PrepareForSegue ((DetailViewController)segue.DestinationViewController);
		}

		void PrepareForSegue (DetailViewController detailViewController)
		{
			NSIndexPath selectedIndexPath = CollectionView.GetIndexPathsForSelectedItems () [0];

			detailViewController.ImageIdentifier = DataSource.GetIdentifier (selectedIndexPath);
			detailViewController.DataSource = DataSource;
		}
	}
}
