using System;
using MapKit;
using CoreGraphics;
using CoreLocation;

namespace Footprint
{
	public class CoordinateConverter
	{
		// We pick one of the anchors on the floorplan as an origin point that we will compute distance relative to.
		MKMapPoint fromAnchorMKPoint;
		CGPoint fromAnchorFloorplanPoint;

		public nfloat PixelsPerMeter { get; private set; }

		nfloat radiansRotated;

		// This will anchor image to geographic coordinates
		public CoordinateConverter (CLLocationCoordinate2D topLeft, CLLocationCoordinate2D bottomRight, CGSize imageSize)
		{
			var topLeftAnchor = new GeoAnchor {
				LatitudeLongitude = topLeft,
				Pixel = new CGPoint (0, 0)
			};

			var bottomRightAnchor = new GeoAnchor {
				LatitudeLongitude = bottomRight,
				Pixel = new CGPoint (imageSize.Width, imageSize.Height)
			};

			var anchorPair = new Tuple<GeoAnchor, GeoAnchor> (topLeftAnchor, bottomRightAnchor);
			Init (anchorPair);
		}

		public CoordinateConverter (Tuple<GeoAnchor, GeoAnchor> anchors)
		{
			Init (anchors);
		}

		void Init (Tuple<GeoAnchor, GeoAnchor> anchors)
		{
			// To compute the distance between two geographical co-ordinates, we first need to
			// convert to MapKit co-ordinates
			fromAnchorFloorplanPoint = anchors.Item1.Pixel;
			fromAnchorMKPoint = MKMapPoint.FromCoordinate (anchors.Item1.LatitudeLongitude);
			MKMapPoint toAnchorMKPoint = MKMapPoint.FromCoordinate (anchors.Item2.LatitudeLongitude);

			// So that we can use MapKit's helper function to compute distance.
			// this helper function takes into account the curvature of the earth.
			var distanceBetweenPointsMeters = (nfloat)MKGeometry.MetersBetweenMapPoints (fromAnchorMKPoint, toAnchorMKPoint);

			var dx = anchors.Item1.Pixel.X - anchors.Item2.Pixel.X;
			var dy = anchors.Item1.Pixel.Y - anchors.Item2.Pixel.Y;

			// Distance between two points in pixels (on the floorplan image)
			var distanceBetweenPointsPixels = Hypot (dx, dy);

			// This gives us pixels/meter
			PixelsPerMeter = distanceBetweenPointsPixels / distanceBetweenPointsMeters;

			// Get the 2nd anchor's eastward/southward distance in meters from the first anchor point.
			var hyp = FetchRect (fromAnchorMKPoint, toAnchorMKPoint);

			// Angle of diagonal to east (in geographic)
			nfloat angleFromEastAndHypo = NMath.Atan2 (hyp.South, hyp.East);

			// Angle of diagonal to horizontal (in floorplan)
			nfloat angleFromXAndHypo = NMath.Atan2 (dy, dx);

			// Rotation amount from the geographic anchor line segment
			// to the floorplan anchor line segment
			// This is angle between X axis and East direction. This angle shows how you floor plan exists in real world
			radiansRotated = angleFromXAndHypo - angleFromEastAndHypo;
		}

		// Convert the specified geographic coordinate to floorplan point
		public CGPoint Convert (CLLocationCoordinate2D coordinate)
		{
			// Get the distance east & south with respect to the first anchor point in meters
			EastSouthDistance rect = FetchRect (fromAnchorMKPoint, MKMapPoint.FromCoordinate (coordinate));

			// Convert the east-south anchor point distance to pixels (still in east-south)
			var scaleTransform = CGAffineTransform.MakeScale (PixelsPerMeter, PixelsPerMeter);
			CGPoint pixelsXYInEastSouth = scaleTransform.TransformPoint (new CGPoint (rect.East, rect.South));

			// Rotate the east-south distance to be relative to floorplan horizontal
			// This gives us an xy distance in pixels from the anchor point.
			var rotateTransform = CGAffineTransform.MakeRotation (radiansRotated);
			CGPoint xy = rotateTransform.TransformPoint (pixelsXYInEastSouth);

			// From Anchor point may not be top letf corner.
			// however, we need the pixels from the (0, 0) of the floorplan
			// so we adjust by the position of the anchor point in the floorplan
			xy.X += fromAnchorFloorplanPoint.X;
			xy.Y += fromAnchorFloorplanPoint.Y;

			return xy;
		}

		// Two points in rectangular co-ordinate system create a rectange
		static EastSouthDistance FetchRect (MKMapPoint fromAnchorMKPoint, MKMapPoint toPoint)
		{
			double latitude = MKMapPoint.ToCoordinate (fromAnchorMKPoint).Latitude;
			nfloat metersPerMapPoint = (nfloat)MKGeometry.MetersPerMapPointAtLatitude (latitude);

			var eastSouthDistance = new EastSouthDistance {
				East = (nfloat)(toPoint.X - fromAnchorMKPoint.X) * metersPerMapPoint,
				South = (nfloat)(toPoint.Y - fromAnchorMKPoint.Y) * metersPerMapPoint
			};
			return eastSouthDistance;
		}

		static nfloat Hypot (nfloat x, nfloat y)
		{
			return NMath.Sqrt (x * x + y * y);
		}
	}
}

