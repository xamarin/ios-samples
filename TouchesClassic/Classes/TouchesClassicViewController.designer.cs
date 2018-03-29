// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TouchesClassic
{
    [Register ("TouchesClassicViewController")]
    partial class TouchesClassicViewController
    {
        [Outlet]
        UIKit.UILabel touchPhaseLabel { get; set; }


        [Outlet]
        UIKit.UILabel touchTrackingLabel { get; set; }


        [Outlet]
        UIKit.UILabel touchInfoLabel { get; set; }


        [Outlet]
        UIKit.UILabel touchInstructionLabel { get; set; }


        [Outlet]
        UIKit.UIImageView firstImage { get; set; }


        [Outlet]
        UIKit.UIImageView secondImage { get; set; }


        [Outlet]
        UIKit.UIImageView thirdImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (firstImage != null) {
                firstImage.Dispose ();
                firstImage = null;
            }

            if (secondImage != null) {
                secondImage.Dispose ();
                secondImage = null;
            }

            if (thirdImage != null) {
                thirdImage.Dispose ();
                thirdImage = null;
            }

            if (touchInfoLabel != null) {
                touchInfoLabel.Dispose ();
                touchInfoLabel = null;
            }

            if (touchInstructionLabel != null) {
                touchInstructionLabel.Dispose ();
                touchInstructionLabel = null;
            }

            if (touchPhaseLabel != null) {
                touchPhaseLabel.Dispose ();
                touchPhaseLabel = null;
            }

            if (touchTrackingLabel != null) {
                touchTrackingLabel.Dispose ();
                touchTrackingLabel = null;
            }
        }
    }
}