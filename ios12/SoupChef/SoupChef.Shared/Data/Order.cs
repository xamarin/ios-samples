/*
See LICENSE folder for this sample’s licensing information.

Abstract:
This type encapsulates the attributes of a soup order.
*/

using System;
using Foundation;
using SoupKit.Support;
using System.Linq;
using SoupChef;
using Intents;
using UIKit;
using System.Collections.Generic;

namespace SoupKit.Data
{
    //public class MenuItemOption : NSObject, ILocalizable, INSCoding
    //{
    //    public const string Cheese = "CHEESE";
    //    public const string RedPepper = "RED_PEPPER";
    //    public const string Croutons = "CROUTONS";

    //    public static string[] All = new string[] { Cheese, RedPepper, Croutons };

    //    public string LocalizedString
    //    {
    //        get
    //        {
    //            string usageComment = $"UI representation for MenuItemOption value: {Value}";
    //            return NSBundle.MainBundle.GetLocalizedString(Value, usageComment, null);
    //        }
    //    }

    //    public MenuItemOption(string value)
    //    {
    //        if (All.Contains(value))
    //        {
    //            _value = value;
    //        }
    //        else
    //        {
    //            throw new ArgumentException($"Invalid menuItemOption value: {value}");
    //        }
    //    }

    //    string _value;
    //    public string Value
    //    {
    //        get { return _value; }
    //    }

    //    [Export("isEqual:")]
    //    override public bool IsEqual(NSObject anObject)
    //    {
    //        var other = anObject as MenuItemOption;
    //        if (other is null)
    //        {
    //            return false;
    //        }
    //        if (ReferenceEquals(other, this))
    //        {
    //            return true;
    //        }
    //        return Value.Equals(other.Value);
    //    }

    //    public override nuint GetNativeHash()
    //    {
    //        return !(Value is null) ? (nuint)this.Value?.GetHashCode() : 0;
    //    }

    //    #region INSCoding
    //    [Export("initWithCoder:")]
    //    public MenuItemOption(NSCoder coder)
    //    {
    //        _value = (NSString)coder.DecodeObject("Value");
    //    }

    //    public void EncodeTo(NSCoder encoder)
    //    {
    //        encoder.Encode((NSString)_value, "Value");
    //    }
    //    #endregion
    //}

    enum MenuItemOption
    {
        cheese = 0,// = "Cheese"
        redPepper = 1,// = "Red Pepper"
        croutons = 2,// = "Croutons"
    }

    /// <summary>
    /// This type encapsulates the attributes of a soup order.
    /// </summary>
    class Order :/* NSObject,*/ ILocalizableCurrency//: NSObject, ILocalizableShortcutString, INSCoding
    {
        public Order(NSDate date, NSUuid identifier, int quantity, MenuItem menuItem, List<MenuItemOption> menuItemOptions)
        {
            Date = date;
            Identifier = identifier;
            MenuItem = menuItem;
            Quantity = quantity;
            MenuItemOptions = menuItemOptions;
        }

        public NSDate Date { get; private set; }
        public NSUuid Identifier { get; private set; }
        public MenuItem MenuItem { get; private set; }
        public int Quantity { get; set; }
        public List<MenuItemOption> MenuItemOptions { get; set; }
        public float Total => Quantity * MenuItem.Price;

        public override bool Equals(object obj)
        {
            var rhs = obj as Order;

            return MenuItem == rhs.MenuItem &&
            rhs.Quantity == Quantity &&
            rhs.MenuItemOptions == MenuItemOptions;
        }

        // SoupChef considers orders with the same contents (menuItem, quantity, 
        // menuItemOptions) to be identical. The data and idenfier properties 
        // are unique to an instance of an order (regardless of contents) and 
        // are not considered when determining equality.
        //[Export("isEqual:")]
        //override public bool IsEqual(NSObject anObject)
        //{
        //    var other = anObject as Order;

        //    if (other is null)
        //    {
        //        return false;
        //    }

        //    if (ReferenceEquals(other, this))
        //    {
        //        return true;
        //    }

        //    // MenuItem check
        //    if (MenuItem is null)
        //    {
        //        if (!(other.MenuItem is null))
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (!MenuItem.IsEqual(other.MenuItem))
        //        {
        //            return false;
        //        }
        //    }

        //    // Quantity check
        //    if (Quantity != other.Quantity)
        //    {
        //        return false;
        //    }

        //    // MenuItemOptions check
        //    //if (MenuItemOptions is null)
        //    //{
        //    //    if (!(other.MenuItemOptions is null))
        //    //    {
        //    //        return false;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    if (!MenuItemOptions.IsEqualToSet(other.MenuItemOptions))
        //    //    {
        //    //        return false;
        //    //    }
        //    //}

