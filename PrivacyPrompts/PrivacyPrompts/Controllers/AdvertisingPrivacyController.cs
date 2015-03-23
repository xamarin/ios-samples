using System;
using Foundation;
using UIKit;
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
		protected override string CheckAccess ()
		{
			return
				ASIdentifierManager.SharedManager.IsAdvertisingTrackingEnabled ?
				"granted" : "denied";
		}

	}
}
