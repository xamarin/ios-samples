// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace FileSystem
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView textView { get; set; }

        [Action ("CreateDirectory:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CreateDirectory (UIKit.UIButton sender);

        [Action ("ListAll:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ListAll (UIKit.UIButton sender);

        [Action ("ListDirectories:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ListDirectories (UIKit.UIButton sender);

        [Action ("OpenReadMe:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OpenReadMe (UIKit.UIButton sender);

        [Action ("OpenTestFile:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OpenTestFile (UIKit.UIButton sender);

        [Action ("WriteFile:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void WriteFile (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (textView != null) {
                textView.Dispose ();
                textView = null;
            }
        }
    }
}