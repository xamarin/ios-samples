using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;
using StoreKit;

using Purchase;

namespace Consumables
{
	public class InAppPurchaseManager : PurchaseManager
	{
		protected override void CompleteTransaction (string productId)
		{
			if (productId == ConsumableViewController.Buy5ProductId)
				CreditManager.Add(5); // 5 * qty
			else if (productId == ConsumableViewController.Buy10ProductId)
				CreditManager.Add(10); // 10 * qty
			else
				Console.WriteLine ("Shouldn't happen, there are only two products");
		}

		/// <summary>
		/// Probably could not connect to the App Store (network unavailable?)
		/// </summary>
		public override void RequestFailed (SKRequest request, NSError error)
		{
			Console.WriteLine (" ** InAppPurchaseManager RequestFailed() {0}", error.LocalizedDescription);

			// send out a notification for the failed transaction
			NSDictionary userInfo = new NSDictionary ("error", error);
			NSNotificationCenter.DefaultCenter.PostNotificationName (InAppPurchaseManagerRequestFailedNotification, this, userInfo);
		}
	}
}
