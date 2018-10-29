
using CoreGraphics;
using Foundation;
using Intents;
using IntentsUI;
using System;
using UIKit;

namespace SoupChef
{
    /// <summary>
    /// Create a custom user interface that shows in the Siri interface, as well as with 3D touches on a shortcut on the Cover Sheet or in Spotlight.
    /// </summary>
    public partial class IntentViewController : UIViewController, IINUIHostedViewControlling
    {
        protected IntentViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any required interface initialization here.
        }

        [Export("configureViewForParameters:ofInteraction:interactiveBehavior:context:completion:")]
        public void ConfigureView(NSSet<INParameter> parameters,
                                  INInteraction interaction,
                                  INUIInteractiveBehavior interactiveBehavior,
                                  INUIHostedViewContext context,
                                  INUIHostedViewControllingConfigureViewHandler completion)
        {

            if (interaction.Intent is OrderSoupIntent intent)
            {
                /*
                 Different UIs can be displayed depending if the intent is in the confirmation phase or the handle phase.
                 This example uses view controller containment to manage each of the different views via a dedicated view controller.
                */

                if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Ready)
                {
                    var viewController = new InvoiceViewController(intent);
                    AttachChild(viewController);
                    completion(true, parameters, DesiredSize());
                }
                else if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Success)
                {
                    if (interaction.IntentResponse is OrderSoupIntentResponse response)
                    {
                        var viewController = new OrderConfirmedViewController(intent, response);
                        AttachChild(viewController);
                        completion(true, parameters, DesiredSize());
                    }
                }
            }
            else
            {
                completion(false, parameters, CGSize.Empty);
            }
        }

        public void Configure(INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion)
        {
            throw new NotImplementedException();
        }

        CGSize DesiredSize()
        {
            var width = this.ExtensionContext?.GetHostedViewMaximumAllowedSize().Width ?? 320f;
            return new CGSize(width, 170f);
        }

        private void AttachChild(UIViewController viewController)
        {
            AddChildViewController(viewController);

            var subview = viewController.View;
            if (subview != null)
            {
                View.AddSubview(subview);
                subview.TranslatesAutoresizingMaskIntoConstraints = false;

                // Set the child controller's view to be the exact same size as the parent controller's view.
                subview.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
                subview.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

                subview.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
                subview.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
            }

            viewController.DidMoveToParentViewController(this);
        }
    }
}
