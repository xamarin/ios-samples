using System;
using System.Drawing;
using Foundation;
using UIKit;
using CoreGraphics;

namespace UIKit
{
	public static class UIStringDrawing
	{
		public static UIColor FillColor { get; set; } = UIColor.Black;

		public static CGSize DrawString (this NSString item, CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment) {

			// Get paragraph style
			var labelStyle = NSParagraphStyle.Default.MutableCopy() as UIKit.NSMutableParagraphStyle;

			// Adjust alignment
			labelStyle.Alignment = alignment;

			// Adjust line break mode
			labelStyle.LineBreakMode = mode;

			// Define attributes
			var attributes = new UIStringAttributes () {
				Font = font,
				ForegroundColor = UIStringDrawing.FillColor,
				ParagraphStyle = labelStyle
			};

			// Preform drawing
			item.DrawString(rect, attributes);

			// Return new bounding size
			return new CGSize (rect.Width, rect.Height);
		}
			
	}
}

