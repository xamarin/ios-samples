/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A data manager that manages data conforming to `Codable` and stores it in `UserDefaults`.
*/

using System;
using Foundation;
using CoreFoundation;
using SoupKit.Support;

namespace SoupKit.Data
{
    public struct UserDefaultsStorageDescriptor
    {
        public string Key { get; set; }
        public UserDefaultsStorageDescriptor(string key)
        {
            Key = key;
        }
    }

    public static class NotificationKeys
    {
        // Clients of `DataManager` that want to know when the data changes can 
        // listen for this notification.
        public const string DataChanged = "DataChangedNotification";
    }

    public class DataManager<ManagedDataType> : NSObject 
        where ManagedDataType : NSObject, INSCoding
    {
        // This sample uses App Groups to share a suite of data between the 
        // main app and the different extensions.
        protected NSUserDefaults UserDefaults = NSUserDefaultsHelper.DataSuite;

        // To prevent data races, all access to `UserDefaults` uses this queue.
        protected DispatchQueue UserDefaultsAccessQueue = new DispatchQueue("User Defaults Access Queue");

        // Storage and observation information.
        protected UserDefaultsStorageDescriptor StorageDescriptor;

        // A flag to avoid receiving notifications about data this instance just 
        // wrote to `UserDefaults`.
        protected bool IgnoreLocalUserDefaultsChanges = false;

        // The observer object handed back after registering to observe a 
        // property.
        IDisposable UserDefaultsObserver;

        // The data managed by this `DataManager`.
        protected ManagedDataType ManagedDataBackingInstance;

        // Access to `managedDataBackingInstance` needs to occur on a dedicated 
        // queue to avoid data races.
        protected DispatchQueue DataAccessQueue = new DispatchQueue("Data Access Queue");

        // Public access to the managed data for clients of `DataManager`
        public ManagedDataType ManagedData
        {
            get
            {
                ManagedDataType data = null;
                DataAccessQueue.DispatchSync(() => data = ManagedDataBackingInstance);
                return data;
            }
        }

        // See note below about createInitialData and initialData
        public DataManager(UserDefaultsStorageDescriptor storageDescriptor, ManagedDataType initialData)
        {
            StorageDescriptor = storageDescriptor;
            LoadData();
            if (ManagedDataBackingInstance is null)
            {
                ManagedDataBackingInstance = initialData;
                WriteData();
            }
            ObserveChangesInUserDefaults();
        }

        // createInitialData
        //
        // The Swift version of this app has a createInitialData method.
        // Each child class of the DataManager class overrides this method, and
        // then the DataManager base class calls the derived versions to get
        // the initial data. C# gives a compiler warning for this ("Virtual
        // member call in constructor"). Since in C# the base class constructor
        // is run before the child class constructor, having the base clas
        // constructor call out to a method on the derived class is calling
        // a method on an object that has not yet been fully constructed.
        // The C# version of this sample works around this problem by passing 
        // in the initial data to the constructor.

        void ObserveChangesInUserDefaults()
        {
            var weakThis = new WeakReference<DataManager<ManagedDataType>>(this);
            Action<NSObservedChange> changeHandler = (change) =>
            {
                if (weakThis.TryGetTarget(out var dataManager))
                {
                    // Ignore any change notifications coming from data this 
                    // instance just saved to `NSUserDefaults`.
                    if (dataManager is null || dataManager.IgnoreLocalUserDefaultsChanges)
                    {
                        return;
                    }

                    // The underlying data changed in `NSUserDefaults`, so 
                    // update this instance with the change and notify clients 
                    // of the change.
                    dataManager.LoadData();
                    dataManager.NotifyClientsDataChanged();
                }
            };
            UserDefaultsObserver = UserDefaults.AddObserver(
                StorageDescriptor.Key,
                NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New,
                changeHandler
            );
        }

        // Notifies clients the data changed by posting an `NSNotification` with 
        // the key `NotificationKeys.DataChanged`
        void NotifyClientsDataChanged()
        {
            var notification = NSNotification.FromName(NotificationKeys.DataChanged, this);
            NSNotificationCenter.DefaultCenter.PostNotification(notification);
        }

        protected virtual void FinishUnarchiving(NSObject unarchivedData)
        {
            throw new NotImplementedException();
        }

        // Loads the data from `NSUserDefaults`.
        void LoadData()
        {
            UserDefaultsAccessQueue.DispatchSync(() =>
            {
                NSData archivedData = UserDefaults.DataForKey(StorageDescriptor.Key);
                try
                {
                    // Let the derived classes handle the specifics of 
                    // putting the unarchived data in the correct format.
                    // This is necessary because the derived classes
                    // (SoupMenuManager, SoupOrderMenuManager) are using
                    // generic data formats (NSMutableSet<T> or NSMutableArray<T>) 
                    // and these types cannot be casted directly from the 
                    // deserialized data.
                    NSObject unarchivedData = NSKeyedUnarchiver.UnarchiveObject(archivedData);
                    FinishUnarchiving(unarchivedData);
                }
                catch (Exception e)
                {
                    if (!(e is null))
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
            });
        }

        // Writes the data to `NSUserDefaults`
        protected void WriteData()
        {
            UserDefaultsAccessQueue.DispatchAsync(() =>
            {
                try
                {
                    NSData encodedData = NSKeyedArchiver.ArchivedDataWithRootObject(ManagedDataBackingInstance);
                    IgnoreLocalUserDefaultsChanges = true;
                    UserDefaults.SetValueForKey(encodedData, (NSString)StorageDescriptor.Key);
                    IgnoreLocalUserDefaultsChanges = false;
                    NotifyClientsDataChanged();
                }
                catch (Exception e)
                {
                    throw new Exception($"Could not save data. Reason: {e.Message}");
                }
            });
        }
    }
}
