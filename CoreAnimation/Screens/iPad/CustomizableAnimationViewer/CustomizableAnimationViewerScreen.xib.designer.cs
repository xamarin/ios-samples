// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Example_CoreAnimation.Screens.iPad.CustomizableAnimationViewer
{
	[Register ("CustomizableAnimationViewerScreen")]
	partial class CustomizableAnimationViewerScreen
	{
		[Outlet]
		UIKit.UIButton btnContents { get; set; }

		[Outlet]
		UIKit.UIButton btnStart { get; set; }

		[Outlet]
		UIKit.UIImageView imgToAnimate { get; set; }

		[Outlet]
		UIKit.UISegmentedControl sgmtCurves { get; set; }

		[Outlet]
		UIKit.UISlider sldrDelay { get; set; }

		[Outlet]
		UIKit.UISlider sldrDuration { get; set; }

		[Outlet]
		UIKit.UISwitch swtchAutoReverse { get; set; }

		[Outlet]
		UIKit.UITextField txtRepeateCount { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnStart != null) {
				btnStart.Dispose ();
				btnStart = null;
			}

			if (imgToAnimate != null) {
				imgToAnimate.Dispose ();
				imgToAnimate = null;
			}

			if (sgmtCurves != null) {
				sgmtCurves.Dispose ();
				sgmtCurves = null;
			}

			if (sldrDelay != null) {
				sldrDelay.Dispose ();
				sldrDelay = null;
			}

			if (sldrDuration != null) {
				sldrDuration.Dispose ();
				sldrDuration = null;
			}

			if (txtRepeateCount != null) {
				txtRepeateCount.Dispose ();
				txtRepeateCount = null;
			}

			if (swtchAutoReverse != null) {
				swtchAutoReverse.Dispose ();
				swtchAutoReverse = null;
			}

			if (btnContents != null) {
				btnContents.Dispose ();
				btnContents = null;
			}
		}
	}
}
