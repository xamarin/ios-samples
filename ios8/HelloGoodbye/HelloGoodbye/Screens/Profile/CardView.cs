using System;
using System.Drawing;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;

namespace HelloGoodbye
{
	public class CardView : UIView, IUIAccessibilityContainer
	{
		const float PhotoWidth = 80;
		const float BorderWidth = 5;
		const float HorizontalPadding = 20;
		const float VerticalPadding = 20;
		const float InterItemHorizontalSpacing = 30;
		const float InterItemVerticalSpacing = 10;
		const float TitleValueSpacing = 0;

		UIView backgroundView;
		UIImageView photo;
		UILabel ageTitleLabel;
		UILabel ageValueLabel;
		UILabel hobbiesTitleLabel;
		UILabel hobbiesValueLabel;
		UILabel elevatorPitchTitleLabel;
		UILabel elevatorPitchValueLabel;
		NSLayoutConstraint photoAspectRatioConstraint;

		public CardView ()
		{
			BackgroundColor = StyleUtilities.CardBorderColor;

			backgroundView = new UIView {
				BackgroundColor = StyleUtilities.CardBackgroundColor,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			AddSubview (backgroundView);

			AddProfileViews ();
			AddAllConstraints ();
		}

		void AddProfileViews()
		{
			photo = new UIImageView {
				IsAccessibilityElement = true,
				AccessibilityLabel = "Profile photo".LocalizedString ("Accessibility label for profile photo"),
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			AddSubview (photo);

			ageTitleLabel = StyleUtilities.CreateStandardLabel ();
			ageTitleLabel.Text = "Age".LocalizedString("Age of the user");
			AddSubview(ageTitleLabel);

			ageValueLabel = StyleUtilities.CreateDetailLabel ();
			AddSubview (ageValueLabel);

			hobbiesTitleLabel = StyleUtilities.CreateStandardLabel ();
			hobbiesTitleLabel.Text = "Hobbies".LocalizedString ("The user's hobbies");
			AddSubview (hobbiesTitleLabel);

			hobbiesValueLabel = StyleUtilities.CreateDetailLabel ();
			AddSubview (hobbiesValueLabel);

			elevatorPitchTitleLabel = StyleUtilities.CreateStandardLabel ();
			elevatorPitchTitleLabel.Text = "Elevator Pitch".LocalizedString ("The user's elevator pitch for finding a partner");
			AddSubview (elevatorPitchTitleLabel);

			elevatorPitchValueLabel = StyleUtilities.CreateDetailLabel ();
			AddSubview (elevatorPitchValueLabel);

			this.SetAccessibilityElements (NSArray.FromNSObjects (
				photo,
				ageTitleLabel,
				ageValueLabel,
				hobbiesTitleLabel,
				hobbiesValueLabel,
				elevatorPitchTitleLabel,
				elevatorPitchValueLabel
			));
		}

		void AddAllConstraints()
		{
			var constraints = new List<NSLayoutConstraint> ();

			// Fill the card with the background view (leaving a border around it)
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (backgroundView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1f, BorderWidth),
				NSLayoutConstraint.Create (backgroundView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1f, BorderWidth),
				NSLayoutConstraint.Create (this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, backgroundView, NSLayoutAttribute.Trailing, 1f, BorderWidth),
				NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, backgroundView, NSLayoutAttribute.Bottom, 1f, BorderWidth)
			});

