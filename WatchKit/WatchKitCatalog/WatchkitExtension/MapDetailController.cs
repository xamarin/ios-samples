/* 
 * This controller displays a map and demonstrates use of setting its coordinate region, zoom level, and addition and removal of annotations.
*/

using System;

using UIKit;
using MapKit;
using WatchKit;
using Foundation;
using CoreGraphics;
using CoreLocation;

namespace WatchkitExtension
{
	public partial class MapDetailController : WKInterfaceController
	{
		MKCoordinateRegion currentRegion;
		MKCoordinateSpan currentSpan;

		public MapDetailController ()
		{
			currentSpan = new MKCoordinateSpan (1.0, 1.0);
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine ("{0} will activate", this);

			GoToApple (null);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine ("{0} did deactivate", this);
		}

		partial void GoToTokyo (NSObject obj)
		{
			var coordinate = new CLLocationCoordinate2D (35.4, 139.4);

			SetMapToCoordinate (coordinate);
		}

		partial void GoToApple (NSObject obj)
		{
			var coordinate = new CLLocationCoordinate2D (37.331793, -122.029584);

			SetMapToCoordinate (coordinate);
		}

		void SetMapToCoordinate (CLLocationCoordinate2D coordinate)
		{
			var region = new MKCoordinateRegion (coordinate, currentSpan);
			currentRegion = region;

			var newCenterPoint = MKMapPoint.FromCoordinate (coordinate);

			map.SetVisible (new MKMapRect (newCenterPoint.X, newCenterPoint.Y, currentSpan.LatitudeDelta, currentSpan.LongitudeDelta));
			map.SetRegion (region);
		}

		partial void ZoomOut (NSObject obj)
		{
			var span = new MKCoordinateSpan (currentSpan.LatitudeDelta * 2, currentSpan.LongitudeDelta * 2);
			var region = new MKCoordinateRegion (currentRegion.Center, span);

			currentSpan = span;
			map.SetRegion (region);
		}

		partial void ZoomIn (NSObject obj)
		{
			var span = new MKCoordinateSpan (currentSpan.LatitudeDelta * 0.5f, currentSpan.LongitudeDelta * 0.5f);
			var region = new MKCoordinateRegion (currentRegion.Center, span);

			currentSpan = span;
			map.SetRegion (region);
		}

		partial void AddPinAnnotations (NSObject obj)
		{
			map.AddAnnotation (currentRegion.Center, WKInterfaceMapPinColor.Red);

			var greenCoordinate = new CLLocationCoordinate2D (currentRegion.Center.Latitude, currentRegion.Center.Longitude - 0.3f);
			map.AddAnnotation (greenCoordinate, WKInterfaceMapPinColor.Green);

			var purpleCoordinate = new CLLocationCoordinate2D (currentRegion.Center.Latitude, currentRegion.Center.Longitude + 0.3f);
			map.AddAnnotation (purpleCoordinate, WKInterfaceMapPinColor.Purple);
		}

		partial void AddImageAnnotations (NSObject obj)
		{
			var firstCoordinate = new CLLocationCoordinate2D (currentRegion.Center.Latitude, currentRegion.Center.Longitude - 0.3f);

			// Uses image in Watch app bundle.
			map.AddAnnotation (firstCoordinate, "Whale", CGPoint.Empty);

			var secondCoordinate = new CLLocationCoordinate2D (currentRegion.Center.Latitude, currentRegion.Center.Longitude + 0.3f);

			// Uses image in WatchKit Extension bundle.
			using (var image = UIImage.FromBundle ("Bumblebee"))
				map.AddAnnotation (secondCoordinate, image, CGPoint.Empty);
		}

		partial void RemoveAll (NSObject obj)
		{
			map.RemoveAllAnnotations ();
		}
	}
}

