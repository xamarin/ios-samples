using System;
using UIKit;

namespace AdaptiveElements
{
    /// <summary>
    /// SmallElementViewController is contained within ExampleContainerViewController.
    /// It shows a small version of the element.Tapping on it presents a LargeElementViewController which shows more details.
    /// </summary>
    public partial class SmallElementViewController : UIViewController
    {
        public SmallElementViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            /*
             * `ViewDidLoad()` is NOT the place to do everything.
             * However, it is a good place to do work that does not depend on
             * any other objects or views, and that will be the same no matter where this view controller is used.
             * In this view controller, the following code qualifies:
             */

            // Add a constraint to make this view always square.
            var constraint = base.View.WidthAnchor.ConstraintEqualTo(base.View.HeightAnchor);
            constraint.Active = true;

            /*
             * The SmallElementViewController is just a preview.
             * Tap on our view to show the details in the LargeElementViewController.
             */

            var tapGestureRecognizer = new UITapGestureRecognizer(this.Tapped);
            base.View.AddGestureRecognizer(tapGestureRecognizer);
        }

        private void Tapped(UITapGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State == UIGestureRecognizerState.Ended)
            {
                // Create the larger view controller:
                var storyboard = UIStoryboard.FromName("Main", null);
                var newElementViewController = storyboard.InstantiateViewController("largeElement");

                /*
                 * And present it.
                 * We use the `.overFullScreen` presentation style so the ExampleContainerViewController
                 * underneath will go through the normal layout process, even while the presentation is active.
                 */

                newElementViewController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                base.PresentViewController(newElementViewController, true, null);
            }
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            /*
             * When we are removed from our parent view controller
             * (which could happen when the parent changes to a different Design),
             * if we presented the ElementViewController, then dismiss it.
             */
            if (parent == null && base.PresentedViewController != null)
            {
                this.DismissViewController(false, null);
            }
        }
    }
}