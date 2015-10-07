using System;

using WatchKit;
using Foundation;

using Emporium;

namespace EmporiumWatchKitExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		[Outlet ("statusLabel")]
		public WKInterfaceLabel StatusLabel { get; set; }

		public InterfaceController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("makePaymentPressed")]
		void MakePaymentPressed ()
		{
			// We'll send the product as a dictionary, and convert it to a `Product`
			// value in our app delegate.
			var product = new ProductContainer {
				Name = "Example Charge",
				Description = "An example charge made by a WatchKit Extension",
				Price = "14.99"
			};

			// Create our activity handoff type (registered in the iOS app's Info.plist).
			var activityType = AppConfiguration.UserActivity.Payment;

			// Use Handoff to route the wearer to the payment controller on phone
			var userInfo = new NSDictionary ("product", product.Dictionary);
			UpdateUserActivity (activityType, userInfo, null);

			// Tell the user to use handoff to pay.
			StatusLabel.SetText ("Use handoff to pay!");
		}
	}
}