using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using HealthKit;


namespace HKWork
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		private HKHealthStore healthKitStore = new HKHealthStore ();

		public override UIWindow Window {
			get;
			set;
		}
		
		public override void OnActivated (UIApplication application)
		{
			ValidateAuthorization ();
		}

		private void ValidateAuthorization ()
		{
			//Request / Validate that the app has permission to store heart-rate data
			var heartRateId = HKQuantityTypeIdentifierKey.HeartRate;
			var heartRateType = HKObjectType.GetQuantityType (heartRateId);
			var typesToWrite = new NSSet (new [] { heartRateType });
			//We aren't reading any data for this sample
			var typesToRead = new NSSet ();
			healthKitStore.RequestAuthorizationToShare (
				typesToWrite, 
				typesToRead, 
				ReactToHealthCarePermissions);
		}

		//Note that this will be called on a background thread
		void ReactToHealthCarePermissions (bool success, NSError error)
		{
			/*
			 * The success and error arguments specify whether the user interacted
			 * with the permissions dialog. This sample doesn't use that information.
			 */

			//Instead, the important thing is to confirm that we can write heart-rate data
			var access = healthKitStore.GetAuthorizationStatus (HKObjectType.GetQuantityType (HKQuantityTypeIdentifierKey.HeartRate));
			if (access.HasFlag (HKAuthorizationStatus.SharingAuthorized)) {
				HeartRateModel.Instance.Enabled = true;
			} else {
				HeartRateModel.Instance.Enabled = false;
			}
		}
	}
}

