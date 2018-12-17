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

namespace Share
{
    [Register ("ShareViewController")]
    partial class ShareViewController
    {
        [Outlet ("instructionsLabel")]
        public UILabel InstructionsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel InstructionsLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (InstructionsLabel != null) {
                InstructionsLabel.Dispose ();
                InstructionsLabel = null;
            }
        }
    }
}