using System;

using WatchKit;
using Foundation;
using CoreLocation;

namespace WatchAppExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		NSTimer timer;

		public InterfaceController (IntPtr handle) : base (handle)
		{
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			timer = NSTimer.CreateRepeatingScheduledTimer (5, UpdateUserInterface);
			UpdateUserInterface (timer);

			// Configure interface objects here.
			Console.WriteLine ("{0} awake with context", this);
		}

		public override void WillActivate ()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine ("{0} did deactivate", this);
		}

		void UpdateUserInterface(NSTimer t)
		{
			WKInterfaceController.OpenParentApplication (new NSDictionary (), (replyInfo, error) => {
				if(error != null) {
					Console.WriteLine (error);
					return;
				}

				var status = (CLAuthorizationStatus)((NSNumber)replyInfo["status"]).UInt32Value;
				var longitude = ((NSNumber)replyInfo["lon"]).DoubleValue;
				var latitude = ((NSNumber)replyInfo["lat"]).DoubleValue;

				Console.WriteLine ("authorization status {0}", status);
				switch(status) {
					case CLAuthorizationStatus.AuthorizedAlways:
						SetCooridinate(longitude, latitude);
						HideWarning();
						break;

					case CLAuthorizationStatus.NotDetermined:
						SetNotAvailable();
						ShowWarning("Launch the iOS app first");
						break;

					case CLAuthorizationStatus.Denied:
						SetNotAvailable();
						ShowWarning("Enable Location Service on iPhone");
						break;

					default:
						throw new NotImplementedException();
				}
			});
		}

		void SetCooridinate(double longitude, double latitude)
		{
			LongitudeValueLbl.SetText (longitude.ToString ());
			LatitudeValueLbl.SetText (latitude.ToString ());
		}

		void SetNotAvailable()
		{
			LongitudeValueLbl.SetText ("not available");
			LatitudeValueLbl.SetText ("not available");
		}

		void ShowWarning(string warning)
		{
			WarningLbl.SetText (warning);
			WarningLbl.SetHidden (false);
		}

		void HideWarning()
		{
			WarningLbl.SetHidden (true);
		}
	}
}

