using System;
using System.Collections.Generic;
using StoreKit;
using Foundation;
using UIKit;
using CoreGraphics;

using SharedCode;
/*
Prices will appear in the iOS Simulator, but downloads cannot be tested except on a real device

http://developer.apple.com/library/ios/#documentation/NetworkingInternet/Conceptual/StoreKitGuide/DevelopingwithStoreKit/DevelopingwithStoreKit.html

NOTE: Store Kit can be tested in the iOS Simulator, except for hosted content downloads.
*/

namespace NonConsumables
{
	public class HostedProductsViewController : UIViewController
	{
		public static string hostedImagesProductId = "com.xamarin.storekit.hosted.monotouchimages";   //"com.xamarin.storekitdoc.monotouchimages",
		public static string hostedFilesystemProductId = "com.xamarin.storekit.hosted.monotouchfilesystem";                //"com.xamarin.storekitdoc.monotouchfilesystem";

		UIButton hostedImagesButton, hostedFilesystemButton, restoreButton;
		UILabel hostedImagesTitle, hostedImagesDescription, hostedFilesystemTitle, hostedFilesystemDescription;
		UITextView bookTextDisplay;
		UIImageView bookIcon;
		List<string> products;
		bool pricesLoaded = false;
		bool hostedImagesPurchased, hostedFilesystemPurchased;
		NSObject priceObserver, requestObserver;

		CustomPaymentObserver theObserver;
		InAppPurchaseManager iap;

		public HostedProductsViewController ()
		{
			// two products for sale on this page
			products = new List<string>() {hostedImagesProductId, hostedFilesystemProductId};
			iap = new InAppPurchaseManager();
			theObserver = new CustomPaymentObserver(iap);

			// Call this once upon startup of in-app-purchase activities
			// This also kicks off the TransactionObserver which handles the various communications
			SKPaymentQueue.DefaultQueue.AddTransactionObserver(theObserver);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			#region UI layout stuff, not relevant to example
			Title = "Hosted Products";
			View.BackgroundColor = UIColor.White;

			hostedImagesButton = UIButton.FromType (UIButtonType.RoundedRect);
			hostedImagesButton.SetTitle ("loading...", UIControlState.Disabled);
			hostedImagesButton.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
			hostedImagesButton.SetTitle ("Buy...", UIControlState.Normal);
			hostedImagesButton.Enabled = false;

			hostedFilesystemButton = UIButton.FromType (UIButtonType.RoundedRect);
			hostedFilesystemButton.SetTitle ("loading...", UIControlState.Disabled);
			hostedFilesystemButton.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
			hostedFilesystemButton.SetTitle ("Buy...", UIControlState.Normal);
			hostedFilesystemButton.Enabled = false;

			hostedImagesTitle = new UILabel(new CGRect(10, 5, 300, 30));
			hostedImagesTitle.Font = UIFont.BoldSystemFontOfSize(18f);
			hostedImagesDescription = new UILabel(new CGRect(10, 30, 300, 30));
			hostedImagesButton.Frame = new CGRect(10, 65, 180, 40);

			hostedFilesystemTitle = new UILabel(new CGRect(10, 110, 300, 30));
			hostedFilesystemTitle.Font = UIFont.BoldSystemFontOfSize(18f);
			hostedFilesystemDescription = new UILabel(new CGRect(10, 135, 300, 30));
			hostedFilesystemButton.Frame = new CGRect(10, 170, 180, 40);

			restoreButton = UIButton.FromType (UIButtonType.RoundedRect);
			restoreButton.SetTitle ("Restore", UIControlState.Normal);
			restoreButton.Frame = new CGRect(200, 170, 110, 40);

			bookTextDisplay = new UITextView(new CGRect(10, 215, 300, 200));
			bookTextDisplay.Text = "";
			bookTextDisplay.ScrollEnabled = true;
			bookTextDisplay.Editable = false;
			bookIcon = new UIImageView(new CGRect(240, 210, 60, 60));

			View.AddSubview (hostedImagesButton);
			View.AddSubview (hostedImagesTitle);
			View.AddSubview (hostedImagesDescription);
			View.AddSubview (hostedFilesystemButton);
			View.AddSubview (hostedFilesystemTitle);
			View.AddSubview (hostedFilesystemDescription);
			View.AddSubview (restoreButton);
			View.AddSubview (bookTextDisplay);
			View.AddSubview (bookIcon);
			#endregion

			hostedImagesButton.TouchUpInside += (sender, e) => {
				if (hostedImagesPurchased) {
					// paid for, therefore allow access
					HostedProductManager.Read (hostedImagesProductId, bookTextDisplay, bookIcon);
				} else {
					// initiate payment
					iap.PurchaseProduct (hostedImagesProductId);
				}
			};
			hostedFilesystemButton.TouchUpInside += (sender, e) => {
				if (hostedFilesystemPurchased) {
					// paid for, therefore allow access
					HostedProductManager.Read (hostedFilesystemProductId, bookTextDisplay, bookIcon);
				} else {
					// initiate payment
					iap.PurchaseProduct (hostedFilesystemProductId);
				}
			};
			restoreButton.TouchUpInside += (sender, e) => {
				iap.Restore();
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);

			// setup the observer to wait for prices to come back from StoreKit <- AppStore
			priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerProductsFetchedNotification,
				(notification) => {
					var info = notification.UserInfo;
					var NSimagesProductId = new NSString (hostedImagesProductId);
					var NSfilesystemProductId = new NSString (hostedFilesystemProductId);

					if (info == null) {
						// if info is null, probably NO valid prices returned, therefore it doesn't exist at all
						hostedImagesDescription.Text = "check iTunes connect setup";
						hostedFilesystemDescription.Text = "check iTunes connect setup";
						hostedImagesButton.SetTitle ("invalid product id", UIControlState.Disabled);
						hostedFilesystemButton.SetTitle ("invalid product id", UIControlState.Disabled);
						return;
					}

					// we only update the button with a price if the user hasn't already purchased it
					if (!hostedImagesPurchased && info.ContainsKey (NSimagesProductId)) {
						pricesLoaded = true;

						var product = (SKProduct)info [NSimagesProductId];
						Print (product);
						SetVisualState(hostedImagesButton, hostedImagesTitle, hostedImagesDescription, product);
					}
					// we only update the button with a price if the user hasn't already purchased it
					if (!hostedFilesystemPurchased && info.ContainsKey (NSfilesystemProductId)) {
						pricesLoaded = true;

						var product = (SKProduct)info[NSfilesystemProductId];
						Print (product);
						SetVisualState(hostedFilesystemButton, hostedFilesystemTitle, hostedFilesystemDescription, product);
					}
				});

