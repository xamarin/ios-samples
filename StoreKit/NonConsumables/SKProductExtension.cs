using System;
using StoreKit;
using Foundation;
using UIKit;

namespace NonConsumables {
	public static class SKProductExtension {
		/// <remarks>
		/// Use Apple's sample code for formatting a SKProduct price
		/// https://developer.apple.com/library/ios/#DOCUMENTATION/StoreKit/Reference/SKProduct_Reference/Reference/Reference.html#//apple_ref/occ/instp/SKProduct/priceLocale
		/// Objective-C version:
		///    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
		///    [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
		///    [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
		///    [numberFormatter setLocale:product.priceLocale];
		///    NSString *formattedString = [numberFormatter stringFromNumber:product.price];
		/// </remarks>
		public static string LocalizedPrice (this SKProduct product)
	    {
			var formatter = new NSNumberFormatter ();
			formatter.FormatterBehavior = NSNumberFormatterBehavior.Version_10_4;   
			formatter.NumberStyle = NSNumberFormatterStyle.Currency;
			formatter.Locale = product.PriceLocale;
            var formattedString = formatter.StringFromNumber(product.Price);
            Console.WriteLine(" ** formatter.StringFromNumber("+product.Price+") = " + formattedString + " for locale " + product.PriceLocale.LocaleIdentifier);
			return formattedString;
		}
	}
}