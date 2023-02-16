using System;
using UIKit;

namespace Appearance {
	public partial class AppearanceViewController : UIViewController {
		public AppearanceViewController (IntPtr handle) : base (handle)
		{
			// Set the default appearance values
			UIButton.Appearance.TintColor = UIColor.LightGray;
			UIButton.Appearance.SetTitleColor (UIColor.FromRGB (0, 127, 14), UIControlState.Normal);

			UISlider.Appearance.ThumbTintColor = UIColor.Red;
			UISlider.Appearance.MinimumTrackTintColor = UIColor.Orange;
			UISlider.Appearance.MaximumTrackTintColor = UIColor.Yellow;

			UIProgressView.Appearance.ProgressTintColor = UIColor.Yellow;
			UIProgressView.Appearance.TrackTintColor = UIColor.Orange;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// setting the values directly OVERRIDES the Appearance defaults
			slider2.ThumbTintColor = UIColor.FromRGB (0, 127, 70);
			slider2.MinimumTrackTintColor = UIColor.FromRGB (66, 255, 63);
			slider2.MaximumTrackTintColor = UIColor.FromRGB (197, 255, 132);

			progress2.ProgressTintColor = UIColor.FromRGB (66, 255, 63);
			progress2.TrackTintColor = UIColor.FromRGB (197, 255, 132);
		}
	}
}
