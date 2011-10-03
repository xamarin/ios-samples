using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreLocation;
using Example_CoreLocation.MainScreen;

namespace Example_CoreLocation
{
	public class MainViewController : UIViewController
	{
		#region -= declarations =-
		
		MainViewController_iPhone mainViewController_iPhone;
		MainViewController_iPad mainViewController_iPad;

		IMainScreen mainScreen = null;

		// location stuff
		CLLocationManager iPhoneLocationManager = null;
		// uncomment this if you want to use the delegate pattern
		//LocationDelegate locationDelegate = null;
		
		#endregion
		
		#region -= constructors =-
		//
		// Constructor invoked from the NIB loader
		//
		public MainViewController (IntPtr p) : base(p)
		{
		}
		
		public MainViewController () : base()
		{
		}
		#endregion
		
		public override void ViewDidLoad ()
		{
			// all your base
			base.ViewDidLoad ();
			
			// load the appropriate view, based on the device type
			this.LoadViewForDevice ();

			// initialize our location manager and callback handler
			iPhoneLocationManager = new CLLocationManager ();
			
			// uncomment this if you want to use the delegate pattern:
			//locationDelegate = new LocationDelegate (mainScreen);
			//iPhoneLocationManager.Delegate = locationDelegate;
			
			// you can set the update threshold and accuracy if you want:
			//iPhoneLocationManager.DistanceFilter = 10; // move ten meters before updating
			//iPhoneLocationManager.HeadingFilter = 3; // move 3 degrees before updating
			
			// you can also set the desired accuracy:
			iPhoneLocationManager.DesiredAccuracy = 1000; // 1000 meters/1 kilometer
			// you can also use presets, which simply evalute to a double value:
			//iPhoneLocationManager.DesiredAccuracy = CLLocation.AccuracyNearestTenMeters;
			
			// handle the updated location method and update the UI
			iPhoneLocationManager.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
				mainScreen.LblAltitude.Text = e.NewLocation.Altitude.ToString () + "meters";
				mainScreen.LblLongitude.Text = e.NewLocation.Coordinate.Longitude.ToString () + "º";
				mainScreen.LblLatitude.Text = e.NewLocation.Coordinate.Latitude.ToString () + "º";
				mainScreen.LblCourse.Text = e.NewLocation.Course.ToString () + "º";
				mainScreen.LblSpeed.Text = e.NewLocation.Speed.ToString () + "meters/s";
				
				// get the distance from here to paris
				mainScreen.LblDistanceToParis.Text = (e.NewLocation.DistanceFrom(new CLLocation(48.857, 2.351)) / 1000).ToString() + "km";
			};
			
			// handle the updated heading method and update the UI
			iPhoneLocationManager.UpdatedHeading += (object sender, CLHeadingUpdatedEventArgs e) => {
				mainScreen.LblMagneticHeading.Text = e.NewHeading.MagneticHeading.ToString () + "º";
				mainScreen.LblTrueHeading.Text = e.NewHeading.TrueHeading.ToString () + "º";
			};
			
			// start updating our location, et. al.
			if (CLLocationManager.LocationServicesEnabled)
				iPhoneLocationManager.StartUpdatingLocation ();
			if (CLLocationManager.HeadingAvailable)
				iPhoneLocationManager.StartUpdatingHeading ();
		}
		
		#region -= protected methods =-
		
		// Loads either the iPad or iPhone view, based on the current device
		protected void LoadViewForDevice()
		{
			// load the appropriate view based on the device
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
					mainViewController_iPad = new MainViewController_iPad ();
					this.View.AddSubview (mainViewController_iPad.View);
					mainScreen = mainViewController_iPad as IMainScreen;
			} else {
					mainViewController_iPhone = new MainViewController_iPhone ();
					this.View.AddSubview (mainViewController_iPhone.View);
					mainScreen = mainViewController_iPhone as IMainScreen;
			}
		}

		#endregion
		
		// If you don't want to use events, you could define a delegate to do the 
		// updates as well, as shown here.
		public class LocationDelegate : CLLocationManagerDelegate
		{
			IMainScreen ms;
			
			public LocationDelegate (IMainScreen mainScreen) : base()
			{
				ms = mainScreen;
			}
			
			public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
			{
				ms.LblAltitude.Text = newLocation.Altitude.ToString () + "meters";
				ms.LblLongitude.Text = newLocation.Coordinate.Longitude.ToString () + "º";
				ms.LblLatitude.Text = newLocation.Coordinate.Latitude.ToString () + "º";
				ms.LblCourse.Text = newLocation.Course.ToString () + "º";
				ms.LblSpeed.Text = newLocation.Speed.ToString () + "meters/s";
				
				// get the distance from here to paris
				ms.LblDistanceToParis.Text = (newLocation.DistanceFrom(new CLLocation(48.857, 2.351)) / 1000).ToString() + "km";
			}
			
			public override void UpdatedHeading (CLLocationManager manager, CLHeading newHeading)
			{
				ms.LblMagneticHeading.Text = newHeading.MagneticHeading.ToString () + "º";
				ms.LblTrueHeading.Text = newHeading.TrueHeading.ToString () + "º";
			}
		}
	}
}
