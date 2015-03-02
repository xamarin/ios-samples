/*
 * This controller displays sliders and their various configurations.
*/

using System;

using UIKit;
using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class SliderDetailController : WKInterfaceController
	{
		public SliderDetailController ()
		{
			coloredSlider.SetColor (UIColor.Red);
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine (string.Format ("{0} will activate", this));
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine (string.Format ("{0} did deactivate", this));
		}

		partial void SliderAction (Single value)
		{
			Console.WriteLine ("Slider value is now: {0}", value);
		}
	}
}

