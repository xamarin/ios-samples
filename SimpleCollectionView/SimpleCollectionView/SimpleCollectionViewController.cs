using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace SimpleCollectionView
{
    public class SimpleCollectionViewController : UICollectionViewController
    {
        static NSString animalCellId = new NSString ("AnimalCell");
        static NSString headerId = new NSString ("Header");
        List<IAnimal> animals;

        public SimpleCollectionViewController (UICollectionViewLayout layout) : base (layout)
        {
            animals = new List<IAnimal> ();
            for (int i = 0; i < 20; i++) {
                animals.Add (new Monkey ());
            }
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            CollectionView.RegisterClassForCell (typeof(AnimalCell), animalCellId);
            CollectionView.RegisterClassForSupplementaryView (typeof(Header), UICollectionElementKindSection.Header, headerId);

			UIMenuController.SharedMenuController.MenuItems = new UIMenuItem[] {
				new UIMenuItem ("Custom", new Selector ("custom"))
			};
		}

        public override nint NumberOfSections (UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount (UICollectionView collectionView, nint section)
        {
            return animals.Count;
        }

        public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var animalCell = (AnimalCell)collectionView.DequeueReusableCell (animalCellId, indexPath);

            var animal = animals [indexPath.Row];

            animalCell.Image = animal.Image;

            return animalCell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            var headerView = (Header)collectionView.DequeueReusableSupplementaryView (elementKind, headerId, indexPath);
            headerView.Text = "Supplementary View";
            return headerView;
        }

        public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem (indexPath);
            cell.ContentView.BackgroundColor = UIColor.Yellow;
        }

        public override void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem (indexPath);
            cell.ContentView.BackgroundColor = UIColor.White;
        }

        public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

//      public override bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath)
//      {
//          return false;
//      }

        // for edit menu
        public override bool ShouldShowMenu (UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        public override bool CanPerformAction (UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender)
		{
			// Selector should be the same as what's in the custom UIMenuItem
			if (action == new Selector ("custom"))
				return true;
			else
				return false;
        }

        public override void PerformAction (UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender)
        {
			System.Diagnostics.Debug.WriteLine ("code to perform action");
        }

		// CanBecomeFirstResponder and CanPerform are needed for a custom menu item to appear
		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}

		/*public override bool CanPerform (Selector action, NSObject withSender)
		{
			if (action == new Selector ("custom"))
				return true;
			else
				return false;
		}*/

        public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate (toInterfaceOrientation, duration);

            var lineLayout = CollectionView.CollectionViewLayout as LineLayout;
            if (lineLayout != null)
            {
                if((toInterfaceOrientation == UIInterfaceOrientation.Portrait) || (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown))
                    lineLayout.SectionInset = new UIEdgeInsets (400,0,400,0);
                else
                    lineLayout.SectionInset  = new UIEdgeInsets (220, 0.0f, 200, 0.0f);
            }
        }

    }

    public class AnimalCell : UICollectionViewCell
    {
        UIImageView imageView;

        [Export ("initWithFrame:")]
        public AnimalCell (CGRect frame) : base (frame)
        {
            BackgroundView = new UIView{BackgroundColor = UIColor.Orange};

            SelectedBackgroundView = new UIView{BackgroundColor = UIColor.Green};

            ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
            ContentView.Layer.BorderWidth = 2.0f;
            ContentView.BackgroundColor = UIColor.White;
            ContentView.Transform = CGAffineTransform.MakeScale (0.8f, 0.8f);

            imageView = new UIImageView (UIImage.FromBundle ("placeholder.png"));
            imageView.Center = ContentView.Center;
            imageView.Transform = CGAffineTransform.MakeScale (0.7f, 0.7f);

            ContentView.AddSubview (imageView);
        }

        public UIImage Image {
            set {
                imageView.Image = value;
            }
        }

		[Export("custom")]
		void Custom()
		{
			// Put all your custom menu behavior code here
			Console.WriteLine ("custom in the cell");
		}


		public override bool CanPerform (Selector action, NSObject withSender)
		{
			if (action == new Selector ("custom"))
				return true;
			else
				return false;
		}
    }

    public class Header : UICollectionReusableView
    {
        UILabel label;

        public string Text {
            get {
                return label.Text;
            }
            set {
                label.Text = value;
                SetNeedsDisplay ();
            }
        }

        [Export ("initWithFrame:")]
        public Header (CGRect frame) : base (frame)
        {
            label = new UILabel (){Frame = new CGRect (0,0,300,50), BackgroundColor = UIColor.Yellow};
            AddSubview (label);
        }
    }

}

