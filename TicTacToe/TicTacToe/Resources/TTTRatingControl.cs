using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace TicTacToe
{
	public class TTTRatingControl : UIControl
	{
		const int MinimumRating = 0;
		const int MaximumRating = 0;
		int rating;
		UIImageView backgroundImageView;
		List<UIButton> buttons;

		public TTTRatingControl (RectangleF frame) : base (frame)
		{
			rating = MinimumRating;
		}
		//		static UIImage backgroundImage ()
		//		{
		//			UIImage backgroundImage;
		//			float cornerRadius = 4f;
		//			float capSize = 2f * cornerRadius;
		//			float rectSize = 2f * capSize + 1f;
		//			RectangleF rect = new RectangleF (0f, 0f, rectSize, rectSize);
		//			UIGraphics.BeginImageContextWithOptions (rect.Size, false, 0f);
		//
		//			UIColor.FromWhiteAlpha (0f, 0.2f);
		//			UIBezierPath bezierPath = UIBezierPath.FromRoundedRect (rect, cornerRadius);
		//			bezierPath.Fill ();
		//
		//			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
		//			image = image.CreateResizableImage (new UIEdgeInsets (capSize, capSize,
		//			                                                      capSize, capSize));
		//			image = image.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);
		//			UIGraphics.EndImageContext ();
		//
		//			backgroundImage = image;
		//
		//			return backgroundImage;
		//		}
	}
}

