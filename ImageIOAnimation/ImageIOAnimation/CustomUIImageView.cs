using CoreGraphics;
using ImageIO;
using UIKit;

namespace ImageIOAnimation {
	public class CustomUIImageView : UIImageView {

		private AnimatedImage animatedImage;
		public AnimatedImage AnimatedImage {
			get { return animatedImage; }

			set {
				SetAnimatedImage (value);
			}
		}

		public CustomUIImageView (CGRect bounds) : base (bounds) { }

		protected void SetImage (UIImage image)
		{
			this.Image = image;
		}

		void MyAnimationBlock (System.nint arg0, CGImage arg1, out bool done)
		{
			// TODO: ObjC uses strongSelf + weakSelf to check whether the user has dismissed the view
			if (this.animatedImage.IsEqual (animatedImage)) {
				UIImage loadedImage = UIImage.FromImage (arg1);
				SetImage (loadedImage);
				animatedImage.Size = loadedImage.Size;
				done = false;
			} else {
				done = true;
			}
		}

		void SetAnimatedImage (AnimatedImage anImage)
		{
			if (this.AnimatedImage != null && (this.AnimatedImage == anImage || this.AnimatedImage.IsEqual (anImage))) {
				return;
			}

			this.animatedImage = anImage;
			this.SetImage (null);
			CGImageAnimation animation = new CGImageAnimation ();

			if (anImage.data != null) {
				animation.CGAnimateImageData (anImage.data, null, MyAnimationBlock);
			} else if (anImage.url != null) {
				animation.CGAnimateImageAtUrl (anImage.url, null, MyAnimationBlock);
			}
		}
	}
}
