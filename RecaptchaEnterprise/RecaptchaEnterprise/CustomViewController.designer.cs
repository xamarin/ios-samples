// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace RecaptchaEnterprise
{
	[Register ("CustomViewController")]
	partial class CustomViewController
	{
		[Outlet]
		UIKit.UITextField _recaptchaEntry { get; set; }

		[Outlet]
		WebKit.WKWebView RecaptchaWebViewUI { get; set; }

		[Outlet]
		UIKit.UIButton ShowButtonUI { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (RecaptchaWebViewUI != null) {
				RecaptchaWebViewUI.Dispose ();
				RecaptchaWebViewUI = null;
			}

			if (ShowButtonUI != null) {
				ShowButtonUI.Dispose ();
				ShowButtonUI = null;
			}

			if (_recaptchaEntry != null) {
				_recaptchaEntry.Dispose ();
				_recaptchaEntry = null;
			}
		}
	}
}