			// Position the photo
			// The constant for the aspect ratio constraint will be updated once a photo is set
			photoAspectRatioConstraint = NSLayoutConstraint.Create (photo, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0f, 0f);
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (photo, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1f, HorizontalPadding),
				NSLayoutConstraint.Create (photo, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1f, VerticalPadding),
				NSLayoutConstraint.Create (photo, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0f, PhotoWidth),
				NSLayoutConstraint.Create (photo, NSLayoutAttribute.Bottom, NSLayoutRelation.LessThanOrEqual, this, NSLayoutAttribute.Bottom, 1f, -VerticalPadding),
				photoAspectRatioConstraint
			});

			// Position the age to the right of the photo, with some spacing
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (ageTitleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, photo, NSLayoutAttribute.Trailing, 1f, InterItemHorizontalSpacing),
				NSLayoutConstraint.Create (ageTitleLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, photo, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (ageValueLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ageTitleLabel, NSLayoutAttribute.Bottom, 1f, TitleValueSpacing),
				NSLayoutConstraint.Create (ageValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ageTitleLabel, NSLayoutAttribute.Leading, 1f, 0f)
			});

			// Position the hobbies to the right of the age
			constraints.AddRange (new NSLayoutConstraint [] {
				NSLayoutConstraint.Create (hobbiesTitleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, ageTitleLabel, NSLayoutAttribute.Trailing, 1f, InterItemHorizontalSpacing),
				NSLayoutConstraint.Create (hobbiesTitleLabel, NSLayoutAttribute.FirstBaseline, NSLayoutRelation.Equal, ageTitleLabel, NSLayoutAttribute.FirstBaseline, 1f, 0f),
				NSLayoutConstraint.Create (hobbiesValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, ageValueLabel, NSLayoutAttribute.Trailing, 1f, InterItemHorizontalSpacing),
				NSLayoutConstraint.Create (hobbiesValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, hobbiesTitleLabel, NSLayoutAttribute.Leading, 1f, 0f),
				NSLayoutConstraint.Create (hobbiesValueLabel, NSLayoutAttribute.FirstBaseline, NSLayoutRelation.Equal, ageValueLabel, NSLayoutAttribute.FirstBaseline, 1f, 0f),
				NSLayoutConstraint.Create (hobbiesTitleLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.LessThanOrEqual, this, NSLayoutAttribute.Trailing, 1f, -HorizontalPadding),
				NSLayoutConstraint.Create (hobbiesValueLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.LessThanOrEqual, this, NSLayoutAttribute.Trailing, 1f, -HorizontalPadding)
			});

			// Position the elevator pitch below the age and the hobbies
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (elevatorPitchTitleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ageTitleLabel, NSLayoutAttribute.Leading, 1f, 0f),
				NSLayoutConstraint.Create (elevatorPitchTitleLabel, NSLayoutAttribute.Top, NSLayoutRelation.GreaterThanOrEqual, ageValueLabel, NSLayoutAttribute.Bottom, 1f, InterItemVerticalSpacing),
				NSLayoutConstraint.Create (elevatorPitchTitleLabel, NSLayoutAttribute.Top, NSLayoutRelation.GreaterThanOrEqual, hobbiesValueLabel, NSLayoutAttribute.Bottom, 1f, InterItemVerticalSpacing),
				NSLayoutConstraint.Create (elevatorPitchTitleLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, this, NSLayoutAttribute.Trailing, 1f, -HorizontalPadding),
				NSLayoutConstraint.Create (elevatorPitchValueLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, elevatorPitchTitleLabel, NSLayoutAttribute.Bottom, 1f, TitleValueSpacing),
				NSLayoutConstraint.Create (elevatorPitchValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, elevatorPitchTitleLabel, NSLayoutAttribute.Leading, 1f, 0f),
				NSLayoutConstraint.Create (elevatorPitchValueLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, this, NSLayoutAttribute.Trailing, 1f, -HorizontalPadding),
				NSLayoutConstraint.Create (elevatorPitchValueLabel, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1f, -VerticalPadding)
			});
			AddConstraints (constraints.ToArray ());
		}

		public void Update(Person person)
		{
			if (person == null)
				throw new ArgumentNullException ("person");

			photo.Image = person.Photo;
			UpdatePhotoConstraint ();

			ageValueLabel.Text = NSNumberFormatter.LocalizedStringFromNumbernumberStyle (NSNumber.FromInt32 (person.Age), NSNumberFormatterStyle.Decimal);
			hobbiesValueLabel.Text = person.Hobbies;
			elevatorPitchValueLabel.Text = person.ElevatorPitch;
		}

		void UpdatePhotoConstraint()
		{
			CGSize size = photo.Image.Size;
			nfloat ratio = size.Height / size.Width;
			photoAspectRatioConstraint.Constant = ratio * PhotoWidth;
		}
	}
}

