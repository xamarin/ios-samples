using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace CustomTransitions
{
	public partial class SlideTransitionDelegate : NSObject, IUITabBarControllerDelegate
    {
        public SlideTransitionDelegate (IntPtr handle) : base(handle)
		{
		}


		UITabBarController _tabBarController;
		[Outlet]
		public UITabBarController tabBarController
		{
			get { return _tabBarController; }
			set
			{
				if (_tabBarController == null) {
					_tabBarController = value;
					_tabBarController.Delegate = this;
					_tabBarController.View.AddGestureRecognizer(panGestureRecognizer);
					return;
				}

				if (value != _tabBarController)
				{
					_tabBarController.View.RemoveGestureRecognizer(panGestureRecognizer);

					if (_tabBarController.Delegate == this) _tabBarController.Delegate = null;


					_tabBarController = value;
					_tabBarController.Delegate = this;
					_tabBarController.View.AddGestureRecognizer(panGestureRecognizer);
				}
			}
		}


		UIPanGestureRecognizer _panGestureRecognizer;
		UIPanGestureRecognizer panGestureRecognizer { get {
				if (_panGestureRecognizer == null) {
					_panGestureRecognizer = new UIPanGestureRecognizer(() => PanGestureRecognizerDidPan(_panGestureRecognizer));
				}
				return _panGestureRecognizer;
			}  set {
				_panGestureRecognizer = value;
			} 
		}


		void PanGestureRecognizerDidPan(UIPanGestureRecognizer sender)
		{
			if (tabBarController.GetTransitionCoordinator() != null) {
				return;
			}

			if (sender.State == UIGestureRecognizerState.Began || sender.State == UIGestureRecognizerState.Changed) {
				BeginInteractiveTransitionIfPossible(sender);
			}	
		}

		void BeginInteractiveTransitionIfPossible(UIPanGestureRecognizer sender) {
			CGPoint translation = sender.TranslationInView(tabBarController.View);

			if (translation.X > 0F && tabBarController.SelectedIndex > 0)
			{
				tabBarController.SelectedIndex--;
			}
			else if (translation.X < 0F && (tabBarController.SelectedIndex + 1) < tabBarController.ViewControllers.Length)
			{
				tabBarController.SelectedIndex++;
			} else {
				if (!(translation.X == 0 && translation.Y == 0)) {
					sender.Enabled = false;
					sender.Enabled = true;
				}
			}

			IUIViewControllerTransitionCoordinator cooordinator = _tabBarController.GetTransitionCoordinator();

			if (cooordinator != null) {
				cooordinator.AnimateAlongsideTransition((obj) => { }, (IUIViewControllerTransitionCoordinatorContext context) =>
					{
						if (context.IsCancelled && sender.State == UIGestureRecognizerState.Changed)
						{
							BeginInteractiveTransitionIfPossible(sender);
						}
					});
			}
			    
		}

		// TabBarControllerDelegate
		[Export("tabBarController:animationControllerForTransitionFromViewController:toViewController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForTransition(UITabBarController tabBarController, UIViewController fromViewController, UIViewController toViewController) {

			System.Diagnostics.Debug.Assert(tabBarController == _tabBarController, "Not the tab bar associated with private variable");

			UIViewController[] viewControllers = tabBarController.ViewControllers;

			if (Array.IndexOf(viewControllers, toViewController) > Array.IndexOf(viewControllers, fromViewController)) {
				return new SlideTransitionAnimator(UIRectEdge.Left);
			}

			return new SlideTransitionAnimator(UIRectEdge.Right);
		}


		[Export("tabBarController:interactionControllerForAnimationController:")]
		public IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController(UITabBarController tabBarController, IUIViewControllerAnimatedTransitioning animationController)
		{

			System.Diagnostics.Debug.Assert(tabBarController == _tabBarController, "Not the tab bar associated with private variable");

			UIViewController[] viewControllers = tabBarController.ViewControllers;

			if (panGestureRecognizer.State == UIGestureRecognizerState.Began || panGestureRecognizer.State == UIGestureRecognizerState.Changed)
			{
				return new SlideTransitionInteractionController(panGestureRecognizer);
			}

			return null;
		}

	}
}