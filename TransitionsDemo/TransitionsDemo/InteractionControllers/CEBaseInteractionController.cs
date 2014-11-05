using System;
using UIKit;

namespace TransitionsDemo.InteractionControllers
{
	public enum CEInteractionOperation
	{
		Pop = 0,
		Dismiss,
		Tab
	}

	public class CEBaseInteractionController : UIPercentDrivenInteractiveTransition 
	{
		protected bool shouldCompleteTransition;
		protected UIViewController viewController;
		protected UIGestureRecognizer gestureRecognizer;
		protected CEInteractionOperation operation;

		public bool InteractionInProgress { get; set; }

		public virtual void WireToViewController(UIViewController viewController, CEInteractionOperation operation)
		{
		}
	}
}

