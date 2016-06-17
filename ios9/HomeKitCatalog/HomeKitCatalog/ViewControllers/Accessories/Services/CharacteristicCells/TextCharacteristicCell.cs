using System;

using CoreGraphics;
using Foundation;
using UIKit;

namespace HomeKitCatalog
{
	// A `CharacteristicCell` subclass that contains a text field. Used for text-input characteristics.
	[Register ("TextCharacteristicCell")]
	public class TextCharacteristicCell : CharacteristicCell, IUITextFieldDelegate
	{
		[Outlet ("textField")]
		public UITextField TextField { get; set; }

		public override HomeKit.HMCharacteristic Characteristic {
			get {
				return base.Characteristic;
			}
			set {
				base.Characteristic = value;

				TextField.Alpha = Enabled ? 1 : DisabledAlpha;
				TextField.UserInteractionEnabled = Enabled;
			}
		}

		#region ctor

		public TextCharacteristicCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public TextCharacteristicCell (NSCoder coder)
			: base (coder)
		{
		}

		[Export ("initWithFrame:")]
		public TextCharacteristicCell (CGRect frame)
			: base (frame)
		{
		}

		#endregion

		// If notify is false, sets the text field's text from the value.
		public override void SetValue (NSObject newValue, bool notify)
		{
			base.SetValue (newValue, notify);

			var v = newValue as NSString;
			if (!notify && v != null)
				TextField.Text = v;
		}

		// Dismiss the keyboard when "Go" is clicked
		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			TextField.ResignFirstResponder ();
			return true;
		}

		// Sets the value of the characteristic when editing is complete.
		[Export ("textFieldDidEndEditing:")]
		public void EditingEnded (UITextField textField)
		{
			SetValue ((NSString)TextField.Text, true);
		}
	}
}