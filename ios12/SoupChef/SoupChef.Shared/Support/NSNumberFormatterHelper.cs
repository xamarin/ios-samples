
namespace SoupChef.Support
{
    using System;
    using Foundation;

    /// <summary>
    /// Convenience utility to format numbers as currency
    /// </summary>
    public static class NSNumberFormatterHelper
    {
        public static NSNumberFormatter CurrencyFormatter => new NSNumberFormatter { NumberStyle = NSNumberFormatterStyle.Currency };
    }
}