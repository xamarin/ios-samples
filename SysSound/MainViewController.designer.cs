// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace SysSound_iOS
{
	[Register ("SysSound_iOSViewController")]
	partial class SysSound_iOSViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton playAlertButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton playSystemButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton VibrateButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (playAlertButton != null) {
				playAlertButton.Dispose ();
				playAlertButton = null;
			}
			if (playSystemButton != null) {
				playSystemButton.Dispose ();
				playSystemButton = null;
			}
			if (VibrateButton != null) {
				VibrateButton.Dispose ();
				VibrateButton = null;
			}
		}
	}
}
