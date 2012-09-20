using System;
using System.Collections.Generic;
using MonoTouch.StoreKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace NonConsumables {
	public class HostedProductsViewController : UIViewController {
		public static string oldeStyleProductId = "com.xamarin.storekitdoc.monotouchimages",
		eightBitProductId = "com.xamarin.storekitdoc.monotouchfilesystem";

		UIButton oldStyleButton, eightBitButton, restoreButton;
		UILabel oldStyleTitle, oldStyleDescription, eightBitTitle, eightBitDescription;
		UITextView bookTextDisplay;
		UIImageView bookIcon;
		List<string> products;
		bool pricesLoaded = false;
		bool oldeStylePurchased, eightBitPurchased;
		NSObject priceObserver, requestObserver;
		
		InAppPurchaseManager iap;
		
		public HostedProductsViewController () : base()
		{
			// two products for sale on this page
			products = new List<string>() {oldeStyleProductId, eightBitProductId};
			iap = new InAppPurchaseManager();
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			#region UI layout stuff, not relevant to example
			Title = "Hosted Products";
			View.BackgroundColor = UIColor.White;

			oldStyleButton = UIButton.FromType (UIButtonType.RoundedRect);
			oldStyleButton.SetTitle ("loading...", UIControlState.Disabled);
			oldStyleButton.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
			oldStyleButton.SetTitle ("Buy...", UIControlState.Normal);
			oldStyleButton.Enabled = false;

			eightBitButton = UIButton.FromType (UIButtonType.RoundedRect);
			eightBitButton.SetTitle ("loading...", UIControlState.Disabled);
			eightBitButton.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
			eightBitButton.SetTitle ("Buy...", UIControlState.Normal);
			eightBitButton.Enabled = false;

			oldStyleTitle = new UILabel(new RectangleF(10, 5, 300, 30));
			oldStyleTitle.Font = UIFont.BoldSystemFontOfSize(18f);
			oldStyleDescription = new UILabel(new RectangleF(10, 30, 300, 30));
			oldStyleButton.Frame = new RectangleF(10, 65, 180, 40);

			eightBitTitle = new UILabel(new RectangleF(10, 110, 300, 30));
			eightBitTitle.Font = UIFont.BoldSystemFontOfSize(18f);
			eightBitDescription = new UILabel(new RectangleF(10, 135, 300, 30));
			eightBitButton.Frame = new RectangleF(10, 170, 180, 40);


			restoreButton = UIButton.FromType (UIButtonType.RoundedRect);
			restoreButton.SetTitle ("Restore", UIControlState.Normal);
			restoreButton.Frame = new RectangleF(200, 170, 110, 40);

			bookTextDisplay = new UITextView(new RectangleF(10, 215, 300, 200));
			bookTextDisplay.Text = "";
			bookTextDisplay.ScrollEnabled = true;
			bookTextDisplay.Editable = false;
			bookIcon = new UIImageView(new RectangleF(240, 210, 60, 60));


			View.AddSubview (oldStyleButton);
			View.AddSubview (oldStyleTitle);
			View.AddSubview (oldStyleDescription);
			View.AddSubview (eightBitButton);			
			View.AddSubview (eightBitTitle);
			View.AddSubview (eightBitDescription);
			View.AddSubview (restoreButton);
			View.AddSubview (bookTextDisplay);
			View.AddSubview (bookIcon);
			#endregion	

			oldStyleButton.TouchUpInside += (sender, e) => {
				if (oldeStylePurchased) {
					// paid for, therefore allow access
					HostedProductManager.Read (oldeStyleProductId, bookTextDisplay, bookIcon);
				} else {
					// initiate payment
					iap.PurchaseProduct (oldeStyleProductId);
				}
			};	
			eightBitButton.TouchUpInside += (sender, e) => {
				if (eightBitPurchased) {
					// paid for, therefore allow access
					HostedProductManager.Read (eightBitProductId, bookTextDisplay, bookIcon);
				} else {
					// initiate payment
					iap.PurchaseProduct (eightBitProductId);
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
				var NSgreyscaleProductId = new NSString(oldeStyleProductId);
				var NSsepiaProductId = new NSString(eightBitProductId);
				
				// we only update the button with a price if the user hasn't already purchased it
				if (!oldeStylePurchased && info.ContainsKey(NSgreyscaleProductId)) {
					pricesLoaded = true;

					var product = (SKProduct) info.ObjectForKey(NSgreyscaleProductId);
					
					Console.WriteLine("Product id: " + product.ProductIdentifier);
					Console.WriteLine("Product title: " + product.LocalizedTitle);
					Console.WriteLine("Product description: " + product.LocalizedDescription);
					Console.WriteLine("Product price: " + product.Price);
					Console.WriteLine("Product l10n price: " + product.LocalizedPrice());	
					Console.WriteLine("Product downloadable: " + product.Downloadable);	// iOS6
					Console.WriteLine("Product version:      " + product.DownloadContentVersion);    // iOS6
					Console.WriteLine("Product length:       " + product.DownloadContentLengths[0]); // iOS6

					oldStyleButton.Enabled = true;
					oldStyleTitle.Text = product.LocalizedTitle;
					oldStyleDescription.Text = product.LocalizedDescription;
					oldStyleButton.SetTitle("Buy " + product.LocalizedPrice(), UIControlState.Normal);
				}
				// we only update the button with a price if the user hasn't already purchased it
				if (!eightBitPurchased && info.ContainsKey(NSsepiaProductId)) {
					pricesLoaded = true;

					var product = (SKProduct) info.ObjectForKey(NSsepiaProductId);
					
					Console.WriteLine("Product id: " + product.ProductIdentifier);
					Console.WriteLine("Product title: " + product.LocalizedTitle);
					Console.WriteLine("Product description: " + product.LocalizedDescription);
					Console.WriteLine("Product price: " + product.Price);
					Console.WriteLine("Product l10n price: " + product.LocalizedPrice());
					Console.WriteLine("Product downloadable: " + product.Downloadable); // iOS6
					Console.WriteLine("Product version:      " + product.DownloadContentVersion);    // iOS6
					Console.WriteLine("Product length:       " + product.DownloadContentLengths[0]); // iOS6

					eightBitButton.Enabled = true;
					eightBitTitle.Text = product.LocalizedTitle;
					eightBitDescription.Text = product.LocalizedDescription;
					eightBitButton.SetTitle("Buy " + product.LocalizedPrice(), UIControlState.Normal);
				}
			});
			
			// only if we can make payments, request the prices
			if (iap.CanMakePayments()) {
				// now go get prices, if we don't have them already
				if (!pricesLoaded)
					iap.RequestProductData(products); // async request via StoreKit -> App Store
			} else {
				// can't make payments (purchases turned off in Settings?)
				oldStyleButton.SetTitle ("AppStore disabled", UIControlState.Disabled);
				eightBitButton.SetTitle ("AppStore disabled", UIControlState.Disabled);
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
				oldStyleButton.SetTitle ("Network down?", UIControlState.Disabled);
				eightBitButton.SetTitle ("Network down?", UIControlState.Disabled);
			});
		}

		void UpdateButtons () {
			// set whether the user already has purchased these products
			if (HostedProductManager.HasPurchased(oldeStyleProductId)) {
				oldStyleButton.Enabled = true;
				oldStyleButton.SetTitle("Read Images Chapter", UIControlState.Normal);
				oldeStylePurchased = true;
			}
			if (HostedProductManager.HasPurchased(eightBitProductId)) {
				eightBitButton.Enabled = true;
				eightBitButton.SetTitle("Read FileSystem Chapter ", UIControlState.Normal);
				eightBitPurchased = true;
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			// remove the observer when the view isn't visible
			NSNotificationCenter.DefaultCenter.RemoveObserver (priceObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (requestObserver);

			base.ViewWillDisappear (animated);
		}
	}
}