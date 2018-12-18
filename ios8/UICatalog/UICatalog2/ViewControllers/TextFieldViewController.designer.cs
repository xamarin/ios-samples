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
    [Register ("TextFieldViewController")]
    partial class TextFieldViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField customTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField secureTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField specificKeyboardTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tintedTextField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (customTextField != null) {
                customTextField.Dispose ();
                customTextField = null;
            }

            if (secureTextField != null) {
                secureTextField.Dispose ();
                secureTextField = null;
            }

            if (specificKeyboardTextField != null) {
                specificKeyboardTextField.Dispose ();
                specificKeyboardTextField = null;
            }

            if (textField != null) {
                textField.Dispose ();
                textField = null;
            }

            if (tintedTextField != null) {
                tintedTextField.Dispose ();
                tintedTextField = null;
            }
        }
    }
}