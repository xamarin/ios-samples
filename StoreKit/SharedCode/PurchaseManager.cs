using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using StoreKit;
using Foundation;

namespace SharedCode
{
	public abstract class PurchaseManager : SKProductsRequestDelegate
	{
		public static readonly NSString InAppPurchaseManagerProductsFetchedNotification = new NSString("InAppPurchaseManagerProductsFetchedNotification");
		public static readonly NSString InAppPurchaseManagerTransactionFailedNotification  = new NSString("InAppPurchaseManagerTransactionFailedNotification");
		public static readonly NSString InAppPurchaseManagerTransactionSucceededNotification  = new NSString("InAppPurchaseManagerTransactionSucceededNotification");
		public static readonly NSString InAppPurchaseManagerRequestFailedNotification = new NSString("InAppPurchaseManagerRequestFailedNotification");

		protected SKProductsRequest ProductsRequest { get; set; }

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

		// received response to RequestProductData - with price,title,description info
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
			Console.WriteLine("PurchaseProduct {0}", appStoreProductId);
			SKPayment payment = SKPayment.PaymentWithProduct (appStoreProductId);
			SKPaymentQueue.DefaultQueue.AddPayment (payment);
		}

		public void FailedTransaction (SKPaymentTransaction transaction)
		{
			//SKErrorPaymentCancelled == 2
			string errorDescription = transaction.Error.Code == 2 ? "User CANCELLED FailedTransaction" : "FailedTransaction";
			Console.WriteLine("{0} Code={1} {2}", errorDescription, transaction.Error.Code, transaction.Error.LocalizedDescription);

			FinishTransaction(transaction, false);
		}

		public void CompleteTransaction (SKPaymentTransaction transaction)
		{
			Console.WriteLine ("CompleteTransaction {0}", transaction.TransactionIdentifier);
			string productId = transaction.Payment.ProductIdentifier;

			// Register the purchase, so it is remembered for next time
			CompleteTransaction (productId);
			FinishTransaction (transaction, true);
		}

		protected abstract void CompleteTransaction (string productId);

		public void FinishTransaction(SKPaymentTransaction transaction, bool wasSuccessful)
		{
			Console.WriteLine ("FinishTransaction {0}", wasSuccessful);
			// remove the transaction from the payment queue.
			SKPaymentQueue.DefaultQueue.FinishTransaction (transaction);		// THIS IS IMPORTANT - LET'S APPLE KNOW WE'RE DONE !!!!

			NSDictionary userInfo = new NSDictionary ("transaction", transaction);
			var notificationKey = wasSuccessful ? InAppPurchaseManagerTransactionSucceededNotification : InAppPurchaseManagerTransactionFailedNotification;
			NSNotificationCenter.DefaultCenter.PostNotificationName (notificationKey, this, userInfo);
		}

		/// <summary>
		/// Probably could not connect to the App Store (network unavailable?)
		/// </summary>
		public override void RequestFailed (SKRequest request, NSError error)
		{
			Console.WriteLine (" ** RequestFailed ** {0}", error.LocalizedDescription);

			// send out a notification for the failed transaction
			NSDictionary userInfo = new NSDictionary ("error", error);
			NSNotificationCenter.DefaultCenter.PostNotificationName (InAppPurchaseManagerRequestFailedNotification, this, userInfo);
		}

		/// <summary>
		/// Restore any transactions that occurred for this Apple ID, either on
		/// this device or any other logged in with that account.
		/// </summary>
		public void Restore()
		{
			Console.WriteLine (" ** Restore **");
			// theObserver will be notified of when the restored transactions start arriving <- AppStore
			SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();
		}

		public virtual void RestoreTransaction (SKPaymentTransaction transaction)
		{
			// Restored Transactions always have an 'original transaction' attached
			Console.WriteLine("RestoreTransaction {0}; OriginalTransaction {1}",transaction.TransactionIdentifier, transaction.OriginalTransaction.TransactionIdentifier);
			string productId = transaction.OriginalTransaction.Payment.ProductIdentifier;
			// Register the purchase, so it is remembered for next time
			RestoreTransaction (productId);
			FinishTransaction(transaction, true);
		}

		protected virtual void RestoreTransaction (string productId)
		{

		}
	}
}
