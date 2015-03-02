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
			PhotoFilterManager.Purchase(productId);
		}

		protected override void RestoreTransaction (string productId)
		{
			// it's as though it was purchased again
			PhotoFilterManager.Purchase(productId);
		}
	}
}
