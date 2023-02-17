/*
See LICENSE folder for this sample’s licensing information.

Abstract:
This type encapsulates the attributes of a soup menu item.
*/

using System;
using Foundation;
using SoupKit.Support;
using System.Globalization;

namespace SoupKit.Data {
	public class MenuItem : NSObject, ILocalizable, ILocalizableCurrency, INSCoding {
		public string ItemNameKey { get; set; }
		public NSDecimalNumber Price { get; set; }
		public string IconImageName { get; set; }
		public bool IsAvailable { get; set; }
		public bool IsDailySpecial { get; set; }
		public MenuItem (string itemNameKey, NSDecimalNumber price, string iconImageName, bool isAvailable, bool isDailySpecial)
		{
			ItemNameKey = itemNameKey;
			Price = price;
			IconImageName = iconImageName;
			IsAvailable = isAvailable;
			IsDailySpecial = isDailySpecial;
		}

		public string LocalizedCurrencyValue {
			get {
				return NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber (Price);
			}
		}

		public string LocalizedString {
			get {
				return NSBundleHelper.SoupKitBundle.GetLocalizedString (ItemNameKey, "Menu item title") ?? "";
			}
		}

		// In the Swift version of this sample, `MenuItem` is a struct. Since
		// it's a class in C#, provide a clone method.
		public MenuItem Clone ()
		{
			return new MenuItem (ItemNameKey, Price, IconImageName, IsAvailable, IsDailySpecial);
		}

		// Necessary so this class works as expected in collections.
		[Export ("isEqual:")]
		override public bool IsEqual (NSObject anObject)
		{
			var other = anObject as MenuItem;

			if (other is null) {
				return false;
			}

			if (ReferenceEquals (other, this)) {
				return true;
			}

			bool result =
				(ItemNameKey.Equals (other.ItemNameKey) &&
				 Price.IsEqual (other.Price) &&
				 IconImageName.Equals (other.IconImageName) &&
				 IsAvailable == other.IsAvailable &&
				 IsDailySpecial == other.IsDailySpecial);

			return result;
		}

		// Necessary so this class works as expected in collections.
		public override nuint GetNativeHash ()
		{
			nuint hashValue = (ItemNameKey is null) ? 0 : (nuint) ItemNameKey.GetHashCode ();
			hashValue = hashValue ^ (nuint) Price.GetNativeHash ();
			hashValue = hashValue ^ (IconImageName is null ? 0 : (nuint) IconImageName.GetHashCode ());
			hashValue = hashValue ^ (nuint) IsAvailable.GetHashCode ();
			hashValue = hashValue ^ (nuint) IsDailySpecial.GetHashCode ();
			return hashValue;
		}

		#region INSCoding
		[Export ("initWithCoder:")]
		public MenuItem (NSCoder coder)
		{
			ItemNameKey = (NSString) coder.DecodeObject ("ItemNameKey");
			Price = (NSDecimalNumber) coder.DecodeObject ("Price");
			IconImageName = (NSString) coder.DecodeObject ("IconImageName");
			IsAvailable = (bool) coder.DecodeBool ("IsAvailable");
			IsDailySpecial = (bool) coder.DecodeBool ("IsDailySpecial");
		}

		public void EncodeTo (NSCoder encoder)
		{
			encoder.Encode ((NSString) ItemNameKey, "ItemNameKey");
			encoder.Encode (Price, "Price");
			encoder.Encode ((NSString) IconImageName, "IconImageName");
			encoder.Encode (IsAvailable, "IsAvailable");
			encoder.Encode (IsDailySpecial, "IsDailySpecial");
		}
		#endregion
	}
}
