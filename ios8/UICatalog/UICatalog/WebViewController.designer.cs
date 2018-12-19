// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
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
