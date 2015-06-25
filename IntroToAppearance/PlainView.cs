using System;
using UIKit;
using CoreGraphics;

namespace Appearance {
	public class PlainView : UIView {

		UIButton plainButton;
		UISlider slider;
		UIProgressView progress;

		public PlainView ()
		{
			plainButton = UIButton.FromType(UIButtonType.RoundedRect);
			plainButton.SetTitle ("Plain Button", UIControlState.Normal);
			slider = new UISlider();
			progress = new UIProgressView();

			plainButton.Frame = new CGRect(20, 150, 130, 40);
			slider.Frame = new CGRect(20, 190, 250, 20);
			progress.Frame = new CGRect(20, 230, 250, 20);
			
			slider.Value = 0.75f;

			progress.Progress = 0.35f;

			Add (plainButton);
			Add (slider);
			Add (progress);
		}
	}
}