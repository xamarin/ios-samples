using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace AdaptivePhotos
{
	public class AAPLRatingControl : UIControl
	{
		private const int AAPLRatingControlMinimumRating = 0;
		private const int AAPLRatingControlMaximumRating = 4;

		private int currentrating;
		private UIVisualEffectView backgroundView;

		private NSArray ImageViews { get; set; }

		public int Rating {
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

		public AAPLRatingControl ()
		{
			Rating = AAPLRatingControlMinimumRating;
			var blurredEffect = UIBlurEffect.FromStyle (UIBlurEffectStyle.Light);
			backgroundView = new UIVisualEffectView (blurredEffect);
			backgroundView.ContentView.BackgroundColor = UIColor.FromWhiteAlpha (0.7f, 0.3f);
			Add (backgroundView);

			var imageViews = new NSMutableArray ();
			for (int rating = AAPLRatingControlMinimumRating; rating <= AAPLRatingControlMaximumRating; rating++) {
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

		private void UpdateImageViews ()
		{
			for (int i = 0; i < ImageViews.Count; i++)
				ImageViews.GetItem <UIImageView> (i).Highlighted = (i + AAPLRatingControlMinimumRating <= Rating);
		}

		private void UpdateRatingWithTouches (NSSet touches, UIEvent evt)
		{
			UITouch touch = (UITouch)touches.AnyObject;
			PointF position = touch.LocationInView (this);
			UIView touchedView = HitTest (position, evt);

			for (int i = 0; i < ImageViews.Count; i++) {
				if (ImageViews.GetItem<UIView> (i) == touchedView) {
					Rating = AAPLRatingControlMinimumRating + i;
					SendActionForControlEvents (UIControlEvent.ValueChanged);
				}
			}
		}

		private void SetupConstraints ()
		{
			backgroundView.TranslatesAutoresizingMaskIntoConstraints = false;
			NSDictionary views = NSDictionary.FromObjectAndKey (backgroundView, new NSString ("backgroundView"));
			var constraints = NSLayoutConstraint.FromVisualFormat ("|[backgroundView]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);
			AddConstraints (constraints);
			constraints = NSLayoutConstraint.FromVisualFormat ("V:|[backgroundView]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);
			AddConstraints (constraints);

			UIImageView lastImageView = null;
			for (int i = 0; i < ImageViews.Count; i++) {
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

