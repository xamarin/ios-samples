using CoreGraphics;
using Foundation;
using UIKit;

namespace UICatalog {
	public class CustomInputAccessoryView : UIView {
		
		UILabel titleLabel;

		[Export ("initWithFrame:")]
		public CustomInputAccessoryView (CGRect frame) : base (frame)
		{
			Initialize (string.Empty);
		}

		public CustomInputAccessoryView (string title) : this (CGRect.Empty)
		{
			Initialize (title);
		}

		void Initialize (string title)
		{
			titleLabel = new UILabel (CGRect.Empty) {
				Font = UIFont.SystemFontOfSize (60, UIFontWeight.Medium),
				Text = title,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			AddSubview (titleLabel);
			TranslatesAutoresizingMaskIntoConstraints = false;

			var viewsDictionary = NSMutableDictionary.FromObjectAndKey (titleLabel, (NSString)"titleLabel");
			AddConstraints (NSLayoutConstraint.FromVisualFormat ("H:|-[titleLabel]-|", 0, null, viewsDictionary));
			AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|-[titleLabel]-60-|", 0, null, viewsDictionary));
		}
	}
}

