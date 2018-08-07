/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A data manager that manages an array of `Order` structs.
*/

using Foundation;
using SoupKit.Support;
using Intents;
using System;
using System.Linq;

namespace SoupKit.Data
{
    // A concrete `DataManager` for reading and writing data of type `NSMutableArray<Order>`.
    public class SoupOrderDataManager : DataManager<NSMutableArray<Order>>
    {
        public SoupOrderDataManager() : base(new UserDefaultsStorageDescriptor(NSUserDefaultsHelper.StorageKeys.OrderHistory), new NSMutableArray<Order>()) { }

        // Converts an `Order` into `OrderSoupIntent` and donates it as an 
        // interaction to the system so that this order can be suggested in the 
        // future or turned into a voice shortcut for quickly placing the same 
        // order in the future.
        void DonateInteraction(Order order)
        {
            var interaction = new INInteraction(order.Intent, null);
            interaction.DonateInteraction((error) =>
            {
                if (!(error is null))
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
        // Convenience method to access the data with a property name that makes 
        // sense in the caller's context.
        public NSMutableArray<Order> OrderHistory
        {
            get
            {
                return ManagedData as NSMutableArray<Order>;
            }
        }

        public void PlaceOrder(Order order)
        {
            // Access to `ManagedDataBackingInstance` is only valid on 
            // `DataAccessQueue`.
            DataAccessQueue.DispatchSync(() =>
            {
                if (ManagedDataBackingInstance.Count == 0)
                {
                    // Getting an error trying to insert at 0 for an empty
                    // NSMutableArray<Order>
                    ManagedDataBackingInstance.Add(order);
                }
                else
                {
                    ManagedDataBackingInstance.Insert(order, 0);
                }
            });

            // Access to UserDefaults is gated behind a separate access queue.
            WriteData();

            // Donate an interaction to the system.
            DonateInteraction(order);
        }
        #endregion

        #region Support methods for unarchiving saved data
        override protected void FinishUnarchiving(NSObject unarchivedData)
        {
            var array = (NSArray)unarchivedData;
            Order[] orders = NSArray.FromArray<Order>(array);
            ManagedDataBackingInstance = new NSMutableArray<Order>(orders);
        }
        #endregion
    }
}
