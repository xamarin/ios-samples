using System;

using UIKit;
using Foundation;

namespace Chat
{
	public partial class ChatInputView : UIView
	{
		static readonly UIColor ButtonTextColorNormal = UIColor.FromRGB (1, 122, 255);
		static readonly UIColor ButtonTextColorDisabled = UIColor.FromRGB (142, 142, 147);
		static readonly UIFont ButtonFont = UIFont.SystemFontOfSize (17, UIFontWeight.Bold);

		static readonly UIColor InputBackgroundColor =  UIColor.FromWhiteAlpha (250, 1);
		static readonly UIColor InputBorderColor = UIColor.FromRGB (200, 200, 205);
		const float BorderWidth = 0.5f;
		const float CornerRadius = 5;

		const float ToolbarMinHeight = 44;

		public UITextView TextView { get; private set; }
		public UIButton SendButton { get; private set; }

		public ChatInputView()
		{
			TextView = new UITextView ();
			TextView.Layer.BorderColor = InputBorderColor.CGColor;
			TextView.Layer.BorderWidth = BorderWidth;
			TextView.Layer.CornerRadius = CornerRadius;
			TextView.BackgroundColor = InputBackgroundColor;
			TextView.TranslatesAutoresizingMaskIntoConstraints = false;

			SendButton = new UIButton ();
			SendButton.SetTitle ("Send", UIControlState.Normal);
			SendButton.Font = ButtonFont;
			SendButton.SetTitleColor (ButtonTextColorNormal, UIControlState.Normal);
			SendButton.SetTitleColor (ButtonTextColorDisabled, UIControlState.Disabled);
			SendButton.TranslatesAutoresizingMaskIntoConstraints = false;

			AddSubviews (TextView, SendButton);

			var c1 = NSLayoutConstraint.FromVisualFormat ("H:|-[input]-[button]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"input", TextView,
				"button", SendButton
			);
			var c2 = NSLayoutConstraint.FromVisualFormat ("V:|-[input]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"input", TextView
			);
			// We want Send button was centered when Toolbar has MinHeight (pin button in this state)
			var c3 = NSLayoutConstraint.Create(SendButton, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1, -ToolbarMinHeight / 2);
			AddConstraints (c1);
			AddConstraints (c2);
			AddConstraint (c3);
		}
	}
}