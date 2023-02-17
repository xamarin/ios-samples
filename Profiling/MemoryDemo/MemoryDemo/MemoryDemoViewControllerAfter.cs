using System;

using CoreGraphics;
using Foundation;
using UIKit;

#if !GREEDY

namespace MemoryDemo {
	public class MemoryDemoViewController : UICollectionViewController {
		// Id used for cell reuse
		public const string CellId = "ImageCell";

		// Declare image at the class level
		readonly UIImage image;

		public MemoryDemoViewController (UICollectionViewLayout layout) : base (layout)
		{
			// Create the image from the test.png file
			image = UIImage.FromFile ("test.png");

			CollectionView.ContentSize = UIScreen.MainScreen.Bounds.Size;
			CollectionView.BackgroundColor = UIColor.White;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CollectionView.RegisterClassForCell (typeof (ImageCell), CellId);
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return 10000;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Dequeue a cell from the reuse pool
			var imageCell = (ImageCell) collectionView.DequeueReusableCell (CellId, indexPath);

			// Reuse the image declared at the class level
			imageCell.ImageView.Image = image;

			return imageCell;
		}
	}

	public class ImageCell : UICollectionViewCell {
		public UIImageView ImageView { get; private set; }

		[Export ("initWithFrame:")]
		public ImageCell (CGRect frame) : base (frame)
		{
			ImageView = new UIImageView (new CGRect (0f, 0f, 50f, 50f));
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			ContentView.AddSubview (ImageView);
		}
	}
}

#endif // !GREEDY

