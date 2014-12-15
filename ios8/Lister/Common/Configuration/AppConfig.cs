using System;
using System.IO;

using Foundation;
using System.Linq;

namespace Common
{
	public class AppConfig : NSObject
	{
		const string FirstLaunchUserDefaultsKey = "FirstLaunchUserDefaultsKey";
		const string StorageOptionUserDefaultsKey = "StorageOptionUserDefaultsKey";
		const string StoredUbiquityIdentityTokenKey = "UbiquityIdentityTokenKey";

		public static readonly string ListerFileExtension = "lister";

		public event EventHandler StorageOptionChanged;

		static readonly AppConfig sharedAppConfiguration = new AppConfig();
		public static AppConfig SharedAppConfiguration {
			get {
				return sharedAppConfiguration;
			}
		}

		StorageType? storageOption;
		public StorageType StorageOption {
			get {
				if (!storageOption.HasValue)
					storageOption = (StorageType)(int)NSUserDefaults.StandardUserDefaults.IntForKey (StorageOptionUserDefaultsKey);

				return storageOption.Value;
			}
			set {
				if (!storageOption.HasValue || value != storageOption.Value) {
					storageOption = value;
					NSUserDefaults.StandardUserDefaults.SetInt ((int)value, StorageOptionUserDefaultsKey);

					var handler = StorageOptionChanged;
					if (handler != null)
						handler (this, EventArgs.Empty);
				}
			}
		}

		public bool IsCloudAvailable {
			get {
				// if it is null you should check your Entitlements.plist
				return NSFileManager.DefaultManager.UbiquityIdentityToken != null;
			}
		}

		public StorageState StorageState {
			get {
				return new StorageState {
					StorageOption = StorageOption,
					AccountDidChange = HasUbiquityIdentityChanged(),
					CloudAvailable = IsCloudAvailable
				};
			}
		}

		public string TodayDocumentName {
			get {
				return "Today";
			}
		}

		public string TodayDocumentNameAndExtension {
			get {
				return Path.ChangeExtension (TodayDocumentName, ListerFileExtension);
			}
		}

		public string DefaultListerDraftName {
			get {
				return "List";
			}
		}

		public void RunHandlerOnFirstLaunch(Action firstLaunchHandler)
		{
			NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;

			defaults.RegisterDefaults (new NSMutableDictionary {
				{ (NSString)FirstLaunchUserDefaultsKey, NSObject.FromObject(true) },
				{ (NSString)StorageOptionUserDefaultsKey, NSObject.FromObject(StorageType.NotSet) }
			});

			if (defaults.BoolForKey(FirstLaunchUserDefaultsKey)) {
				defaults.SetBool (false, FirstLaunchUserDefaultsKey);
				firstLaunchHandler();
			}
		}

		#region Identity

		bool HasUbiquityIdentityChanged()
		{
			if (StorageOption != StorageType.Cloud)
				return false;

			bool hasChanged = false;
			NSObject currentToken = NSFileManager.DefaultManager.UbiquityIdentityToken;
			NSObject storedToken = FetchStoredUbiquityIdentityToken ();

			bool currentTokenNullStoredNonNull = currentToken == null && storedToken != null;
			bool storedTokenNullCurrentNonNull = storedToken == null && currentToken != null;
			bool currentNotEqualStored = currentToken != null && storedToken != null
			                             && !currentToken.Equals (storedToken);

			if (currentTokenNullStoredNonNull || storedTokenNullCurrentNonNull || currentNotEqualStored) {
				StoreUbiquityIdentityToken ();
				hasChanged = true;
			}

			return hasChanged;
		}

		public void StoreUbiquityIdentityToken()
		{
			NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
			var token = NSFileManager.DefaultManager.UbiquityIdentityToken;

			if (token != null) {
				// the account has changed
				NSData ubiquityIdentityTokenArchive = NSKeyedArchiver.ArchivedDataWithRootObject (token);
				defaults [StoredUbiquityIdentityTokenKey] = ubiquityIdentityTokenArchive;
			} else {
				// the is no signed-in account
				defaults.RemoveObject (StoredUbiquityIdentityTokenKey);
			}

			defaults.Synchronize ();
		}

		NSObject FetchStoredUbiquityIdentityToken()
		{
			NSObject storedToken = null;

			// Determine if the iCloud account associated with this device has changed since the last time the user launched the app.
			var tokenArchive = (NSData)NSUserDefaults.StandardUserDefaults[StoredUbiquityIdentityTokenKey];
			if (tokenArchive != null)
				storedToken = NSKeyedUnarchiver.UnarchiveObject (tokenArchive);

			return storedToken;
		}

		#endregion

	}
}
