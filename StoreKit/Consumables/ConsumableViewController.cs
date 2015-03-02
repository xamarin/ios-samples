using System;
using System.Collections.Generic;

using UIKit;
using StoreKit;
using Foundation;
using CoreGraphics;

using SharedCode;

namespace Consumables
{
	public class ConsumableViewController : UIViewController
	{
		public static string Buy5ProductId = "com.xamarin.storekit.testing.consume5credits";
		public static string Buy10ProductId = "com.xamarin.storekit.testing.consume10credits";

		UIButton buy5Button, buy10Button;
		UILabel buy5Title, buy5Description, buy10Title, buy10Description, balanceLabel, infoLabel;
		List<string> products;
		bool pricesLoaded = false;

		NSObject priceObserver, succeededObserver, failedObserver, requestObserver;

		CustomPaymentObserver theObserver;
		InAppPurchaseManager iap;

		#region localized strings
		/// <summary>
		/// String.Format(Buy, "price"); // "Buy {0}"
		/// </summary>
		string Buy {
			get {return Foundation.NSBundle.MainBundle.LocalizedString ("Buy {0}", "Buy {0}");}
		}
		/// <summary>
		/// String.Format(Balance, "balance"); // "{0} monkey credits"
		/// </summary>
		string Balance {
			get {return Foundation.NSBundle.MainBundle.LocalizedString ("{0} monkey credits", "{0} monkey credits");}
		}
		/// <summary>
		/// No placeholders
		/// </summary>
		string Footer {
			get {return Foundation.NSBundle.MainBundle.LocalizedString ("Notice how you can keep buying the same items over and over. That's what makes these products 'consumable'.", "Notice how you can keep buying the same items over and over. That's what makes these products 'consumable'.");}
		}
		#endregion

		public ConsumableViewController ()
		{
			// two products for sale on this page
			products = new List<string>() { Buy5ProductId, Buy10ProductId };

			iap = new InAppPurchaseManager();
			theObserver = new CustomPaymentObserver(iap);

			// Call this once upon startup of in-app-purchase activities
			// This also kicks off the TransactionObserver which handles the various communications
			SKPaymentQueue.DefaultQueue.AddTransactionObserver(theObserver);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			EdgesForExtendedLayout = UIRectEdge.None;

			#region UI layout stuff, not relevant to example
			Title = "Consumable Products";
			View.BackgroundColor = UIColor.White;

			buy5Button = UIButton.FromType (UIButtonType.RoundedRect);
			buy5Button.SetTitle ("loading...", UIControlState.Disabled);
			buy5Button.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
			buy5Button.SetTitle ("Buy...", UIControlState.Normal);
			buy5Button.Enabled = false;

			buy10Button = UIButton.FromType (UIButtonType.RoundedRect);
			buy10Button.SetTitle ("loading...", UIControlState.Disabled);
			buy10Button.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
			buy10Button.SetTitle ("Buy...", UIControlState.Normal);
			buy10Button.Enabled = false;

			buy5Title = new UILabel(new CGRect(10, 10, 300, 30));
			buy5Title.Font = UIFont.BoldSystemFontOfSize(18f);
			buy5Description = new UILabel(new CGRect(10, 40, 300, 30));
			buy5Button.Frame = new CGRect(10, 80, 200, 40);

			buy10Title = new UILabel(new CGRect(10, 140, 300, 30));
			buy10Title.Font = UIFont.BoldSystemFontOfSize(18f);
			buy10Description = new UILabel(new CGRect(10, 170, 300, 30));
			buy10Button.Frame = new CGRect(10, 210, 200, 40);

			balanceLabel = new UILabel(new CGRect(10, 280, 300, 40));
			balanceLabel.Font = UIFont.BoldSystemFontOfSize(24f);

			infoLabel = new UILabel(new CGRect(10, 340, 300, 80));
			infoLabel.Lines = 3;
			infoLabel.Text = Footer;

			View.AddSubview (buy5Button);
			View.AddSubview (buy5Title);
			View.AddSubview (buy5Description);
			View.AddSubview (buy10Button);
			View.AddSubview (buy10Title);
			View.AddSubview (buy10Description);
			View.AddSubview (balanceLabel);
			View.AddSubview (infoLabel);
			#endregion

			buy5Button.TouchUpInside += (sender, e) => iap.PurchaseProduct (Buy5ProductId);
			buy10Button.TouchUpInside += (sender, e) => iap.PurchaseProduct (Buy10ProductId);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);

			// setup the observer to wait for prices to come back from StoreKit <- AppStore
			priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerProductsFetchedNotification,
				(notification) => {
					var info = notification.UserInfo;
					if (info == null)
						return;

					var NSBuy5ProductId = new NSString (Buy5ProductId);

					if (info.ContainsKey (NSBuy5ProductId)) {
						pricesLoaded = true;

						var product = (SKProduct)info [NSBuy5ProductId];
						Print (product);
						SetVisualState (buy5Button, buy5Title, buy5Description, product);
					}

					var NSBuy10ProductId = new NSString (Buy10ProductId);
					if (info.ContainsKey (NSBuy10ProductId)) {
						pricesLoaded = true;

						var product = (SKProduct)info [NSBuy10ProductId];
						Print (product);
						SetVisualState (buy10Button, buy10Title, buy10Description, product);
					}
				});

			// only if we can make payments, request the prices
			if (iap.CanMakePayments()) {
				// now go get prices, if we don't have them already
				if (!pricesLoaded)
					iap.RequestProductData(products); // async request via StoreKit -> App Store
			} else {
				// can't make payments (purchases turned off in Settings?)
				buy5Button.SetTitle ("AppStore disabled", UIControlState.Disabled);
				buy10Button.SetTitle ("AppStore disabled", UIControlState.Disabled);
			}

			balanceLabel.Text = String.Format (Balance, CreditManager.Balance());// + " monkey credits";

			succeededObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerTransactionSucceededNotification,
			(notification) => {
				balanceLabel.Text = String.Format (Balance, CreditManager.Balance());// + " monkey credits";
			});
			failedObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerTransactionFailedNotification,
			(notification) => {
				// TODO:
				Console.WriteLine ("Transaction Failed");
			});

			requestObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerRequestFailedNotification,
			                                                                 (notification) => {
				// TODO:
				Console.WriteLine ("Request Failed");
				buy5Button.SetTitle ("Network down?", UIControlState.Disabled);
				buy10Button.SetTitle ("Network down?", UIControlState.Disabled);
			});
		}

		public override void ViewWillDisappear (bool animated)
		{
			// remove the observer when the view isn't visible
			NSNotificationCenter.DefaultCenter.RemoveObserver (priceObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (succeededObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (failedObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (requestObserver);

			base.ViewWillDisappear (animated);
		}

		void Print(SKProduct product)
		{
			Console.WriteLine("Product id: {0}", product.ProductIdentifier);
			Console.WriteLine("Product title: {0}", product.LocalizedTitle);
			Console.WriteLine("Product description: {0}", product.LocalizedDescription);
			Console.WriteLine("Product price: {0}", product.Price);
			Console.WriteLine("Product l10n price: {0}", product.LocalizedPrice());
		}

		void SetVisualState(UIButton buyBtn, UILabel title, UILabel description, SKProduct product)
		{
			buyBtn.Enabled = true;
			buyBtn.SetTitle(String.Format (Buy, product.LocalizedPrice()), UIControlState.Normal);

			title.Text = product.LocalizedTitle;
			description.Text = product.LocalizedDescription;
		}
	}
}
