using System;
using System.Drawing;

using MonoTouch.UIKit;

namespace HelloGoodbye
{
	public static class StyleUtilities
	{
		const float kOverlayCornerRadius = 10;
		const float kButtonVerticalContentInset = 10;
		const float kButtonHorizontalContentInset = 10;
		const float kOverlayMargin = 20;
		const float kContentVerticalMargin = 50;
		const float kContentHorizontalMargin = 30;

		public static UIColor ForegroundColor {
			get {
				return new UIColor (75f / 255f, 35.0f / 255f, 106f / 255f, 1f);
			}
		}

		public static UIColor OverlayColor {
			get {
				return UIAccessibility.IsReduceTransparencyEnabled ? UIColor.White
						                                           : UIColor.FromWhiteAlpha (white:1f, alpha: 0.8f);
			}
		}

		public static UIColor CardBorderColor {
			get {
				return ForegroundColor;
			}
		}

		public static UIColor CardBackgroundColor {
			get {
				return UIColor.White;
			}
		}

		public static UIColor DetailColor {
			get {
				return UIAccessibility.DarkerSystemColosEnabled ? UIColor.Black
						                                        : UIColor.Gray;
			}
		}

		public static UIColor DetailOnOverlayColor {
			get {
				return UIColor.Black;
			}
		}

		public static UIColor DetailOnOverlayPlaceholderColor {
			get {
				return UIColor.DarkGray;
			}
		}

		public static UIColor PreviewTabLabelColor {
			get {
				return UIColor.White;
			}
		}

		public static float OverlayCornerRadius {
			get {
				return kOverlayCornerRadius;
			}
		}

		public static float OverlayMargin {
			get {
				return kOverlayMargin;
			}
		}

		public static float ContentHorizontalMargin {
			get {
				return kContentHorizontalMargin;
			}
		}

		public static float ContentVerticalMargin {
			get {
				return kContentVerticalMargin;
			}
		}

		public static string FontName {
			get {
				return UIAccessibility.IsBoldTextEnabled ? "Avenir-Medium" : "Avenir-Light";
			}
		}

		public static UIFont StandardFont {
			get {
				return UIFont.FromName (FontName, 14);
			}
		}

		public static UIFont LargeFont {
			get {
				return UIFont.FromName (FontName, 18f);
			}
		}

		public static UILabel CreateStandardLabel()
		{
			UILabel label = new UILabel {
				TextColor = ForegroundColor,
				Font = StandardFont,
				Lines = 0, // don't force it to be a single line
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			return label;
		}

		public static UILabel CreateDetailLabel()
		{
			UILabel label = CreateStandardLabel();
			label.TextColor = DetailColor;
			return label;
		}

		public static UIButton CreateOverlayRoundedRectButton()
		{
			UIButton button = new UIButton (UIButtonType.Custom);
			button.TranslatesAutoresizingMaskIntoConstraints = false;
			button.SetTitleColor (ForegroundColor, UIControlState.Normal);
			button.TitleLabel.Font = LargeFont;
			button.SetBackgroundImage (CreateOverlayRoundedRectImage (), UIControlState.Normal);
			button.ContentEdgeInsets = new UIEdgeInsets (kButtonVerticalContentInset, kButtonHorizontalContentInset, kButtonVerticalContentInset, kButtonHorizontalContentInset);
			return button;

		}

		private static UIImage CreateOverlayRoundedRectImage()
		{
			UIImage roundedRectImage = null;
			SizeF imageSize = new SizeF(2 * kOverlayCornerRadius, 2 * kOverlayCornerRadius);
			UIGraphics.BeginImageContextWithOptions (imageSize, false, UIScreen.MainScreen.Scale);

			var rect = new RectangleF (PointF.Empty, imageSize);
			UIBezierPath roundedRect = UIBezierPath.FromRoundedRect (rect, kOverlayCornerRadius);
			OverlayColor.SetColor ();
			roundedRect.Fill ();

			roundedRectImage = UIGraphics.GetImageFromCurrentImageContext ();
			roundedRectImage = roundedRectImage.CreateResizableImage (new UIEdgeInsets (kOverlayCornerRadius, kOverlayCornerRadius, kOverlayCornerRadius, kOverlayCornerRadius));
			UIGraphics.EndImageContext ();

			return roundedRectImage;
		}

	}
}