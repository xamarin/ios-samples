using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace AdaptivePhotos
{
	public class ProfileViewController : CustomViewController
	{
		List<NSLayoutConstraint> constraints;
		User user;

		string NameText {
			get {
				return User.Name;
			}
		}

		string ConversationsText {
			get {
				return string.Format ("{0} conversations", User.Conversations.Count);
			}
		}

		string PhotosText {
			get {
				nuint photosCount = 0;
				for (nint i = 0; i < (nint)User.Conversations.Count; i++)
					photosCount += User.Conversations.GetItem <Conversation> (i).Photos.Count;

				return string.Format ("{0} photos", photosCount);
			}
		}

		UIImageView ImageView { get; set; }

		UILabel NameLabel { get; set; }

		UILabel ConversationsLabel { get; set; }

		UILabel PhotosLabel { get; set; }

		public  User User { 
			get {
				return user;
			}

			set {
				if (user != value) {
					user = value;
					if (IsViewLoaded)
						UpdateUser ();
				}
			}
		}

		public ProfileViewController ()
		{
			Title = "Profile";
		}

		public override void LoadView ()
		{
			var view = new UIView ();
			view.BackgroundColor = UIColor.White;

			ImageView = new UIImageView {
				ContentMode = UIViewContentMode.ScaleAspectFit,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			view.Add (ImageView);

			NameLabel = new UILabel {
				Font = UIFont.PreferredHeadline,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			view.Add (NameLabel);

			ConversationsLabel = new UILabel {
				Font = UIFont.PreferredBody,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			view.Add (ConversationsLabel);

			PhotosLabel = new UILabel {
				Font = UIFont.PreferredBody,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			view.Add (PhotosLabel);

			View = view;
			UpdateUser ();
			UpdateConstraintsForTraitCollection (TraitCollection);
		}

		public void UpdateConstraintsForTraitCollection (UITraitCollection collection)
		{
			var views = NSDictionary.FromObjectsAndKeys (
	            new object[] { TopLayoutGuide, ImageView, NameLabel, ConversationsLabel, PhotosLabel },
	            new object[] { "topLayoutGuide", "imageView", "nameLabel", "conversationsLabel", "photosLabel" }
            );

			var newConstraints = new List<NSLayoutConstraint> ();
			if (collection.VerticalSizeClass == UIUserInterfaceSizeClass.Compact) {
				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|[imageView]-[nameLabel]-|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("[imageView]-[conversationsLabel]-|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("[imageView]-[photosLabel]-|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("V:|[topLayoutGuide]-[nameLabel]-[conversationsLabel]-[photosLabel]",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("V:|[topLayoutGuide][imageView]|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.Add (NSLayoutConstraint.Create (ImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 
					View, NSLayoutAttribute.Width, 0.5f, 0.0f));
			} else {
				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|[imageView]|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|-[nameLabel]-|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|-[conversationsLabel]-|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("|-[photosLabel]-|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));

				newConstraints.AddRange (NSLayoutConstraint.FromVisualFormat ("V:[topLayoutGuide]-[nameLabel]-[conversationsLabel]-[photosLabel]-20-[imageView]|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views));
			}

			if (constraints != null)
				View.RemoveConstraints (constraints.ToArray ());

			constraints = newConstraints;
			View.AddConstraints (constraints.ToArray ());
		}

		public override void WillTransitionToTraitCollection (UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.WillTransitionToTraitCollection (traitCollection, coordinator);
			coordinator.AnimateAlongsideTransition ((UIViewControllerTransitionCoordinatorContext) => {
				UpdateConstraintsForTraitCollection (traitCollection);
				View.SetNeedsLayout ();
			}, (UIViewControllerTransitionCoordinatorContext) => {
			});
		}

		void UpdateUser ()
		{
			NameLabel.Text = NameText;
			ConversationsLabel.Text = ConversationsText;
			PhotosLabel.Text = PhotosText;
			ImageView.Image = User.LastPhoto.Image;
		}
	}
}

