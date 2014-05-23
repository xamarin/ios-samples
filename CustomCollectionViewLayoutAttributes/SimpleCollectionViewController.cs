using System;
using System.Collections.Generic;
using CoreGraphics;

using UIKit;
using Foundation;


namespace SimpleCollectionView
{
    public class SimpleCollectionViewController : UICollectionViewController
    {
        static NSString animalCellId = new NSString ("AnimalCell");
        List<IAnimal> animals;
        UIGestureRecognizer tapRecognizer;

        public SimpleCollectionViewController (UICollectionViewLayout layout) : base (layout)
        {
            animals = new List<IAnimal> ();
            for (int i = 0; i < 20; i++) {
				animals.Add (i % 2 == 0 ? (IAnimal) new Monkey () : (IAnimal) new Tamarin ());
            }
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            tapRecognizer = new UITapGestureRecognizer (Tapped);
            CollectionView.AddGestureRecognizer (tapRecognizer);
            CollectionView.RegisterClassForCell (typeof (AnimalCell), animalCellId);
			CollectionView.BackgroundColor = UIColor.LightGray;
        }

        void Tapped ()
        {
            if (tapRecognizer.State == UIGestureRecognizerState.Ended) {
                var pinchPoint = tapRecognizer.LocationInView (CollectionView);
				var tappedCellPath = GetIndexPathsForVisibleItems (pinchPoint);
                if (tappedCellPath != null) {
					animals.RemoveAt ((int)tappedCellPath.Row);
					CollectionView.DeleteItems (new NSIndexPath[] { tappedCellPath });
                }
            }
        }

		public NSIndexPath GetIndexPathsForVisibleItems (CGPoint touchPoint)
		{
			for (int i = 0; i < CollectionView.VisibleCells.Length; i++) {
				if (CollectionView.VisibleCells [i].Frame.Contains (touchPoint))
					return CollectionView.IndexPathForCell (CollectionView.VisibleCells [i]);
			}

			return null;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
        {
            return animals.Count;
        }

        public override UICollectionViewCell GetCell (UICollectionView collectionView, Foundation.NSIndexPath indexPath)
        {
            var animalCell = (AnimalCell) collectionView.DequeueReusableCell (animalCellId, indexPath);

			var animal = animals [(int)indexPath.Row];
            animalCell.Image = animal.Image;

            return animalCell;
        }
    }

    public class AnimalCell : UICollectionViewCell
    {
        UIImageView imageView;

        [Export ("initWithFrame:")]
        public AnimalCell (CoreGraphics.CGRect frame) : base (frame)
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.Orange };

            SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green };

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

		public override void ApplyLayoutAttributes (UICollectionViewLayoutAttributes layoutAttributes)
		{
			var attributes = layoutAttributes as CustomCollectionViewLayoutAttributes;
			if (attributes != null) {
				var data = attributes.Data;
				attributes.Center = new CGPoint (data.Center.X + data.Radius * attributes.Distance * (float) Math.Cos (2 * attributes.Row * Math.PI / data.CellCount),
				                                data.Center.Y + data.Radius * attributes.Distance * (float) Math.Sin (2 * attributes.Row * Math.PI / data.CellCount));

				if (!nfloat.IsNaN (attributes.Center.X) && !nfloat.IsNaN (attributes.Center.Y) &&
					UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
					Center = attributes.Center;
			}

			base.ApplyLayoutAttributes (layoutAttributes);
		}
    }
}

