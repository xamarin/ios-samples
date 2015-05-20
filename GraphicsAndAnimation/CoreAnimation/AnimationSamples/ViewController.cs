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

			TransitionButton.TouchUpInside += (object sender, EventArgs e) => {
				SecondViewController vc2 = (SecondViewController)Storyboard.InstantiateViewController("SecondViewController");

				vc2.ModalTransitionStyle = UIModalTransitionStyle.PartialCurl;

				PresentViewController(vc2, true, null);
			};

			ViewTransitionButton.TouchUpInside += (object sender, EventArgs e) => {
				DemoViewTransition vc3 = new DemoViewTransition();

				PresentViewController(vc3, true, null);
			};

			ViewAnimationButton.TouchUpInside += (object sender, EventArgs e) => {
				ViewPropertyAnimation vc4 = new ViewPropertyAnimation();

				PresentViewController(vc4, true, null);
			};

			ImplicitButton.TouchUpInside += (object sender, EventArgs e) => {
				ImplicitLayerAnimation vc5 = new ImplicitLayerAnimation();

				PresentViewController(vc5, true, null);
			};

			ExplicitButton.TouchUpInside += (object sender, EventArgs e) => {
				ExplicitLayerAnimation vc6 = new ExplicitLayerAnimation();

				PresentViewController(vc6, true, null);
			};
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

