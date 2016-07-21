using System;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

namespace MessagesExtension {
	public partial class BuildIceCreamViewController : UIViewController, IUICollectionViewDataSource {
		public static readonly string StoryboardIdentifier = "BuildIceCreamViewController";

		public IBuildIceCreamViewControllerDelegate Builder { get; set; }
		string promt;

		IceCreamPart[] iceCreamParts;
		IceCreamPart[] IceCreamParts {
			get {
				return iceCreamParts;
			}
			set {
				iceCreamParts = value;

				if (IsViewLoaded)
					CollectionView.ReloadData ();
			}
		}

		IceCream iceCream;
		public IceCream IceCream {
			get {
				return iceCream;
			}
			set {
				iceCream = value;

				if (iceCream == null)
					return;

				if (iceCream.Base == null) {
					IceCreamParts = Base.All;
					promt = "Select a base";
				} else if (iceCream.Scoops == null) {
					IceCreamParts = Scoops.All;
					promt = "Add some scoops";
				} else if (iceCream.Topping == null) {
					IceCreamParts = Topping.All;
					promt = "Finish with a topping";
				}
			}
		}

		public BuildIceCreamViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Make sure the prompt and ice cream view are showing the correct information.
			PromtLabel.Text = promt;
			IceCreamView.IceCream = iceCream;
			CollectionView.DecelerationRate = UIScrollView.DecelerationRateFast;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			// There is nothing to layout of there are no ice cream parts to pick from.
			if (iceCreamParts.Length == 0)
				return;

			var layout = CollectionView.CollectionViewLayout as UICollectionViewFlowLayout;
			if (layout == null)
				throw new Exception ("Expected the collection view to have a UICollectionViewFlowLayout");

			// The ideal cell width is 1/3 of the width of the collection view.
			layout.ItemSize = new CGSize (NMath.Floor (View.Bounds.Size.Width / 3f), layout.ItemSize.Height);

			// Set the cell height using the aspect ratio of the ice cream part images.
			var iceCreamPartImageSize = iceCreamParts [0].Image.Size;

			if (iceCreamPartImageSize.Width < 0)
				return;

			var imageAspectRatio = iceCreamPartImageSize.Width / iceCreamPartImageSize.Height;
			layout.ItemSize = new CGSize (layout.ItemSize.Width, layout.ItemSize.Width / imageAspectRatio);
			// Set the collection view's height constraint to match the cell size.
			CollectionViewHeightConstraint.Constant = layout.ItemSize.Height;

			// Adjust the collection view's `contentInset` so the first item is centered.
			var contentInset = CollectionView.ContentInset;
			contentInset.Left = contentInset.Right = (View.Bounds.Width - layout.ItemSize.Width) / 2f;

			CollectionView.ContentInset = contentInset;

			// Calculate the ideal height of the ice cream view.
			nfloat iceCreamViewContentHeight = 0f;
			IceCreamView.ArrangedSubviews.ToList ().ForEach (s => iceCreamViewContentHeight += s.IntrinsicContentSize.Height);

			var iceCreamPartImageScale = layout.ItemSize.Height / iceCreamPartImageSize.Height;
			IceCreamViewHeightConstraint.Constant = iceCreamViewContentHeight * iceCreamPartImageScale;
		}

		partial void DidTapSelect (NSObject sender)
		{
			// Determine the index path of the centered cell in the collection view.
			var layout = CollectionView.CollectionViewLayout as IceCreamPartCollectionViewLayout;
			if (layout == null)
				throw new Exception ("Expected the collection view to have a IceCreamPartCollectionViewLayout");

			var halfWidth = CollectionView.Bounds.Width / 2f;
			var indexPath = layout.IndexPathForVisibleItemClosest (CollectionView.ContentOffset.X + halfWidth);
			if (indexPath == null)
				return;

			Builder?.Build (this, iceCreamParts [indexPath.Row]);
		}

		public nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return iceCreamParts.Count ();
		}

		public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = CollectionView.DequeueReusableCell (IceCreamPartCell.ReuseIdentifier, indexPath) as IceCreamPartCell;

			if (cell == null)
				throw new Exception ("Unable to dequeue a BodyPartCell");

			var iceCreamPart = iceCreamParts [indexPath.Row];
			cell.ImageView.Image = iceCreamPart.Image;

			return cell;
		}
	}
}
