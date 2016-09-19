// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace WatchTables.OnWatchExtension
{
    [Register ("DetailController")]
    partial class DetailController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        WatchKit.WKInterfaceLabel SelectedLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SelectedLabel != null) {
                SelectedLabel.Dispose ();
                SelectedLabel = null;
            }
        }
    }
}