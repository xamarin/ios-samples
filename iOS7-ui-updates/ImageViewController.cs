using CoreGraphics;
using UIKit;
using System;

namespace ImageView {

	public class ImageViewController : UIViewController {
		
		UIImageView imageView;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "ImageView";
			View.BackgroundColor = UIColor.White;

		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			nfloat y = 10;

			//EdgeForExtendedLayout example
//			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) { 
//				this.EdgesForExtendedLayout = UIRectEdge.None;
//			}

			//TopLayoutGuide example
//			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) { 
//				y = this.TopLayoutGuide.Length;
//
//			}

			// a simple image
			imageView = new UIImageView (UIImage.FromBundle ("MonkeySFO.png")); // Build Action:Content
			imageView.Frame = new CGRect (10, y, imageView.Image.CGImage.Width, imageView.Image.CGImage.Height);
			View.AddSubview (imageView);

		}
	}
}