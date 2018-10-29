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
using System.Collections.Generic;

namespace SoupKit.Data
{
    class SoupMenuManager : DataManager<List<MenuItem>>
    {
        private List<MenuItem> DefaultMenu = new List<MenuItem>
        {
            new MenuItem("Chicken Noodle Soup", "CHICKEN_NOODLE_SOUP", new NSDecimalNumber(4.55m), "chicken_noodle_soup", true, true),
            new MenuItem("Clam Chowder", "CLAM_CHOWDER", new NSDecimalNumber(3.75m), "new_england_clam_chowder", true, false),
            new MenuItem("Tomato Soup", "TOMATO_SOUP", new NSDecimalNumber(2.95m), "tomato_soup", true, false)
        };

        public SoupOrderDataManager OrderManager { get; set; }

        public SoupMenuManager() : base(new UserDefaultsStorageDescriptor(NSUserDefaultsHelper.StorageKeys.SoupMenu)) { }

        #region Public API for clients of `SoupMenuManager`

        public List<MenuItem> AvailableItems
        {
            get
            {
                List<MenuItem> result = null;
                DataAccessQueue.DispatchSync(() =>
                {
                    result = ManagedData.Where(data => data.IsAvailable).OrderBy(data => data.ItemName).ToList();
                });

                return result;
            }
        }

        public List<MenuItem> AvailableDailySpecialItems
        {
            get
            {
                List<MenuItem> result = null;
                DataAccessQueue.DispatchSync(() =>
                {
                    result = ManagedData.Where(data => data.IsAvailable && data.IsDailySpecial)
                                      .OrderBy(data => data.ItemName)
                                      .ToList();
                });

                return result;
            }
        }

        public List<MenuItem> DailySpecialItems
        {
            get
            {
                List<MenuItem> result = null;
                DataAccessQueue.DispatchSync(() =>
                {
                    result = ManagedData.Where(data => data.IsDailySpecial).OrderBy(data => data.ItemName).ToList();
                });

                return result;
            }
        }

        public List<MenuItem> RegularItems
        {
            get
            {
                List<MenuItem> result = null;
                DataAccessQueue.DispatchSync(() =>
                {
                    result = ManagedData.Where(data => !data.IsDailySpecial).OrderBy(data => data.ItemName).ToList();
                });

                return result;
            }
        }

        public List<MenuItem> AvailableRegularItems
        {
            get
            {
                List<MenuItem> result = null;
                DataAccessQueue.DispatchSync(() =>
                {
                    result = ManagedData.Where(data => !data.IsDailySpecial && data.IsAvailable).OrderBy(data => data.ItemName).ToList();
                });

                return result;
            }
        }

        public void ReplaceMenuItem(MenuItem previousMenuItem, MenuItem menuItem)
        {
            DataAccessQueue.DispatchSync(() =>
            {
                ManagedData.Remove(previousMenuItem);
                ManagedData.Add(menuItem);
            });

            // Access to NSUserDefaults is gated behind a separate access queue.
            WriteData();

            RemoveDonation(menuItem);
            UpdateShortcuts();
        }

        public MenuItem FindItem(string identifier)
        {
            MenuItem result = null;
            DataAccessQueue.DispatchSync(() =>
            {
                result = ManagedData.FirstOrDefault(data => data.ItemName == identifier);
            });

            return result;
        }
        #endregion

        #region Supporting methods for using the Intents framework.

        /// Inform Siri of changes to the menu.
        public void UpdateShortcuts()
        {
            UpdateMenuItemShortcuts();
            UpdateSuggestions();
        }

        /// Each time an order is placed, we instantiate an INInteraction object and donate it to the system (see SoupOrderDataManager extension).
        /// After instantiating the INInteraction, its identifier property is set to the same value as the identifier
        /// property for the corresponding order. Compile a list of all the order identifiers to pass to the INInteraction delete method.
        private void RemoveDonation(MenuItem menuItem)
        {
            if (!menuItem.IsAvailable)
            {
                var orderHistory = OrderManager?.OrderHistory;
                if (orderHistory != null)
                {
                    var ordersAssociatedWithRemovedMenuItem = orderHistory.Where(order => order.MenuItem.ItemName == menuItem.ItemName);
                    var orderIdentifiersToRemove = ordersAssociatedWithRemovedMenuItem.Select(order => order.Identifier.AsString());

                    INInteraction.DeleteInteractions(orderIdentifiersToRemove.ToArray(), (error) =>
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
        }

        /// Configures a daily soup special to be made available as a relevant shortcut. This item
        /// is not available on the regular menu to demonstrate how relevant shortcuts are able to
        /// suggest tasks the user may want to start, but haven't used in the app before.
        private void UpdateSuggestions()
        {
            var dailySpecialSuggestedShortcuts = AvailableDailySpecialItems.Select(menuItem =>
            {
                var order = new Order(new NSDate(), new NSUuid(), 1, menuItem, new List<MenuItemOption>());
                var orderIntent = order.Intent;

                var shortcut = new INShortcut(orderIntent);
                var suggestedShortcut = new INRelevantShortcut(shortcut);

                var localizedTitle = NSBundleHelper.SoupKitBundle.GetLocalizedString("ORDER_LUNCH_TITLE", "Relevant shortcut title");
                var template = new INDefaultCardTemplate(localizedTitle);

                // Need a different string for the subtitle because of capitalization difference
                //template.subtitle = NSString.deferredLocalizedIntentsString(with: menuItem.shortcutNameKey + "_SUBTITLE") as String
                template.Subtitle = menuItem.ItemName;
                //template.image = INImage(named: menuItem.iconImageName)
                template.Image = INImage.FromName(menuItem.IconImageName);
                //var image = UIImage.FromBundle(menuItem.IconImageName);
                //if (!(image is null))
                //{
                //    var data = image.AsPNG();
                //    template.Image = INImage.FromData(data);
                //}

                suggestedShortcut.WatchTemplate = template;

                // Make a lunch suggestion when arriving to work.
                var routineRelevanceProvider = new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.Work);

                // This sample uses a single relevance provider, but using multiple relevance providers is supported.
                suggestedShortcut.RelevanceProviders = new INRelevanceProvider[] { routineRelevanceProvider };

                return suggestedShortcut;
            });

            INRelevantShortcutStore.DefaultStore.SetRelevantShortcuts(dailySpecialSuggestedShortcuts.ToArray(), (error) =>
            {
                if (!(error is null))
                {
                    Console.WriteLine($"Providing relevant shortcut failed. \n{error}");
                }
                else
                {
                    Console.WriteLine("Providing relevant shortcut succeeded.");
                }
            });
        }

        /// Provides shortcuts for orders the user may want to place, based on a menu item's availability.
        /// The results of this method are visible in the iOS Settings app.
        private void UpdateMenuItemShortcuts()
        {
            var availableShortcuts = AvailableRegularItems.Select(menuItem =>
            {
                var order = new Order(new NSDate(), new NSUuid(), 1, menuItem, new List<MenuItemOption>());
                return new INShortcut(order.Intent);
            });

            INVoiceShortcutCenter.SharedCenter.SetShortcutSuggestions(availableShortcuts.ToArray());
        }

        #endregion

        #region Support methods for unarchiving saved data
        //override protected void FinishUnarchiving(NSObject unarchivedData)
        //{
        //    NSSet set = (NSSet)unarchivedData;
        //    ManagedDataBackingInstance = new NSMutableSet<MenuItem>(set.ToArray<MenuItem>());
        //}
        #endregion
    }
}
