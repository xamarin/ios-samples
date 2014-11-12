using System;
using UIKit;

namespace DynamicsCatalog {

	public partial class SnapViewController : UIViewController {

		public UIDynamicAnimator Animator { get; private set; }

		public SnapViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.AddGestureRecognizer (new UITapGestureRecognizer ((gesture) => {
				Animator = new UIDynamicAnimator (View);
				Animator.AddBehavior (new UISnapBehavior (square, gesture.LocationInView (View)));
			}));
		}
	}
}