using System;
using System.IO;

using Foundation;
using System.Linq;
using UIKit;

namespace ListerKit
{
	public class AppConfig : NSObject
	{
		const string FirstLaunchUserDefaultsKey = "FirstLaunchUserDefaultsKey";
		const string StorageOptionUserDefaultsKey = "StorageOptionUserDefaultsKey";
		const string StoredUbiquityIdentityTokenKey = "UbiquityIdentityTokenKey";

		public static readonly string UserActivityListColorUserInfoKey = "listColor";
		public static readonly string ListerFileExtension = "lister";

		// TODO: move to Info.plist. Now it is hard to switch accounts
		public static readonly string ListerFileUTI = @"com.xamarin.lister";
		public static string ApplicationGroupsPrimary {
			get {
				return string.Format ("group.{0}.security", NSBundle.MainBundle.BundleIdentifier);
			}
		}

//		public event EventHandler StorageOptionChanged;

		static readonly AppConfig sharedAppConfiguration = new AppConfig();
		public static AppConfig SharedAppConfiguration {
			get {
				return sharedAppConfiguration;
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

		NSUserDefaults ApplicationUserDefaults {
			get {
				return new NSUserDefaults (ApplicationGroupsPrimary);
			}
		}

		public string DefaultListerDraftName {
			get {
				return "List";
			}
		}

		StorageType? storageOption;
		public StorageType StorageOption {
			get {
				if (!storageOption.HasValue)
					storageOption = (StorageType)(int)ApplicationUserDefaults.IntForKey (StorageOptionUserDefaultsKey);

				return storageOption.Value;
			}
			set {
				if (!storageOption.HasValue || value != storageOption.Value) {
					storageOption = value;
					ApplicationUserDefaults.SetInt ((int)value, StorageOptionUserDefaultsKey);
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

		bool IsFirstLaunch {
			get {
				RegisterDefaults();
				return ApplicationUserDefaults.BoolForKey (FirstLaunchUserDefaultsKey);
			}
			set {
				ApplicationUserDefaults.SetBool (value, FirstLaunchUserDefaultsKey);
			}
		}

		void RegisterDefaults ()
		{
			NSUserDefaults defaults = ApplicationUserDefaults;

			defaults.RegisterDefaults (new NSDictionary (FirstLaunchUserDefaultsKey, true
#if TARGET_PLATFORM_IPHONE
				, StorageOptionUserDefaultsKey, (int)StorageType.NotSet
#endif
			));
		}

		public void RunHandlerOnFirstLaunch(Action firstLaunchHandler)
		{
			if (IsFirstLaunch) {
				IsFirstLaunch = false;
				firstLaunchHandler ();
			}
		}

		#region Identity

		bool HasUbiquityIdentityChanged()
		{
			bool hasChanged = false;
			NSObject currentToken = NSFileManager.DefaultManager.UbiquityIdentityToken;
			NSObject storedToken = FetchStoredUbiquityIdentityToken ();

			bool currentTokenNullStoredNonNull = currentToken == null && storedToken != null;
			bool storedTokenNullCurrentNonNull = storedToken == null && currentToken != null;
			bool currentNotEqualStored = currentToken != null && storedToken != null
			                             && !currentToken.Equals (storedToken);

			if (currentTokenNullStoredNonNull || storedTokenNullCurrentNonNull || currentNotEqualStored) {
				PersistAccount ();
				hasChanged = true;
			}

			return hasChanged;
		}

		void PersistAccount()
		{
			NSUserDefaults defaults = ApplicationUserDefaults;
			NSObject token = NSFileManager.DefaultManager.UbiquityIdentityToken;

			if (token != null) {
				// The account has changed.
				NSData ubiquityIdentityTokenArchive = NSKeyedArchiver.ArchivedDataWithRootObject(token);
				defaults [StoredUbiquityIdentityTokenKey] = ubiquityIdentityTokenArchive;;
			} else {
				// There is no signed-in account.
				defaults.RemoveObject(StoredUbiquityIdentityTokenKey);
			}
		}

		NSObject FetchStoredUbiquityIdentityToken()
		{
			NSObject storedToken = null;

			// Determine if the iCloud account associated with this device has changed since the last time the user launched the app.
			var tokenArchive = (NSData)ApplicationUserDefaults[StoredUbiquityIdentityTokenKey];
			if (tokenArchive != null)
				storedToken = NSKeyedUnarchiver.UnarchiveObject (tokenArchive);

			return storedToken;
		}

		#endregion

		#region Conveience Methods

#if TARGET_PLATFORM_IPHONE

		public ListsController ListsControllerForCurrentConfigurationWithPathExtension (string listerFileExtension, Action storageOptionChangeHandler)
		{
			// This will be called if the storage option is either Local or NotSet.
			if (AppConfig.SharedAppConfiguration.StorageOption != StorageType.Cloud) {
//				return new Local LocalListCoordinator alloc] initWithPathExtension:pathExtension firstQueryUpdateHandler:firstQueryHandler];
			} else {
//				return [[AAPLCloudListCoordinator alloc] initWithPathExtension:pathExtension firstQueryUpdateHandler:firstQueryHandler];
			}
		}

		public IListCoordinator ListsCoordinatorForCurrentConfigurationWithPathExtension (string listerFileExtension, Action storageOptionChangeHandler)
		{
			throw new NotImplementedException ();
		}

#endif

		#endregion
	}
}
