using System;
using System.Drawing;

using UIKit;
using CoreGraphics;

namespace HelloGoodbye
{
	public class PhotoBackgroundViewController : UIViewController
	{
		UIImageView _backgroundView;
		UIView _containerView;

		UIImage _backgroundImage;
		public UIImage BackgroundImage {
			get {
				return _backgroundImage;
			}
			set {
				if (value == null)
					throw new ArgumentNullException ("value");

				_backgroundImage = value;

				if(_backgroundView != null)
					_backgroundView.Image = _backgroundImage;
			}
		}

		public PhotoBackgroundViewController ()
		{
		}

		public override void LoadView ()
		{
			_containerView = new UIView {
				(_backgroundView = new UIImageView (BackgroundImage))
			};

			_containerView.ClipsToBounds = true;
			View = _containerView;
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			CGRect bounds = View.Bounds;
			CGSize imageSize = _backgroundView.Image.Size;
			nfloat imageAspectRatio = imageSize.Width / imageSize.Height;
			nfloat viewAspectRatio = bounds.Width / bounds.Height;

			if (viewAspectRatio > imageAspectRatio) {
				// Let the background run off the top and bottom of the screen, so it fills the width
				var scaledSize = new CGSize (bounds.Width, bounds.Width / imageAspectRatio);
				var location = new CGPoint (0, (bounds.Height - scaledSize.Height) / 2f);
				_backgroundView.Frame = new CGRect (location, scaledSize);
			} else {
				// Let the background run off the left and right of the screen, so it fills the height
				var scaledSize = new CGSize (imageAspectRatio * bounds.Height, bounds.Height);
				var location = new CGPoint ((bounds.Width - scaledSize.Width) / 2f, 0);
				_backgroundView.Frame = new CGRect (location, scaledSize);
			}
		}
	}
}

