using Foundation;
using System;
using UIKit;

namespace UICatalog {
	public partial class SliderViewController : UITableViewController {
		public SliderViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureCustomSlider ();
		}

		private void ConfigureCustomSlider ()
		{
			customSlider.SetMinTrackImage (UIImage.FromBundle ("slider_blue_track"), UIControlState.Normal);
			customSlider.SetMaxTrackImage (UIImage.FromBundle ("slider_green_track"), UIControlState.Normal);
			customSlider.SetThumbImage (UIImage.FromBundle ("slider_thumb"), UIControlState.Normal);
		}

		partial void CustomSliderValueChanged (NSObject sender)
		{
			Console.WriteLine ($"A 'Custom' slider changed its value: {customSlider.Value}");
		}

		partial void DefaultSliderValueChanged (NSObject sender)
		{
			Console.WriteLine ($"A 'Default' slider changed its value: {defaultSlider.Value}");
		}

		partial void TintedSliderValueChanged (NSObject sender)
		{
			Console.WriteLine ($"A 'Tinted' slider changed its value: {tintedSlider.Value}");
		}
	}
}
