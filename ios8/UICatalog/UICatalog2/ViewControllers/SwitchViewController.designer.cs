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
    [Register ("SwitchViewController")]
    partial class SwitchViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch defaultSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch tintedSwitch { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (defaultSwitch != null) {
                defaultSwitch.Dispose ();
                defaultSwitch = null;
            }

            if (tintedSwitch != null) {
                tintedSwitch.Dispose ();
                tintedSwitch = null;
            }
        }
    }
}