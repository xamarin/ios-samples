using System;
using System.Drawing;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;

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
			var mainFrame = new RectangleF (0f, 44f, View.Frame.Width, View.Frame.Height - 44f);
			
			transitionViewController = new TransitionViewController ();
			transitionViewController.View.Frame = mainFrame;
			backViewController = new BackTransitionViewController ();
			backViewController.View.Frame = mainFrame;

			View.AddSubview (transitionViewController.View);

			transitionViewController.SetToolbarVisibility (InterfaceOrientation);
			transitionViewController.TransitionClicked += (s, e) => {
				UIView.Animate (1, 0, transitionViewController.SelectedTransition, () => {
					transitionViewController.View.RemoveFromSuperview ();
					View.AddSubview (backViewController.View);
				}, null);
				UIView.BeginAnimations ("ViewChange");
			};

			transitionViewController.ContentsClicked += () => {
				if (ContentsButtonClicked != null) {
					ContentsButtonClicked (null, null);
				}
			};
			
			backViewController.BackClicked += (s, e) => {
				UIView.Animate (.75, 0, transitionViewController.SelectedTransition, () => {
					backViewController.View.RemoveFromSuperview ();
					View.AddSubview (transitionViewController.View);
				}, null);
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

