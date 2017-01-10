using System;

using UIKit;

namespace CustomTransitions
{
	public partial class CPFirstViewController : UIViewController
	{
		public CPFirstViewController(IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Perform any additional setup after loading the view, typically from a nib.
		}


		partial void ButtonAction(UIButton sender)
		{
			UIViewController secondVC = Storyboard.InstantiateViewController("SecondViewController");

			// transitioningDelegate does not hold a strong reference to its
			// destination object.  To prevent presentationController from being
			// released prior to calling -presentViewController:animated:completion:
			// the NS_VALID_UNTIL_END_OF_SCOPE attribute is appended to the declaration.
			//AAPLCustomPresentationController* presentationController NS_VALID_UNTIL_END_OF_SCOPE; !!!

			CustomPresentationController presentationController = new CustomPresentationController(secondVC, this);
			secondVC.TransitioningDelegate = presentationController;

			PresentViewController(secondVC, true, null);
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

