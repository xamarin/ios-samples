// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MapDemo
{
	[Register ("MapDemoViewController")]
	partial class MapDemoViewController
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
