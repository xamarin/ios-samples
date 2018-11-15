// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CoreLocation
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel altitudeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel courseLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel distanceLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel latitudeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel longitudeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel magneticHeadingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel speedLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel trueHeadingLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (altitudeLabel != null) {
                altitudeLabel.Dispose ();
                altitudeLabel = null;
            }

            if (courseLabel != null) {
                courseLabel.Dispose ();
                courseLabel = null;
            }

            if (distanceLabel != null) {
                distanceLabel.Dispose ();
                distanceLabel = null;
            }

            if (latitudeLabel != null) {
                latitudeLabel.Dispose ();
                latitudeLabel = null;
            }

            if (longitudeLabel != null) {
                longitudeLabel.Dispose ();
                longitudeLabel = null;
            }

            if (magneticHeadingLabel != null) {
                magneticHeadingLabel.Dispose ();
                magneticHeadingLabel = null;
            }

            if (speedLabel != null) {
                speedLabel.Dispose ();
                speedLabel = null;
            }

            if (trueHeadingLabel != null) {
                trueHeadingLabel.Dispose ();
                trueHeadingLabel = null;
            }
        }
    }
}