using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;

namespace DropDestination {
	/**
	 Wraps a GridView of a given fixed cell size with a
	 scroll view. May be subclassed for additional drop
	 interaction functionality -- see
	 DroppableImageGridViewController.
	 */
	public class ImageGridViewController : UIViewController {
		#region Computed Properties
		public CGSize FixedCellSize { get; set; } = CGSize.Empty;
		public UIScrollView ScrollView { get; set; } = new UIScrollView ();
		public GridView ContainerView { get; set; }
		#endregion

		#region Constructors
		public ImageGridViewController ()
		{
		}

		public ImageGridViewController (NSCoder coder) : base (coder)
		{
		}

		public ImageGridViewController (CGSize cellSize)
		{
			// Initialize
			FixedCellSize = cellSize;
			View.UserInteractionEnabled = true;
		}
		#endregion

		#region Public Methods
		public void AddImageNamed (string name)
		{
			var image = UIImage.FromBundle (name);
			if (image == null) return;
			var view = NextView ();
			view.Image = image;
		}

		public DraggableImageView NextView ()
		{
			var newView = new DraggableImageView () {
				ClipsToBounds = true,
				ContentMode = UIViewContentMode.ScaleAspectFill
			};
			ContainerView.AddArrangedView (newView);
			ScrollView.SetNeedsLayout ();
			return newView;
		}

		public void RemoveViewsInSet (List<UIView> viewsToRemove)
		{
			var indicesToRemove = new NSMutableIndexSet ();

			foreach (UIView view in viewsToRemove) {
				var index = ContainerView.IndexForView (view);
				if (index > -1) indicesToRemove.Add ((nuint) index);
			}

			if (indicesToRemove.Count == 0) return;

			ContainerView.RemoveArrangedViewsAtIndexes (indicesToRemove);
			ScrollView.SetNeedsLayout ();
		}
		#endregion

		#region Override Methods
		public override void LoadView ()
		{
			base.LoadView ();

			ScrollView.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height);
			View.AddSubview (ScrollView);

			ContainerView = new GridView (FixedCellSize, new CGRect (0, 0, ScrollView.Frame.Width, ScrollView.Frame.Height));
			ScrollView.AddSubview (ContainerView);

			ContainerView.GridSizeChanged += (size) => {
				ScrollView.ContentSize = size;
			};
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			ScrollView.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height);
			ContainerView.Width = View.Frame.Width;
		}
		#endregion
	}
}
