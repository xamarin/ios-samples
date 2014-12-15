using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace AdaptivePhotos
{
	public class PhotoViewController : CustomViewController
	{
		Photo photo;

		public Photo Photo {
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

		UIImageView ImageView { get; set; }

		OverlayView OverlayButton { get; set; }

		RatingControl RatingControl { get; set; }

		public PhotoViewController ()
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

			var ratingControl = new RatingControl ();
			ratingControl.TranslatesAutoresizingMaskIntoConstraints = false;
			ratingControl.AddTarget (RatingChanges, UIControlEvent.ValueChanged);
			RatingControl = ratingControl;
			View.Add (RatingControl);

			var overlayButton = new OverlayView ();
			overlayButton.TranslatesAutoresizingMaskIntoConstraints = false;
			OverlayButton = overlayButton;
			View.Add (OverlayButton);

			UpdatePhoto ();

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("|[imageView]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"imageView", imageView));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:|[imageView]|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"imageView", imageView));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("[ratingControl]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"ratingControl", ratingControl));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("[overlayButton]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"overlayButton", overlayButton));

			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("V:[overlayButton]-[ratingControl]-|",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"overlayButton", overlayButton,
				"ratingControl", ratingControl));

			var constraints = new List<NSLayoutConstraint> ();

			constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|-(>=20)-[ratingControl]",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"ratingControl", ratingControl));

			constraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|-(>=20)-[overlayButton]",
				NSLayoutFormatOptions.DirectionLeadingToTrailing,
				"overlayButton", overlayButton));

			foreach (var constraint in constraints)
				constraint.Priority = (int)UILayoutPriority.Required - 1;

			View.AddConstraints (constraints.ToArray ());
		}

		public override Photo ContainedPhoto (Photo photo)
		{
			return Photo;
		}

		void RatingChanges (object sender, EventArgs e)
		{
			photo.Rating = ((RatingControl)sender).Rating;
		}

		void UpdatePhoto ()
		{
			if (ImageView == null)
				return;

			ImageView.Image = Photo.Image;
			OverlayButton.Text = Photo.Comment;
			RatingControl.Rating = Photo.Rating;
		}
	}
}

