// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace WorldCities
{
	[Register ("WorldCitiesViewController")]
	partial class WorldCitiesViewController
	{
		[Outlet]
		MapKit.MKMapView MapView { get; set; }

		[Action ("SelectorChanged:")]
		partial void SelectorChanged (Foundation.NSObject sender);
	}
}
