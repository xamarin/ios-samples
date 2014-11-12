using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace AnimationSamples
{
	public partial class AnimationSamplesViewController : UIViewController
	{
		public AnimationSamplesViewController () : base ("AnimationSamplesViewController", null)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TransitionButton.TouchUpInside += (object sender, EventArgs e) => {

				#region demo animated controller transition
				SecondViewController vc2 = new SecondViewController {
					ModalTransitionStyle = UIModalTransitionStyle.PartialCurl
				};

				PresentViewController (vc2, true, null);
				#endregion

				#region demo view transition
//				DemoViewTransition vc3 = new DemoViewTransition();
//				
//				PresentViewController (vc3, true, null);
				#endregion

				#region demo view animation
//				ViewAnimation vc4 = new ViewAnimation();
//				
//				PresentViewController (vc4, true, null);
				#endregion

				#region demo implecit layer animation
//				var vc5 = new ImplicitLayerAnimation();
//				
//				PresentViewController (vc5, true, null);
				#endregion

				#region demo explicit layer animation
//				var vc6 = new ExplicitLayerAnimation();
//				
//				PresentViewController (vc6, true, null);
				#endregion
			};
		}


	
	}
}

