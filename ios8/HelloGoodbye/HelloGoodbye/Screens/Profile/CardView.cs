using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace HelloGoodbye
{
	public class CardView : UIView, IUIAccessibilityContainer
	{
		private const float PhotoWidth = 80;
		private const float BorderWidth = 5;
		private const float HorizontalPadding = 20;
		private const float VerticalPadding = 20;
		private const float InterItemHorizontalSpacing = 30;
		private const float InterItemVerticalSpacing = 10;
		private const float TitleValueSpacing = 0;

		private UIView _backgroundView;
		private UIImageView _photo;
		private UILabel _ageTitleLabel;
		private UILabel _ageValueLabel;
		private UILabel _hobbiesTitleLabel;
		private UILabel _hobbiesValueLabel;
		private UILabel _elevatorPitchTitleLabel;
		private UILabel _elevatorPitchValueLabel;
		private NSLayoutConstraint _photoAspectRatioConstraint;

		public CardView ()
		{
			BackgroundColor = StyleUtilities.CardBorderColor;

			_backgroundView = new UIView {
				BackgroundColor = StyleUtilities.CardBackgroundColor,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			AddSubview (_backgroundView);

			AddProfileViews ();
			AddAllConstraints ();
		}

		private void AddProfileViews()
		{
			_photo = new UIImageView {
				IsAccessibilityElement = true,
				AccessibilityLabel = "Profile photo".LocalizedString ("Accessibility label for profile photo"),
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			AddSubview (_photo);

			_ageTitleLabel = StyleUtilities.CreateStandardLabel ();
			_ageTitleLabel.Text = "Age".LocalizedString("Age of the user");
			AddSubview(_ageTitleLabel);

			_ageValueLabel = StyleUtilities.CreateDetailLabel ();
			AddSubview (_ageValueLabel);

			_hobbiesTitleLabel = StyleUtilities.CreateStandardLabel ();
			_hobbiesTitleLabel.Text = "Hobbies".LocalizedString ("The user's hobbies");
			AddSubview (_hobbiesTitleLabel);

			_hobbiesValueLabel = StyleUtilities.CreateDetailLabel ();
			AddSubview (_hobbiesValueLabel);

			_elevatorPitchTitleLabel = StyleUtilities.CreateStandardLabel ();
			_elevatorPitchTitleLabel.Text = "Elevator Pitch".LocalizedString ("The user's elevator pitch for finding a partner");
			AddSubview (_elevatorPitchTitleLabel);

			_elevatorPitchValueLabel = StyleUtilities.CreateDetailLabel ();
			AddSubview (_elevatorPitchValueLabel);

			this.SetAccessibilityElements (NSArray.FromNSObjects (
				_photo,
				_ageTitleLabel,
				_ageValueLabel,
				_hobbiesTitleLabel,
				_hobbiesValueLabel,
				_elevatorPitchTitleLabel,
				_elevatorPitchValueLabel
			));
		}

		private void AddAllConstraints()
		{
			var constraints = new List<NSLayoutConstraint> ();

			// Fill the card with the background view (leaving a border around it)
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (_backgroundView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1f, BorderWidth),
				NSLayoutConstraint.Create (_backgroundView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1f, BorderWidth),
				NSLayoutConstraint.Create (this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, _backgroundView, NSLayoutAttribute.Trailing, 1f, BorderWidth),
				NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _backgroundView, NSLayoutAttribute.Bottom, 1f, BorderWidth)
			});

			// Position the photo
			// The constant for the aspect ratio constraint will be updated once a photo is set
			_photoAspectRatioConstraint = NSLayoutConstraint.Create (_photo, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0f, 0f);
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (_photo, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1f, HorizontalPadding),
				NSLayoutConstraint.Create (_photo, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1f, VerticalPadding),
				NSLayoutConstraint.Create (_photo, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0f, PhotoWidth),
				NSLayoutConstraint.Create (_photo, NSLayoutAttribute.Bottom, NSLayoutRelation.LessThanOrEqual, this, NSLayoutAttribute.Bottom, 1f, -VerticalPadding),
				_photoAspectRatioConstraint
			});

			// Position the age to the right of the photo, with some spacing
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (_ageTitleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _photo, NSLayoutAttribute.Trailing, 1f, InterItemHorizontalSpacing),
				NSLayoutConstraint.Create (_ageTitleLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _photo, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (_ageValueLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _ageTitleLabel, NSLayoutAttribute.Bottom, 1f, TitleValueSpacing),
				NSLayoutConstraint.Create (_ageValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _ageTitleLabel, NSLayoutAttribute.Leading, 1f, 0f)
			});

			// Position the hobbies to the right of the age
			constraints.AddRange (new NSLayoutConstraint [] {
				NSLayoutConstraint.Create (_hobbiesTitleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, _ageTitleLabel, NSLayoutAttribute.Trailing, 1f, InterItemHorizontalSpacing),
				NSLayoutConstraint.Create (_hobbiesTitleLabel, NSLayoutAttribute.FirstBaseline, NSLayoutRelation.Equal, _ageTitleLabel, NSLayoutAttribute.FirstBaseline, 1f, 0f),
				NSLayoutConstraint.Create (_hobbiesValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, _ageValueLabel, NSLayoutAttribute.Trailing, 1f, InterItemHorizontalSpacing),
				NSLayoutConstraint.Create (_hobbiesValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _hobbiesTitleLabel, NSLayoutAttribute.Leading, 1f, 0f),
				NSLayoutConstraint.Create (_hobbiesValueLabel, NSLayoutAttribute.FirstBaseline, NSLayoutRelation.Equal, _ageValueLabel, NSLayoutAttribute.FirstBaseline, 1f, 0f),
				NSLayoutConstraint.Create (_hobbiesTitleLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.LessThanOrEqual, this, NSLayoutAttribute.Trailing, 1f, -HorizontalPadding),
				NSLayoutConstraint.Create (_hobbiesValueLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.LessThanOrEqual, this, NSLayoutAttribute.Trailing, 1f, -HorizontalPadding)
			});

			// Position the elevator pitch below the age and the hobbies
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (_elevatorPitchTitleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _ageTitleLabel, NSLayoutAttribute.Leading, 1f, 0f),
				NSLayoutConstraint.Create (_elevatorPitchTitleLabel, NSLayoutAttribute.Top, NSLayoutRelation.GreaterThanOrEqual, _ageValueLabel, NSLayoutAttribute.Bottom, 1f, InterItemVerticalSpacing),
				NSLayoutConstraint.Create (_elevatorPitchTitleLabel, NSLayoutAttribute.Top, NSLayoutRelation.GreaterThanOrEqual, _hobbiesValueLabel, NSLayoutAttribute.Bottom, 1f, InterItemVerticalSpacing),
				NSLayoutConstraint.Create (_elevatorPitchTitleLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, this, NSLayoutAttribute.Trailing, 1f, -HorizontalPadding),
				NSLayoutConstraint.Create (_elevatorPitchValueLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _elevatorPitchTitleLabel, NSLayoutAttribute.Bottom, 1f, TitleValueSpacing),
				NSLayoutConstraint.Create (_elevatorPitchValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, _elevatorPitchTitleLabel, NSLayoutAttribute.Leading, 1f, 0f),
				NSLayoutConstraint.Create (_elevatorPitchValueLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, this, NSLayoutAttribute.Trailing, 1f, -HorizontalPadding),
				NSLayoutConstraint.Create (_elevatorPitchValueLabel, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1f, -VerticalPadding)
			});
			AddConstraints (constraints.ToArray ());
		}

		public void Update(Person person)
		{
			if (person == null)
				throw new ArgumentNullException ("person");

			_photo.Image = person.Photo;
			UpdatePhotoConstraint ();

			_ageValueLabel.Text = NSNumberFormatter.LocalizedStringFromNumbernumberStyle (NSNumber.FromInt32 (person.Age), NSNumberFormatterStyle.Decimal);
			_hobbiesValueLabel.Text = person.Hobbies;
			_elevatorPitchValueLabel.Text = person.ElevatorPitch;
		}

		private void UpdatePhotoConstraint()
		{
			SizeF size = _photo.Image.Size;
			float ratio = size.Height / size.Width;
			_photoAspectRatioConstraint.Constant = ratio * PhotoWidth;
		}
	}
}

