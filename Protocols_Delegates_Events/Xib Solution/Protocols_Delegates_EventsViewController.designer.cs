// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Protocols_Delegates_Events
{
    [Register ("Protocols_Delegates_EventsViewController")]
    partial class Protocols_Delegates_EventsViewController
    {
        [Outlet]
        MapKit.MKMapView map { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (map != null) {
                map.Dispose ();
                map = null;
            }
        }
    }
}