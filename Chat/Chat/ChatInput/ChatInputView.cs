using System;

using UIKit;
using Foundation;

namespace Chat
{
	public partial class ChatInputView : UIView
	{
		public UITextView TextView { get; private set; }
		public UIButton SendButton { get; private set; }

		public ChatInputView()
		{
			TextView = new UITextView ();
			TextView.Layer.BorderColor = UIColor.FromRGB (200, 200, 205).CGColor;
			TextView.Layer.BorderWidth = (float)0.5;
			TextView.Layer.CornerRadius = 5;
			TextView.BackgroundColor = UIColor.FromWhiteAlpha (250, 1);
			TextView.TranslatesAutoresizingMaskIntoConstraints = false;

			SendButton = new UIButton ();
			SendButton.SetTitle ("Send", UIControlState.Normal);
			SendButton.Font = UIFont.SystemFontOfSize (15, UIFontWeight.Bold);
			SendButton.TranslatesAutoresizingMaskIntoConstraints = false;
			SendButton.SetTitleColor (UIColor.FromRGB (142, 142, 147), UIControlState.Disabled);
			SendButton.SetTitleColor (UIColor.FromRGB (1, 122, 255), UIControlState.Normal);

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
			var c3 = NSLayoutConstraint.FromVisualFormat ("V:[button]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"button", SendButton
			);
			AddConstraints (c1);
			AddConstraints (c2);
			AddConstraints (c3);
		}
	}
}