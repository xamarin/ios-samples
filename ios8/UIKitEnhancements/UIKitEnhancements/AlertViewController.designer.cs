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

namespace UIKitEnhancements
{
	[Register ("AlertViewController")]
	partial class AlertViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ActionSheet { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton OKAlert { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton OKCancelAlert { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton TextInputAlert { get; set; }

		[Action ("ActionSheet_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void ActionSheet_TouchUpInside (UIButton sender);

		[Action ("OKAlert_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OKAlert_TouchUpInside (UIButton sender);

		[Action ("OKCancelAlert_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OKCancelAlert_TouchUpInside (UIButton sender);

		[Action ("TextInputAlert_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void TextInputAlert_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (ActionSheet != null) {
				ActionSheet.Dispose ();
				ActionSheet = null;
			}
			if (OKAlert != null) {
				OKAlert.Dispose ();
				OKAlert = null;
			}
			if (OKCancelAlert != null) {
				OKCancelAlert.Dispose ();
				OKCancelAlert = null;
			}
			if (TextInputAlert != null) {
				TextInputAlert.Dispose ();
				TextInputAlert = null;
			}
		}
	}
}
