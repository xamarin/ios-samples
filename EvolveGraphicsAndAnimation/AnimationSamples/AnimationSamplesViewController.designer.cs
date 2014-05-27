// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace AnimationSamples
{
	[Register ("AnimationSamplesViewController")]
	partial class AnimationSamplesViewController
	{
		[Outlet]
		UIKit.UIButton TransitionButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TransitionButton != null) {
				TransitionButton.Dispose ();
				TransitionButton = null;
			}
		}
	}
}
