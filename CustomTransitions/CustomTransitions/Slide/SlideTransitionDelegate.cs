using System;

using Foundation;
using UIKit;

namespace CustomTransitions
{
	[Register ("SlideTransitionDelegate")]
	public class SlideTransitionDelegate : NSObject, IUITabBarControllerDelegate
	{
		public SlideTransitionDelegate (IntPtr handle) : base(handle)
		{
		}

		UITabBarController tabBarController;

		[Outlet]
		public UITabBarController TabBarController
		{
			get {
				return tabBarController;
			}
			set {
				if (tabBarController == null) {
					tabBarController = value;
					tabBarController.Delegate = this;
					tabBarController.View.AddGestureRecognizer (PanGestureRecognizer);
					return;
				}

				if (value != tabBarController) {
					tabBarController.View.RemoveGestureRecognizer (PanGestureRecognizer);

					if (tabBarController.Delegate == this)
						tabBarController.Delegate = null;

					tabBarController = value;
					tabBarController.Delegate = this;
					tabBarController.View.AddGestureRecognizer (PanGestureRecognizer);
				}
			}
		}

		UIPanGestureRecognizer panGestureRecognizer;

		UIPanGestureRecognizer PanGestureRecognizer {
			get {
				panGestureRecognizer = panGestureRecognizer ?? new UIPanGestureRecognizer (() => PanGestureRecognizerDidPan (panGestureRecognizer));
				return panGestureRecognizer;
			} set {
				panGestureRecognizer = value;
			}
		}

		void PanGestureRecognizerDidPan (UIPanGestureRecognizer sender)
		{
			if (tabBarController.GetTransitionCoordinator () != null)
				return;

			if (sender.State == UIGestureRecognizerState.Began || sender.State == UIGestureRecognizerState.Changed)
				BeginInteractiveTransitionIfPossible (sender);
		}

		void BeginInteractiveTransitionIfPossible (UIPanGestureRecognizer sender)
		{
			var translation = sender.TranslationInView (tabBarController.View);

			if (translation.X > 0f && tabBarController.SelectedIndex > 0) {
				tabBarController.SelectedIndex --;
			} else if (translation.X < 0f && (tabBarController.SelectedIndex + 1) < tabBarController.ViewControllers.Length) {
				tabBarController.SelectedIndex ++;
			} else if (!(Math.Abs (translation.X) < nfloat.Epsilon && Math.Abs (translation.Y) < nfloat.Epsilon)) {
				sender.Enabled = false;
				sender.Enabled = true;
			}

			var cooordinator = tabBarController.GetTransitionCoordinator ();

			if (cooordinator == null)
				return;

			// TODO nullable first argument?
			cooordinator.AnimateAlongsideTransition ((_) => { }, (context) => {
				if (context.IsCancelled && sender.State == UIGestureRecognizerState.Changed)
					BeginInteractiveTransitionIfPossible (sender);
			});
		}

		[Export ("tabBarController:animationControllerForTransitionFromViewController:toViewController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForTransition (UITabBarController tabBarController, UIViewController fromViewController, UIViewController toViewController)
		{
			UIViewController[] viewControllers = tabBarController.ViewControllers;

			if (Array.IndexOf (viewControllers, toViewController) > Array.IndexOf (viewControllers, fromViewController))
				return new SlideTransitionAnimator (UIRectEdge.Left);

			return new SlideTransitionAnimator (UIRectEdge.Right);
		}

		[Export ("tabBarController:interactionControllerForAnimationController:")]
		public IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController (UITabBarController tabBarController, IUIViewControllerAnimatedTransitioning animationController)
		{
			UIViewController[] viewControllers = tabBarController.ViewControllers;

			if (PanGestureRecognizer.State == UIGestureRecognizerState.Began || PanGestureRecognizer.State == UIGestureRecognizerState.Changed)
				return new SlideTransitionInteractionController (PanGestureRecognizer);

			return null;
		}
	}
}