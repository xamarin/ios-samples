// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Example_CoreLocation.MainScreen
{
    [Register ("MainViewController_iPad")]
    partial class MainViewController_iPad
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAltitude { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCourse { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDistanceToParis { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLatitude { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLongitude { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblMagneticHeading { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblSpeed { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTrueHeading { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblAltitude != null) {
                lblAltitude.Dispose ();
                lblAltitude = null;
            }

            if (lblCourse != null) {
                lblCourse.Dispose ();
                lblCourse = null;
            }

            if (lblDistanceToParis != null) {
                lblDistanceToParis.Dispose ();
                lblDistanceToParis = null;
            }

            if (lblLatitude != null) {
                lblLatitude.Dispose ();
                lblLatitude = null;
            }

            if (lblLongitude != null) {
                lblLongitude.Dispose ();
                lblLongitude = null;
            }

            if (lblMagneticHeading != null) {
                lblMagneticHeading.Dispose ();
                lblMagneticHeading = null;
            }

            if (lblSpeed != null) {
                lblSpeed.Dispose ();
                lblSpeed = null;
            }

            if (lblTrueHeading != null) {
                lblTrueHeading.Dispose ();
                lblTrueHeading = null;
            }
        }
    }
}