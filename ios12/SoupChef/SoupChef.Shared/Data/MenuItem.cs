
using System;
using Foundation;
using SoupKit.Support;
using System.Globalization;

namespace SoupKit.Data
{
    /// <summary>
    /// This type encapsulates the attributes of a soup menu item.
    /// </summary>
    public class MenuItem : /*NSObject, */ILocalizableShortcut, ILocalizableCurrency//, INSCoding
    {
        public MenuItem(string itemName, string shortcutNameKey, NSDecimalNumber price, string iconImageName, bool isAvailable, bool isDailySpecial)
        {
            ItemName = itemName;
            ShortcutNameKey = shortcutNameKey;
            Price = price;
            IconImageName = iconImageName;
            IsAvailable = isAvailable;
            IsDailySpecial = isDailySpecial;
        }

        public string ItemName { get; set; }
        public string ShortcutNameKey { get; set; }
        public NSDecimalNumber Price { get; set; }
        public string IconImageName { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDailySpecial { get; set; }

        public string LocalizedCurrencyValue => NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber(Price) ?? string.Empty;

        public string ShortcutLocalizationKey => ShortcutNameKey;

        // In the Swift version of this sample, `MenuItem` is a struct. Since
        // it's a class in C#, provide a clone method.
        public MenuItem Clone()
        {
            return new MenuItem(ItemName, ShortcutNameKey, Price, IconImageName, IsAvailable, IsDailySpecial);
        }

        // Necessary so this class works as expected in collections.
        //[Export("isEqual:")]
        //override public bool IsEqual(NSObject anObject)
        //{
        //    var other = anObject as MenuItem;

        //    if (other is null)
        //    {
        //        return false;
        //    }

        //    if (ReferenceEquals(other, this))
        //    {
        //        return true;
        //    }

        //    bool result = 
        //        (ItemName.Equals(other.ItemName) &&
        //         Price.IsEqual(other.Price) &&
        //         IconImageName.Equals(other.IconImageName) &&
        //         IsAvailable == other.IsAvailable &&
        //         IsDailySpecial == other.IsDailySpecial);

        //    return result;
        //}

        //// Necessary so this class works as expected in collections.
        //public override nuint GetNativeHash()
        //{
        //    nuint hashValue = (ItemName is null) ? 0 : (nuint)ItemName.GetHashCode();
        //    hashValue = hashValue ^ (nuint)Price.GetNativeHash();
        //    hashValue = hashValue ^ (IconImageName is null ? 0 : (nuint)IconImageName.GetHashCode());
        //    hashValue = hashValue ^ (nuint)IsAvailable.GetHashCode();
        //    hashValue = hashValue ^ (nuint)IsDailySpecial.GetHashCode();
        //    return hashValue;
        //}

        //#region INSCoding
        //[Export("initWithCoder:")]
        //public MenuItem(NSCoder coder)
        //{
        //    ItemName = (NSString)coder.DecodeObject("ItemNameKey");
        //    Price = (NSDecimalNumber)coder.DecodeObject("Price");
        //    IconImageName = (NSString)coder.DecodeObject("IconImageName");
        //    IsAvailable = (bool)coder.DecodeBool("IsAvailable");
        //    IsDailySpecial = (bool)coder.DecodeBool("IsDailySpecial");
        //}

        //public void EncodeTo(NSCoder encoder)
        //{
        //    encoder.Encode((NSString)ItemName, "ItemNameKey");
        //    encoder.Encode(Price, "Price");
        //    encoder.Encode((NSString)IconImageName, "IconImageName");
        //    encoder.Encode(IsAvailable, "IsAvailable");
        //    encoder.Encode(IsDailySpecial, "IsDailySpecial");
        //}
        //#endregion
    }
}
