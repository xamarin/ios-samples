using System;
using System.Linq;
using StoreKit;
using Foundation;
using UIKit;

namespace Consumables {
	internal class CustomPaymentObserver : SKPaymentTransactionObserver {
		private InAppPurchaseManager theManager;
		
		public CustomPaymentObserver(InAppPurchaseManager manager)
		{
			theManager = manager;
		}
		
		// called when the transaction status is updated
		public override void UpdatedTransactions (SKPaymentQueue queue, SKPaymentTransaction[] transactions)
		{
			Console.WriteLine ("UpdatedTransactions");
			foreach (SKPaymentTransaction transaction in transactions)
			{
			    switch (transaction.TransactionState)
			    {
			        case SKPaymentTransactionState.Purchased:
			           theManager.CompleteTransaction(transaction);
			            break;
			        case SKPaymentTransactionState.Failed:
			           theManager.FailedTransaction(transaction);
			            break;
// Consumable products do not get Restored, so this is not implemented in this sample.
//			        case SKPaymentTransactionState.Restored:
//			            theManager.RestoreTransaction(transaction);
//			            break;
			        default:
			            break;
			    }
			}
		}
		
		public override void PaymentQueueRestoreCompletedTransactionsFinished(SKPaymentQueue queue)
		{
			throw new NotImplementedException("Consumable product purchases do not get Restored, so this is not implemented in this sample.");
		}
	}
}

