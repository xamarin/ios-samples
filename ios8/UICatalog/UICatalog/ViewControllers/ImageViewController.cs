using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("ImageViewController")]
	public partial class ImageViewController : UIViewController
	{
		public ImageViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// The root view of the view controller set in Interface Builder is a UIImageView.
			UIImageView imageView = View as UIImageView;

			imageView.AnimationImages = new UIImage[] {
				UIImage.FromBundle ("image_animal_1"),
				UIImage.FromBundle ("image_animal_2"),
				UIImage.FromBundle ("image_animal_3"),
				UIImage.FromBundle ("image_animal_4"),
				UIImage.FromBundle ("image_animal_5")
			};

			// We want the image to be scaled to the correct aspect ratio within imageView's bounds.
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			// If the image does not have the same aspect ratio as imageView's bounds, then imageView's backgroundColor will be applied to the "empty" space.
			imageView.BackgroundColor = UIColor.White;

			imageView.AnimationDuration = 5;
			imageView.StartAnimating ();

			imageView.IsAccessibilityElement = true;
			imageView.AccessibilityLabel = "Animated".Localize ();
		}
	}
}
