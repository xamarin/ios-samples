using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("SliderViewController")]
	public class SliderViewController : UITableViewController
	{
		[Outlet]
		UISlider DefaultSlider { get; set; }

		[Outlet]
		UISlider TintedSlider { get; set; }

		[Outlet]
		UISlider CustomSlider { get; set; }

		public SliderViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureDefaultSlider ();
			ConfigureTintedSlider ();
			ConfigureCustomSlider ();
		}

		void ConfigureDefaultSlider ()
		{
			DefaultSlider.MinValue = 0;
			DefaultSlider.MaxValue = 100;
			DefaultSlider.Value = 42;
			DefaultSlider.Continuous = true;

			DefaultSlider.ValueChanged += OnSliderValueChanged;
		}

		void ConfigureTintedSlider ()
		{
			TintedSlider.MinimumTrackTintColor = ApplicationColors.Blue;
			TintedSlider.MaximumTrackTintColor = ApplicationColors.Purple;

			TintedSlider.ValueChanged += OnSliderValueChanged;
		}

		void ConfigureCustomSlider ()
		{
			var leftTrackImage = UIImage.FromBundle ("slider_blue_track");
			CustomSlider.SetMinTrackImage (leftTrackImage, UIControlState.Normal);

			var rightTrackImage = UIImage.FromBundle ("slider_green_track");
			CustomSlider.SetMaxTrackImage (rightTrackImage, UIControlState.Normal);

			var thumbImage = UIImage.FromBundle ("slider_thumb");
			CustomSlider.SetThumbImage (thumbImage, UIControlState.Normal);

			CustomSlider.MinValue = 0;
			CustomSlider.MaxValue = 100;
			CustomSlider.Continuous = false;
			CustomSlider.Value = 84;

			CustomSlider.ValueChanged += OnSliderValueChanged;
		}

		static void OnSliderValueChanged (object sender, EventArgs e)
		{
			Console.WriteLine ("A slider changed its value: {0}.", sender);
		}
	}
}
