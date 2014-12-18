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

	public class BluetoothPrivacyController : PrivacyDetailViewController
	{
		CBCentralManager cbManager = new CBCentralManager ();

		public BluetoothPrivacyController()
		{
			CheckAccess = CheckBluetoothAccess;
			RequestAccess = RequestBluetoothAccess;
		}

	 	string CheckBluetoothAccess ()
		{
			CBCentralManagerState state = cbManager.State;
			return state.ToString ();
		}

		void RequestBluetoothAccess ()
		{
			if (cbManager.State == CBCentralManagerState.PoweredOn)
				cbManager.ScanForPeripherals (new CBUUID [0]);
			else {
				UIAlertView alert = new UIAlertView ("Error", "Bluetooth must be enabled",
					null, "Okay", null);
				alert.Show ();
			}
		}
	}

}
