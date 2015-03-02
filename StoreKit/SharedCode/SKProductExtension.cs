using System;
using StoreKit;
using Foundation;
using UIKit;

namespace SharedCode
{
	public static class SKProductExtension
	{
		public static string LocalizedPrice (this SKProduct product)
		{
			var formatter = new NSNumberFormatter {
				FormatterBehavior = NSNumberFormatterBehavior.Version_10_4,
				NumberStyle = NSNumberFormatterStyle.Currency,
				Locale = product.PriceLocale,
			};

			string formattedString = formatter.StringFromNumber (product.Price);
			return formattedString;
		}
	}
}
