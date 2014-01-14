// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Example_CoreAnimation.Screens.iPad.ViewTransitions
{
	[Register ("TransitionViewController")]
	partial class TransitionViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnContents { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnTransition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISegmentedControl sgmntTransitionType { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnContents != null) {
				btnContents.Dispose ();
				btnContents = null;
			}

			if (btnTransition != null) {
				btnTransition.Dispose ();
				btnTransition = null;
			}

			if (sgmntTransitionType != null) {
				sgmntTransitionType.Dispose ();
				sgmntTransitionType = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}
		}
	}
}
