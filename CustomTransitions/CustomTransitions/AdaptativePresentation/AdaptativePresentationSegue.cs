using UIKit;
using System;

namespace CustomTransitions
{
    public partial class AdaptativePresentationSegue : UIStoryboardSegue
    {
		public AdaptativePresentationSegue(string identifier, UIViewController source, UIViewController destination) 
			: base (identifier, source, destination)  { 

		}

		public AdaptativePresentationSegue(IntPtr handle)
			: base (handle)
		{
		}

		public override void Perform()
		{
			APSecondViewController sourceViewController = (APSecondViewController)DestinationViewController;
			APSecondViewController destinationViewController = (APSecondViewController)DestinationViewController;

			AdaptativePresentationController presentationController = new AdaptativePresentationController(destinationViewController, sourceViewController);

			destinationViewController.TransitioningDelegate = presentationController;
			destinationViewController.presentationControllerInstance = presentationController;

			SourceViewController.PresentViewController(destinationViewController, true, null);
		}

    }
}
