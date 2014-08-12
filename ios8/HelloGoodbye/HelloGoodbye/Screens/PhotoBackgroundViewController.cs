using System;

using MonoTouch.UIKit;
using System.Drawing;

namespace HelloGoodbye
{
	public class PhotoBackgroundViewController : UIViewController
	{
		private UIImageView _backgroundView;
		private UIView _containerView;

		private UIImage _backgroundImage;
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

			RectangleF bounds = View.Bounds;
			SizeF imageSize = _backgroundView.Image.Size;
			float imageAspectRatio = imageSize.Width / imageSize.Height;
			float viewAspectRatio = bounds.Width / bounds.Height;

			if (viewAspectRatio > imageAspectRatio) {
				// Let the background run off the top and bottom of the screen, so it fills the width
				var scaledSize = new SizeF (bounds.Width, bounds.Width / imageAspectRatio);
				var location = new PointF (0, (bounds.Height - scaledSize.Height) / 2f);
				_backgroundView.Frame = new RectangleF (location, scaledSize);
			} else {
				// Let the background run off the left and right of the screen, so it fills the height
				var scaledSize = new SizeF (imageAspectRatio * bounds.Height, bounds.Height);
				var location = new PointF ((bounds.Width - scaledSize.Width) / 2f, 0);
				_backgroundView.Frame = new RectangleF (location, scaledSize);
			}
		}
	}
}

