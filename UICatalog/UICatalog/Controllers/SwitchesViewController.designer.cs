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
    [Register ("SwitchesViewController")]
    partial class SwitchesViewController
    {
        [Outlet]
        UIKit.UISwitch defaultSwitch { get; set; }


        [Outlet]
        UIKit.UISwitch tintedSwitch { get; set; }


        [Action ("DefaultSwittcherValueChanged:")]
        partial void DefaultSwittcherValueChanged (Foundation.NSObject sender);


        [Action ("DefaultValueChanged:")]
        partial void DefaultValueChanged (Foundation.NSObject sender);


        [Action ("TintedValueChanged:")]
        partial void TintedValueChanged (Foundation.NSObject sender);

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