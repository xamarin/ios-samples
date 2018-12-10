// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace AVTouch
{
    [Register ("AvTouchViewController")]
    partial class AvTouchViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel currentTimeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel durationLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel fileNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        AVTouch.CALevelMeter lvlMeter { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider progressSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIToolbar toolbar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider volumeSlider { get; set; }


        [Action ("ForwardButtonPressed:")]
        partial void ForwardButtonPressed (UIKit.UIBarButtonItem sender);


        [Action ("ForwardButtonReleased:")]
        partial void ForwardButtonReleased (UIKit.UIBarButtonItem sender);


        [Action ("PlayButtonPressed:")]
        partial void PlayButtonPressed (UIKit.UIBarButtonItem sender);


        [Action ("ProgressSliderMoved:")]
        partial void ProgressSliderMoved (UIKit.UISlider sender);


        [Action ("RewindButtonPressed:")]
        partial void RewindButtonPressed (UIKit.UIBarButtonItem sender);


        [Action ("RewindButtonReleased:")]
        partial void RewindButtonReleased (UIKit.UIBarButtonItem sender);


        [Action ("VolumeSliderMoved:")]
        partial void VolumeSliderMoved (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
            if (currentTimeLabel != null) {
                currentTimeLabel.Dispose ();
                currentTimeLabel = null;
            }

            if (durationLabel != null) {
                durationLabel.Dispose ();
                durationLabel = null;
            }

            if (fileNameLabel != null) {
                fileNameLabel.Dispose ();
                fileNameLabel = null;
            }

            if (lvlMeter != null) {
                lvlMeter.Dispose ();
                lvlMeter = null;
            }

            if (progressSlider != null) {
                progressSlider.Dispose ();
                progressSlider = null;
            }

            if (toolbar != null) {
                toolbar.Dispose ();
                toolbar = null;
            }

            if (volumeSlider != null) {
                volumeSlider.Dispose ();
                volumeSlider = null;
            }
        }
    }
}