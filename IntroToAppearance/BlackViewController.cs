using System;
using CoreGraphics;
using System.Linq;
using UIKit;
using Foundation;
using CoreImage;

namespace Appearance {
	public class BlackViewController : UIViewController {
		PlainView plainView;

		public BlackViewController ()
		{
			Title = "Black";

			var style = UIButton.AppearanceWhenContainedIn(typeof(PlainView));
			style.SetTitleColor(UIColor.Black, UIControlState.Normal);	

			var style1 = UISlider.AppearanceWhenContainedIn(typeof(PlainView));
			style1.ThumbTintColor = UIColor.DarkGray;
			style1.MaximumTrackTintColor = UIColor.Gray;
			style1.MinimumTrackTintColor = UIColor.LightGray;


			var style2 = UIProgressView.AppearanceWhenContainedIn(typeof(PlainView));
			style2.ProgressTintColor = UIColor.DarkGray;
			style2.TrackTintColor = UIColor.LightGray;

		}
		
		
		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();

			plainView = new PlainView();
			plainView.Frame = View.Bounds;
			View.BackgroundColor = UIColor.White;

			View.AddSubview(plainView);
		}
	}
}

