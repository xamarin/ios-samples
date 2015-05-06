using System;

using UIKit;
using Foundation;

namespace Chat
{
	public partial class ChatInputView : UIView
	{
		static readonly UIColor ButtonTextColorNormal = UIColor.FromRGB (1, 122, 255);
		static readonly UIColor ButtonTextColorDisabled = UIColor.FromRGB (142, 142, 147);
		static readonly UIFont ButtonFont = UIFont.SystemFontOfSize (17f, UIFontWeight.Bold);

		static readonly UIColor InputBackgroundColor = UIColor.FromWhiteAlpha (250, 1);
		static readonly UIColor InputBorderColor = UIColor.FromRGB (200, 200, 205);
		const float BorderWidth = 0.5f;
		const float CornerRadius = 5;

		public static readonly float ToolbarMinHeight = 44f;

		public UITextView TextView { get; private set; }

		public UIButton SendButton { get; private set; }

		public ChatInputView ()
		{
			TextView = new UITextView ();
			TextView.Layer.BorderColor = InputBorderColor.CGColor;
			TextView.Layer.BorderWidth = BorderWidth;
			TextView.Layer.CornerRadius = CornerRadius;
			TextView.BackgroundColor = InputBackgroundColor;
			TextView.TranslatesAutoresizingMaskIntoConstraints = false;

			TextView.ScrollIndicatorInsets = new UIEdgeInsets (CornerRadius, 0f, CornerRadius, 0f);
			TextView.TextContainerInset = new UIEdgeInsets (4f, 2f, 4f, 2f);
			TextView.ContentInset = new UIEdgeInsets (1f, 0f, 1f, 0f);
			TextView.ScrollEnabled = true;
			TextView.ScrollsToTop = false;
			TextView.UserInteractionEnabled = true;
			TextView.Font = UIFont.SystemFontOfSize (16f);
			TextView.TextAlignment = UITextAlignment.Natural;
			TextView.ContentMode = UIViewContentMode.Redraw;


			SendButton = new UIButton ();
			SendButton.SetTitle ("Send", UIControlState.Normal);
			SendButton.Font = ButtonFont;
			SendButton.SetTitleColor (ButtonTextColorNormal, UIControlState.Normal);
			SendButton.SetTitleColor (ButtonTextColorDisabled, UIControlState.Disabled);
			SendButton.TranslatesAutoresizingMaskIntoConstraints = false;

			AddSubviews (TextView, SendButton);

			var c1 = NSLayoutConstraint.FromVisualFormat ("H:|-[input]-[button]-|",
				(NSLayoutFormatOptions)0,
				"input", TextView,
				"button", SendButton
			);
			var top = NSLayoutConstraint.Create (TextView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1f, 7f);
			var bot = NSLayoutConstraint.Create (TextView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1f, -7f);
			AddConstraint (top);
			AddConstraint (bot);
			// We want Send button was centered when Toolbar has MinHeight (pin button in this state)
			var c2 = NSLayoutConstraint.Create (SendButton, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1f, -ToolbarMinHeight / 2);
			AddConstraints (c1);
			AddConstraint (c2);
		}
	}
}