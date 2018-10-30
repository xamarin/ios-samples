
namespace SoupChef.Data
{
    using Foundation;
    using Intents;
    using SoupChef;
    using SoupChef.Support;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    enum MenuItemOption
    {
        Cheese = 0,// = "Cheese"
        RedPepper = 1,// = "Red Pepper"
        Croutons = 2,// = "Croutons"
    }

    /// <summary>
    /// This type encapsulates the attributes of a soup order.
    /// </summary>
    class Order : ILocalizableCurrency
    {
        public Order(int quantity, MenuItem menuItem, List<MenuItemOption> menuItemOptions)
        {
            MenuItem = menuItem;
            Quantity = quantity;
            MenuItemOptions = menuItemOptions;
        }

        public DateTime Date { get; private set; } = DateTime.UtcNow;

        public Guid Identifier { get; private set; } = Guid.NewGuid();

        public MenuItem MenuItem { get; private set; }

        public int Quantity { get; set; }

        public List<MenuItemOption> MenuItemOptions { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public float Total => Quantity * MenuItem.Price;

        [Newtonsoft.Json.JsonIgnore]
        public string LocalizedCurrencyValue => NSNumberFormatterHelper.CurrencyFormatter.StringFromNumber(Total) ?? string.Empty;

        public override int GetHashCode()
        {
            return MenuItem.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = obj as Order;

            var result = item != null;
            if (result)
            {
                result = MenuItem == item.MenuItem &&
                         Quantity == item.Quantity &&
                         MenuItemOptions == item.MenuItemOptions;
            }

            return result;
        }

        #region intent helpers

        [Newtonsoft.Json.JsonIgnore]
        public OrderSoupIntent Intent
        {
            get
            {
                var orderSoupIntent = new OrderSoupIntent();
                orderSoupIntent.Quantity = new NSNumber(Quantity);
                orderSoupIntent.Soup = new INObject(MenuItem.ShortcutNameKey, MenuItem.ItemName);
                orderSoupIntent.SetImage(INImage.FromName(MenuItem.IconImageName), "soup");

                var options = MenuItemOptions.Select(arg => new INObject(arg.ToString(), arg.ToString())).ToArray();
                orderSoupIntent.Options = NSArray<INObject>.FromNSObjects(options);

                var phrase = NSBundle.MainBundle.GetLocalizedString("ORDER_SOUP_SUGGESTED_PHRASE", "Suggested phrase for ordering a specific soup");
                orderSoupIntent.SuggestedInvocationPhrase = string.Format(phrase, MenuItem.ItemName);

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

                    result = new Order(quantity.Int32Value, menuItem, new List<MenuItemOption>(rawOptions));
                }
            }

            return result;
        }

        #endregion
    }
}