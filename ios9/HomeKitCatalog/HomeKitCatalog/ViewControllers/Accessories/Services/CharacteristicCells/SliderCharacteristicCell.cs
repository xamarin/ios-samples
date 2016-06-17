using System;

using CoreGraphics;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A `CharacteristicCell` subclass that contains a slider.
	// Used for numeric characteristics that have a continuous range of options.
	[Register ("SliderCharacteristicCell")]
	public class SliderCharacteristicCell : CharacteristicCell
	{
		[Outlet ("valueSlider")]
		public UISlider ValueSlider { get; set; }

		protected override bool UpdatesImmediately {
			get {
				return false;
			}
		}

		public override HMCharacteristic Characteristic {
			get {
				return base.Characteristic;
			}
			set {
				// These are sane defaults in case the max and min are not set.
				HMCharacteristicMetadata metadata;
				if(value != null && (metadata = value.Metadata) != null) {
					ValueSlider.MinValue = metadata.MinimumValue.FloatValue;
					ValueSlider.MaxValue = metadata.MaximumValue.FloatValue;
				} else {
					ValueSlider.MinValue = 0;
					ValueSlider.MaxValue = 100;
				}

				base.Characteristic = value;

				ValueSlider.Alpha = Enabled ? 1 : DisabledAlpha;
				ValueSlider.UserInteractionEnabled = Enabled;
			}
		}

		#region ctor

		public SliderCharacteristicCell (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public SliderCharacteristicCell (NSCoder coder)
			: base (coder)
		{
		}

		[Export ("initWithFrame:")]
		public SliderCharacteristicCell (CGRect frame)
			: base (frame)
		{
		}

		#endregion

		// If notify is false, sets the valueSlider's represented value.
		public override void SetValue (NSObject newValue, bool notify)
		{
			base.SetValue (newValue, notify);

			var v = newValue as NSNumber;
			if (v != null && !notify)
				ValueSlider.Value = v.FloatValue;
		}

		// Restricts a value to the step value provided in the cell's
		// characteristic's metadata.
		float RoundedValueForSliderValue (float value)
		{
			HMCharacteristicMetadata metadata = Characteristic.Metadata;
			if (metadata == null)
				return value;

			float stepValue = metadata.StepValue.FloatValue;
			if (stepValue > 0) {
				var newStep = (float)Math.Round (value / stepValue);
				var stepped = newStep * stepValue;
				return stepped;
			}
			return value;
		}

		// Responds to a slider change and sets the cell's value.
		[Export ("didChangeSliderValue:")]
		void DidChangeSliderValue (UISlider slider)
		{
			var value = RoundedValueForSliderValue (slider.Value);
			SetValue (new NSNumber (value), true);
		}
	}
}