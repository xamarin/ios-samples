using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;
using StoreKit;

using SharedCode;

namespace NonConsumables
{
	public class InAppPurchaseManager : PurchaseManager
	{
		protected override void CompleteTransaction (string productId)
		{
			// Register the purchase, so it is remembered for next time
			HostedProductManager.Purchase(productId);
		}

		protected override void RestoreTransaction (string productId)
		{
			HostedProductManager.Purchase(productId); // it's as though it was purchased again
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
	}
}
