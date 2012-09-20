using System;
using System.Collections.Generic;
using MonoTouch.StoreKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Consumables {
	public class ConsumableViewController : UIViewController {
		public static string Buy5ProductId = "com.xamarin.storekit.testing.consume5credits",
			   Buy10ProductId = "com.xamarin.storekit.testing.consume10credits";

		UIButton buy5Button, buy10Button;
		UILabel buy5Title, buy5Description, buy10Title, buy10Description, balanceLabel, infoLabel;
		List<string> products;
		bool pricesLoaded = false;

		NSObject priceObserver, succeededObserver, failedObserver, requestObserver;
		
		InAppPurchaseManager iap;

		#region localized strings
		/// <summary>
		/// String.Format(Buy, "price"); // "Buy {0}"
		/// </summary>
		string Buy {
			get {return MonoTouch.Foundation.NSBundle.MainBundle.LocalizedString ("Buy {0}", "Buy {0}");}
		}
		/// <summary>
		/// String.Format(Balance, "balance"); // "{0} monkey credits"
		/// </summary>
		string Balance {
			get {return MonoTouch.Foundation.NSBundle.MainBundle.LocalizedString ("{0} monkey credits", "{0} monkey credits");}
		}
		/// <summary>
		/// No placeholders
		/// </summary>
		string Footer {
			get {return MonoTouch.Foundation.NSBundle.MainBundle.LocalizedString ("Notice how you can keep buying the same items over and over. That's what makes these products 'consumable'.", "Notice how you can keep buying the same items over and over. That's what makes these products 'consumable'.");}
		}
		#endregion

		public ConsumableViewController () : base()
		{
			// two products for sale on this page
			products = new List<string>() { Buy5ProductId, Buy10ProductId };
			iap = new InAppPurchaseManager();
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

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

			buy5Title = new UILabel(new RectangleF(10, 10, 300, 30));
			buy5Title.Font = UIFont.BoldSystemFontOfSize(18f);
			buy5Description = new UILabel(new RectangleF(10, 40, 300, 30));
			buy5Button.Frame = new RectangleF(10, 80, 200, 40);

			buy10Title = new UILabel(new RectangleF(10, 140, 300, 30));
			buy10Title.Font = UIFont.BoldSystemFontOfSize(18f);
			buy10Description = new UILabel(new RectangleF(10, 170, 300, 30));
			buy10Button.Frame = new RectangleF(10, 210, 200, 40);

			balanceLabel = new UILabel(new Rectangle(10, 280, 300, 40));
			balanceLabel.Font = UIFont.BoldSystemFontOfSize(24f);
			
			infoLabel = new UILabel(new RectangleF(10, 340, 300, 80));
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

			buy5Button.TouchUpInside += (sender, e) => {
				iap.PurchaseProduct (Buy5ProductId);
			};	
			buy10Button.TouchUpInside += (sender, e) => {
				iap.PurchaseProduct (Buy10ProductId);
			};		
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);
			
			// setup the observer to wait for prices to come back from StoreKit <- AppStore
			priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerProductsFetchedNotification, 
			(notification) => {
				var info = notification.UserInfo;
				if (info == null) return;

				var NSBuy5ProductId = new NSString(Buy5ProductId);
				var NSBuy10ProductId = new NSString(Buy10ProductId);

				if (info.ContainsKey(NSBuy5ProductId)) {
					pricesLoaded = true;

					var product = (SKProduct) info.ObjectForKey(NSBuy5ProductId);
					
					Console.WriteLine("Product id: " + product.ProductIdentifier);
					Console.WriteLine("Product title: " + product.LocalizedTitle);
					Console.WriteLine("Product description: " + product.LocalizedDescription);
					Console.WriteLine("Product price: " + product.Price);
					Console.WriteLine("Product l10n price: " + product.LocalizedPrice());	

					buy5Button.Enabled = true;
					buy5Title.Text = product.LocalizedTitle;
					buy5Description.Text = product.LocalizedDescription;
					buy5Button.SetTitle(String.Format (Buy, product.LocalizedPrice()), UIControlState.Normal);
				}
				if (info.ContainsKey(NSBuy10ProductId)) {
					pricesLoaded = true;

					var product = (SKProduct) info.ObjectForKey(NSBuy10ProductId);
					
					Console.WriteLine("Product id: " + product.ProductIdentifier);
					Console.WriteLine("Product title: " + product.LocalizedTitle);
					Console.WriteLine("Product description: " + product.LocalizedDescription);
					Console.WriteLine("Product price: " + product.Price);
					Console.WriteLine("Product l10n price: " + product.LocalizedPrice());	

					buy10Button.Enabled = true;
					buy10Title.Text = product.LocalizedTitle;
					buy10Description.Text = product.LocalizedDescription;
					buy10Button.SetTitle(String.Format (Buy, product.LocalizedPrice()), UIControlState.Normal);
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
	}
}