using System;
using CoreGraphics;
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace BracketStripes
{
	public class ZoomImageView : UIView, IUIScrollViewDelegate
	{
		private UIScrollView scrollView;
		private UIImageView imageView;

		public UIImage Image {
			get {
				return imageView.Image;
			}

			set {
				imageView.Image = ResizeImage (value);
				value.Dispose ();

				imageView.SizeToFit ();
				NeedsSizing = true;
				SetNeedsLayout ();
			}
		}

		private bool NeedsSizing { get; set; }

		public ZoomImageView ()
		{
			BackgroundColor = UIColor.White;

			scrollView = new UIScrollView (CGRect.Empty) {
				WeakDelegate = this
			};

			AddSubview (scrollView);

			imageView = new UIImageView (CGRect.Empty);
			scrollView.AddSubview (imageView);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			scrollView.Frame = Bounds;

			if (NeedsSizing)
				PerformSizing ();
		}

		private void PerformSizing ()
		{
			scrollView.ZoomScale = scrollView.MinimumZoomScale = scrollView.MaximumZoomScale = 1f;

			var image = imageView.Image;
			scrollView.ContentSize = image.Size;

			if (image != null) {
				nfloat aspect = image.Size.Width / image.Size.Height;

				if (aspect * Bounds.Size.Height > Bounds.Size.Width)
					scrollView.ZoomScale = scrollView.MinimumZoomScale = Bounds.Size.Width / image.Size.Width;
				else
					scrollView.ZoomScale = scrollView.MinimumZoomScale = Bounds.Size.Height / image.Size.Height;
			}

			CenterImageInScrollView ();
			NeedsSizing = false;
		}

		private UIImage ResizeImage (UIImage view)
		{
			UIImage resultImage = null;

			var newSize = new CGSize (view.Size.Width / 2, view.Size.Height / 2);
			UIGraphics.BeginImageContext (newSize);
			view.Draw (new CGRect (0, 0, newSize.Width, newSize.Height));
			resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return resultImage;
		}

		private void CenterImageInScrollView ()
		{
			var boundsSize = scrollView.Bounds.Size;
			CGRect frameToCenter = imageView.Frame;

			if (frameToCenter.Size.Width < boundsSize.Width)
				frameToCenter.X = ((int)boundsSize.Width - frameToCenter.Size.Width) / 2;
			else
				frameToCenter.X = 0;

			if (frameToCenter.Size.Height < boundsSize.Height)
				frameToCenter.Y = ((int)boundsSize.Height - frameToCenter.Size.Height) / 2;
			else
				frameToCenter.Y = 0;

			imageView.Frame = frameToCenter;
		}

		private void HandleDidZoom (object sender, EventArgs e)
		{
			CenterImageInScrollView ();
		}

		[Export ("viewForZoomingInScrollView:")]
		public virtual UIView ViewForZoomingInScrollView (UIScrollView scrollView)
		{
			return imageView;
		}

		[Export ("scrollViewDidZoom:")]
		public virtual void DidZoom (UIScrollView scrollView)
		{
			CenterImageInScrollView ();
		}
	}
}
