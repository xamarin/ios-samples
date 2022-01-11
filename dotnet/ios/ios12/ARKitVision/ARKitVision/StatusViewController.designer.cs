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

namespace ARKitVision;

[Register ("StatusViewController")]
partial class StatusViewController
{
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel messageLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIVisualEffectView messagePanel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton restartExperienceButton { get; set; }

        [Action ("restartExperience:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void restartExperience (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
                if (messageLabel != null)
                {
                        messageLabel.Dispose ();
                        messageLabel = null;
                }

                if (messagePanel != null)
                {
                        messagePanel.Dispose ();
                        messagePanel = null;
                }

                if (restartExperienceButton != null)
                {
                        restartExperienceButton.Dispose ();
                        restartExperienceButton = null;
                }
        }
}
