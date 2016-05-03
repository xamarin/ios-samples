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

namespace AUv3Host
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UITableView AudioUnitTableView { get; set; }


        [Outlet]
        UIKit.UILabel NoViewLabel { get; set; }


        [Outlet]
        UIKit.UIButton PlayButton { get; set; }


        [Outlet]
        UIKit.UITableView PresetTableView { get; set; }


        [Outlet]
        UIKit.UIView ViewContainer { get; set; }


        [Action ("TogglePlay:")]
        partial void TogglePlay (UIKit.UIButton sender);


        [Action ("ToggleView:")]
        partial void ToggleView (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AudioUnitTableView != null) {
                AudioUnitTableView.Dispose ();
                AudioUnitTableView = null;
            }

            if (NoViewLabel != null) {
                NoViewLabel.Dispose ();
                NoViewLabel = null;
            }

            if (PlayButton != null) {
                PlayButton.Dispose ();
                PlayButton = null;
            }

            if (PresetTableView != null) {
                PresetTableView.Dispose ();
                PresetTableView = null;
            }

            if (ViewContainer != null) {
                ViewContainer.Dispose ();
                ViewContainer = null;
            }
        }
    }
}