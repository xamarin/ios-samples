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

namespace WatchNotifications_iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton localButton { get; set; }

		[Action ("UIButton7_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton7_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (localButton != null) {
				localButton.Dispose ();
				localButton = null;
			}
		}
	}
}
