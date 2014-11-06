using System;
using System.Linq;
using StoreKit;
using Foundation;
using UIKit;

namespace NonConsumables {
	internal class CustomPaymentObserver : SKPaymentTransactionObserver {
		private InAppPurchaseManager theManager;
		
		public CustomPaymentObserver(InAppPurchaseManager manager)
		{
			theManager = manager;
		}

		/// <Docs>
		/// New iOS 6 method to track downloads of hosted-content
		/// </Docs>
		public override void UpdatedDownloads (SKPaymentQueue queue, SKDownload[] downloads)
		{
			Console.WriteLine (" -- PaymentQueueUpdatedDownloads");
			foreach (SKDownload download in downloads) {
				switch (download.DownloadState) {
				case SKDownloadState.Active:
					Console.WriteLine ("Download progress:" + download.Progress);
					Console.WriteLine ("Time remaining:   " + download.TimeRemaining);
					break;
				case SKDownloadState.Finished:
					Console.WriteLine ("Finished!!!!");
					Console.WriteLine ("Content URL:" + download.ContentUrl);

					// UNPACK HERE!
					theManager.SaveDownload (download);

					break;
				case SKDownloadState.Failed:
					Console.WriteLine ("Failed");
					break;
				case SKDownloadState.Cancelled:
					Console.WriteLine ("Cancelled");
					break;
				case SKDownloadState.Paused:
				case SKDownloadState.Waiting:
					break;
				default:
					break;
				}
			}
		}

		/// <summary>
		/// Called when the transaction status is updated
		/// </summary>
		public override void UpdatedTransactions (SKPaymentQueue queue, SKPaymentTransaction[] transactions)
		{
			Console.WriteLine ("UpdatedTransactions");
			foreach (SKPaymentTransaction transaction in transactions) {
			    switch (transaction.TransactionState) {
			        case SKPaymentTransactionState.Purchased:
						if (transaction.Downloads != null && transaction.Downloads.Length > 0) {
							// complete the transaction AFTER downloading
							SKPaymentQueue.DefaultQueue.StartDownloads (transaction.Downloads);
						} else {
							// complete the transaction now
			         	  theManager.CompleteTransaction(transaction);
						}
			            break;
			        case SKPaymentTransactionState.Failed:
			           theManager.FailedTransaction(transaction);
			            break;
			        case SKPaymentTransactionState.Restored:
			            theManager.RestoreTransaction(transaction);
			            break;
			        default:
			            break;
			    }
			}
		}
		
		public override void PaymentQueueRestoreCompletedTransactionsFinished (SKPaymentQueue queue)
		{
			// Restore succeeded
			Console.WriteLine(" ** RESTORE PaymentQueueRestoreCompletedTransactionsFinished ");
		}
		public override void RestoreCompletedTransactionsFailedWithError (SKPaymentQueue queue, NSError error)
		{
			// Restore failed somewhere...
			Console.WriteLine(" ** RESTORE RestoreCompletedTransactionsFailedWithError " + error.LocalizedDescription);
		}
	}
}

