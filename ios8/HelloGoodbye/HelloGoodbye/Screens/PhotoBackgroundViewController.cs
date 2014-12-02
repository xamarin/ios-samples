using System;
using System.Drawing;

using UIKit;
using CoreGraphics;

namespace HelloGoodbye
{
	public class PhotoBackgroundViewController : UIViewController
	{
		UIImageView backgroundView;
		UIView containerView;

		UIImage backgroundImage;
		public UIImage BackgroundImage {
			get {
				return backgroundImage;
			}
			set {
				if (value == null)
					throw new ArgumentNullException ("value");

				backgroundImage = value;

				if(backgroundView != null)
					backgroundView.Image = backgroundImage;
			}
		}

		public PhotoBackgroundViewController ()
		{
		}

		public override void LoadView ()
		{
			containerView = new UIView {
				(backgroundView = new UIImageView (BackgroundImage))
			};

			containerView.ClipsToBounds = true;
			View = containerView;
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			CGRect bounds = View.Bounds;
			CGSize imageSize = backgroundView.Image.Size;
			nfloat imageAspectRatio = imageSize.Width / imageSize.Height;
			nfloat viewAspectRatio = bounds.Width / bounds.Height;

			if (viewAspectRatio > imageAspectRatio) {
				// Let the background run off the top and bottom of the screen, so it fills the width
				var scaledSize = new CGSize (bounds.Width, bounds.Width / imageAspectRatio);
				var location = new CGPoint (0, (bounds.Height - scaledSize.Height) / 2f);
				backgroundView.Frame = new CGRect (location, scaledSize);
			} else {
				// Let the background run off the left and right of the screen, so it fills the height
				var scaledSize = new CGSize (imageAspectRatio * bounds.Height, bounds.Height);
				var location = new CGPoint ((bounds.Width - scaledSize.Width) / 2f, 0);
				backgroundView.Frame = new CGRect (location, scaledSize);
			}
		}
	}
}

