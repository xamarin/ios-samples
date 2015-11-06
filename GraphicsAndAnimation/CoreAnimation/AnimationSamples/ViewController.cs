using System;
using UIKit;

namespace AnimationSamples
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			TransitionButton.TouchUpInside += (sender, e) => {
				var vc2 = (SecondViewController)Storyboard.InstantiateViewController ("SecondViewController");
				vc2.ModalTransitionStyle = UIModalTransitionStyle.PartialCurl;
				PresentViewController (vc2, true, null);
			};

			ViewTransitionButton.TouchUpInside += (sender, e) => {
				var vc3 = new DemoViewTransition ();
				PresentViewController (vc3, true, null);
			};

			ViewAnimationButton.TouchUpInside += (sender, e) => {
				var vc4 = new ViewPropertyAnimation ();
				PresentViewController (vc4, true, null);
			};

			ImplicitButton.TouchUpInside += (sender, e) => {
				var vc5 = new ImplicitLayerAnimation ();
				PresentViewController (vc5, true, null);
			};

			ExplicitButton.TouchUpInside += (sender, e) => {
				var vc6 = new ExplicitLayerAnimation ();
				PresentViewController (vc6, true, null);
			};
		}
	}
}