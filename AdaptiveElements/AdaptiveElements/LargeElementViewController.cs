using System;
using UIKit;

namespace AdaptiveElements {
	/// <summary>
	/// LargeElementViewController is used in two different ways:
	/// 1. Contained within ExampleContainerViewController, when its width is large.
	/// 2. Presented by SmallElementViewController, when the ExampleContainerViewController's width is small.
	/// It shows a large version of the element.When it is presented, tapping on it will dismiss it.
	/// </summary>
	public partial class LargeElementViewController : UIViewController {
		private NSLayoutConstraint widthConstraint;

		public LargeElementViewController (IntPtr handle) : base (handle) { }

		public override void UpdateViewConstraints ()
		{
			base.UpdateViewConstraints ();

			/*
             * If we are not being presented full-screen,
             * then add a constraint to make this view no wider than our superview's readable content guide.
             */

			if (base.PresentingViewController == null &&
				this.widthConstraint == null &&
				base.View.Superview != null) {
				this.widthConstraint = base.View.WidthAnchor.ConstraintLessThanOrEqualTo (base.View.Superview.ReadableContentGuide.WidthAnchor);
				if (this.widthConstraint != null) {
					this.widthConstraint.Active = true;
				}
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			/*
             * When this view appears, if we are being presented,
             * add a tap gesture recognizer so we can dismiss when we are tapped.
             */

			if (this.IsBeingPresented) {
				var tapGestureRecognizer = new UITapGestureRecognizer (this.Tapped);
				base.View.AddGestureRecognizer (tapGestureRecognizer);
			}
		}

		private void Tapped (UITapGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State == UIGestureRecognizerState.Ended) {
				DismissViewController (true, null);
			}
		}
	}
}
