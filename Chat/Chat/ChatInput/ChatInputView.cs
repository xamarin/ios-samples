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

		public static readonly float ToolbarMinHeight = 44;

		public UITextView TextView { get; private set; }
		public UIButton SendButton { get; private set; }

		public ChatInputView()
		{
			TextView = new UITextView ();
			TextView.Changed += OnTextChanged;
			TextView.Started += OnTextChanged;
			TextView.Ended += OnTextChanged;
			TextView.Layer.BorderColor = InputBorderColor.CGColor;
			TextView.Layer.BorderWidth = BorderWidth;
			TextView.Layer.CornerRadius = CornerRadius;
			TextView.BackgroundColor = InputBackgroundColor;
			TextView.TranslatesAutoresizingMaskIntoConstraints = false;

			TextView.ScrollIndicatorInsets = new UIEdgeInsets (CornerRadius, 0, CornerRadius, 0);
			TextView.TextContainerInset = new UIEdgeInsets (4, 2, 4, 2);
			TextView.ContentInset = new UIEdgeInsets (1, 0, 1, 0);
			TextView.ScrollEnabled = true;
			TextView.ScrollsToTop = false;
			TextView.UserInteractionEnabled = true;
			TextView.Font = UIFont.SystemFontOfSize (16);
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
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"input", TextView,
				"button", SendButton
			);
//			var c2 = NSLayoutConstraint.FromVisualFormat ("V:|-[input]-|",
//				NSLayoutFormatOptions.DirectionLeadingToTrailing,
//				"input", TextView
//			);
//			AddConstraints (c2);
			var top = NSLayoutConstraint.Create(TextView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 7);
			var bot = NSLayoutConstraint.Create(TextView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1, -7);
			AddConstraint (top);
			AddConstraint (bot);
			// We want Send button was centered when Toolbar has MinHeight (pin button in this state)
			var c3 = NSLayoutConstraint.Create(SendButton, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1, -ToolbarMinHeight / 2);
			AddConstraints (c1);
			AddConstraint (c3);
		}

		void OnTextChanged (object sender, EventArgs e)
		{
			SetNeedsDisplay ();
		}
	}
}