using System;

using UIKit;

namespace XamariniOSRatingBar
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			btn1.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn2.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn3.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn4.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn5.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);

			lblRating.Text = "Your Rating:0/5";

		}

		partial void UIButton17_TouchUpInside(UIButton sender)
		{
			btn1.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn2.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn3.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn4.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn5.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);


			lblRating.Text = "Your Rating:5/5";
		}

		partial void UIButton16_TouchUpInside(UIButton sender)
		{
			btn1.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn2.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn3.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn4.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);


			btn5.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);

			lblRating.Text = "Your Rating:4/5";
		}

		partial void UIButton15_TouchUpInside(UIButton sender)
		{

			btn1.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn2.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn3.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);



			btn4.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn5.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);

			lblRating.Text = "Your Rating:3/5";
		}

		partial void UIButton14_TouchUpInside(UIButton sender)
		{
			btn1.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);
			btn2.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);


			btn3.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn4.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn5.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);

			lblRating.Text = "Your Rating:2/5";
		}

		partial void UIButton13_TouchUpInside(UIButton sender)
		{
			btn1.SetBackgroundImage(UIImage.FromFile("yellow-star.png"), UIControlState.Normal);

			btn2.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn3.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn4.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);
			btn5.SetBackgroundImage(UIImage.FromFile("gray-star.png"), UIControlState.Normal);

			lblRating.Text = "Your Rating:1/5";
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
