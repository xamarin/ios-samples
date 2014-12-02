using System;
using System.Drawing;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreText;

namespace HelloGoodbye
{
	public class ProfileViewController : PhotoBackgroundViewController
	{
		const float LabelControlMinimumSpacing = 20;
		const float MinimumVerticalSpacingBetweenRows = 20;
		const float PreviewTabMinimumWidth = 80;
		const float PreviewTabHeight = 30;
		const float PreviewTabCornerRadius = 10;
		const float PreviewTabHorizontalPadding = 30;
		const float CardRevealAnimationDuration = 0.3f;

		Person person;
		UILabel ageValueLabel;
		UITextField hobbiesField;
		UITextField elevatorPitchField;
		UIImageView previewTab;
		CardView cardView;
		NSLayoutConstraint cardRevealConstraint;
		bool cardWasRevealedBeforePan;

		bool IsCardRevealed {
			get {
				return cardRevealConstraint.Constant < 0;
			}
		}

		nfloat CardHeight {
			get {
				return cardView.Frame.Height;
			}
		}

		public ProfileViewController ()
		{
			Title = @"Profile".LocalizedString("Title of the profile page");
			BackgroundImage = UIImage.FromBundle ("girl-bg.jpg");

			// Create the model.  If we had a backing service, this model would pull data from the user's account settings.
			person = new Person {
				Photo = UIImage.FromBundle("girl.jpg"),
				Age = 37,
				Hobbies = "Music, swing dance, wine",
				ElevatorPitch = "I can keep a steady beat.",
			};
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UIView containerView = View;
			var constraints = new List<NSLayoutConstraint> ();

			UIView overlayView = AddOverlayViewToView (containerView, constraints);
			UIView[] ageControls = AddAgeControlsToView (overlayView, constraints);
			hobbiesField = AddTextFieldWithName ("Hobbies".LocalizedString ("The user's hobbies"), person.Hobbies, overlayView, ageControls, constraints);
			elevatorPitchField = AddTextFieldWithName ("Elevator Pitch".LocalizedString ("The user's elevator pitch for finding a partner"), person.ElevatorPitch, overlayView, new UIView[] { hobbiesField }, constraints);

			AddCardAndPreviewTab (constraints);
			containerView.AddConstraints (constraints.ToArray());
		}

