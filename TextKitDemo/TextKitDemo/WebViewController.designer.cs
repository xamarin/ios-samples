// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace TextKitDemo
{
	[Register ("WebViewController")]
	partial class WebViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem backwardButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem doneButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem forwardButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolBar { get; set; }

		[Outlet]
		public MonoTouch.UIKit.UIWebView webView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (backwardButton != null) {
				backwardButton.Dispose ();
				backwardButton = null;
			}

			if (forwardButton != null) {
				forwardButton.Dispose ();
				forwardButton = null;
			}

			if (toolBar != null) {
				toolBar.Dispose ();
				toolBar = null;
			}

			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}

			if (doneButton != null) {
				doneButton.Dispose ();
				doneButton = null;
			}
		}
	}
}
