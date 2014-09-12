using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AdaptivePhotos
{
	public class RatingControl : UIControl
	{
		readonly nint AAPLRatingControlMinimumRating = 0;
		readonly nint AAPLRatingControlMaximumRating = 4;

		nint currentrating;
		UIVisualEffectView backgroundView;

		NSArray ImageViews { get; set; }

		public nint Rating {
			get {
				return currentrating;
			}

			set {
				if (currentrating != value) {
					currentrating = value;
					UpdateImageViews ();
				}
			} 
		}

		public override bool IsAccessibilityElement {
			get {
				return false;
			}
			set {
				base.IsAccessibilityElement = value;
			}
		}

		public RatingControl ()
		{
			Rating = AAPLRatingControlMinimumRating;
			var blurredEffect = UIBlurEffect.FromStyle (UIBlurEffectStyle.Light);
			backgroundView = new UIVisualEffectView (blurredEffect);
			backgroundView.ContentView.BackgroundColor = UIColor.FromWhiteAlpha (0.7f, 0.3f);
			Add (backgroundView);

			var imageViews = new NSMutableArray ();
			for (nint rating = AAPLRatingControlMinimumRating; rating <= AAPLRatingControlMaximumRating; rating++) {
				UIImageView imageView = new UIImageView ();
				imageView.UserInteractionEnabled = true;

				imageView.Image = UIImage.FromBundle ("ratingInactive");
				imageView.HighlightedImage = UIImage.FromBundle ("ratingActive");
				imageView.HighlightedImage = imageView.HighlightedImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);

				imageView.AccessibilityLabel = string.Format ("{0} stars", rating + 1);
				Add (imageView);
				imageViews.Add (imageView);
			}

			ImageViews = imageViews;
			UpdateImageViews ();
			SetupConstraints ();
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			UpdateRatingWithTouches (touches, evt);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			UpdateRatingWithTouches (touches, evt);
		}

		void UpdateImageViews ()
		{
			for (nint i = 0; i < (nint)ImageViews.Count; i++)
				ImageViews.GetItem <UIImageView> (i).Highlighted = (i + AAPLRatingControlMinimumRating <= Rating);
		}

		void UpdateRatingWithTouches (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;
			CGPoint position = touch.LocationInView (this);
			UIView touchedView = HitTest (position, evt);

			for (nint i = 0; i < (nint)ImageViews.Count; i++) {
				if (ImageViews.GetItem<UIView> (i) == touchedView) {
					Rating = AAPLRatingControlMinimumRating + i;
					SendActionForControlEvents (UIControlEvent.ValueChanged);
				}
			}
		}

		void SetupConstraints ()
		{
			backgroundView.TranslatesAutoresizingMaskIntoConstraints = false;
			NSDictionary views = NSDictionary.FromObjectAndKey (backgroundView, new NSString ("backgroundView"));
			var constraints = NSLayoutConstraint.FromVisualFormat ("|[backgroundView]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);
			AddConstraints (constraints);
			constraints = NSLayoutConstraint.FromVisualFormat ("V:|[backgroundView]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);
			AddConstraints (constraints);

			UIImageView lastImageView = null;
			for (nint i = 0; i < (nint)ImageViews.Count; i++) {
				var imageView = ImageViews.GetItem <UIImageView> (i);
				imageView.TranslatesAutoresizingMaskIntoConstraints = false;

				NSDictionary currentImageViews = null;

				if (lastImageView != null) {
					currentImageViews = NSDictionary.FromObjectsAndKeys (new object[] { imageView, lastImageView }, 
						new string[] { "imageView", "lastImageView" });
				} else {
					currentImageViews = NSDictionary.FromObjectAndKey (imageView, new NSString ("imageView"));
				}

				constraints = NSLayoutConstraint.FromVisualFormat ("V:|-4-[imageView]-4-|", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, currentImageViews);
				AddConstraints (constraints);
				AddConstraint (NSLayoutConstraint.Create (imageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 
					imageView, NSLayoutAttribute.Height, 1.0f, 0.0f));

				if (lastImageView != null) {
					constraints = NSLayoutConstraint.FromVisualFormat ("[lastImageView][imageView(==lastImageView)]", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, currentImageViews);
				} else {
					constraints = NSLayoutConstraint.FromVisualFormat ("|-4-[imageView]", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, currentImageViews);
				}

				AddConstraints (constraints);
				lastImageView = imageView;
			}
				
			NSDictionary actualImageViews = NSDictionary.FromObjectAndKey (lastImageView, new NSString ("lastImageView"));
			constraints = NSLayoutConstraint.FromVisualFormat ("[lastImageView]-4-|", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, actualImageViews);
			AddConstraints (constraints);
		}
	}
}

