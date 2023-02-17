using System;

using CoreGraphics;
using Intents;
using IntentsUI;
using UIKit;

namespace ElizaChatIntentUI {
	// As an example, this extension's Info.plist has been configured to handle interactions for INSendMessageIntent.
	// You will want to replace this or add other intents as appropriate.
	// The intents whose interactions you wish to handle must be declared in the extension's Info.plist.

	// You can test this example integration by saying things to Siri like:
	// "Send a message using <myApp>"
	public partial class IntentViewController : UIViewController, IINUIHostedViewControlling {
		protected IntentViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any required interface initialization here.
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public void Configure (INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion)
		{
			// Do configuration here, including preparing views and calculating a desired size for presentation.

			if (completion != null)
				completion (DesiredSize ());
		}

		CGSize DesiredSize ()
		{
			return ExtensionContext.GetHostedViewMaximumAllowedSize ();
		}
	}
}
