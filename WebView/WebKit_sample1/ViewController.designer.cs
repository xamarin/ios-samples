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

namespace WebView
{
    [Register("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UIButton openSafari { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UIButton safariButton { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UIKit.UIButton webviewButton { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (openSafari != null)
            {
                openSafari.Dispose();
                openSafari = null;
            }

            if (safariButton != null)
            {
                safariButton.Dispose();
                safariButton = null;
            }

            if (webviewButton != null)
            {
                webviewButton.Dispose();
                webviewButton = null;
            }
        }
    }
}