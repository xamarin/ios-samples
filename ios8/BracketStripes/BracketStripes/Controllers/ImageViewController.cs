using System;
using CoreGraphics;
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BracketStripes
{
	public class ImageViewController : UIViewController
	{
		public Action<ImageViewController> ImageViewControllerDidFinish { get; set; }

		public ZoomImageView ImageView { get; set; }

		public ImageViewController (UIImage image)
		{
			ImageView = new ZoomImageView {
				Image = image,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
			};
		}

		public override void LoadView ()
		{
			base.LoadView ();

			AutomaticallyAdjustsScrollViewInsets = false;
			View = ImageView;

			var iosBlueColor = new UIColor (0f, 122f / 255f, 1f, 1f);
			NavigationController.NavigationBar.TintColor = iosBlueColor;

			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done);
			NavigationItem.RightBarButtonItem.Clicked += ((sender, e) => ImageViewControllerDidFinish (this));
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationItem.RightBarButtonItem.Clicked -= RightBarButtonItemClickHandler;
			base.ViewWillDisappear (animated);
		}

		private void RightBarButtonItemClickHandler (object sender, EventArgs e)
		{
			ImageViewControllerDidFinish (this);
		}
	}
}
