using System;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace HelloGoodbye
{
	public class MatchesViewController : PhotoBackgroundViewController
	{
		const float HelloGoodbyeVerticalMargin = 5f;
		const float SwipeAnimationDuration = 0.5f;
		const float ZoomAnimationDuration = 0.3f;
		const float FadeAnimationDuration = 0.3f;

		CardView cardView;
		UIView swipeInstructionsView;
		UIView allMatchesViewedExplanatoryView;

		NSLayoutConstraint[] cardViewVerticalConstraints;

		List<Person> matches;
		int currentMatchIndex;

		UIAccessibilityCustomAction helloAction;
		UIAccessibilityCustomAction goodbyeAction;

		Person CurrentMatch {
			get {
				return currentMatchIndex < matches.Count ? matches [currentMatchIndex] : null;
			}
		}

		public MatchesViewController ()
		{
			string path = NSBundle.MainBundle.PathForResource ("matches", "plist");
			NSArray serializedMatches = NSArray.FromFile (path);

			matches = new List<Person> ();

			for (nuint i = 0; i < serializedMatches.Count; i++) {
				var sMatch = serializedMatches.GetItem<NSDictionary> (i);
				Person match = Person.PersonFromDictionary (sMatch);
				matches.Add (match);
			}

			Title = "Matches".LocalizedString("Title of the matches page");

			BackgroundImage = UIImage.FromBundle ("dessert.jpg");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UIView containerView = View;
			var constraints = new List<NSLayoutConstraint> ();

			// Show instructions for how to say hello and goodbye
			swipeInstructionsView = AddSwipeInstructionsToContainerView (containerView, constraints);

			// Add a dummy view to center the card between the explanatory view and the bottom layout guide
			UIView dummyView = AddDummyViewToContainerView (containerView, swipeInstructionsView, BottomLayoutGuide, constraints);

			// Create and add the card
			CardView cardView = AddCardViewToView (containerView);

			// Define the vertical positioning of the card
			// These constraints will be removed when the card animates off screen
			cardViewVerticalConstraints = new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (cardView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, dummyView, NSLayoutAttribute.CenterY, 1f, 0f),
				NSLayoutConstraint.Create (cardView, NSLayoutAttribute.Top, NSLayoutRelation.GreaterThanOrEqual, swipeInstructionsView, NSLayoutAttribute.Bottom, 1f, HelloGoodbyeVerticalMargin)
			};
			constraints.AddRange (cardViewVerticalConstraints);

			// Ensure that the card is centered horizontally within the container view, and doesn't exceed its width
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (cardView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.CenterX, 1f, 0f),
				NSLayoutConstraint.Create (cardView, NSLayoutAttribute.Left, NSLayoutRelation.GreaterThanOrEqual, containerView, NSLayoutAttribute.Left, 1f, 0f),
				NSLayoutConstraint.Create (cardView, NSLayoutAttribute.Right, NSLayoutRelation.LessThanOrEqual, containerView, NSLayoutAttribute.Right, 1f, 0f),
			});

			// When the matches run out, we'll show this message
			allMatchesViewedExplanatoryView = AddAllMatchesViewExplanatoryViewToContainerView (containerView, constraints);
			containerView.AddConstraints (constraints.ToArray());
		}

		UIView AddDummyViewToContainerView(UIView containerView, INativeObject topItem, INativeObject bottomItem, List<NSLayoutConstraint> constraints)
		{
			UIView dummyView = new UIView{
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			containerView.AddSubview (dummyView);

			// The horizontal layout of the dummy view does not matter, but for completeness, we give it a width of 0 and center it horizontally.
			constraints.AddRange (new NSLayoutConstraint[] {
				NSLayoutConstraint.Create (dummyView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 0f, 0f),
				NSLayoutConstraint.Create (dummyView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.CenterX, 1f, 0f),
				NSLayoutConstraint.Create (dummyView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topItem, NSLayoutAttribute.Bottom, 1f, 0f),
				NSLayoutConstraint.Create (dummyView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bottomItem, NSLayoutAttribute.Top, 1f, 0f)
			});

			return dummyView;
		}

		CardView AddCardViewToView(UIView containerView)
		{
			CardView cv = new CardView ();
			cv.Update (CurrentMatch);
			cv.TranslatesAutoresizingMaskIntoConstraints = false;
			this.cardView = cv;
			containerView.AddSubview (cv);

			UISwipeGestureRecognizer swipeUpRecognizer = new UISwipeGestureRecognizer(HandleSwipeUp);
			swipeUpRecognizer.Direction = UISwipeGestureRecognizerDirection.Up;
			cv.AddGestureRecognizer (swipeUpRecognizer);

			UISwipeGestureRecognizer swipeDownRecognizer = new UISwipeGestureRecognizer (HandleSwipeDown);
			swipeDownRecognizer.Direction = UISwipeGestureRecognizerDirection.Down;
			cv.AddGestureRecognizer (swipeDownRecognizer);

			string sayHelloName = "Say hello".LocalizedString (@"Accessibility action to say hello");
			helloAction = new UIAccessibilityCustomAction (sayHelloName, SayHello);

			string sayGoodbyeName = "Say goodbye".LocalizedString ("Accessibility action to say goodbye");
			goodbyeAction = new UIAccessibilityCustomAction (sayGoodbyeName, SayGoodbye);

			UIView[] elements = NSArray.FromArray<UIView> ((NSArray)cv.GetAccessibilityElements ());
			foreach (UIView element in elements)
				element.AccessibilityCustomActions = new UIAccessibilityCustomAction[] { helloAction, goodbyeAction };

			return cv;
		}

		void HandleSwipeUp(UISwipeGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State == UIGestureRecognizerState.Recognized)
				SayHello (helloAction);
		}

		void HandleSwipeDown(UISwipeGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State == UIGestureRecognizerState.Recognized)
				SayGoodbye (goodbyeAction);
		}

		UIView AddAllMatchesViewExplanatoryViewToContainerView(UIView containerView, List<NSLayoutConstraint> constraints)
		{
			UIView overlayView = AddOverlayViewToContainerView (containerView);

			// Start out hidden
			// This view will become visible once all matches have been viewed
			overlayView.Alpha = 0f;

			UILabel label = StyleUtilities.CreateStandardLabel ();
			label.Font = StyleUtilities.LargeFont;
			label.Text = "Stay tuned for more matches!".LocalizedString ("Shown when all matches have been viewed");
			overlayView.AddSubview (label);

			// Center the overlay view
			constraints.Add (NSLayoutConstraint.Create (overlayView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.CenterX, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (overlayView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.CenterY, 1f, 0f));

			// Position the label in the overlay view
			constraints.Add (NSLayoutConstraint.Create (label, NSLayoutAttribute.Top, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Top, 1f, StyleUtilities.ContentVerticalMargin));
			constraints.Add (NSLayoutConstraint.Create (label, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Bottom, 1f, -1 * StyleUtilities.ContentVerticalMargin));
			constraints.Add (NSLayoutConstraint.Create (label, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Leading, 1f, StyleUtilities.ContentHorizontalMargin));
			constraints.Add (NSLayoutConstraint.Create (label, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Trailing, 1f, -1 * StyleUtilities.ContentHorizontalMargin));

			return overlayView;
		}

		bool SayHello(UIAccessibilityCustomAction customAction)
		{
			AnimateCardsForHello (true);
			return true;
		}

		bool SayGoodbye(UIAccessibilityCustomAction customAction)
		{
			AnimateCardsForHello (false);
			return true;
		}

		void AnimateCardsForHello(bool forHello)
		{
			AnimateCardOffScreenToTop (forHello, () => {
				currentMatchIndex++;
				Person nextMatch = CurrentMatch;
				if (nextMatch != null) {
					// Show the next match's profile in the card
					cardView.Update (nextMatch);

					// Ensure that the view's layout is up to date before we animate it
					View.LayoutIfNeeded ();

					if (UIAccessibility.IsReduceMotionEnabled) {
						// Fade the card into view
						FadeCardIntoView ();
					} else {
						// Zoom the new card from a tiny point into full view
						ZoomCardIntoView ();
					}
				} else {
					// Hide the card
					cardView.Hidden = true;

					// Fade in the "Stay tuned for more matches" blurb
					UIView.Animate (FadeAnimationDuration, () => {
						swipeInstructionsView.Alpha = 0f;
						allMatchesViewedExplanatoryView.Alpha = 1f;
					});
				}

				UIAccessibility.PostNotification (UIAccessibilityPostNotification.LayoutChanged, null);
			});
		}

		void FadeCardIntoView()
		{
			cardView.Alpha = 0f;
			UIView.Animate (FadeAnimationDuration, () => {
				cardView.Alpha = 1f;
			});
		}

		void ZoomCardIntoView()
		{
			cardView.Transform = CGAffineTransform.MakeScale(0f, 0f);
			UIView.Animate (ZoomAnimationDuration, () => {
				cardView.Transform = CGAffineTransform.MakeIdentity ();
			});
		}

		void AnimateCardOffScreenToTop(bool toTop, Action completion)
		{
			NSLayoutConstraint offScreenConstraint = null;
			if (toTop)
				offScreenConstraint = NSLayoutConstraint.Create (cardView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1f, 0f);
			else
				offScreenConstraint = NSLayoutConstraint.Create (cardView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1f, 0f);

			View.LayoutIfNeeded ();

			UIView.Animate (SwipeAnimationDuration, () => {
				// Slide the card off screen
				View.RemoveConstraints (cardViewVerticalConstraints);
				View.AddConstraint (offScreenConstraint);
				View.LayoutIfNeeded ();
			}, () => {
				// Bring the card back into view
				View.RemoveConstraint (offScreenConstraint);
				View.AddConstraints (cardViewVerticalConstraints);

				if (completion != null)
					completion ();
			});
		}

		UIView AddSwipeInstructionsToContainerView(UIView containerView, List<NSLayoutConstraint>constraints)
		{
			UIView overlayView = AddOverlayViewToContainerView (containerView);

			UILabel swipeInstructionsLabel = StyleUtilities.CreateStandardLabel ();
			swipeInstructionsLabel.Font = StyleUtilities.LargeFont;
			overlayView.AddSubview (swipeInstructionsLabel);
			swipeInstructionsLabel.Text = "Swipe ↑ to say \"Hello!\"\nSwipe ↓ to say \"Goodbye...\"".LocalizedString ("Instructions for the Matches page");
			swipeInstructionsLabel.AccessibilityLabel = "Swipe up to say \"Hello!\"\nSwipe down to say \"Goodbye\"".LocalizedString ("Accessibility instructions for the Matches page");

			float overlayMargin = StyleUtilities.OverlayMargin;
			NSLayoutConstraint topMarginConstraint = NSLayoutConstraint.Create(overlayView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, TopLayoutGuide, NSLayoutAttribute.Bottom, 1f, overlayMargin);
			float priority = (int)UILayoutPriority.Required - 1;
			topMarginConstraint.Priority = priority;
			constraints.Add (topMarginConstraint);

			// Position the label inside the overlay view
			constraints.Add (NSLayoutConstraint.Create (swipeInstructionsLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.Top, 1f, HelloGoodbyeVerticalMargin));
			constraints.Add (NSLayoutConstraint.Create (swipeInstructionsLabel, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, overlayView, NSLayoutAttribute.CenterX, 1f, 0f));
			constraints.Add (NSLayoutConstraint.Create (overlayView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, swipeInstructionsLabel, NSLayoutAttribute.Bottom, 1f, HelloGoodbyeVerticalMargin));

			// Center the overlay view horizontally
			constraints.Add (NSLayoutConstraint.Create (overlayView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Left, 1f, overlayMargin));
			constraints.Add (NSLayoutConstraint.Create (overlayView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Right, 1f, -overlayMargin));

			return overlayView;
		}

		UIView AddOverlayViewToContainerView(UIView containerView)
		{
			UIView overlayView = new UIView {
				BackgroundColor = StyleUtilities.OverlayColor,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			overlayView.Layer.CornerRadius = StyleUtilities.OverlayCornerRadius;
			containerView.AddSubview(overlayView);

			return overlayView;
		}
	}
}

