using System;
using Foundation;
using UIKit;

namespace iTravel {
	public partial class ImageViewController : UIViewController {
		public string PictureName { get; set; }

		protected ImageViewController(IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (string.IsNullOrEmpty (PictureName))
				Console.WriteLine ("PictureName property is empty string or null, it's not good.");

			var imagePath = NSBundle.MainBundle.PathForResource (PictureName, "jpg");
			ImageView.Image = UIImage.FromFile (imagePath);
		}
	}
}

