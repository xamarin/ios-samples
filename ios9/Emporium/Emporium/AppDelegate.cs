using Foundation;
using UIKit;

namespace Emporium
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window { get; set; }

		UINavigationController RootViewController {
			get {
				var window = Window;
				return (window != null) ? window.RootViewController as UINavigationController : null;
			}
		}

		// Here we handle our WatchKit activity handoff request. We'll take the dictionary passed in as part of the activity's userInfo property, and immediately
		// present a payment sheet. If you're using handoff to allow WatchKit apps to request Apple Pay payments you try to display the payment sheet as soon as
		// possible.
		public override bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			// Create a new product detail view controller using the supplied product.
			var rawDictionary = userActivity.UserInfo? ["product"] as NSDictionary;
			if (rawDictionary != null) {
				var productContainer = new ProductContainer (rawDictionary);
				Product product = productContainer.Product;

				// Firstly, we'll create a product detail page. We can instantiate it from our storyboard...
				var viewController = (ProductTableViewController)RootViewController?.Storyboard?.InstantiateViewController ("ProductTableViewController");

				// Manually set the product we want to display.
				if (viewController != null) {
					viewController.Product = product;

					// The rootViewController should be a navigation controller. Pop to it if needed.
					RootViewController?.PopToRootViewController (false);

					// Push the view controller onto our app, so it's the first thing the user sees.
					RootViewController?.PushViewController (viewController, false);

					// We also want to immediately show the payment sheet, so we'll trigger that too.
					viewController.ApplyPayButtonClicked ();
				}
			}
			return true;
		}
	}
}