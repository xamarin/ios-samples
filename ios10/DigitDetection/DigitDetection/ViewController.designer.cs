using System;

using UIKit;
using Foundation;

namespace DigitDetection
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet ("accuracyLabel")]
		UILabel AccuracyLabel { get; set; }

		[Outlet ("digitView")]
		DrawView DigitView { get; set; }

		[Outlet ("predictionLabel")]
		UILabel PredictionLabel { get; set; }

		[Action ("tappedClear:")]
		partial void TappedClear (UIButton sender);

		[Action ("tappedDeepButton:")]
		partial void TappedDeepButton (UIButton sender);

		[Action ("tappedDetectDigit:")]
		partial void TappedDetectDigit (UIButton sender);

		[Action ("tappedTestSet:")]
		partial void TappedTestSet (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (AccuracyLabel != null) {
				AccuracyLabel.Dispose ();
				AccuracyLabel = null;
			}

			if (DigitView != null) {
				DigitView.Dispose ();
				DigitView = null;
			}

			if (PredictionLabel != null) {
				PredictionLabel.Dispose ();
				PredictionLabel = null;
			}
		}
	}
}