using System;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace AirLocate {

	class PeripheralManagerDelegate : CBPeripheralManagerDelegate {

		public override void StateUpdated (CBPeripheralManager peripheral)
		{
		}
	}

	public partial class ConfigurationViewController : UITableViewController {

		bool enabled;
		NSUuid uuid;
		NSNumber major;
		NSNumber minor;
		NSNumber power;

		CBPeripheralManager peripheralManager;
		NSNumberFormatter numberFormatter;
		UIBarButtonItem doneButton;
		UIBarButtonItem saveButton;

		public ConfigurationViewController (IntPtr handle) : base (handle)
		{
			var peripheralDelegate = new PeripheralManagerDelegate ();
			peripheralManager = new CBPeripheralManager (peripheralDelegate, DispatchQueue.DefaultGlobalQueue);
			numberFormatter = new NSNumberFormatter () {
				NumberStyle = NSNumberFormatterStyle.Decimal
			};
			uuid = Defaults.DefaultProximityUuid;
			power = Defaults.DefaultPower;
		}

		public override void ViewWillAppear (bool animated)
		{
			enabled = enabledSwitch.On = peripheralManager.Advertising;
			base.ViewWillAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.Title = "Configure";

			enabledSwitch.ValueChanged += (sender, e) => {
				enabled = enabledSwitch.On;
			};

			uuidTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			uuidTextField.InputView = new UuidPickerView (uuidTextField);
			uuidTextField.EditingDidBegin += HandleEditingDidBegin;
			uuidTextField.EditingDidEnd += (sender, e) => {
				uuid = new NSUuid (uuidTextField.Text);
				NavigationItem.RightBarButtonItem = saveButton;
			};
			uuidTextField.Text = uuid.AsString ();

			majorTextField.KeyboardType = UIKeyboardType.NumberPad;
			majorTextField.ReturnKeyType = UIReturnKeyType.Done;
			majorTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			majorTextField.EditingDidBegin += HandleEditingDidBegin;
			majorTextField.EditingDidEnd += (sender, e) => {
				major = numberFormatter.NumberFromString (majorTextField.Text);
				NavigationItem.RightBarButtonItem = saveButton;
			};

			minorTextField.KeyboardType = UIKeyboardType.NumberPad;
			minorTextField.ReturnKeyType = UIReturnKeyType.Done;
			minorTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			minorTextField.EditingDidBegin += HandleEditingDidBegin;
			minorTextField.EditingDidEnd += (sender, e) => {
				minor = numberFormatter.NumberFromString (minorTextField.Text);
				NavigationItem.RightBarButtonItem = saveButton;
			};

			measuredPowerTextField.KeyboardType = UIKeyboardType.NumberPad;
			measuredPowerTextField.ReturnKeyType = UIReturnKeyType.Done;
			measuredPowerTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			measuredPowerTextField.EditingDidBegin += HandleEditingDidBegin;
			measuredPowerTextField.EditingDidEnd += (sender, e) => {
				power = numberFormatter.NumberFromString (measuredPowerTextField.Text);
				NavigationItem.RightBarButtonItem = saveButton;
			};

			doneButton = new UIBarButtonItem (UIBarButtonSystemItem.Done, (sender, e) => {
				uuidTextField.ResignFirstResponder ();
				majorTextField.ResignFirstResponder ();
				minorTextField.ResignFirstResponder ();
				measuredPowerTextField.ResignFirstResponder ();
				TableView.ReloadData ();
			});

			saveButton = new UIBarButtonItem (UIBarButtonSystemItem.Save, (sender, e) => {
				if (peripheralManager.State < CBPeripheralManagerState.PoweredOn) {
					new UIAlertView ("Bluetooth must be enabled", "To configure your device as a beacon", null, "OK", null).Show ();
					return;
				}

				if (enabled) {
					CLBeaconRegion region = Helpers.CreateRegion (uuid, major, minor);
					if (region != null)
						peripheralManager.StartAdvertising (region.GetPeripheralData (power));
				} else {
					peripheralManager.StopAdvertising ();
				}

				NavigationController.PopViewControllerAnimated (true);
			});

			NavigationItem.RightBarButtonItem = saveButton;
		}

		// identical code share across all UITextField
		void HandleEditingDidBegin (object sender, EventArgs e)
		{
			NavigationItem.RightBarButtonItem = doneButton;
		}
	}
}