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
    [Register ("ButtonViewController")]
    partial class ButtonViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton attributedTextButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton imageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton systemContactAddButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton systemDetailDisclosureButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton systemTextButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (attributedTextButton != null) {
                attributedTextButton.Dispose ();
                attributedTextButton = null;
            }

            if (imageButton != null) {
                imageButton.Dispose ();
                imageButton = null;
            }

            if (systemContactAddButton != null) {
                systemContactAddButton.Dispose ();
                systemContactAddButton = null;
            }

            if (systemDetailDisclosureButton != null) {
                systemDetailDisclosureButton.Dispose ();
                systemDetailDisclosureButton = null;
            }

            if (systemTextButton != null) {
                systemTextButton.Dispose ();
                systemTextButton = null;
            }
        }
    }
}