		UIView AddOverlayViewToView (UIView containerView, List<NSLayoutConstraint> constraints)
		{
			UIView overlayView = new UIView {
				BackgroundColor = StyleUtilities.OverlayColor,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			overlayView.Layer.CornerRadius = StyleUtilities.OverlayCornerRadius;

			containerView.AddSubview (overlayView);

			// Cover the view controller with the overlay, leaving a margin on all sides
			float margin = StyleUtilities.OverlayMargin;
			constraints.Add(NSLayoutConstraint.Create(overlayView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, TopLayoutGuide, NSLayoutAttribute.Bottom, 1f, margin));
			constraints.Add(NSLayoutConstraint.Create(overlayView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, BottomLayoutGuide, NSLayoutAttribute.Bottom, 1f,  -margin));
			constraints.Add (NSLayoutConstraint.Create (overlayView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Left, 1f, margin));
			constraints.Add (NSLayoutConstraint.Create (overlayView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Right, 1f, -margin));
			return overlayView;
		}

		UITextField AddTextFieldWithName (string name, string text, UIView overlayView, IEnumerable<UIView> previousRowItems, List<NSLayoutConstraint> constraints)
		{
			UILabel titleLabel = StyleUtilities.CreateStandardLabel ();
			titleLabel.Text = name;
			overlayView.AddSubview (titleLabel);

			var attributes = new CTStringAttributes ();
			attributes.ForegroundColor = StyleUtilities.DetailOnOverlayPlaceholderColor.CGColor;

			UITextField valueField = new UITextField {
				WeakDelegate = this,
				Font = StyleUtilities.StandardFont,
				TextColor = StyleUtilities.DetailOnOverlayColor,
				Text = text,
				AttributedPlaceholder = new NSAttributedString("Type here...".LocalizedString("Placeholder for profile text fields"), attributes),
			TranslatesAutoresizingMaskIntoConstraints = false
			};
			overlayView.AddSubview (valueField);

			// Ensure sufficient spacing from the row above this one
			foreach (UIView previousRowItem in previousRowItems)
				constraints.Add (NSLayoutConstraint.Create (titleLabel, NSLayoutAttribute.Top, NSLayoutRelation.GreaterThanOrEqual, previousRowItem, NSLayoutAttribute.Bottom, 1f, MinimumVerticalSpacingBetweenRows));

			// Place the title directly above the value
			constraints.Add (NSLayoutConstraint.Create (valueField, NSLayoutAttribute.Top, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.Bottom, 1f, 0f));

			// Position the title and value within the overlay view
			constraints.Add (NSLayoutConstraint.Create (titleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Leading, 1f, StyleUtilities.ContentHorizontalMargin));
			constraints.Add (NSLayoutConstraint.Create (valueField, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.Leading, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (valueField, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Trailing, 1f, -1 * StyleUtilities.ContentHorizontalMargin));

			return valueField;
		}

		UIView[] AddAgeControlsToView(UIView overlayView, List<NSLayoutConstraint> constraints)
		{
			UILabel ageTitleLabel = StyleUtilities.CreateStandardLabel ();
			ageTitleLabel.Text = "Your age".LocalizedString ("The user's age");
			overlayView.AddSubview (ageTitleLabel);

			AgeSlider ageSlider = new AgeSlider {
				Value = person.Age,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			ageSlider.ValueChanged += OnAgeUpdate;
			overlayView.AddSubview (ageSlider);

			// Display the current age next to the slider
			ageValueLabel = AddAgeValueLabelToView (overlayView);
			UpdateAgeValueLabelFromSlider (ageSlider);

//			// Position the age title and value side by side, within the overlay view
			constraints.Add (NSLayoutConstraint.Create (ageTitleLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Top, 1f, StyleUtilities.ContentVerticalMargin));
			constraints.Add (NSLayoutConstraint.Create (ageTitleLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Leading, 1f, StyleUtilities.ContentHorizontalMargin));
			constraints.Add (NSLayoutConstraint.Create (ageSlider, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ageTitleLabel, NSLayoutAttribute.Trailing, 1f, LabelControlMinimumSpacing));
			constraints.Add (NSLayoutConstraint.Create (ageSlider, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, ageTitleLabel, NSLayoutAttribute.CenterY, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (ageValueLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ageSlider, NSLayoutAttribute.Trailing, 1f, LabelControlMinimumSpacing));
			constraints.Add (NSLayoutConstraint.Create (ageValueLabel, NSLayoutAttribute.FirstBaseline, NSLayoutRelation.Equal, ageTitleLabel, NSLayoutAttribute.FirstBaseline, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (ageValueLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Trailing, 1f, -1 * StyleUtilities.ContentHorizontalMargin));

			return new UIView[]{ ageTitleLabel, ageSlider, ageValueLabel };
		}

		UILabel AddAgeValueLabelToView(UIView overlayView)
		{
			UILabel ageValueLabel = StyleUtilities.CreateStandardLabel ();
			ageValueLabel.IsAccessibilityElement = false;
			overlayView.AddSubview (ageValueLabel);

			return ageValueLabel;
		}

		void UpdateAgeValueLabelFromSlider(AgeSlider ageSlider)
		{
			NSNumber number = NSNumber.FromFloat (ageSlider.Value);
			ageValueLabel.Text = NSNumberFormatter.LocalizedStringFromNumbernumberStyle (number, NSNumberFormatterStyle.Decimal);
		}

		void AddCardAndPreviewTab(List<NSLayoutConstraint> constraints)
		{
			previewTab = AddPreviewTab ();
			previewTab.TranslatesAutoresizingMaskIntoConstraints = false;

			PreviewLabel previewLabel = AddPreviewLabel ();
			previewLabel.TranslatesAutoresizingMaskIntoConstraints = false;

			CardView cardView = AddCardView ();
			cardView.TranslatesAutoresizingMaskIntoConstraints = false;

			// Pin the tab to the bottom center of the screen
			cardRevealConstraint = NSLayoutConstraint.Create (previewTab, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1f, 0f);
			constraints.Add (cardRevealConstraint);
			constraints.Add(NSLayoutConstraint.Create(previewTab, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1f, 0f));
			// Center the preview label within the tab
			constraints.Add(NSLayoutConstraint.Create (previewLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, previewTab, NSLayoutAttribute.Leading, 1f, PreviewTabHorizontalPadding));
			constraints.Add (NSLayoutConstraint.Create (previewLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, previewTab, NSLayoutAttribute.Trailing, 1f, -PreviewTabHorizontalPadding));
			constraints.Add (NSLayoutConstraint.Create (previewLabel, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, previewTab, NSLayoutAttribute.CenterY, 1f, 0f));

			// Pin the top of the card to the bottom of the tab
			constraints.Add (NSLayoutConstraint.Create (cardView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, previewTab, NSLayoutAttribute.Bottom, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (cardView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, previewTab, NSLayoutAttribute.CenterX, 1f, 0f));

			// Ensure that the card fits within the view
			constraints.Add (NSLayoutConstraint.Create (cardView, NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, View, NSLayoutAttribute.Width, 1f, 0f));
		}

		UIImageView AddPreviewTab()
		{
			UIImage previewTabBackgroundImage = CreatePreviewTabBackgroundImage ();
			UIImageView previewTab = new UIImageView(previewTabBackgroundImage);
			previewTab.UserInteractionEnabled = true;
			View.AddSubview(previewTab);

			UIPanGestureRecognizer revealGestureRecognizer = new UIPanGestureRecognizer(DidSlidePreviewTab);
			previewTab.AddGestureRecognizer(revealGestureRecognizer);
			return previewTab;
		}

		UIImage CreatePreviewTabBackgroundImage()
		{
			// The preview tab should be flat on the bottom, and have rounded corners on top.
			var size = new SizeF (PreviewTabMinimumWidth, PreviewTabHeight);
			UIGraphics.BeginImageContextWithOptions (size, false, UIScreen.MainScreen.Scale);

			var rect = new RectangleF (0f, 0f, PreviewTabMinimumWidth, PreviewTabHeight);
			var radii = new SizeF (PreviewTabCornerRadius, PreviewTabCornerRadius);
			var roundedTopCornersRect = UIBezierPath.FromRoundedRect (rect, UIRectCorner.TopLeft | UIRectCorner.TopRight, radii);

			StyleUtilities.ForegroundColor.SetColor ();
			roundedTopCornersRect.Fill ();
			UIImage previewTabBackgroundImage = UIGraphics.GetImageFromCurrentImageContext ();

			var caps = new UIEdgeInsets (0f, PreviewTabCornerRadius, 0f, PreviewTabCornerRadius);
			previewTabBackgroundImage = previewTabBackgroundImage.CreateResizableImage (caps);

			UIGraphics.EndImageContext ();
			return previewTabBackgroundImage;
		}

		void DidSlidePreviewTab(UIPanGestureRecognizer gestureRecognizer)
		{
			switch (gestureRecognizer.State) {
				case UIGestureRecognizerState.Began:
					cardWasRevealedBeforePan = IsCardRevealed;
					break;

				case UIGestureRecognizerState.Changed:
					nfloat cardHeight = CardHeight;
					nfloat cardRevealConstant = gestureRecognizer.TranslationInView (View).Y;
					if (cardWasRevealedBeforePan) {
						cardRevealConstant += -1 * cardHeight;
					}
						// Never let the card tab move off screen
					cardRevealConstant = NMath.Min (0, cardRevealConstant);
						// Never let the card have a gap below it
					cardRevealConstant = NMath.Max (-1 * cardHeight, cardRevealConstant);
					cardRevealConstraint.Constant = cardRevealConstant;
					break;

				case UIGestureRecognizerState.Ended:
				// Card was closer to the bottom of the screen
					if (cardRevealConstraint.Constant > (-0.5 * CardHeight))
						DismissCard ();
					else
						RevealCard ();
					break;

				case UIGestureRecognizerState.Cancelled:
					if (cardWasRevealedBeforePan)
						RevealCard ();
					else
						DismissCard ();
					break;
			}
		}

		void RevealCard()
		{
			View.LayoutIfNeeded ();

			UIView.Animate (CardRevealAnimationDuration, () => {
				cardRevealConstraint.Constant = -1 * CardHeight;
				View.LayoutIfNeeded ();
			}, () => {
				UIAccessibility.PostNotification(UIAccessibilityPostNotification.LayoutChanged, null);
			});
		}

		void DismissCard()
		{
			View.LayoutIfNeeded ();

			UIView.Animate (CardRevealAnimationDuration, () => {
				cardRevealConstraint.Constant = 0f;
				View.LayoutIfNeeded ();
			}, () => {
				UIAccessibility.PostNotification (UIAccessibilityPostNotification.LayoutChanged, null);
			});
		}

		PreviewLabel AddPreviewLabel()
		{
			PreviewLabel previewLabel = new PreviewLabel();
			previewLabel.ActivatePreviewLabel += DidActivatePreviewLabel;
			View.AddSubview(previewLabel);

			return previewLabel;
		}

		void DidActivatePreviewLabel (object sender, EventArgs e)
		{
			if (IsCardRevealed)
				DismissCard ();
			else
				RevealCard ();
		}

		CardView AddCardView()
		{
			cardView = new CardView();
			cardView.Update (person);
			View.AddSubview (cardView);

			return cardView;
		}

		void OnAgeUpdate (object sender, EventArgs e)
		{
			var ageSlider = (AgeSlider)sender;
			// Turn the value into a valid age
			int age = (int)Math.Round(ageSlider.Value);
			ageSlider.Value = age;

			// Display the updated age next to the slider
			UpdateAgeValueLabelFromSlider (ageSlider);

			// Update the model
			person.Age = age;

			// Update the card view with the new data
			cardView.Update (person);
		}

		#region UITextFieldDelegate

		[Export("textFieldDidBeginEditing:")]
		public void TextFieldDidBeginEditing(UITextField textField)
		{
			// Add a Done button so that the user can dismiss the keyboard easily
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, DoneButtonPressed);
		}

		[Export("textFieldDidEndEditing:")]
		public void TextFieldDidEndEditing(UITextField textField)
		{
			// Remove the Done button
			NavigationItem.RightBarButtonItem = null;

			// Update the model
			if (textField == hobbiesField)
				person.Hobbies = textField.Text;
			else if (textField == elevatorPitchField)
				person.ElevatorPitch = textField.Text;

			// Update the card view with the new data
			cardView.Update(person);
		}
		#endregion

		void DoneButtonPressed(object sender, EventArgs args)
		{
			// End editing on whichever text field is first responder
			hobbiesField.ResignFirstResponder ();
			elevatorPitchField.ResignFirstResponder ();
		}
	}
}

