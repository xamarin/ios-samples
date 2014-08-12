using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace AdaptivePhotos
{
	public class AAPLPhotoViewController : CustomViewController
	{
		private AAPLPhoto photo;

		public AAPLPhoto Photo { 
			get {
				return photo;
			}

			set {
				if (photo != value) {
					photo = value;
					UpdatePhoto ();
				}
			}
		}

		private UIImageView ImageView { get; set; }

		private AAPLOverlayView OverlayButton { get; set; }

		private AAPLRatingControl RatingControl { get; set; }

		public AAPLPhotoViewController ()
		{
		}

		public override void LoadView ()
		{
			View = new UIView ();
			View.BackgroundColor = UIColor.White;

			var imageView = new UIImageView ();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.TranslatesAutoresizingMaskIntoConstraints = false;
			ImageView = imageView;
			View.Add (ImageView);

			var ratingControl = new AAPLRatingControl ();
			ratingControl.TranslatesAutoresizingMaskIntoConstraints = false;
			ratingControl.AddTarget (RatingChanges, UIControlEvent.ValueChanged);
			RatingControl = ratingControl;
			View.Add (RatingControl);

			var overlayButton = new AAPLOverlayView ();
			overlayButton.TranslatesAutoresizingMaskIntoConstraints = false;
			OverlayButton = overlayButton;
			View.Add (OverlayButton);

			UpdatePhoto ();

			var views = NSDictionary.FromObjectsAndKeys (
	            new object[] { imageView, ratingControl, overlayButton },
	            new object[] { "imageView", "ratingControl", "overlayButton" }
            );

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("|[imageView]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|[imageView]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("[ratingControl]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("[overlayButton]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:[overlayButton]-[ratingControl]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

			var constraints = new List<NSLayoutConstraint> ();

			constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|-(>=20)-[ratingControl]",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

			constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|-(>=20)-[overlayButton]",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

			foreach (var constraint in constraints)
				constraint.Priority = (int)UILayoutPriority.Required - 1;

			View.AddConstraints (constraints.ToArray ());
		}

		public override AAPLPhoto Aapl_containedPhoto (AAPLPhoto photo)
		{
			return Photo;
		}

		private void RatingChanges (object sender, EventArgs e)
		{
			photo.Rating = ((AAPLRatingControl)sender).Rating;
		}

		private void UpdatePhoto ()
		{
			if (ImageView == null)
				return;

			ImageView.Image = Photo.Image;
			OverlayButton.Text = Photo.Comment;
			RatingControl.Rating = Photo.Rating;
		}
	}
}

