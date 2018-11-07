// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace HttpClient
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton IssueRequestButton { get; set; }
        [Action ("RunHttpRequest:")]
        partial void RunHttpRequest (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (IssueRequestButton != null) {
                IssueRequestButton.Dispose ();
                IssueRequestButton = null;
            }
        }
    }
}