using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using StoreKit;

using Purchase;

namespace Consumables
{
	public class InAppPurchaseManager : PurchaseManager
	{
		CustomPaymentObserver theObserver;

		public InAppPurchaseManager ()
		{
			theObserver = new CustomPaymentObserver(this);

			// Call this once upon startup of in-app-purchase activities
			// This also kicks off the TransactionObserver which handles the various communications
			SKPaymentQueue.DefaultQueue.AddTransactionObserver(theObserver);
		}

		// Verify that the iTunes account can make this purchase for this application
		public bool CanMakePayments()
		{
			return SKPaymentQueue.CanMakePayments;
		}

		// request multiple products at once
		public void RequestProductData (List<string> productIds)
		{
			NSString[] array = productIds.Select (pId => (NSString)pId).ToArray();
			NSSet productIdentifiers = NSSet.MakeNSObjectSet<NSString>(array);

			//set up product request for in-app purchase
			ProductsRequest  = new SKProductsRequest(productIdentifiers);
			ProductsRequest.Delegate = this; // SKProductsRequestDelegate.ReceivedResponse
			ProductsRequest.Start();
		}

		public override void ReceivedResponse (SKProductsRequest request, SKProductsResponse response)
		{
			SKProduct[] products = response.Products;

			NSMutableDictionary userInfo = new NSMutableDictionary ();
			for (int i = 0; i < products.Length; i++)
				userInfo.Add ((NSString)products [i].ProductIdentifier, products [i]);
			NSNotificationCenter.DefaultCenter.PostNotificationName (InAppPurchaseManagerProductsFetchedNotification, this, userInfo);

			foreach (string invalidProductId in response.InvalidProducts)
				Console.WriteLine ("Invalid product id: {0}", invalidProductId);
		}

		public void PurchaseProduct(string appStoreProductId)
		{
			Console.WriteLine("PurchaseProduct " + appStoreProductId);
			//SKMutablePayment payment = SKMutablePayment.PaymentWithProduct (appStoreProductId);
			//payment.Quantity = 4;
			SKPayment payment = SKPayment.PaymentWithProduct (appStoreProductId);
			SKPaymentQueue.DefaultQueue.AddPayment (payment);
		}

		public void CompleteTransaction (SKPaymentTransaction transaction)
		{
			Console.WriteLine("CompleteTransaction " + transaction.TransactionIdentifier);
			var productId = transaction.Payment.ProductIdentifier;
			//var qty = transaction.Payment.Quantity;
			if (productId == ConsumableViewController.Buy5ProductId)
				CreditManager.Add(5); // 5 * qty
			else if (productId == ConsumableViewController.Buy10ProductId)
				CreditManager.Add(10); // 10 * qty
			else
				Console.WriteLine ("Shouldn't happen, there are only two products");

			FinishTransaction(transaction, true);
		}
		public void FailedTransaction (SKPaymentTransaction transaction)
		{
			//SKErrorPaymentCancelled == 2
			string errorDescription = transaction.Error.Code == 2 ? "User CANCELLED FailedTransaction" : "FailedTransaction";
			Console.WriteLine("{0} Code={1} {2}", errorDescription, transaction.Error.Code, transaction.Error.LocalizedDescription);

			FinishTransaction(transaction,false);
		}
		public void FinishTransaction(SKPaymentTransaction transaction, bool wasSuccessful)
		{
			Console.WriteLine ("FinishTransaction {0}", wasSuccessful);
			// remove the transaction from the payment queue.
			SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);		// THIS IS IMPORTANT - LET'S APPLE KNOW WE'RE DONE !!!!

			using (var pool = new NSAutoreleasePool()) {
				NSDictionary userInfo = new NSDictionary ("transaction", transaction);
				if (wasSuccessful) {
					// send out a notification that we’ve finished the transaction
					NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerTransactionSucceededNotification,this,userInfo);
				} else {
					// send out a notification for the failed transaction
					NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerTransactionFailedNotification,this,userInfo);
				}
			}
		}

		/// <summary>
		/// Probably could not connect to the App Store (network unavailable?)
		/// </summary>
		public override void RequestFailed (SKRequest request, NSError error)
		{
			Console.WriteLine (" ** InAppPurchaseManager RequestFailed() {0}", error.LocalizedDescription);
			using (var pool = new NSAutoreleasePool()) {
				NSDictionary userInfo = new NSDictionary ("error", error);
				// send out a notification for the failed transaction
				NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerRequestFailedNotification,this,userInfo);
			}
		}
	}
}
