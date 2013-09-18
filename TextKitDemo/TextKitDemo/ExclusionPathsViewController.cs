using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace TextKitDemo
{
	/* 
	 * Implements the ExclusionPaths view, which shows how a bezier can be used
	 * to alter the flow of the rendered text.
	 */
	public partial class ExclusionPathsViewController : TextViewController
	{
		UIBezierPath originalButterflyPath = (UIBezierPath)NSKeyedUnarchiver.UnarchiveFile ("butterflyPath.plist");
		PointF gestureStartingPoint;
		PointF gestureStartingCenter;

		public ExclusionPathsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			imageView.Image = UIImage.FromFile ("butterfly@2x.png");

			if (model != null)
				textView.AttributedText = model.GetAttributedText ();

			textView.TextAlignment = UITextAlignment.Justified;
			textView.TextContainer.ExclusionPaths = TranslatedBezierPath ();

			UIPanGestureRecognizer gesture = new UIPanGestureRecognizer (HandlePanGesture);

			imageView.UserInteractionEnabled = true;
			imageView.AddGestureRecognizer (gesture);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			textView.TextContainer.ExclusionPaths = TranslatedBezierPath ();
		}

		void HandlePanGesture (UIPanGestureRecognizer gesture)
		{
			if (gesture.State == UIGestureRecognizerState.Began) {
				gestureStartingPoint = gesture.TranslationInView (textView);
				gestureStartingCenter = imageView.Center;
			}
			else if (gesture.State == UIGestureRecognizerState.Changed) {
				PointF currentPoint = gesture.TranslationInView(textView);
				float distanceX = currentPoint.X - gestureStartingPoint.X;
				float distanceY = currentPoint.Y - gestureStartingPoint.Y;

				PointF newCenter = gestureStartingCenter;

				newCenter.X += distanceX;
				newCenter.Y += distanceY;

				imageView.Center = newCenter;

				textView.TextContainer.ExclusionPaths = TranslatedBezierPath ();
			}
			else if (gesture.State == UIGestureRecognizerState.Ended) {
				gestureStartingPoint  = new PointF (0, 0);
				gestureStartingCenter = new PointF (0, 0);
			}
		}

		UIBezierPath[] TranslatedBezierPath ()
		{
			RectangleF butterflyImageRect = textView.ConvertRectFromView (imageView.Frame, View);
			UIBezierPath[] newButterflyPath = new UIBezierPath[1];
			newButterflyPath [0] = (UIBezierPath)((NSObject)originalButterflyPath).Copy ();
			newButterflyPath [0].ApplyTransform (CGAffineTransform.MakeTranslation (butterflyImageRect.X, butterflyImageRect.Y));
			return newButterflyPath;
		}

		public override void PreferredContentSizeChanged ()
		{
			UIFontDescriptor descriptor = textView.Font.FontDescriptor;
			textView.Font = UIFont.GetPreferredFontForTextStyle (descriptor.FontAttributes.TextStyle);
		}
	}
}

