// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MapCallouts
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		MonoTouch.MapKit.MKMapView mapView { get; set; }

		[Action ("cityAction:")]
		partial void cityAction (MonoTouch.Foundation.NSObject sender);

		[Action ("bridgeAction:")]
		partial void bridgeAction (MonoTouch.Foundation.NSObject sender);

		[Action ("allAction:")]
		partial void allAction (MonoTouch.Foundation.NSObject sender);
	}
}
