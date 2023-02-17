/*
See LICENSE folder for this sample’s licensing information.

Abstract:
The application delegate.
*/

using Foundation;
using ObjCRuntime;
using UIKit;
using Intents;
using System;
using System.Linq;
using SoupKit.Support;
using SoupKit;

namespace SoupChef {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		public override UIWindow Window {
			get;
			set;
		}

		public override bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			var intent = userActivity.GetInteraction ()?.Intent as OrderSoupIntent;
			if (!(intent is null)) {
				HandleIntent (intent);
				return true;
			} else if (userActivity.ActivityType == NSUserActivityHelper.ViewMenuActivityType) {
				HandleUserActivity ();
				return true;
			}
			return false;
		}

		void HandleIntent (OrderSoupIntent intent)
		{
			var handler = new OrderSoupIntentHandler ();
			handler.HandleOrderSoup (intent, (response) => {
				if (response.Code != OrderSoupIntentResponseCode.Success) {
					Console.WriteLine ("Quantity must be greater than 0 to add to order");
				}
			});
		}

		void HandleUserActivity ()
		{
			var rootViewController = Window?.RootViewController as UINavigationController;
			var orderHistoryViewController = rootViewController?.ViewControllers?.FirstOrDefault () as OrderHistoryTableViewController;
			if (orderHistoryViewController is null) {
				Console.WriteLine ("Failed to access OrderHistoryTableViewController.");
				return;
			}
			var segue = OrderHistoryTableViewController.SegueIdentifiers.SoupMenu;
			orderHistoryViewController.PerformSegue (segue, null);
		}
	}
}

