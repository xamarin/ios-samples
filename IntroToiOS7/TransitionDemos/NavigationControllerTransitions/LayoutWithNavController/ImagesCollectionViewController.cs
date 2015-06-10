using System;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Threading;
using MonoTouch.Dialog.Utilities;

namespace LayoutWithNavController
{
	public class ImagesCollectionViewController : UICollectionViewController
	{
		static readonly NSString cellId = new NSString ("ImageCell");

		// for use with UseLayoutToLayoutNavigationTransitions
		CircleLayout circleLayout;
		ImagesCollectionViewController controller2;

		Monkeys monkeys;

		public ImagesCollectionViewController (UICollectionViewLayout layout) : base (layout)
		{
			CollectionView.ContentSize = UIScreen.MainScreen.Bounds.Size;
			CollectionView.BackgroundColor = UIColor.White;

			monkeys = Monkeys.Instance;

			Title = (layout is CircleLayout) ? "Circle" : "Grid";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
              
			// register the ImageCell so it can be created from a DequeueReusableCell call
			CollectionView.RegisterClassForCell (typeof(ImageCell), cellId);
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return monkeys.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// get an ImageCell from the pool. DequeueReusableCell will create one if necessary
			ImageCell imageCell = (ImageCell)collectionView.DequeueReusableCell (cellId, indexPath);

			// update the image for the speaker
			imageCell.UpdateImage (monkeys [indexPath.Row].ImageFile);
                
			return imageCell;
		}

		public override bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return Layout is UICollectionViewFlowLayout;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// UseLayoutToLayoutNavigationTransitions when item is selected

			circleLayout = new CircleLayout (monkeys.Count) { ItemSize = new CGSize (100, 100) };
			controller2 = new ImagesCollectionViewController (circleLayout);
			controller2.UseLayoutToLayoutNavigationTransitions = true;

			NavigationController.PushViewController (controller2, true);
		}
		// class to use for cell
		class ImageCell : UICollectionViewCell
		{
			UIImageView imageView;

			[Export ("initWithFrame:")]
			ImageCell (CGRect frame) : base (frame)
			{
				// create an image view to use in the cell
				imageView = new UIImageView (new CGRect (0, 0, 100, 100)); 
				imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

				// populate the content view
				ContentView.AddSubview (imageView);

				// scale the content view down so that the background view is visible, effecively as a border
				ContentView.Transform = CGAffineTransform.MakeScale (0.9f, 0.9f);

				// background view displays behind content view and selected background view
				BackgroundView = new UIView { BackgroundColor = UIColor.Black };

				// selected background view displays over background view when cell is selected
				SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Yellow };
			}

			internal void UpdateImage (string path)
			{
				imageView.Image = UIImage.FromFile (path);
			}
		}
	}
}

