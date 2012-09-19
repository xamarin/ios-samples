using System;

using MonoTouch.MapKit;
using MonoTouch.UIKit;

namespace RegionDefiner
{
	public class MapDelegate : MKMapViewDelegate
	{
		public override MKOverlayView GetViewForOverlay (MKMapView mapView, MonoTouch.Foundation.NSObject overlay)
		{
			if (RegionDefinerViewController.PolygonView != null && RegionDefinerViewController.PolygonView.Polygon == RegionDefinerViewController.Polygon) 
				return RegionDefinerViewController.PolygonView;

			RegionDefinerViewController.Polygon = overlay as MKPolygon;
			RegionDefinerViewController.PolygonView = new MKPolygonView (RegionDefinerViewController.Polygon) {
				FillColor = new UIColor (0, 1, 0, .3f),
				StrokeColor = new UIColor (0, 1, 0, 0.9f),
				LineWidth = 1.0f
			};

			return RegionDefinerViewController.PolygonView;
		}
	}
}




