using System;

using CoreGraphics;
using Intents;
using IntentsUI;
using UIKit;

namespace TasksNotesSiriIntentUI
{
    /*

        This sample does not currently customize the IntentUI

        The project has been included for customization by you!

    */
    public partial class IntentViewController : UIViewController, IINUIHostedViewControlling
    {
        protected IntentViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any required interface initialization here.
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public void Configure(INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion)
        {
            // Do configuration here, including preparing views and calculating a desired size for presentation.

            if (completion != null)
                completion(DesiredSize());
        }

        CGSize DesiredSize()
        {
            return ExtensionContext.GetHostedViewMaximumAllowedSize();
        }
    }
}
