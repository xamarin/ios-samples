// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Example_CoreAnimation.Screens.iPad.LayerAnimation
{
	[Register ("LayerAnimationScreen")]
	partial class LayerAnimationScreen
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnAnimate { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnContents { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imgToAnimate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAnimate != null) {
				btnAnimate.Dispose ();
				btnAnimate = null;
			}

			if (btnContents != null) {
				btnContents.Dispose ();
				btnContents = null;
			}

			if (imgToAnimate != null) {
				imgToAnimate.Dispose ();
				imgToAnimate = null;
			}
		}
	}
}
