
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;
using MonoTouch.CoreLocation;

namespace Example_ContentControls.Screens.iPhone.Maps
{
	public partial class MapWithOverlayScreen : UIViewController
	{
		MKCircle _circleOverlay = null;
		MKCircleView _circleView = null;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public MapWithOverlayScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public MapWithOverlayScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MapWithOverlayScreen () : base("MapWithOverlayScreen", null)
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
			
			this.Title = "Pyramids of Giza";
			
			// create our location and zoom for the pyramids.
			CLLocationCoordinate2D coords = new CLLocationCoordinate2D(29.976111, 31.132778);
			MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(.75), MilesToLongitudeDegrees(.75, coords.Latitude));

			// set the coords and zoom on the map
			this.mapMain.Region = new MKCoordinateRegion(coords, span);
			
			// show the sat view.
			this.mapMain.MapType = MKMapType.Satellite;
			
			// add an overlay with the coords
			this._circleOverlay = MKCircle.Circle(coords, 200);
			this.mapMain.AddOverlay(this._circleOverlay);
						
			// set our delegate.
			//this.mapMain.Delegate = new MapDelegate();
			
			//-- OR --
			//- override the GetVIewForOverlay directly, in which case we don't need a delegate
			this.mapMain.GetViewForOverlay += (m, o) => {
				if(this._circleView == null)
				{
					this._circleView = new MKCircleView(this._circleOverlay);
					this._circleView.FillColor = UIColor.Blue;
					this._circleView.Alpha = 0.5f;
				}
				return this._circleView;
			};
		}
		
		/// <summary>
		/// The map delegate is much like the table delegate.
		/// </summary>
		protected class MapDelegate : MKMapViewDelegate
		{
			protected MKCircle _circle = null;
			protected MKCircleView _circleView = null;			
			
			public MapDelegate(MKCircle circle, MKCircleView circleView)
			{
				this._circle = circle;
				this._circleView = circleView;
			}

			public override MKOverlayView GetViewForOverlay(MKMapView mapView, NSObject overlay)
			{
				if ((_circle != null) && (_circleView == null))
				{
					_circleView = new MKCircleView(_circle);
					_circleView.FillColor = UIColor.Blue;
					_circleView.Alpha = 0.5f;
				}
				return _circleView;
			} 
			
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

