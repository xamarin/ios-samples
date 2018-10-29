/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A DataManager subclass that persists the active menu items.
*/

using System;
using Foundation;
using System.Linq;
using SoupKit.Support;
using Intents;
using UIKit;

namespace SoupKit.Data
{
    public class SoupMenuManager : DataManager<NSMutableSet<MenuItem>>
    {
        static NSMutableSet<MenuItem> DefaultMenu = new NSMutableSet<MenuItem>(
            new MenuItem("CHICKEN_NOODLE_SOUP", new NSDecimalNumber(4.55m), "chicken_noodle_soup", false, true),
            new MenuItem("CLAM_CHOWDER", new NSDecimalNumber(3.75m), "new_england_clam_chowder", true, false),
            new MenuItem("TOMATO_SOUP", new NSDecimalNumber(2.95m), "tomato_soup", true, false)
        );

        public SoupOrderDataManager OrderManager { get; set; }

        public SoupMenuManager() : base(new UserDefaultsStorageDescriptor(NSUserDefaultsHelper.StorageKeys.SoupMenu), DefaultMenu) { }

        #region Public API for clients of `SoupMenuManager`
        public MenuItem[] DailySpecialItems
        {
            get
            {
                var specials = new MenuItem[] { };
                var predicate = NSPredicate.FromExpression((evaluatedObject, bindings) =>
                {
                    return ((MenuItem)evaluatedObject).IsDailySpecial;
                });
                DataAccessQueue.DispatchSync(() =>
                {
                    specials = new NSSet<MenuItem>(ManagedDataBackingInstance)
                        .FilterUsingPredicate(predicate)
                        .ToArray<MenuItem>();
                });
                return specials;
            }
        }

        public MenuItem[] AllRegularItems
        {
            get
            {
                var regularItems = new MenuItem[] { };
                var predicate = NSPredicate.FromExpression((evaluatedObject, bindings) =>
                {
                    return !((MenuItem)evaluatedObject).IsDailySpecial;
                });
                DataAccessQueue.DispatchSync(() =>
                {
                    regularItems = new NSSet<MenuItem>(ManagedDataBackingInstance)
                        .FilterUsingPredicate(predicate)
                        .ToArray<MenuItem>();
                });
                return regularItems;
            }
        }

        public MenuItem[] AvailableRegularItems
        {
            get
            {
                var availableRegularItems = new MenuItem[] { };
                var predicate = NSPredicate.FromExpression((evaluatedObject, bindings) =>
                {
                    var menuItem = (MenuItem)evaluatedObject;
                    return !menuItem.IsDailySpecial && menuItem.IsAvailable;
                });
                DataAccessQueue.DispatchSync(() =>
                {
                    availableRegularItems = new NSSet<MenuItem>(ManagedDataBackingInstance)
                        .FilterUsingPredicate(predicate)
                        .ToArray<MenuItem>();
                });
                return availableRegularItems;
            }
        }

        public void ReplaceMenuItem(MenuItem previousMenuItem, MenuItem menuItem)
        {
            DataAccessQueue.DispatchSync(() =>
            {
                ManagedDataBackingInstance.Remove(previousMenuItem);
                ManagedDataBackingInstance.Add(menuItem);
            });

            // Access to NSUserDefaults is gated behind a separate access queue.
            WriteData();

            // Inform Siri of changes to the menu.
            RemoveDonation(menuItem);
            Suggest(menuItem);
        }

        public MenuItem FindItem(string identifier)
        {
            MenuItem[] matchedItems = new MenuItem[] { };
            var predicate = NSPredicate.FromExpression((evaluatedObject, bindings) =>
            {
                return ((MenuItem)evaluatedObject).ItemNameKey == identifier;
            });
            matchedItems = new NSSet<MenuItem>(ManagedDataBackingInstance)
                .FilterUsingPredicate(predicate)
                .ToArray<MenuItem>();

            return matchedItems.FirstOrDefault();
        }
        #endregion

        #region Supporting methods for using the Intents framework.
        // Each time an order is placed we instantiate an INInteraction object 
        // and donate it to the system (see SoupOrderDataManager extension).
        // After instantiating the INInteraction, its identifier property is 
        // set to the same value as the identifier property for the 
        // corresponding order. Compile a list of all the order identifiers to 
        // pass to the INInteraction delete method.
        void RemoveDonation(MenuItem menuItem)
        {
            if (!menuItem.IsAvailable)
            {
                Order[] orderHistory = OrderManager?.OrderHistory.ToArray<Order>();
                if (orderHistory is null)
                {
                    return;
                }

                string[] orderIdentifiersToRemove = orderHistory
                    .Where<Order>((order) => order.MenuItem.ItemNameKey == menuItem.ItemNameKey)
                    .Select<Order, string>((order) => order.Identifier.ToString())
                    .ToArray<string>();

                INInteraction.DeleteInteractions(orderIdentifiersToRemove, (error) =>
                {
                    if (!(error is null))
                    {
                        Console.WriteLine($"Failed to delete interactions with error {error.ToString()}");
                    }
                    else
                    {
                        Console.WriteLine("Successfully deleted interactions");
                    }
                });
            }
        }

        // Configures a daily soup special to be made available as a relevant 
        // shortcut. This item is not available on the regular menu to 
        // demonstrate how relevant shortcuts are able to suggest tasks the user 
        // may want to start, but hasn't used in the app before.
        void Suggest(MenuItem menuItem)
        {
            //if (menuItem.IsDailySpecial && menuItem.IsAvailable)
            //{
            //    var order = new Order(1, menuItem, new NSMutableSet<MenuItemOption>());
            //    var orderIntent = order.Intent;

            //    var shortcut = new INShortcut(orderIntent);
            //    var suggestedShortcut = new INRelevantShortcut(shortcut);

            //    var localizedTitle = NSBundle.MainBundle.GetLocalizedString(
            //        "ORDER_LUNCH_TITLE",
            //        "Relevant shortcut title"
            //    );
            //    var template = new INDefaultCardTemplate(localizedTitle);
            //    template.Subtitle = menuItem.ItemNameKey;

            //    var image = UIImage.FromBundle(menuItem.IconImageName);
            //    if (!(image is null))
            //    {
            //        var data = image.AsPNG();
            //        template.Image = INImage.FromData(data);
            //    }

            //    suggestedShortcut.WatchTemplate = template;

            //    // Make a lunch suggestion when arriving to work.
            //    var routineRelevanceProvider = new INDailyRoutineRelevanceProvider(
            //        INDailyRoutineSituation.Work
            //    );

            //    // This sample uses a single relevance provider, but using 
            //    // multiple relevance providers is supported.
            //    suggestedShortcut.RelevanceProviders =
            //        new INRelevanceProvider[] { routineRelevanceProvider };

            //    var suggestedShortcuts = new INRelevantShortcut[] { suggestedShortcut };
            //    INRelevantShortcutStore.DefaultStore.SetRelevantShortcuts(suggestedShortcuts, (error) =>
            //    {
            //        if (!(error is null))
            //        {
            //            Console.WriteLine($"Providing relevant shortcut failed. \n{error}");
            //        }
            //        else
            //        {
            //            Console.WriteLine("Providing relevant shortcut succeeded.");
            //        }
            //    });
            //}

        }
        #endregion

        #region Support methods for unarchiving saved data
        override protected void FinishUnarchiving(NSObject unarchivedData)
        {
            NSSet set = (NSSet)unarchivedData;
            ManagedDataBackingInstance = new NSMutableSet<MenuItem>(set.ToArray<MenuItem>());
        }
        #endregion
    }
}
