using System;
using System.Drawing;

using UIKit;
using Foundation;

namespace HelloGoodbye
{
	public class AgeSlider : UISlider
	{
		public override string AccessibilityValue {
			get {
				NSNumber number = NSNumber.FromFloat (Value);
				string str = NSNumberFormatter.LocalizedStringFromNumbernumberStyle (number, NSNumberFormatterStyle.Decimal);
				return str;
			}
			set {
				base.AccessibilityValue = value;
			}
		}

		public AgeSlider()
		{
			TintColor = StyleUtilities.ForegroundColor;
			MinValue = 18;
			MaxValue = 120;
		}

		public override void AccessibilityIncrement ()
		{
			Value += 1;
			SendActionForControlEvents (UIControlEvent.ValueChanged);
		}

		public override void AccessibilityDecrement ()
		{
			Value -= 1;
			SendActionForControlEvents (UIControlEvent.ValueChanged);
		}
	}
}
