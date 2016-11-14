using UIKit;

namespace Chat
{
	public class ChatInputView : UIView
	{
		static readonly UIColor ButtonTextColorNormal = UIColor.FromRGB (1, 122, 255);
		static readonly UIColor ButtonTextColorDisabled = UIColor.FromRGB (142, 142, 147);
		static readonly UIFont ButtonFont = UIFont.BoldSystemFontOfSize (17f);

		static readonly UIColor InputBackgroundColor = UIColor.FromWhiteAlpha (250, 1);
		static readonly UIColor InputBorderColor = UIColor.FromRGB (200, 200, 205);
		const float BorderWidth = 0.5f;
		const float CornerRadius = 5;

		public static readonly float ToolbarMinHeight = 44f;

		public UITextView TextView { get; private set; }

		public UIButton SendButton { get; private set; }

		public ChatInputView ()
		{
			TextView = new UITextView {
				TranslatesAutoresizingMaskIntoConstraints = false,
				BackgroundColor = InputBackgroundColor,
				ScrollIndicatorInsets = new UIEdgeInsets (CornerRadius, 0f, CornerRadius, 0f),
				TextContainerInset = new UIEdgeInsets (4f, 2f, 4f, 2f),
				ContentInset = new UIEdgeInsets (1f, 0f, 1f, 0f),
				ScrollEnabled = true,
				ScrollsToTop = false,
				UserInteractionEnabled = true,
				Font = UIFont.SystemFontOfSize (16f),
				TextAlignment = UITextAlignment.Natural,
				ContentMode = UIViewContentMode.Redraw
			};
			TextView.Layer.BorderColor = InputBorderColor.CGColor;
			TextView.Layer.BorderWidth = BorderWidth;
			TextView.Layer.CornerRadius = CornerRadius;

			SendButton = new UIButton {
				TranslatesAutoresizingMaskIntoConstraints = false,
				Font = ButtonFont
			};
			SendButton.SetTitle ("Send", UIControlState.Normal);
			SendButton.SetTitleColor (ButtonTextColorNormal, UIControlState.Normal);
			SendButton.SetTitleColor (ButtonTextColorDisabled, UIControlState.Disabled);

			AddSubviews (TextView, SendButton);

			var c1 = NSLayoutConstraint.FromVisualFormat ("H:|-[input]-[button]-|",
				0,
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