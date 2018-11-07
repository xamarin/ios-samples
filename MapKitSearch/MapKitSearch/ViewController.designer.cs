// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace MapKitSearch
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GetLocationButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField LocationTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField QueryTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SearchButton { get; set; }

        [Action ("GetCurrentLocation:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GetCurrentLocation (UIKit.UIButton sender);

        [Action ("Search:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Search (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (GetLocationButton != null) {
                GetLocationButton.Dispose ();
                GetLocationButton = null;
            }

            if (LocationTextField != null) {
                LocationTextField.Dispose ();
                LocationTextField = null;
            }

            if (QueryTextField != null) {
                QueryTextField.Dispose ();
                QueryTextField = null;
            }

            if (SearchButton != null) {
                SearchButton.Dispose ();
                SearchButton = null;
            }
        }
    }
}