			// only if we can make payments, request the prices
			if (iap.CanMakePayments()) {
				// now go get prices, if we don't have them already
				if (!pricesLoaded)
					iap.RequestProductData(products); // async request via StoreKit -> App Store
			} else {
				// can't make payments (purchases turned off in Settings?)
				hostedImagesButton.SetTitle ("AppStore disabled", UIControlState.Disabled);
				hostedFilesystemButton.SetTitle ("AppStore disabled", UIControlState.Disabled);
			}
			// update the buttons before displaying, to reflect past purchases
			UpdateButtons ();

			priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerTransactionSucceededNotification,
			(notification) => {
				// update the buttons after a successful purchase
				UpdateButtons ();
			});

			requestObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerRequestFailedNotification,
			                                                                 (notification) => {
				// TODO:
				Console.WriteLine ("Request Failed");
				hostedImagesButton.SetTitle ("Network down?", UIControlState.Disabled);
				hostedFilesystemButton.SetTitle ("Network down?", UIControlState.Disabled);
			});
		}

		void UpdateButtons () {
			// set whether the user already has purchased these products
			if (HostedProductManager.HasPurchased(hostedImagesProductId)) {
				hostedImagesButton.Enabled = true;
				hostedImagesButton.SetTitle("Read Images Chapter", UIControlState.Normal);
				hostedImagesPurchased = true;
			}
			if (HostedProductManager.HasPurchased(hostedFilesystemProductId)) {
				hostedFilesystemButton.Enabled = true;
				hostedFilesystemButton.SetTitle("Read FileSystem Chapter ", UIControlState.Normal);
				hostedFilesystemPurchased = true;
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			// remove the observer when the view isn't visible
			NSNotificationCenter.DefaultCenter.RemoveObserver (priceObserver);
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
			Console.WriteLine("Product downloadable: {0}", product.Downloadable);	// iOS6
			Console.WriteLine("Product version: {0}", product.DownloadContentVersion);    // iOS6
			if (product.DownloadContentLengths != null)
				Console.WriteLine("Product length: {0}", product.DownloadContentLengths[0]); // iOS6
		}

		void SetVisualState(UIButton button, UILabel title, UILabel description, SKProduct product)
		{
			button.Enabled = true;
			button.SetTitle (string.Format ("Buy {0}", product.LocalizedPrice ()), UIControlState.Normal);

			title.Text = product.LocalizedTitle;
			description.Text = product.LocalizedDescription;
		}
	}
}
