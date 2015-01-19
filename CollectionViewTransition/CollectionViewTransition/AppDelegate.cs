using System;
using Foundation;
using UIKit;

namespace CollectionViewTransition {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {

		public override UIWindow Window {
			get;
			set;
		}

		public override void FinishedLaunching (UIApplication application)
		{
			Window = new UIWindow (UIScreen.MainScreen.Bounds);
			APLStackLayout stackLayout = new APLStackLayout ();
			var cvc = new APLStackCollectionViewController (stackLayout) {
				Title = "Stack Layout"
			};
			var navigationController = new UINavigationController (cvc);
			navigationController.NavigationBar.Translucent = false;

			var transitionController = new APLTransitionController (cvc.CollectionView, navigationController);
			navigationController.Delegate = new NavigationControllerDelegate (transitionController);

			Window.RootViewController = navigationController;
			Window.MakeKeyAndVisible ();
		}

		public class NavigationControllerDelegate : UINavigationControllerDelegate
		{
			APLTransitionController transitionController;

			public NavigationControllerDelegate (APLTransitionController controller)
			{
				transitionController = controller;
			}

			public override IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController (UINavigationController navigationController, IUIViewControllerAnimatedTransitioning animationController)
			{
				if (animationController == transitionController)
					return transitionController;
				return null;
			}

			public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation (UINavigationController navigationController, UINavigationControllerOperation operation, UIViewController fromViewController, UIViewController toViewController)
			{
				if (!fromViewController.GetType ().IsSubclassOf (typeof(UICollectionViewController)))
				    return null;
				if (!toViewController.GetType ().IsSubclassOf (typeof (UICollectionViewController)))
					return null;
				if (!transitionController.HasActiveInteraction)
					return null;

				transitionController.NavigationOperation = operation;
				return transitionController;
			}
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
