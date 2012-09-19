// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MotionGraphs
{
	[Register ("GraphViewController")]
	partial class GraphViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel primaryGraphLabel { get; set; }

		[Outlet]
		MotionGraphs.GraphView primaryGraph { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel UpdateIntervalLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISegmentedControl SegmentedControl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel xLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel yLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel zLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider UpdateIntervalSlider { get; set; }

		[Action ("OnSliderValueChanged:")]
		partial void OnSliderValueChanged (MonoTouch.UIKit.UISlider sender);

		[Action ("SegmentedControlDidChanged:")]
		partial void SegmentedControlDidChanged (MonoTouch.UIKit.UISegmentedControl sender);
		
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
