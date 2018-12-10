// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace avTouch
{
    [Register ("avTouchController")]
    partial class avTouchController
    {
        [Outlet]
        UIKit.UIButton _playButton { get; set; }


        [Outlet]
        UIKit.UIButton _rewButton { get; set; }


        [Outlet]
        UIKit.UIButton _ffwButton { get; set; }


        [Outlet]
        UIKit.UILabel _currentTime { get; set; }


        [Outlet]
        UIKit.UILabel _duration { get; set; }


        [Outlet]
        UIKit.UISlider _volumeSlider { get; set; }


        [Outlet]
        UIKit.UISlider _progressBar { get; set; }


        [Outlet]
        UIKit.UILabel _fileName { get; set; }


        [Outlet]
        avTouch.CALevelMeter _lvlMeter_in { get; set; }


        [Action ("volumeSliderMoved:")]
        partial void volumeSliderMoved (UIKit.UISlider sender);


        [Action ("progressSliderMoved:")]
        partial void progressSliderMoved (UIKit.UISlider sender);


        [Action ("playButtonPressed:")]
        partial void playButtonPressed (UIKit.UIButton sender);


        [Action ("ffwButtonReleased:")]
        partial void ffwButtonReleased (UIKit.UIButton sender);


        [Action ("rewButtonReleased:")]
        partial void rewButtonReleased (UIKit.UIButton sender);


        [Action ("rewButtonPressed:")]
        partial void rewButtonPressed (UIKit.UIButton sender);


        [Action ("ffwButtonPressed:")]
        partial void ffwButtonPressed (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (_currentTime != null) {
                _currentTime.Dispose ();
                _currentTime = null;
            }

            if (_duration != null) {
                _duration.Dispose ();
                _duration = null;
            }

            if (_ffwButton != null) {
                _ffwButton.Dispose ();
                _ffwButton = null;
            }

            if (_fileName != null) {
                _fileName.Dispose ();
                _fileName = null;
            }

            if (_playButton != null) {
                _playButton.Dispose ();
                _playButton = null;
            }

            if (_progressBar != null) {
                _progressBar.Dispose ();
                _progressBar = null;
            }

            if (_rewButton != null) {
                _rewButton.Dispose ();
                _rewButton = null;
            }

            if (_volumeSlider != null) {
                _volumeSlider.Dispose ();
                _volumeSlider = null;
            }
        }
    }
}