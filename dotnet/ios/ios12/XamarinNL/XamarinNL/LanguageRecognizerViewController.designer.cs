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
    [Register ("LanguageRecognizerViewController")]
    partial class LanguageRecognizerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton DetermineLanguageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DominantLanguageLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UserInput { get; set; }

        [Action ("HandleDetermineLanguageButtonTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleDetermineLanguageButtonTap (UIKit.UIButton sender);

        [Action ("HandleLanguageProbabilitiesButtonTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HandleLanguageProbabilitiesButtonTap (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (DetermineLanguageButton != null) {
                DetermineLanguageButton.Dispose ();
                DetermineLanguageButton = null;
            }

            if (DominantLanguageLabel != null) {
                DominantLanguageLabel.Dispose ();
                DominantLanguageLabel = null;
            }

            if (UserInput != null) {
                UserInput.Dispose ();
                UserInput = null;
            }
        }
    }
}