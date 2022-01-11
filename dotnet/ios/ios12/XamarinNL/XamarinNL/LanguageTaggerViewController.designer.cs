// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace XamarinNL
{
    [Register ("LanguageTaggerViewController")]
    partial class LanguageTaggerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UserInput { get; set; }

        [Action ("HandleNamedEntitiesButtonTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleNamedEntitiesButtonTap (UIKit.UIButton sender);

        [Action ("HandlePartsOfSpeechButtonTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandlePartsOfSpeechButtonTap (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (UserInput != null) {
                UserInput.Dispose ();
                UserInput = null;
            }
        }
    }
}