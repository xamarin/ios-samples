using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

// This version purposefully introduces memory issues for demonstrating debugging with Instruments

namespace MemoryDemo
{
	public partial class MemoryDemoViewController : UICollectionViewController
	{
		// Id used for cell reuse
	    // static NSString cellId = new NSString ("ImageCell");

		// Declare image at the class level
		// UIImage image;

		// This holds onto multiple image instances causing memory to grow
		List<UIImage> images;
		
		public MemoryDemoViewController (UICollectionViewLayout layout) : base (layout)
		{	
			// Create the image from the test.png file
			// image = UIImage.FromFile ("test.png");

			// create a List<UIImage> to demo memory leak
			images = new List<UIImage> ();
			
			CollectionView.ContentSize = UIScreen.MainScreen.Bounds.Size;
			CollectionView.BackgroundColor = UIColor.White;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register the ImageCell class for creation by the reuse system
			// CollectionView.RegisterClassForCell (typeof(ImageCell), cellId);
		}
		
		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return 10000;
		}
		
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Dequeue a cell from the reuse pool
			// var imageCell = (ImageCell)collectionView.DequeueReusableCell (cellId, indexPath);

			// Reuse the image declared at the class level
			// imageCell.ImageView.Image = image;

			// Inefficient cell and image creation
			ImageCell imageCell = new ImageCell ();
			UIImage image = UIImage.FromFile ("test.png");
			images.Add(image);
			imageCell.ImageView.Image = image;
			
			return imageCell;
		}
	}

	public class ImageCell : UICollectionViewCell
	{
		public UIImageView ImageView { get; private set; }

		// Should use initWithFrame: instead, which is called by the reuse system
		public ImageCell () : base ()
		{
			ImageView = new UIImageView (new RectangleF (0, 0, 50, 50)); 
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			ContentView.AddSubview (ImageView);	
		}

		// This should be set at registation and used during dequeueing, not in a subclass
		public override NSString ReuseIdentifier {
			get {
				return new NSString ("ImageCell");
			}
		}
				
//		[Export ("initWithFrame:")]
//		public ImageCell (RectangleF frame) : base (frame)
//		{
//			ImageView = new UIImageView (new RectangleF (0, 0, 50, 50)); 
//			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
//			ContentView.AddSubview (ImageView);
//		}
	}
			
}