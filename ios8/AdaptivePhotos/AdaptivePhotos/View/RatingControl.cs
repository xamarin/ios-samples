using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AdaptivePhotos
{
	public class RatingControl : UIControl
	{
		const int ratingControlMinimumRating = 0;
		const int ratingControlMaximumRating = 4;

		UIVisualEffectView backgroundView;

		NSArray ImageViews { get; set; }

		nuint currentrating;
		public nuint Rating {
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
			Rating = ratingControlMinimumRating;
			var blurredEffect = UIBlurEffect.FromStyle (UIBlurEffectStyle.Light);
			backgroundView = new UIVisualEffectView (blurredEffect);
			backgroundView.ContentView.BackgroundColor = UIColor.FromWhiteAlpha (0.7f, 0.3f);
			Add (backgroundView);

			var imageViews = new NSMutableArray ();
			for (int rating = ratingControlMinimumRating; rating <= ratingControlMaximumRating; rating++) {
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
			for (nuint i = 0; i < ImageViews.Count; i++)
				ImageViews.GetItem <UIImageView> (i).Highlighted = (i + ratingControlMinimumRating <= Rating);
		}

		void UpdateRatingWithTouches (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;
			CGPoint position = touch.LocationInView (this);
			UIView touchedView = HitTest (position, evt);

			for (nuint i = 0; i < ImageViews.Count; i++) {
				if (ImageViews.GetItem<UIView> (i) == touchedView) {
					Rating = ratingControlMinimumRating + i;
					SendActionForControlEvents (UIControlEvent.ValueChanged);
				}
			}
		}

		void SetupConstraints ()
		{
			backgroundView.TranslatesAutoresizingMaskIntoConstraints = false;

			AddConstraints (NSLayoutConstraint.FromVisualFormat ("|[backgroundView]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"backgroundView", backgroundView));

			AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|[backgroundView]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"backgroundView", backgroundView));

			UIImageView lastImageView = null;
			for (nuint i = 0; i < ImageViews.Count; i++) {
				var imageView = ImageViews.GetItem <UIImageView> (i);
				imageView.TranslatesAutoresizingMaskIntoConstraints = false;

				AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|-4-[imageView]-4-|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing,
					"imageView", imageView));

				AddConstraint (NSLayoutConstraint.Create (imageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
					imageView, NSLayoutAttribute.Height, 1.0f, 0.0f));

				if (lastImageView != null) {
					AddConstraints (NSLayoutConstraint.FromVisualFormat ("[lastImageView][imageView(==lastImageView)]",
						NSLayoutFormatOptions.DirectionLeadingToTrailing,
						"lastImageView", lastImageView,
						"imageView", imageView));
				} else {
					AddConstraints (NSLayoutConstraint.FromVisualFormat ("|-4-[imageView]",
						NSLayoutFormatOptions.DirectionLeadingToTrailing,
						"imageView", imageView));
				}

				lastImageView = imageView;
			}

			AddConstraints (NSLayoutConstraint.FromVisualFormat ("[lastImageView]-4-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"lastImageView", lastImageView));
		}
	}
}

