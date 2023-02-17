using System;
using Foundation;
namespace SoupKit.Support {
	public static class NSNumberFormatterHelper {
		public static NSNumberFormatter CurrencyFormatter {
			get {
				var formatter = new NSNumberFormatter ();
				formatter.NumberStyle = NSNumberFormatterStyle.Currency;
				return formatter;
			}
		}
	}
}
