// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace UICatalog
{
    [Register ("WebViewController")]
    partial class WebViewController
    {
        [Outlet]
        UIKit.UITextField addressTextField { get; set; }

        [Outlet]
        WebKit.WKWebView webView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (addressTextField != null) {
                addressTextField.Dispose ();
                addressTextField = null;
            }

            if (webView != null) {
                webView.Dispose ();
                webView = null;
            }
        }
    }
}