using System;

using UIKit;
using Foundation;

namespace DigitDetection
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UILabel accuracyLabel { get; set; }

		[Outlet]
		DigitDetection.DrawView DigitView { get; set; }

		[Outlet]
		UIKit.UILabel predictionLabel { get; set; }

		[Action ("tappedClear:")]
		partial void tappedClear (UIButton sender);

		[Action ("tappedDeepButton:")]
		partial void tappedDeepButton (UIButton sender);

		[Action ("tappedDetectDigit:")]
		partial void tappedDetectDigit (UIButton sender);

		[Action ("tappedTestSet:")]
		partial void tappedTestSet (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (accuracyLabel != null) {
				accuracyLabel.Dispose ();
				accuracyLabel = null;
			}

			if (DigitView != null) {
				DigitView.Dispose ();
				DigitView = null;
			}

			if (predictionLabel != null) {
				predictionLabel.Dispose ();
				predictionLabel = null;
			}
		}
	}
}