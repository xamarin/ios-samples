using System;
using UIKit;

namespace Appearance {
	public partial class BlackViewController : UIViewController {
		private PlainView plainView;

		public BlackViewController (IntPtr handle) : base (handle)
		{
			var style = UIButton.AppearanceWhenContainedIn (typeof (PlainView));
			style.SetTitleColor (UIColor.Black, UIControlState.Normal);

			var style1 = UISlider.AppearanceWhenContainedIn (typeof (PlainView));
			style1.ThumbTintColor = UIColor.DarkGray;
			style1.MaximumTrackTintColor = UIColor.Gray;
			style1.MinimumTrackTintColor = UIColor.LightGray;

			var style2 = UIProgressView.AppearanceWhenContainedIn (typeof (PlainView));
			style2.ProgressTintColor = UIColor.DarkGray;
			style2.TrackTintColor = UIColor.LightGray;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;

			plainView = new PlainView { Frame = View.Bounds };
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
