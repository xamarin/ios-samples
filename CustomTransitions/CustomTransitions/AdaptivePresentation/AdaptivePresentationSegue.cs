using System;

using UIKit;

namespace CustomTransitions {
	public partial class AdaptivePresentationSegue : UIStoryboardSegue {
		public AdaptivePresentationSegue (string identifier, UIViewController source, UIViewController destination) : base (identifier, source, destination)
		{
		}

		public AdaptivePresentationSegue (IntPtr handle) : base (handle)
		{
		}

		public override void Perform ()
		{
			var sourceViewController = (APSecondViewController) DestinationViewController;
			var destinationViewController = (APSecondViewController) DestinationViewController;

			var presentationController = new AdaptivePresentationController (destinationViewController, sourceViewController);

			destinationViewController.TransitioningDelegate = presentationController;
			destinationViewController.PresentationControllerInstance = presentationController;

			SourceViewController.PresentViewController (destinationViewController, true, null);
		}
	}
}
