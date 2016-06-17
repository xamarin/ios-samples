using System;

using UIKit;
using Foundation;
using PassKit;
using System.Collections.Generic;

namespace Emporium
{
	[Register ("ProductTableViewController")]
	public class ProductTableViewController : UITableViewController, IPKPaymentAuthorizationViewControllerDelegate
	{
		static readonly NSString confirmationSegue = (NSString)"ConfirmationSegue";

		readonly NSString[] supportedNetworks = {
			PKPaymentNetwork.Amex,
			PKPaymentNetwork.Discover,
			PKPaymentNetwork.MasterCard,
			PKPaymentNetwork.Visa
		};

		public Product Product { get; set; }

		PKPaymentToken paymentToken;

		[Outlet ("productImageView")]
		public UIImageView ProductImageView { get; set; }

		[Outlet ("productTitleLabel")]
		public UILabel ProductTitleLabel { get; set; }

		[Outlet ("productPriceLabel")]
		public UILabel ProductPriceLabel { get; set; }

		[Outlet ("productDescriptionView")]
		public UITextView ProductDescriptionView { get; set; }

		[Outlet ("applePayView")]
		public UIView ApplePayView { get; set; }

		public ProductTableViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == confirmationSegue) {
				var viewController = (ConfirmationViewController)segue.DestinationViewController;
				viewController.TransactionIdentifier = paymentToken.TransactionIdentifier;
			} else {
				base.PrepareForSegue (segue, sender);
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			ProductTitleLabel.Text = Product.Name;
			ProductDescriptionView.Text = Product.Description;
			ProductPriceLabel.Text = string.Format ("${0}", Product.Price);

			// Display an Apple Pay button if a payment card is available. In your
			// app, you might divert the user to a more traditional checkout if
			// Apple Pay wasn't set up.

			if (PKPaymentAuthorizationViewController.CanMakePaymentsUsingNetworks (supportedNetworks)) {
				var button = new PKPaymentButton (PKPaymentButtonType.Buy, PKPaymentButtonStyle.Black);
				button.TouchUpInside += ApplyPayButtonClicked;

				button.Center = ApplePayView.Center;
				button.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
				ApplePayView.AddSubview (button);
			}

		}

		void ApplyPayButtonClicked (object sender, EventArgs e)
		{
			ApplyPayButtonClicked ();
		}

		public void ApplyPayButtonClicked ()
		{
			if (!PKPaymentAuthorizationViewController.CanMakePaymentsUsingNetworks (supportedNetworks)) {
				ShowAuthorizationAlert ();
				return;
			}

			// Set up our payment request.
			var paymentRequest = new PKPaymentRequest ();

			// Our merchant identifier needs to match what we previously set up in
			// the Capabilities window (or the developer portal).
			paymentRequest.MerchantIdentifier = AppConfiguration.Merchant.Identififer;

			// Both country code and currency code are standard ISO formats. Country
			// should be the region you will process the payment in. Currency should
			// be the currency you would like to charge in.
			paymentRequest.CountryCode = "US";
			paymentRequest.CurrencyCode = "USD";

			// The networks we are able to accept.
			paymentRequest.SupportedNetworks = supportedNetworks;

			// Ask your payment processor what settings are right for your app. In
			// most cases you will want to leave this set to ThreeDS.
			paymentRequest.MerchantCapabilities = PKMerchantCapability.ThreeDS;

			// An array of `PKPaymentSummaryItems` that we'd like to display on the
			// sheet (see the MakeSummaryItems method).
			paymentRequest.PaymentSummaryItems = MakeSummaryItems (false);

			// Request shipping information, in this case just postal address.
			paymentRequest.RequiredShippingAddressFields = PKAddressField.PostalAddress;

			// Display the view controller.
			var viewController = new PKPaymentAuthorizationViewController (paymentRequest);
			viewController.Delegate = this;
			PresentViewController (viewController, true, null);

		}

