// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace RosyWriter
{
    [Register ("RosyWriterViewControllerUniversal")]
    partial class RosyWriterViewControllerUniversal
    {
        [Outlet]
        UIKit.UIView previewView { get; set; }


        [Outlet]
        UIKit.UIBarButtonItem btnRecord { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnRecord != null) {
                btnRecord.Dispose ();
                btnRecord = null;
            }

            if (previewView != null) {
                previewView.Dispose ();
                previewView = null;
            }
        }
    }
}