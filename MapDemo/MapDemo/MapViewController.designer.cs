// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MapDemo
{
	[Register ("MapViewController")]
	partial class MapViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
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
