// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MotionGraphs
{
	[Register ("GraphViewController")]
	partial class GraphViewController
	{
		[Outlet]
		UIKit.UILabel primaryGraphLabel { get; set; }

		[Outlet]
		MotionGraphs.GraphView primaryGraph { get; set; }

		[Outlet]
		UIKit.UILabel UpdateIntervalLabel { get; set; }

		[Outlet]
		UIKit.UISegmentedControl SegmentedControl { get; set; }

		[Outlet]
		UIKit.UILabel xLabel { get; set; }

		[Outlet]
		UIKit.UILabel yLabel { get; set; }

		[Outlet]
		UIKit.UILabel zLabel { get; set; }

		[Outlet]
		UIKit.UISlider UpdateIntervalSlider { get; set; }

		[Action ("OnSliderValueChanged:")]
		partial void OnSliderValueChanged (UIKit.UISlider sender);

		[Action ("SegmentedControlDidChanged:")]
		partial void SegmentedControlDidChanged (UIKit.UISegmentedControl sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (primaryGraphLabel != null) {
				primaryGraphLabel.Dispose ();
				primaryGraphLabel = null;
			}

			if (primaryGraph != null) {
				primaryGraph.Dispose ();
				primaryGraph = null;
			}

			if (UpdateIntervalLabel != null) {
				UpdateIntervalLabel.Dispose ();
				UpdateIntervalLabel = null;
			}

			if (SegmentedControl != null) {
				SegmentedControl.Dispose ();
				SegmentedControl = null;
			}

			if (xLabel != null) {
				xLabel.Dispose ();
				xLabel = null;
			}

			if (yLabel != null) {
				yLabel.Dispose ();
				yLabel = null;
			}

			if (zLabel != null) {
				zLabel.Dispose ();
				zLabel = null;
			}

			if (UpdateIntervalSlider != null) {
				UpdateIntervalSlider.Dispose ();
				UpdateIntervalSlider = null;
			}
		}
	}
}
