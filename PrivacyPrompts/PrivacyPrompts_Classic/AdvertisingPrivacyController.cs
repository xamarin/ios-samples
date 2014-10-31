using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.AddressBook;
using MonoTouch.EventKit;
using MonoTouch.AssetsLibrary;
using MonoTouch.AVFoundation;
using MonoTouch.CoreBluetooth;
using MonoTouch.Accounts;
using MonoTouch.AdSupport;
using MonoTouch.CoreLocation;

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