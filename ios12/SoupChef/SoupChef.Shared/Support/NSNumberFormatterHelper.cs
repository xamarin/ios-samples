
namespace SoupKit.Support
{
    using System;
    using Foundation;

    public static class NSNumberFormatterHelper
    {
        public static NSNumberFormatter CurrencyFormatter => new NSNumberFormatter { NumberStyle = NSNumberFormatterStyle.Currency };
    }
}