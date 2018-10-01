// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ExceptionalAccessibility
{
    [Register ("DogStatsView")]
    partial class DogStatsView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ageLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ageTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel breedLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel breedTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel nameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel nameTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel weightLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel weightTitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ageLabel != null) {
                ageLabel.Dispose ();
                ageLabel = null;
            }

            if (ageTitleLabel != null) {
                ageTitleLabel.Dispose ();
                ageTitleLabel = null;
            }

            if (breedLabel != null) {
                breedLabel.Dispose ();
                breedLabel = null;
            }

            if (breedTitleLabel != null) {
                breedTitleLabel.Dispose ();
                breedTitleLabel = null;
            }

            if (nameLabel != null) {
                nameLabel.Dispose ();
                nameLabel = null;
            }

            if (nameTitleLabel != null) {
                nameTitleLabel.Dispose ();
                nameTitleLabel = null;
            }

            if (weightLabel != null) {
                weightLabel.Dispose ();
                weightLabel = null;
            }

            if (weightTitleLabel != null) {
                weightTitleLabel.Dispose ();
                weightTitleLabel = null;
            }
        }
    }
}