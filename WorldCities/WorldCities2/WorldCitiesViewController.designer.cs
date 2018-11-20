// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace WorldCities
{
    [Register ("WorldCitiesViewController")]
    partial class WorldCitiesViewController
    {
        [Outlet]
        MapKit.MKMapView MapView { get; set; }


        [Action ("SelectorChanged:")]
        partial void SelectorChanged (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (MapView != null) {
                MapView.Dispose ();
                MapView = null;
            }
        }
    }
}