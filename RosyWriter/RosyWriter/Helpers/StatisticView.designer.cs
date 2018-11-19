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

namespace RosyWriter
{
    [Register ("StatisticView")]
    partial class StatisticView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel colorLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel dimensionsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel fpsLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (colorLabel != null) {
                colorLabel.Dispose ();
                colorLabel = null;
            }

            if (dimensionsLabel != null) {
                dimensionsLabel.Dispose ();
                dimensionsLabel = null;
            }

            if (fpsLabel != null) {
                fpsLabel.Dispose ();
                fpsLabel = null;
            }
        }
    }
}