using System;
using System.Collections.Generic;

using CoreFoundation;
using CoreLocation;
using Foundation;

namespace AirLocate
{
	public class CalibrationCompletedEventArgs : EventArgs
	{
		public int MeasurePower { get; set; }

		public NSError Error { get; set; }
	}

	public class CalibrationProgressEventArgs : EventArgs
	{
		public float PercentComplete { get; set; }
	}

	public class CalibrationCancelledError : NSError
	{
		static NSString ErrorDomain = new NSString (Defaults.Identifier);

		public CalibrationCancelledError () :
			base (ErrorDomain, 2, new NSDictionary ("Calibration was cancelled", NSError.LocalizedDescriptionKey))
		{
		}
	}

	public class CalibrationCalculator
	{
		static NSString Rssi = new NSString ("rssi");

		CLLocationManager locationManager;
		CLBeaconRegion region;
		bool isCalibrating;
		NSTimer timer;
		List<CLBeacon[]> rangedBeacons;
		float percentComplete;

		public event EventHandler<CalibrationCompletedEventArgs> CalibrationCompletionHandler;
		public event EventHandler<CalibrationProgressEventArgs> ProgressHandler;

		public CalibrationCalculator (CLBeaconRegion region, EventHandler<CalibrationCompletedEventArgs> handler)
		{
			this.region = region;
			rangedBeacons = new List<CLBeacon[]> ();
			CalibrationCompletionHandler = handler;

			locationManager = new CLLocationManager ();
			locationManager.DidRangeBeacons += (object sender, CLRegionBeaconsRangedEventArgs e) => {
				rangedBeacons.Add (e.Beacons);
				var progress = ProgressHandler;
				if (progress != null) {
					DispatchQueue.MainQueue.DispatchAsync (delegate {
						percentComplete += 1.0f / 20.0f;
						progress (this, new CalibrationProgressEventArgs () { PercentComplete = percentComplete });
					});
				}
			};
		}

		public void CancelCalibration ()
		{
			if (isCalibrating) {
				isCalibrating = false;
				timer.Fire ();
			}
		}

		public void PerformCalibration (EventHandler<CalibrationProgressEventArgs> handler)
		{
			if (!isCalibrating) {
				isCalibrating = true;
				rangedBeacons.Clear ();
				percentComplete = 0.0f;

				ProgressHandler = handler;

				locationManager.StartRangingBeacons (region);
				timer = NSTimer.CreateTimer (20.0f, (r) => {
					locationManager.StopRangingBeacons (region);

					DispatchQueue.DefaultGlobalQueue.DispatchAsync (new Action (delegate {
						NSError error = null;
						List<CLBeacon> allBeacons = new List<CLBeacon> ();
						int measuredPower = 0;
						if (!isCalibrating) {
							error = new CalibrationCancelledError ();
						} else {
							foreach (CLBeacon[] beacons in rangedBeacons) {
								if (beacons.Length > 1) {
									error = new CalibrationCancelledError ();
									break;
								} else {
									allBeacons.AddRange (beacons);
								}
							}

							if (allBeacons.Count == 0) {
								error = new CalibrationCancelledError ();
							} else {
								allBeacons.Sort (delegate (CLBeacon x, CLBeacon y) {
									return (x.ValueForKey (Rssi) as NSNumber).CompareTo (y.ValueForKey (Rssi));
								});
								float power = 0;
								int number = 0;
								int outlierPadding = (int)(allBeacons.Count * 0.1f);
								for (int k = outlierPadding; k < allBeacons.Count - (outlierPadding * 2); k++) {
									power += ((NSNumber)allBeacons [k].ValueForKey (Rssi)).FloatValue;
									number++;
								}
								measuredPower = (int)power / number;
							}
						}

						DispatchQueue.MainQueue.DispatchAsync (delegate {
							CalibrationCompletionHandler (this, new CalibrationCompletedEventArgs () {
								MeasurePower = measuredPower,
								Error = error
							});
						});

						isCalibrating = false;
						rangedBeacons.Clear ();
					}));
				});
				NSRunLoop.Current.AddTimer (timer, NSRunLoopMode.Default);
			}
		}
	}
}
