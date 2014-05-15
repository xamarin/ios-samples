// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MapCallouts
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		MapKit.MKMapView mapView { get; set; }

		[Action ("cityAction:")]
		partial void cityAction (Foundation.NSObject sender);

		[Action ("bridgeAction:")]
		partial void bridgeAction (Foundation.NSObject sender);

		[Action ("allAction:")]
		partial void allAction (Foundation.NSObject sender);
	}
}
