using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;
using StoreKit;

using SharedCode;

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

		public override void RestoreTransaction (SKPaymentTransaction transaction)
		{
			throw new InvalidProgramException ("Can't restore transaction");
		}
	}
}
