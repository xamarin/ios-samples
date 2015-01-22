using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using StoreKit;
using Foundation;

namespace Purchase
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
	}
}