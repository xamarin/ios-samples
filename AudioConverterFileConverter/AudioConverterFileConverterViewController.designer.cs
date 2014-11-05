// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace AudioConverterFileConverter
{
	[Register ("AudioConverterFileConverterViewController")]
	partial class AudioConverterFileConverterViewController
	{
		[Outlet]
		UIKit.UISegmentedControl outputFormatSelector { get; set; }

		[Outlet]
		UIKit.UISegmentedControl outputSampleRateSelector { get; set; }

		[Outlet]
		UIKit.UIButton startButton { get; set; }

		[Outlet]
		UIKit.UILabel fileInfo { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Action ("segmentedControllerValueChanged:")]
		partial void segmentedControllerValueChanged (UIKit.UISegmentedControl sender);

		[Action ("convertButtonPressed:")]
		partial void convertButtonPressed (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (outputFormatSelector != null) {
				outputFormatSelector.Dispose ();
				outputFormatSelector = null;
			}

			if (outputSampleRateSelector != null) {
				outputSampleRateSelector.Dispose ();
				outputSampleRateSelector = null;
			}

			if (startButton != null) {
				startButton.Dispose ();
				startButton = null;
			}

			if (fileInfo != null) {
				fileInfo.Dispose ();
				fileInfo = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}
		}
	}
}
