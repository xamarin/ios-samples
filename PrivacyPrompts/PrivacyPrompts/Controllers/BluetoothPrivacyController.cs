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

	public class BluetoothPrivacyController : PrivacyDetailViewController
	{
		readonly CBCentralManager cbManager = new CBCentralManager ();

		/*
		protected override string CheckAccess ()
		{
			return cbManager.State.ToString ();
		}

		protected override void RequestAccess ()
		{
			if (cbManager.State == CBCentralManagerState.PoweredOn)
				cbManager.ScanForPeripherals (new CBUUID [0]);
			else
				ShowError ();
		}
		*/

		void ShowError()
		{
			UIAlertView alert = new UIAlertView ("Error", "Bluetooth must be enabled", null, "Okay", null);
			alert.Show ();
		}
	}

}
