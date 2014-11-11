// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Example_CoreAnimation.Screens.iPad.BasicUIViewAnimation
{
	[Register ("BasicUIViewAnimationScreen")]
	partial class BasicUIViewAnimationScreen
	{
		[Outlet]
		UIKit.UIButton btnClickMe { get; set; }

		[Outlet]
		UIKit.UIButton btnContents { get; set; }

		[Outlet]
		UIKit.UIImageView imgToAnimate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnClickMe != null) {
				btnClickMe.Dispose ();
				btnClickMe = null;
			}

			if (imgToAnimate != null) {
				imgToAnimate.Dispose ();
				imgToAnimate = null;
			}

			if (btnContents != null) {
				btnContents.Dispose ();
				btnContents = null;
			}
		}
	}
}
