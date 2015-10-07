using System;

using CoreGraphics;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A `CharacteristicCell` subclass that contains a single switch. Used for Boolean characteristics.
	[Register("SwitchCharacteristicCell")]
	public class SwitchCharacteristicCell : CharacteristicCell
	{
		[Outlet ("valueSwitch")]
		public UISwitch ValueSwitch { get; set; }

		public override HMCharacteristic Characteristic {
			get {
				return base.Characteristic;
			}
			set {
				base.Characteristic = value;

				ValueSwitch.Alpha = Enabled ? 1 : DisabledAlpha;
				ValueSwitch.UserInteractionEnabled = Enabled;
			}
		}

		#region ctor

		public SwitchCharacteristicCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public SwitchCharacteristicCell (NSCoder coder)
			: base (coder)
		{
		}

		[Export ("initWithFrame:")]
		public SwitchCharacteristicCell (CGRect frame)
			: base (frame)
		{
		}

		#endregion

		// If notify is false, sets the switch to the value.
		public override void SetValue (NSObject newValue, bool notify)
		{
			base.SetValue (newValue, notify);

			var value = newValue as NSNumber;
			if (!notify && value != null)
				ValueSwitch.SetState (value.BoolValue, true);
		}

		// Responds to the switch updating and sets the value to the switch's value.
		[Export ("didChangeSwitchValue:")]
		void DidChangeSwitchValue (UISwitch valueSwitch)
		{
			SetValue (new NSNumber (valueSwitch.On), true);
		}
	}
}