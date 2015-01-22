using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using StoreKit;

using Purchase;

namespace NonConsumables
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

		// received response to RequestProductData - with price,title,description info
		public override void ReceivedResponse (SKProductsRequest request, SKProductsResponse response)
		{
			SKProduct[] products = response.Products;

			NSMutableDictionary userInfo = new NSMutableDictionary ();
			for (int i = 0; i < products.Length; i++)
				userInfo.Add ((NSString)products [i].ProductIdentifier, products [i]);
			NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerProductsFetchedNotification,this,userInfo);

			foreach (string invalidProductId in response.InvalidProducts) {
				Console.WriteLine("Invalid product id: " + invalidProductId );
			}
		}

		public void PurchaseProduct(string appStoreProductId)
		{
			Console.WriteLine("PurchaseProduct " + appStoreProductId);
			SKPayment payment = SKPayment.PaymentWithProduct (appStoreProductId);
			SKPaymentQueue.DefaultQueue.AddPayment (payment);
		}

		/// <summary>
		/// New iOS6 method to save downloads of hosted-content
		/// </summary>
		public void SaveDownload (SKDownload download)
		{
			Console.WriteLine ("SaveDownload " + download.ContentIdentifier);
			Console.WriteLine ("  from   " + download.ContentUrl.Path);
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			var targetfolder = System.IO.Path.Combine (documentsPath, download.Transaction.Payment.ProductIdentifier);
			Console.WriteLine ("  to  " + targetfolder);

			if (!System.IO.Directory.Exists (targetfolder))
				System.IO.Directory.CreateDirectory (targetfolder);

			foreach (var file in System.IO.Directory.EnumerateFiles
			         (System.IO.Path.Combine(download.ContentUrl.Path,"Contents"))) {
				Console.WriteLine (" file to copy  " + file);

				var fileName = file.Substring (file.LastIndexOf ("/") + 1);
				var newFilePath = System.IO.Path.Combine(targetfolder, fileName);

				if (!System.IO.File.Exists(newFilePath))
					System.IO.File.Copy (file, newFilePath);
				else
					Console.WriteLine ("already exists " + newFilePath);
			}

			CompleteTransaction (download.Transaction); // so it gets 'finished'
		}

		public void CompleteTransaction (SKPaymentTransaction transaction)
		{
			Console.WriteLine ("CompleteTransaction " + transaction.TransactionIdentifier);
			var productId = transaction.Payment.ProductIdentifier;
			// Register the purchase, so it is remembered for next time
			HostedProductManager.Purchase(productId);

			FinishTransaction (transaction, true);
		}
		public void RestoreTransaction (SKPaymentTransaction transaction)
		{
			// Restored Transactions always have an 'original transaction' attached
			Console.WriteLine("RestoreTransaction " + transaction.TransactionIdentifier + "; OriginalTransaction " + transaction.OriginalTransaction.TransactionIdentifier);
			var productId = transaction.OriginalTransaction.Payment.ProductIdentifier;
			// Register the purchase, so it is remembered for next time
			HostedProductManager.Purchase(productId); // it's as though it was purchased again

			FinishTransaction(transaction, true);
		}
		public void FailedTransaction (SKPaymentTransaction transaction)
		{
			//SKErrorPaymentCancelled == 2
			if (transaction.Error.Code == 2) // user cancelled
				Console.WriteLine("User CANCELLED FailedTransaction Code=" + transaction.Error.Code + " " + transaction.Error.LocalizedDescription);
			else // error!
				Console.WriteLine("FailedTransaction Code=" + transaction.Error.Code + " " + transaction.Error.LocalizedDescription);

			FinishTransaction(transaction,false);
		}
		public void FinishTransaction(SKPaymentTransaction transaction, bool wasSuccessful)
		{
			Console.WriteLine("FinishTransaction " + wasSuccessful);
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
			Console.WriteLine (" ** InAppPurchaseManager RequestFailed() " + error.LocalizedDescription);
			using (var pool = new NSAutoreleasePool()) {
				NSDictionary userInfo = new NSDictionary ("error", error);
				// send out a notification for the failed transaction
				NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerRequestFailedNotification,this,userInfo);
			}
		}

		/// <summary>
		/// Restore any transactions that occurred for this Apple ID, either on
		/// this device or any other logged in with that account.
		/// </summary>
		public void Restore()
		{
			Console.WriteLine (" ** InAppPurchaseManager Restore()");
			// theObserver will be notified of when the restored transactions start arriving <- AppStore
			SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();
		}
	}
}
