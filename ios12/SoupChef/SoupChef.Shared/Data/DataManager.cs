/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A data manager that manages data conforming to `Codable` and stores it in `UserDefaults`.
*/

using System;
using Foundation;
using CoreFoundation;
using SoupKit.Support;
//using SoupKit.Support;

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

    class DataManager<T> : NSObject //where T : NSObject, INSCoding
    {
        // This sample uses App Groups to share a suite of data between the main app and the different extensions.
        protected NSUserDefaults UserDefaults = NSUserDefaultsHelper.DataSuite;

        // To prevent data races, all access to `UserDefaults` uses this queue.
        protected DispatchQueue UserDefaultsAccessQueue = new DispatchQueue("User Defaults Access Queue");

        // Storage and observation information.
        protected UserDefaultsStorageDescriptor StorageDescriptor;

        // A flag to avoid receiving notifications about data this instance just wrote to `UserDefaults`.
        protected bool IgnoreLocalUserDefaultsChanges;

        // The observer object handed back after registering to observe a property.
        private IDisposable UserDefaultsObserver;

        // Access to `managedData` needs to occur on a dedicated queue to avoid data races.
        protected DispatchQueue DataAccessQueue = new DispatchQueue("Data Access Queue");

        // The data managed by this `DataManager`. Only access this via on the `dataAccessQueue`.
        public T ManagedData { get; set; }

        public DataManager(UserDefaultsStorageDescriptor storageDescriptor)
        {
            StorageDescriptor = storageDescriptor;
            LoadData();

            if (ManagedData == null)
            {
                DeployInitialData();
                WriteData();
            }

            ObserveChangesInUserDefaults();
        }

        /// Subclasses are expected to implement this method and set their own initial data for `managedData`.
        protected virtual void DeployInitialData()
        {

        }

        private void ObserveChangesInUserDefaults()
        {
            UserDefaultsObserver = UserDefaults.AddObserver(StorageDescriptor.Key,
                                                            NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New,
                                                            (change) =>
                                                            {
                                                                // Ignore any change notifications coming from data this instance just saved to `UserDefaults`.
                                                                if (!IgnoreLocalUserDefaultsChanges)
                                                                {
                                                                    // The underlying data changed in `NSUserDefaults`, so 
                                                                    // update this instance with the change and notify clients 
                                                                    // of the change.
                                                                    LoadData();
                                                                    NotifyClientsDataChanged();
                                                                }
                                                            });

        }

        /// Notifies clients the data changed by posting a `Notification` with the key `dataChangedNotificationKey`
        private void NotifyClientsDataChanged()
        {
            // TODO:
            // NotificationCenter.default.post(Notification(name: dataChangedNotificationKey, object: self))
            var notification = NSNotification.FromName(NotificationKeys.DataChanged, NSObject.FromObject(this));
            NSNotificationCenter.DefaultCenter.PostNotification(notification);
        }

        // Loads the data from `NSUserDefaults`.
        private void LoadData()
        {
            UserDefaultsAccessQueue.DispatchSync(() =>
            {
                var archivedData = UserDefaults.DataForKey(StorageDescriptor.Key);
                var json = archivedData.ToString();
                ManagedData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            });
        }

        // Writes the data to `NSUserDefaults`
        protected void WriteData()
        {
            UserDefaultsAccessQueue.DispatchAsync(() =>
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(ManagedData);

                IgnoreLocalUserDefaultsChanges = true;
                UserDefaults[StorageDescriptor.Key] = NSData.FromString(json);
                //UserDefaults.SetValueForKey(encodedData, new NSString(StorageDescriptor.Key));
                IgnoreLocalUserDefaultsChanges = false;
                NotifyClientsDataChanged();
            });
        }
    }
}