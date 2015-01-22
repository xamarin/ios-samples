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
			HostedProductManager.Purchase(productId);
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

		public void RestoreTransaction (SKPaymentTransaction transaction)
		{
			// Restored Transactions always have an 'original transaction' attached
			Console.WriteLine("RestoreTransaction " + transaction.TransactionIdentifier + "; OriginalTransaction " + transaction.OriginalTransaction.TransactionIdentifier);
			var productId = transaction.OriginalTransaction.Payment.ProductIdentifier;
			// Register the purchase, so it is remembered for next time
			HostedProductManager.Purchase(productId); // it's as though it was purchased again

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
