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