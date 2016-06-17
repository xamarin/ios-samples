using System;
using CoreGraphics;
using System.Linq;
using UIKit;
using Foundation;
using CoreImage;

namespace Appearance {
	public class BlueViewController : UIViewController {
		PlainView plainView;

		public BlueViewController ()
		{
			Title = "Blue";			

			var style = UIButton.AppearanceWhenContainedIn(typeof(PlainView));
			style.SetTitleColor(UIColor.Blue, UIControlState.Normal);	

			var style1 = UISlider.AppearanceWhenContainedIn(typeof(PlainView));
			style1.ThumbTintColor = UIColor.Blue;
			style1.MaximumTrackTintColor = UIColor.FromRGB(0, 114, 255);
			style1.MinimumTrackTintColor = UIColor.FromRGB(0, 255, 255);


			var style2 = UIProgressView.AppearanceWhenContainedIn(typeof(PlainView));
			style2.ProgressTintColor = UIColor.FromRGB(150, 221, 255);
			style2.TrackTintColor = UIColor.FromRGB(211, 255, 243);
		}

		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();

			plainView = new PlainView();
			plainView.Frame = View.Bounds;
			
			plainView.BackgroundColor = UIColor.FromRGB (201,207,255);

			View.AddSubview(plainView);
		}
	}
}