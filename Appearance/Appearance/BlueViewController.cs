using System;
using UIKit;

namespace Appearance {
	public partial class BlueViewController : UIViewController {
		private PlainView plainView;

		public BlueViewController (IntPtr handle) : base (handle)
		{
			var style = UIButton.AppearanceWhenContainedIn (typeof (PlainView));
			style.SetTitleColor (UIColor.Blue, UIControlState.Normal);

			var style1 = UISlider.AppearanceWhenContainedIn (typeof (PlainView));
			style1.ThumbTintColor = UIColor.Blue;
			style1.MaximumTrackTintColor = UIColor.FromRGB (0, 114, 255);
			style1.MinimumTrackTintColor = UIColor.FromRGB (0, 255, 255);

			var style2 = UIProgressView.AppearanceWhenContainedIn (typeof (PlainView));
			style2.ProgressTintColor = UIColor.FromRGB (150, 221, 255);
			style2.TrackTintColor = UIColor.FromRGB (211, 255, 243);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			plainView = new PlainView { Frame = View.Bounds, BackgroundColor = UIColor.FromRGB (201, 207, 255) };
			View.AddSubview (plainView);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (plainView != null) {
				plainView.Dispose ();
				plainView = null;
			}
		}
	}
}
