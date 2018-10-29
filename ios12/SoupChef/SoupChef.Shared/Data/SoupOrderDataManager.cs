/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A data manager that manages an array of `Order` structs.
*/

using Foundation;
using Intents;
using SoupKit.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoupKit.Data
{
    // A concrete `DataManager` for reading and writing data of type `NSMutableArray<Order>`.
    class SoupOrderDataManager : DataManager<List<Order>>
    {
        public SoupOrderDataManager() : base(new UserDefaultsStorageDescriptor(NSUserDefaultsHelper.StorageKeys.OrderHistory)) { }
        //public SoupOrderDataManager() { }
        protected override void DeployInitialData()
        {
            DataAccessQueue.DispatchSync(() =>
            {
                // Order history is empty the first time the app is used.
                ManagedData = new List<Order>();
            });
        }

        // Converts an `Order` into `OrderSoupIntent` and donates it as an 
        // interaction to the system so that this order can be suggested in the 
        // future or turned into a voice shortcut for quickly placing the same 
        // order in the future.
        private void DonateInteraction(Order order)
        {
            var interaction = new INInteraction(order.Intent, null);

            // The order identifier is used to match with the donation so the interaction
            // can be deleted if a soup is removed from the menu.
            interaction.Identifier = order.Identifier.AsString();//.ToString();

            interaction.DonateInteraction((error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Interaction donation failed: {error}");
                }
                else
                {
                    Console.WriteLine("Successfully donated interaction.");
                }
            });
        }

        #region Public API for clients of `SoupOrderDataManager`

        // Convenience method to access the data with a property name that makes sense in the caller's context.
        public List<Order> OrderHistory
        {
            get
            {
                List<Order> result = null;
                DataAccessQueue.DispatchAsync(() =>
                {
                    result = ManagedData;
                });

                return result;
            }
        }

        /// Tries to find an order by its identifier
        public Order Order(NSUuid identifier) 
        {
            return OrderHistory.FirstOrDefault(g => g.Identifier == identifier);
        }

        /// Stores the order in the data manager.
        /// Note: This project does not share data between iOS and watchOS. Orders placed on the watch will not display in the iOS order history.
        public void PlaceOrder(Order order)
        {
            // Access to `managedDataBackingInstance` is only valid on `dataAccessQueue`.
            DataAccessQueue.DispatchSync(() =>
            {
                ManagedData.Insert(0, order);
            });

            // Access to UserDefaults is gated behind a separate access queue.
            WriteData();

            // Donate an interaction to the system.
            DonateInteraction(order);
        }
        #endregion

        //#region Support methods for unarchiving saved data
        //override protected void FinishUnarchiving(NSObject unarchivedData)
        //{
        //    var array = (NSArray)unarchivedData;
        //    Order[] orders = NSArray.FromArray<Order>(array);
        //    ManagedDataBackingInstance = new NSMutableArray<Order>(orders);
        //}
        //#endregion
    }
}
