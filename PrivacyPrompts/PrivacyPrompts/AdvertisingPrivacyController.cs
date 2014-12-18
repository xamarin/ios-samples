using System;
using Foundation;
using UIKit;
using CoreGraphics;
using CoreGraphics;
using AddressBook;
using EventKit;
using AssetsLibrary;
using AVFoundation;
using CoreBluetooth;
using Accounts;
using AdSupport;
using CoreLocation;

namespace PrivacyPrompts
{

	public class AdvertisingPrivacyController : PrivacyDetailViewController
	{

		public AdvertisingPrivacyController()
		{
			CheckAccess = CheckAdvertisingAccess;
			RequestAccess = () => {
			};
			this.requestAccessButton.Enabled = false;
		}

		string CheckAdvertisingAccess ()
		{
			return
				ASIdentifierManager.SharedManager.IsAdvertisingTrackingEnabled ?
				"granted" : "denied";
		}

	}
}
