
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;

namespace Example_ContentControls.Screens.iPhone.Maps
{
	public partial class BasicMapScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public BasicMapScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public BasicMapScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public BasicMapScreen () : base("BasicMapScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Map of Paris!";
			
			// create our location and zoom for paris
			CLLocationCoordinate2D coords = new CLLocationCoordinate2D(48.857, 2.351);
			MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(20), MilesToLongitudeDegrees(20, coords.Latitude));

			// set the coords and zoom on the map
			mapMain.Region = new MKCoordinateRegion(coords, span);
			
			// to animate to a location, then call SetRegion:
			//mapMain.SetRegion(coords, true);
			
			// to show the device location:
			//mapMain.ShowsUserLocation = true;
			
			// change the map type based on what's selected in the segment control
			sgmtMapType.ValueChanged += (s, e) => {
				
				switch(sgmtMapType.SelectedSegment) {
					
					case 0:
						mapMain.MapType = MKMapType.Standard;
						break;
					case 1:
						mapMain.MapType = MKMapType.Satellite;
						break;
					case 2:
						mapMain.MapType = MKMapType.Hybrid;
						break;
				}
			};
		}
		
		/// <summary>
		/// Converts miles to latitude degrees
		/// </summary>
		public double MilesToLatitudeDegrees(double miles)
		{
			double earthRadius = 3960.0;
			double radiansToDegrees = 180.0/Math.PI;
			return (miles/earthRadius) * radiansToDegrees;
		}

		/// <summary>
		/// Converts miles to longitudinal degrees at a specified latitude
		/// </summary>
		public double MilesToLongitudeDegrees(double miles, double atLatitude)
		{
			double earthRadius = 3960.0;
			double degreesToRadians = Math.PI/180.0;
			double radiansToDegrees = 180.0/Math.PI;

			// derive the earth's radius at that point in latitude
			double radiusAtLatitude = earthRadius * Math.Cos(atLatitude * degreesToRadians);
    		return (miles / radiusAtLatitude) * radiansToDegrees;
		}
	}
}

