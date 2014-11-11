using System;
using CoreGraphics;
using CoreFoundation;
using UIKit;

namespace Example_CoreAnimation.Screens.iPad.ViewTransitions
{
	public class Controller : UIViewController, IDetailView
	{
		public event EventHandler ContentsButtonClicked;

		private TransitionViewController transitionViewController;
		private BackTransitionViewController backViewController;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.White;
			var mainFrame = new CGRect (0f, 44f, View.Frame.Width, View.Frame.Height - 44f);
			
			transitionViewController = new TransitionViewController ();
			transitionViewController.View.Frame = mainFrame;
			backViewController = new BackTransitionViewController ();
			backViewController.View.Frame = mainFrame;

			View.AddSubview (transitionViewController.View);

			transitionViewController.SetToolbarVisibility (InterfaceOrientation);
			transitionViewController.TransitionClicked += (s, e) => {
				UIView.Transition(transitionViewController.View, backViewController.View, 0.75,
					transitionViewController.SelectedTransition, null);
			};

			transitionViewController.ContentsClicked += () => {
				if (ContentsButtonClicked != null) {
					ContentsButtonClicked (null, null);
				}
			};
			
			backViewController.BackClicked += (s, e) => {
				UIView.Transition(backViewController.View, transitionViewController.View, 0.75,
					transitionViewController.SelectedTransition, null);
			};
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			transitionViewController.SetToolbarVisibility (toInterfaceOrientation);
			base.WillRotate (toInterfaceOrientation, duration);
		}

		public override void ViewWillAppear (bool animated)
		{
			transitionViewController.SetToolbarVisibility (InterfaceOrientation);
			base.ViewWillAppear (animated);
		}
	}
}

