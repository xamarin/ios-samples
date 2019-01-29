using CoreGraphics;
using System;
using UIKit;

namespace AdaptiveElements
{
    /// <summary>
    /// SimpleExampleViewController demonstrates how to use a view's size to determine an arrangement of its subviews. 
    /// Also, it shows how to add an animation effect during app size changes.
    /// </summary>
    public partial class SimpleExampleViewController : UIViewController
    {
        public SimpleExampleViewController(IntPtr handle) : base(handle) { }

        public override void ViewWillLayoutSubviews()
        {
            /*
             * In viewWillLayoutSubviews, we are guaranteed that our view's size, traits, etc. are up to date.
             * It's a good place to update anything that affects the layout of our subviews.
             * However, be careful, because this method is called frequently!
             * Do as little work as possible, and don't invalidate the layout of any superviews.
             */

            // Step 1: Find our size.
            var size = base.View.Bounds.Size;

            // Step 2: Decide what design to use, based on our rules.
            var useWideDesign = size.Width > size.Height;

            // Step 3: Apply the design to the UI.
            this.stackView.Axis = useWideDesign ? UILayoutConstraintAxis.Horizontal : UILayoutConstraintAxis.Vertical;
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            /*
             * We override viewWillTransition(to size: with coordinator:) in order to add special behavior
             * when the app changes size, especially when the device is rotated.
             * In this demo app, we add an effect to make the items "pop" towards the viewer during the rotation,
             * and then go back to normal afterwards.
             */

            base.ViewWillTransitionToSize(toSize, coordinator);

            // If self.stackView is null, then our view has not yet been loaded, and there is nothing to do.
            if (this.stackView != null)
            {
                // Add alongside and completion animations.
                coordinator.AnimateAlongsideTransition((_) =>
                {
                    /*
                     * Scale the stackView to be larger than normal.
                     * This animates along with the rest of the rotation animation.
                     */
                    this.stackView.Transform = CGAffineTransform.MakeScale(1.4f, 1.4f);
                }, (_) =>
                {
                    /*
                    * The rotation animation is complete. Add an additional 0.5 second
                    * animation to set the scale back to normal.
                    */
                    UIView.Animate(0.5f, () => this.stackView.Transform = CGAffineTransform.MakeIdentity());
                });
            }
        }
    }
}