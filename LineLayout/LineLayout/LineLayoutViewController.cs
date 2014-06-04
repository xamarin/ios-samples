using System;

using Foundation;
using UIKit;

namespace LineLayout
{
	public partial class LineLayoutViewController : UICollectionViewController
	{
		static readonly NSString cellToken = new NSString ("MY_CELL");
		
		public LineLayoutViewController (UICollectionViewFlowLayout layout) : base (layout)
		{			
		}
		
		public override void ViewDidLoad ()
		{
			CollectionView.RegisterClassForCell (typeof(Cell), cellToken);
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return 60;
		}
		
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			Cell cell = (Cell) collectionView.DequeueReusableCell (cellToken, indexPath);
			cell.Label.Text = indexPath.Row.ToString ();
			return cell;
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}
	}
}