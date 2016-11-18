using System;

using UIKit;

namespace CustomTransitions
{
	public partial class APFirstViewController : UIViewController
	{
		partial void PerformCustomSegue(UIButton sender)
		{
			APSecondViewController secondVC = (APSecondViewController)Storyboard.InstantiateViewController("APSecondViewController");

			AdaptativePresentationController presentationController = new AdaptativePresentationController(secondVC, this);

			secondVC.TransitioningDelegate = presentationController;

			PresentViewController(secondVC, true, null);
			//AdaptativePresentationSegue mySegue = new AdaptativePresentationSegue("algo", this, secondVC);
			//PrepareForSegue(mySegue, sender);
			//mySegue.perform();
		}

		public APFirstViewController(IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();



			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

