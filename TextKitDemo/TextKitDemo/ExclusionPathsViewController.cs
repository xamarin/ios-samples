using System;
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;


namespace TextKitDemo
{
	/* 
	 * Implements the ExclusionPaths view, which shows how a bezier can be used
	 * to alter the flow of the rendered text.
	 */
	public partial class ExclusionPathsViewController : TextViewController
	{
		UIBezierPath originalButterflyPath = (UIBezierPath)NSKeyedUnarchiver.UnarchiveFile ("butterflyPath.plist");
		CGPoint gestureStartingPoint;
		CGPoint gestureStartingCenter;

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
				CGPoint currentPoint = gesture.TranslationInView(textView);
				nfloat distanceX = currentPoint.X - gestureStartingPoint.X;
				nfloat distanceY = currentPoint.Y - gestureStartingPoint.Y;

				CGPoint newCenter = gestureStartingCenter;

				newCenter.X += distanceX;
				newCenter.Y += distanceY;

				imageView.Center = newCenter;

				textView.TextContainer.ExclusionPaths = TranslatedBezierPath ();
			}
			else if (gesture.State == UIGestureRecognizerState.Ended) {
				gestureStartingPoint  = new CGPoint (0, 0);
				gestureStartingCenter = new CGPoint (0, 0);
			}
		}

		UIBezierPath[] TranslatedBezierPath ()
		{
			CGRect butterflyImageRect = textView.ConvertRectFromView (imageView.Frame, View);
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

