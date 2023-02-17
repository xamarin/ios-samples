using System;
using System.Collections.Generic;

using UIKit;
using StoreKit;
using Foundation;
using CoreGraphics;

using SharedCode;

namespace NonConsumables {
	public class NonConsumableViewController : UIViewController {
		public static string greyscaleProductId = "com.xamarin.storekit.testing.greyscale";
		public static string sepiaProductId = "com.xamarin.storekit.testing.sepia";

		string testImagePath = "Images/PhotoFilterTest2.jpg";
		UIButton greyscaleButton, sepiaButton, clearButton, restoreButton;
		UILabel greyscaleTitle, greyscaleDescription, sepiaTitle, sepiaDescription, infoLabel;
		UIImageView testFilterImage;
		List<string> products;
		bool pricesLoaded = false;
		bool greyscalePurchased, sepiaPurchased;
		NSObject priceObserver, requestObserver;

		CustomPaymentObserver theObserver;
		InAppPurchaseManager iap;

		public NonConsumableViewController ()
		{
			// two products for sale on this page
			products = new List<string> () { greyscaleProductId, sepiaProductId };

			iap = new InAppPurchaseManager ();
			theObserver = new CustomPaymentObserver (iap);

			// Call this once upon startup of in-app-purchase activities
			// This also kicks off the TransactionObserver which handles the various communications
			SKPaymentQueue.DefaultQueue.AddTransactionObserver (theObserver);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				EdgesForExtendedLayout = UIRectEdge.None;
			}

			#region UI layout stuff, not relevant to example
			Title = "Non-Consumable Products";
			View.BackgroundColor = UIColor.White;

			greyscaleButton = UIButton.FromType (UIButtonType.RoundedRect);
			greyscaleButton.SetTitle ("loading...", UIControlState.Disabled);
			greyscaleButton.SetTitleColor (UIColor.Gray, UIControlState.Disabled);
			greyscaleButton.SetTitle ("Buy...", UIControlState.Normal);
			greyscaleButton.Enabled = false;

			sepiaButton = UIButton.FromType (UIButtonType.RoundedRect);
			sepiaButton.SetTitle ("loading...", UIControlState.Disabled);
			sepiaButton.SetTitleColor (UIColor.Gray, UIControlState.Disabled);
			sepiaButton.SetTitle ("Buy...", UIControlState.Normal);
			sepiaButton.Enabled = false;

			greyscaleTitle = new UILabel (new CGRect (10, 5, 300, 30));
			greyscaleTitle.Font = UIFont.BoldSystemFontOfSize (18f);
			greyscaleDescription = new UILabel (new CGRect (10, 30, 300, 30));
			greyscaleButton.Frame = new CGRect (10, 65, 180, 40);

			sepiaTitle = new UILabel (new CGRect (10, 110, 300, 30));
			sepiaTitle.Font = UIFont.BoldSystemFontOfSize (18f);
			sepiaDescription = new UILabel (new CGRect (10, 135, 300, 30));
			sepiaButton.Frame = new CGRect (10, 170, 180, 40);

			clearButton = UIButton.FromType (UIButtonType.RoundedRect);
			clearButton.SetTitle ("Clear Filter", UIControlState.Normal);
			clearButton.Frame = new CGRect (10, 215, 180, 40);
			clearButton.TouchUpInside += (sender, args) => {
				testFilterImage.Image = UIImage.FromFile (testImagePath);
			};

			restoreButton = UIButton.FromType (UIButtonType.RoundedRect);
			restoreButton.SetTitle ("Restore", UIControlState.Normal);
			restoreButton.Frame = new CGRect (200, 215, 110, 40);

			testFilterImage = new UIImageView (new CGRect (10, 265, 300, 100));
			testFilterImage.Image = UIImage.FromFile (testImagePath);

			infoLabel = new UILabel (new CGRect (10, 370, 300, 80));
			infoLabel.Lines = 3;
			infoLabel.Text = "Notice how you can only purchase each product once. After that the transaction can't be charged again.";

			View.AddSubview (greyscaleButton);
			View.AddSubview (greyscaleTitle);
			View.AddSubview (greyscaleDescription);
			View.AddSubview (sepiaButton);
			View.AddSubview (sepiaTitle);
			View.AddSubview (sepiaDescription);
			View.AddSubview (testFilterImage);
			View.AddSubview (clearButton);
			View.AddSubview (restoreButton);
			View.AddSubview (infoLabel);
			#endregion

