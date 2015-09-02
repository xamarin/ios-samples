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

namespace MonkeyBrowse
{
	[Register ("ThirdViewController")]
	partial class ThirdViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel BusyText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem GoButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView Handoff { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField URL { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIWebView WebView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BusyText != null) {
				BusyText.Dispose ();
				BusyText = null;
			}
			if (GoButton != null) {
				GoButton.Dispose ();
				GoButton = null;
			}
			if (Handoff != null) {
				Handoff.Dispose ();
				Handoff = null;
			}
			if (URL != null) {
				URL.Dispose ();
				URL = null;
			}
			if (WebView != null) {
				WebView.Dispose ();
				WebView = null;
			}
		}
	}
}
