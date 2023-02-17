using System;
using UIKit;

namespace UICatalog {
	public partial class ImageViewController : UIViewController {
		public ImageViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			imageView.AnimationImages = new UIImage []
			{
				UIImage.FromBundle("image_animal_1"),
				UIImage.FromBundle("image_animal_2"),
				UIImage.FromBundle("image_animal_3"),
				UIImage.FromBundle("image_animal_4"),
				UIImage.FromBundle("image_animal_5")
			};

			// We want the image to be scaled to the correct aspect ratio within imageView's bounds.
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			imageView.AnimationDuration = 5;
			imageView.StartAnimating ();
		}
	}
}
