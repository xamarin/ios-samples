using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MemoryDemo
{
	public partial class MemoryDemoViewController : UICollectionViewController
	{
		// Id used for cell reuse
		static NSString cellId = new NSString ("ImageCell");

		// Declare image at the class level
		UIImage image;
		
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

			// Register the ImageCell class for creation by the reuse system
			CollectionView.RegisterClassForCell (typeof(ImageCell), cellId);
		}
		
		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return 10000;
		}
		
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Dequeue a cell from the reuse pool
			ImageCell imageCell = (ImageCell)collectionView.DequeueReusableCell (cellId, indexPath);

			// Reuse the image declared at the class level
			imageCell.ImageView.Image = image;
			
			return imageCell;
		}
	}

	public class ImageCell : UICollectionViewCell
	{
		public UIImageView ImageView { get; private set; }
				
		[Export ("initWithFrame:")]
		public ImageCell (RectangleF frame) : base (frame)
		{
			ImageView = new UIImageView (new RectangleF (0, 0, 50, 50)); 
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			ContentView.AddSubview (ImageView);
		}
	}			
}