		PKPaymentSummaryItem[] MakeSummaryItems (bool requiresInternationalSurcharge)
		{
			var items = new List<PKPaymentSummaryItem> ();

			// Product items have a label (a string) and an amount (NSDecimalNumber).
			// NSDecimalNumber is a Cocoa class that can express floating point numbers
			// in Base 10, which ensures precision. It can be initialized with a
			// double, or in this case, a string.
			var productSummaryItem = PKPaymentSummaryItem.Create ("Sub-total", new NSDecimalNumber (Product.Price));
			items.Add (productSummaryItem);

			var totalSummaryItem = PKPaymentSummaryItem.Create ("Emporium", productSummaryItem.Amount);
			// Apply an international surcharge, if needed.
			if (requiresInternationalSurcharge) {
				var handlingSummaryItem = PKPaymentSummaryItem.Create ("International Handling", new NSDecimalNumber ("9.99"));

				// Note how NSDecimalNumber has its own arithmetic methods.
				totalSummaryItem.Amount = productSummaryItem.Amount.Add (handlingSummaryItem.Amount);
				items.Add (handlingSummaryItem);
			}
			items.Add (totalSummaryItem);

			return items.ToArray ();
		}

		#region IPKPaymentAuthorizationViewControllerDelegate

		/// <summary>
		/// Whenever the user changed their shipping information we will receive a callback here.
		///
		/// Note that for privacy reasons the contact we receive will be redacted,
		/// and only have a city, ZIP, and country.
		///
		/// You can use this method to estimate additional shipping charges and update
		/// the payment summary items.
		/// </summary>
		[Export ("paymentAuthorizationViewController:didSelectShippingContact:completion:")]
		void DidSelectShippingContact (PKPaymentAuthorizationViewController controller, PKContact contact, PKPaymentShippingAddressSelected completion)
		{
			// Create a shipping method. Shipping methods use PKShippingMethod,
			// which inherits from PKPaymentSummaryItem. It adds a detail property
			// you can use to specify information like estimated delivery time.
			var shipping = new PKShippingMethod {
				Label = "Standard Shipping",
				Amount = NSDecimalNumber.Zero,
				Detail = "Delivers within two working days"
			};

			// Note that this is a contrived example. Because addresses can come from
			// many sources on iOS they may not always have the fields you want.
			// Your application should be sure to verify the address is correct,
			// and return the appropriate status. If the address failed to pass validation
			// you should return `InvalidShippingPostalAddress` instead of `Success`.

			var address = contact.PostalAddress;
			var requiresInternationalSurcharge = address.Country != "United States";

			PKPaymentSummaryItem[] summaryItems = MakeSummaryItems (requiresInternationalSurcharge);

			completion (PKPaymentAuthorizationStatus.Success, new [] { shipping }, summaryItems);
		}

		/// <summary>
		/// This is where you would send your payment to be processed - here we will
		/// simply present a confirmation screen. If your payment processor failed the
		/// payment you would return `completion(PKPaymentAuthorizationStatus.Failure)` instead. Remember to never
		/// attempt to decrypt the payment token on device.
		/// </summary>
		public void DidAuthorizePayment (PKPaymentAuthorizationViewController controller, PKPayment payment, Action<PKPaymentAuthorizationStatus> completion)
		{
			paymentToken = payment.Token;
			completion (PKPaymentAuthorizationStatus.Success);
			PerformSegue (confirmationSegue, this);
		}

		public void PaymentAuthorizationViewControllerDidFinish (PKPaymentAuthorizationViewController controller)
		{
			// We always need to dismiss our payment view controller when done.
			DismissViewController (true, null);
		}

		public void WillAuthorizePayment (PKPaymentAuthorizationViewController controller)
		{
		}

		#endregion

		void ShowAuthorizationAlert ()
		{
			var alert = UIAlertController.Create ("Error", "This device cannot make payments.", UIAlertControllerStyle.Alert);
			var action = UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, null);
			alert.AddAction (action);

			PresentViewController (alert, true, null);
		}
	}
}