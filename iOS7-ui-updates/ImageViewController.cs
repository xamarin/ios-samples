using System;

using CoreGraphics;
using UIKit;

namespace ImageView {
	public class ImageViewController : UIViewController {
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "ImageView";
			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			nfloat offset = 10f;

			var imageView = new UIImageView (UIImage.FromBundle ("MonkeySFO.png"));
			imageView.Frame = new CGRect (offset, offset, imageView.Image.CGImage.Width, imageView.Image.CGImage.Height);

			View.AddSubview (imageView);
		}
	}
}