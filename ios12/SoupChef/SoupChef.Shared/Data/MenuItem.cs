
namespace SoupChef.Data
{
    using System;
    using Foundation;
    using SoupChef.Support;
    using System.Globalization;

    /// <summary>
    /// This type encapsulates the attributes of a soup menu item.
    /// </summary>
    public class MenuItem : ILocalizableShortcut, ILocalizableCurrency
    {
        public MenuItem(string itemName, string shortcutNameKey, float price, string iconImageName, bool isAvailable, bool isDailySpecial)
        {
            ItemName = itemName;
            ShortcutNameKey = shortcutNameKey;
            Price = price;
            IconImageName = iconImageName;
            IsAvailable = isAvailable;
            IsDailySpecial = isDailySpecial;
        }

        public string LocalizedCurrencyValue => NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber(Price) ?? string.Empty;

        public string ShortcutLocalizationKey => ShortcutNameKey;

        public string ItemName { get; set; }

        public string ShortcutNameKey { get; set; }

        public float Price { get; set; }

        public string IconImageName { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsDailySpecial { get; set; }

        // In the Swift version of this sample, `MenuItem` is a struct. Since
        // it's a class in C#, provide a clone method.
        public MenuItem Clone()
        {
            return new MenuItem(ItemName, ShortcutNameKey, Price, IconImageName, IsAvailable, IsDailySpecial);
        }
    }
}