			greyscaleButton.TouchUpInside += (sender, e) => {
				if (greyscalePurchased) {
					// paid for, therefore allow access
					PhotoFilterManager.ApplyGreyscale (testImagePath, testFilterImage);
				} else {
					// initiate payment
					iap.PurchaseProduct (greyscaleProductId);
				}
			};
			sepiaButton.TouchUpInside += (sender, e) => {
				if (sepiaPurchased) {
					// paid for, therefore allow access
					PhotoFilterManager.ApplySepia (testImagePath, testFilterImage);
				} else {
					// initiate payment
					iap.PurchaseProduct (sepiaProductId);
				}
			};
			restoreButton.TouchUpInside += (sender, e) => {
				iap.Restore ();
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// setup the observer to wait for prices to come back from StoreKit <- AppStore
			priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerProductsFetchedNotification,
				(notification) => {
					NSDictionary info = notification.UserInfo;
					var NSgreyscaleProductId = new NSString (greyscaleProductId);
					var NSsepiaProductId = new NSString (sepiaProductId);

					if (info == null) {
						// if info is null, probably NO valid prices returned, therefore it doesn't exist at all
						greyscaleDescription.Text = "check iTunes connect setup";
						sepiaDescription.Text = "check iTunes connect setup";
						greyscaleButton.SetTitle ("invalid product id", UIControlState.Disabled);
						sepiaButton.SetTitle ("invalid product id", UIControlState.Disabled);
						return;
					}

					// we only update the button with a price if the user hasn't already purchased it
					if (!greyscalePurchased && info.ContainsKey (NSgreyscaleProductId)) {
						pricesLoaded = true;

						var product = (SKProduct) info [NSgreyscaleProductId];
						Print (product);
						SetVisualState (greyscaleButton, greyscaleTitle, greyscaleDescription, product);
					}
					// we only update the button with a price if the user hasn't already purchased it
					if (!sepiaPurchased && info.ContainsKey (NSsepiaProductId)) {
						pricesLoaded = true;

						var product = (SKProduct) info.ObjectForKey (NSsepiaProductId);
						Print (product);
						SetVisualState (sepiaButton, sepiaTitle, sepiaDescription, product);
					}
				});

			// only if we can make payments, request the prices
			if (iap.CanMakePayments ()) {
				// now go get prices, if we don't have them already
				if (!pricesLoaded)
					iap.RequestProductData (products); // async request via StoreKit -> App Store
			} else {
				// can't make payments (purchases turned off in Settings?)
				greyscaleButton.SetTitle ("AppStore disabled", UIControlState.Disabled);
				sepiaButton.SetTitle ("AppStore disabled", UIControlState.Disabled);
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
					greyscaleButton.SetTitle ("Network down?", UIControlState.Disabled);
					sepiaButton.SetTitle ("Network down?", UIControlState.Disabled);
				});
		}

		void UpdateButtons ()
		{
			// set whether the user already has purchased these products
			if (PhotoFilterManager.HasPurchased (greyscaleProductId)) {
				greyscaleButton.Enabled = true;
				greyscaleButton.SetTitle ("Use Greyscale Filter", UIControlState.Normal);
				greyscalePurchased = true;
			}
			if (PhotoFilterManager.HasPurchased (sepiaProductId)) {
				sepiaButton.Enabled = true;
				sepiaButton.SetTitle ("Use Sepia Filter ", UIControlState.Normal);
				sepiaPurchased = true;
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			// remove the observer when the view isn't visible
			NSNotificationCenter.DefaultCenter.RemoveObserver (priceObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (requestObserver);

			base.ViewWillDisappear (animated);
		}

		void Print (SKProduct product)
		{
			Console.WriteLine ("Product id: {0}", product.ProductIdentifier);
			Console.WriteLine ("Product title: {0}", product.LocalizedTitle);
			Console.WriteLine ("Product description: {0}", product.LocalizedDescription);
			Console.WriteLine ("Product price: {0}", product.Price);
			Console.WriteLine ("Product l10n price: {0}", product.LocalizedPrice ());
		}

		void SetVisualState (UIButton button, UILabel title, UILabel description, SKProduct product)
		{
			var btnTitle = string.Format ("Buy {0}", product.LocalizedPrice ());
			button.Enabled = true;
			button.SetTitle (btnTitle, UIControlState.Normal);

			title.Text = product.LocalizedTitle;
			description.Text = product.LocalizedDescription;
		}
	}
}
