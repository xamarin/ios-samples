// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace AnimationSamples
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ExplicitButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ImplicitButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton TransitionButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ViewAnimationButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ViewTransitionButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ExplicitButton != null) {
				ExplicitButton.Dispose ();
				ExplicitButton = null;
			}
			if (ImplicitButton != null) {
				ImplicitButton.Dispose ();
				ImplicitButton = null;
			}
			if (TransitionButton != null) {
				TransitionButton.Dispose ();
				TransitionButton = null;
			}
			if (ViewAnimationButton != null) {
				ViewAnimationButton.Dispose ();
				ViewAnimationButton = null;
			}
			if (ViewTransitionButton != null) {
				ViewTransitionButton.Dispose ();
				ViewTransitionButton = null;
			}
		}
	}
}
