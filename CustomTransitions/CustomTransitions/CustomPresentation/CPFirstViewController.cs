using System;

using UIKit;

namespace CustomTransitions {
	public partial class CPFirstViewController : UIViewController {
		public CPFirstViewController (IntPtr handle) : base (handle)
		{
		}

		partial void ButtonAction (UIButton sender)
		{
			UIViewController secondVC = Storyboard.InstantiateViewController ("SecondViewController");
			CustomPresentationController presentationController = new CustomPresentationController (secondVC, this);
			secondVC.TransitioningDelegate = presentationController;

			PresentViewController (secondVC, true, null);
		}
	}
}
