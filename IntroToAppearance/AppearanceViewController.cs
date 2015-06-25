using System;
using CoreGraphics;
using System.Linq;
using UIKit;
using Foundation;
using CoreImage;

namespace Appearance
{
	/// <summary>
	/// View containing Buttons, TextView and ImageViews to show off the samples
	/// </summary>
	/// <remarks>
	/// See the 'SampleCode.cs' file for the actual sample code
	/// </remarks> 
	public class AppearanceViewController : UIViewController
	{
		UIButton btnBlack, btnBlue;
		UISlider slider, slider2;
		UIProgressView progress, progress2;
		UILabel overrideLabel;

		public AppearanceViewController () {

		// Set the default appearance values
		UIButton.Appearance.TintColor = UIColor.LightGray;
		UIButton.Appearance.SetTitleColor(UIColor.FromRGB(0,127,14), UIControlState.Normal);
			
		UISlider.Appearance.ThumbTintColor = UIColor.Red;
		UISlider.Appearance.MinimumTrackTintColor = UIColor.Orange;
		UISlider.Appearance.MaximumTrackTintColor = UIColor.Yellow;
			
		UIProgressView.Appearance.ProgressTintColor = UIColor.Yellow;
		UIProgressView.Appearance.TrackTintColor = UIColor.Orange;



			btnBlack = UIButton.FromType(UIButtonType.System);
			btnBlue = UIButton.FromType(UIButtonType.System);
		slider = new UISlider(new CGRect(10,110,300, 30));
		slider2 = new UISlider(new CGRect(10,260,300, 30));

			// Wire up the buttons to the SampleCode class methods
			btnBlack.TouchUpInside += (sender, e) => {
				NavigationController.PushViewController (new BlackViewController(), true);				
			};
			btnBlue.TouchUpInside += (sender, e) => {
				NavigationController.PushViewController (new BlueViewController(), true);				
			};
		}


		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();
			Title = "Appearance";
		
			// Create the buttons and TextView to run the sample code
			//btnBlack = UIButton.FromType(UIButtonType.RoundedRect);
			btnBlack.Frame = new CGRect(10,50,145,50);
			btnBlack.SetTitle("Black theme", UIControlState.Normal);
			
			//btnBlue = UIButton.FromType(UIButtonType.RoundedRect);
			btnBlue.Frame = new CGRect(165,50,145,50);
			btnBlue.SetTitle("Blue theme", UIControlState.Normal);
			
			//slider = new UISlider(new RectangleF(10,110,300, 30));
			slider.Value = 0.75f;			
			//slider2 = new UISlider(new RectangleF(10,260,300, 30));
			slider2.Value = 0.5f;
			
			progress = new UIProgressView(new CGRect(10, 150, 300, 30));
			progress.Progress = 0.35f;
			progress2 = new UIProgressView(new CGRect(10, 300, 300, 30));
			progress2.Progress = 0.85f;
			

			// setting the values directly OVERRIDES the Appearance defaults
			slider2.ThumbTintColor = UIColor.FromRGB (0,127,70); // dark green
			slider2.MinimumTrackTintColor = UIColor.FromRGB (66,255,63);
			slider2.MaximumTrackTintColor = UIColor.FromRGB (197,255,132);

			progress2.ProgressTintColor = UIColor.FromRGB (66,255,63);
			progress2.TrackTintColor = UIColor.FromRGB (197,255,132);
			



			overrideLabel = new UILabel(new CGRect(10, 220, 310, 30));
			overrideLabel.Text = "Overridden values are always observed";
			overrideLabel.BackgroundColor = UIColor.Clear;



			
			// Add the controls to the view
			Add(btnBlack);
			Add(btnBlue);
					
			Add(slider);
			Add(slider2);
	
			Add (progress);
			Add (progress2);

			Add (overrideLabel);
		}
	}
}