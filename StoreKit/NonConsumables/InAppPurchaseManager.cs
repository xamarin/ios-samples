using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;
using StoreKit;

using Purchase;

namespace NonConsumables
{
	public class InAppPurchaseManager : PurchaseManager
	{
		protected override void CompleteTransaction (string productId)
		{
			// Register the purchase, so it is remembered for next time
			PhotoFilterManager.Purchase(productId);
		}

		public void RestoreTransaction (SKPaymentTransaction transaction)
		{
			// Restored Transactions always have an 'original transaction' attached
			Console.WriteLine("RestoreTransaction " + transaction.TransactionIdentifier + "; OriginalTransaction " + transaction.OriginalTransaction.TransactionIdentifier);
			var productId = transaction.OriginalTransaction.Payment.ProductIdentifier;
			// Register the purchase, so it is remembered for next time
			PhotoFilterManager.Purchase(productId); // it's as though it was purchased again
			FinishTransaction(transaction, true);
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
