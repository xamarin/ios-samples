using System;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace HelloGoodbye
{
	public class StartViewController : PhotoBackgroundViewController
	{
		private const float ButtonToButtonVerticalSpacing = 10;
		private const float LogoPadding = 30;

		public StartViewController ()
		{
			Title = "HelloGoodbye".LocalizedString("Title of the start page");
			BackgroundImage = UIImage.FromBundle ("couple.jpg");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UIView containerView = View;
			UIView logoOverlayView = new UIView {
				BackgroundColor = StyleUtilities.OverlayColor,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			logoOverlayView.Layer.CornerRadius = StyleUtilities.OverlayCornerRadius;

			UIImageView logo = new UIImageView (UIImage.FromBundle ("logo")) {
				IsAccessibilityElement = true,
				AccessibilityLabel = "Hello goodbye, meet your match".LocalizedString ("Logo description"),
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			UIButton profileButton = CreateButton ("Profile", "Title of the profile page", ShowProfile);
			UIButton matchesButton = CreateButton ("Matches", "Title of the matches page", ShowMatches);

			containerView.AddSubview (logoOverlayView);
			containerView.AddSubview (logo);
			containerView.AddSubview (profileButton);
			containerView.AddSubview (matchesButton);

			var constraints = new List<NSLayoutConstraint>();

			// Use dummy views space the top of the view, the logo, the buttons, and the bottom of the view evenly apart
			UIView topDummyView = AddDummyViewToContainerView (containerView, TopLayoutGuide, logoOverlayView, constraints);
			UIView middleDummyView = AddDummyViewToContainerView (containerView, logoOverlayView, profileButton, constraints);
			UIView bottomDummyView = AddDummyViewToContainerView (containerView, matchesButton, BottomLayoutGuide, constraints);
			constraints.Add (NSLayoutConstraint.Create (topDummyView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, middleDummyView, NSLayoutAttribute.Height, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (middleDummyView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, bottomDummyView, NSLayoutAttribute.Height, 1f, 0f));

			// Position the logo
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (logoOverlayView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topDummyView, NSLayoutAttribute.Bottom, 1f, 0f),
				NSLayoutConstraint.Create (logoOverlayView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.CenterX, 1f, 0f),
				NSLayoutConstraint.Create (logoOverlayView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, middleDummyView, NSLayoutAttribute.Top, 1f, 0f),
				NSLayoutConstraint.Create (logo, NSLayoutAttribute.Top, NSLayoutRelation.Equal, logoOverlayView, NSLayoutAttribute.Top, 1f, LogoPadding),
				NSLayoutConstraint.Create (logo, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, logoOverlayView, NSLayoutAttribute.Bottom, 1f, -LogoPadding),
				NSLayoutConstraint.Create (logo, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, logoOverlayView, NSLayoutAttribute.Leading, 1f, LogoPadding),
				NSLayoutConstraint.Create (logo, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, logoOverlayView, NSLayoutAttribute.Trailing, 1f, -LogoPadding)
			});

			// Position the profile button
			constraints.Add (NSLayoutConstraint.Create (profileButton, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.CenterX, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (profileButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, middleDummyView, NSLayoutAttribute.Bottom, 1f, 0f));

			// Put the matches button below the profile button
			constraints.Add (NSLayoutConstraint.Create (matchesButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, profileButton, NSLayoutAttribute.Bottom, 1f, ButtonToButtonVerticalSpacing));
			constraints.Add (NSLayoutConstraint.Create (matchesButton, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bottomDummyView, NSLayoutAttribute.Top, 1f, 0f));

			// Align the left and right edges of the two buttons and the logo
			constraints.Add (NSLayoutConstraint.Create (matchesButton, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, profileButton, NSLayoutAttribute.Leading, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (matchesButton, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, profileButton, NSLayoutAttribute.Trailing, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (matchesButton, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, logoOverlayView, NSLayoutAttribute.Leading, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (matchesButton, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, logoOverlayView, NSLayoutAttribute.Trailing, 1f, 0f));

			containerView.AddConstraints (constraints.ToArray());
		}

		private UIView AddDummyViewToContainerView(UIView containerView, INativeObject topItem, INativeObject bottomItem, List<NSLayoutConstraint> constraints)
		{
			UIView dummyView = new UIView {
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			containerView.AddSubview (dummyView);

			// The horizontal layout of the dummy view does not matter, but for completeness, we give it a width of 0 and center it horizontally.
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create(dummyView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0f, 0f),
				NSLayoutConstraint.Create(dummyView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.CenterX, 1f, 0f),
				NSLayoutConstraint.Create(dummyView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topItem, NSLayoutAttribute.Bottom,  1f, 0f),
				NSLayoutConstraint.Create(dummyView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bottomItem, NSLayoutAttribute.Top, 1f, 0f)
			});

			return dummyView;
		}

		private UIButton CreateButton(string titleKey, string titleComment, EventHandler handler)
		{
			UIButton button = StyleUtilities.CreateOverlayRoundedRectButton ();
			button.SetTitle (titleKey.LocalizedString (titleComment), UIControlState.Normal);
			button.TouchUpInside += handler;

			return button;
		}

		private void ShowProfile (object sender, EventArgs e)
		{
			ProfileViewController profileViewController = new ProfileViewController ();
			NavigationController.PushViewController (profileViewController, true);
		}

		private void ShowMatches (object sender, EventArgs e)
		{
			MatchesViewController matchesViewController = new MatchesViewController ();
			NavigationController.PushViewController (matchesViewController, true);
		}
	}
}