        //    return true;
        //}

        //public override nuint GetNativeHash()
        //{
        //    nuint hashValue = (MenuItem is null) ? 0 : MenuItem.GetNativeHash();
        //    hashValue = hashValue ^ (nuint)Quantity.GetHashCode();
        //    //hashValue = hashValue ^ (MenuItemOptions is null ? 0 : MenuItemOptions.GetNativeHash());
        //    return hashValue;
        //}

        public string LocalizedCurrencyValue
        {
            get
            {
                return NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber(Total) ?? string.Empty;
            }
        }

        #region intent helpers
        public OrderSoupIntent Intent
        {
            get
            {
                var orderSoupIntent = new OrderSoupIntent();
                orderSoupIntent.Quantity = new NSNumber(Quantity);

                //var displayString = NSString.DeferredLocalizedIntentsString(with: menuItem.shortcutLocalizationKey) as String
                //orderSoupIntent.setImage(INImage(named: menuItem.iconImageName), forParameterNamed: \OrderSoupIntent.soup)

                orderSoupIntent.Soup = new INObject(MenuItem.ItemName, MenuItem.ShortcutNameKey);
                orderSoupIntent.SetImage(INImage.FromName(MenuItem.IconImageName), "soup");
                //var image = UIImage.FromBundle(MenuItem.IconImageName);
                //if (image != null)
                //{
                //    var data = image.AsPNG();
                //    orderSoupIntent.SetImage(INImage.FromData(data), "soup");
                //}

                orderSoupIntent.Options = NSArray.FromObjects(MenuItemOptions.Select(arg => new INObject(arg.ToString(), arg.ToString())).ToArray()) as NSArray<INObject>;

                var phrase = NSBundle.MainBundle.GetLocalizedString("ORDER_SOUP_SUGGESTED_PHRASE", "Suggested phrase for ordering a specific soup");
                orderSoupIntent.SuggestedInvocationPhrase = string.Format(phrase, MenuItem.ShortcutLocalizationKey);
                //orderSoupIntent.suggestedInvocationPhrase = NSString.deferredLocalizedIntentsString(with: "ORDER_SOUP_SUGGESTED_PHRASE") as String

                return orderSoupIntent;
            }
        }

        public static Order FromOrderSoupIntent(OrderSoupIntent intent)
        {
            Order result = null;
            var menuManager = new SoupMenuManager();

            var soupID = intent.Soup?.Identifier;
            if (!string.IsNullOrEmpty(soupID))
            {
                var menuItem = menuManager.FindItem(soupID);
                if (menuItem != null)
                {
                    var quantity = intent.Quantity;

                    List<MenuItemOption> rawOptions;
                    if (intent.Options is null)
                    {
                        rawOptions = new List<MenuItemOption>();
                    }
                    else
                    {
                        // For the equivalent code in Apple's Swift sample, compactMap
                        // is used. This eliminates nil values from the final result. 
                        // Here, LINQ's Where method is used to filter out the null 
                        // values.
                        rawOptions = intent.Options.Select<INObject, MenuItemOption>((option) =>
                        {
                            var optionID = (MenuItemOption)Enum.Parse(typeof(MenuItemOption), option.Identifier, true);

                            //var optionID = option.Identifier;
                            //return string.IsNullOrEmpty(optionID) ? null : new MenuItemOption(optionID);
                            return optionID;
                        }).ToList();
                    }

                    result = new Order(new NSDate(), new NSUuid(), quantity.Int32Value, menuItem, new List<MenuItemOption>(rawOptions));
                }
            }

            return result;
        }
        #endregion

        //#region INSCoding
        //[Export("initWithCoder:")]
        //public Order(NSCoder coder)
        //{
        //    Date = (NSDate)coder.DecodeObject("Date");
        //    Identifier = (NSUuid)coder.DecodeObject("Identifier");
        //    MenuItem = (MenuItem)coder.DecodeObject("MenuItem");
        //    Quantity = (int)coder.DecodeInt("Quantity");

        //    // Can't decode an NSMutableSet<MenuItemOption> directly. Get an
        //    // NSSet, convert it to NSMutableSet<MenuItemOption>
        //    //var set = (NSSet)(coder.DecodeObject("MenuItemOptions"));
        //    //MenuItemOptions = new NSMutableSet<MenuItemOption>(set.ToArray<MenuItemOption>());
        //}

        //public void EncodeTo(NSCoder encoder)
        //{
        //    encoder.Encode(Date, "Date");
        //    encoder.Encode(Identifier, "Identifier");
        //    encoder.Encode(MenuItem, "MenuItem");
        //    encoder.Encode(Quantity, "Quantity");
        //    //encoder.Encode(MenuItemOptions, "MenuItemOptions");
        //}
        //#endregion
    }
}
