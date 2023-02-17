using System;

using Foundation;
using StoreKit;
using System.Threading.Tasks;
using UIKit;
using MediaPlayer;

namespace MusicKitSample.Controllers {
	public class AuthorizationManager : NSObject {
		#region Fields

		NSObject CloudServiceCapabilitiesDidChangeNotificationToken;
		NSObject StorefrontCountryCodeDidChangeNotificationToken;

		#endregion

		#region Types

		// Notification that is posted whenever there is a change in the capabilities or Storefront identifier of the `SKCloudServiceController`.
		public static NSString CloudServiceDidUpdateNotification { get; } = new NSString ("cloudServiceDidUpdateNotification");

		// Notification that is posted whenever there is a change in the authorization status that other parts of the sample should respond to.
		public static NSString AuthorizationDidUpdateNotification { get; } = new NSString ("authorizationDidUpdateNotification");

		// The `UserDefaults` key for storing and retrieving the Music User Token associated with the currently signed in iTunes Store account.
		public static NSString UserTokenUserDefaultsKey { get; } = new NSString ("UserTokenUserDefaultsKey");

		#endregion

		#region Properties

		// The instance of `SKCloudServiceController` that will be used for querying the available `SKCloudServiceCapability` and Storefront Identifier.
		public SKCloudServiceController CloudServiceController { get; set; }

		// The instance of `AppleMusicManager` that will be used for querying storefront information and user token.
		public AppleMusicManager AppleMusicManager { get; set; }

		// The current set of `SKCloudServiceCapability` that the sample can currently use.
		public SKCloudServiceCapability CloudServiceCapabilities { get; set; }

		// The current set of two letter country code associated with the currently authenticated iTunes Store account.
		public string CloudServiceStorefrontCountryCode { get; set; }

		// The Music User Token associated with the currently signed in iTunes Store account.
		public string UserToken { get; set; }

		#endregion

		#region Constructors

		public AuthorizationManager (AppleMusicManager appleMusicManager)
		{
			AppleMusicManager = appleMusicManager;
			CloudServiceController = new SKCloudServiceController ();
			CloudServiceCapabilities = new SKCloudServiceCapability ();

			/*
		         * It is important that your application listens to the 
		         * `CloudServiceCapabilitiesDidChangeNotification` and 
		         * `StorefrontCountryCodeDidChangeNotification` notifications 
		         * so that your application can update its state and 
		         * functionality when these values change if needed.
		         */
			var notificationCenter = NSNotificationCenter.DefaultCenter;
			CloudServiceCapabilitiesDidChangeNotificationToken = notificationCenter.AddObserver (SKCloudServiceController.CloudServiceCapabilitiesDidChangeNotification,
											async (obj) => await RequestCloudServiceCapabilitiesAsync ());

			if (UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
				StorefrontCountryCodeDidChangeNotificationToken = notificationCenter.AddObserver (SKCloudServiceController.StorefrontCountryCodeDidChangeNotification,
							async (obj) => await RequestStorefrontCountryCodeAsync ());

			/* 
			 * If the application has already been authorized in a 
			 * previous run or manually by the user then it can 
			 * request the current set of `SKCloudServiceCapability`
			 * and Storefront Identifier.
		         */

			if (SKCloudServiceController.AuthorizationStatus == SKCloudServiceAuthorizationStatus.Authorized) {
				Task.Factory.StartNew (async () => {
					await RequestCloudServiceCapabilitiesAsync ();

					// Retrieve the Music User Token for use in the application 
					// if it was stored from a previous run.
					if (NSUserDefaults.StandardUserDefaults.StringForKey (UserTokenUserDefaultsKey) is string token) {
						UserToken = token;
						await RequestStorefrontCountryCodeAsync ();
					} else // The token was not stored previously then request one.
						await RequestUserTokenAsync ();
				});
			}
		}

		#endregion

		#region Object Life Cycle

		protected override void Dispose (bool disposing)
		{
			// Remove all notification observers.
			var notificationCenter = NSNotificationCenter.DefaultCenter;
			notificationCenter.RemoveObserver (CloudServiceCapabilitiesDidChangeNotificationToken);

			if (UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
				notificationCenter.RemoveObserver (StorefrontCountryCodeDidChangeNotificationToken);

			CloudServiceCapabilitiesDidChangeNotificationToken = StorefrontCountryCodeDidChangeNotificationToken = null;

			base.Dispose (disposing);
		}

		#endregion

		#region Public Functionality

		#region Authorization Request Methods

		public async Task RequestCloudServiceAuthorizationAsync ()
		{
			/*
			 * An application should only ever call 
			 * `SKCloudServiceController.RequestAuthorization` when 
			 * their current authorization is 
			 * `SKCloudServiceAuthorizationStatus.NotDetermined`
		         */
			if (SKCloudServiceController.AuthorizationStatus != SKCloudServiceAuthorizationStatus.NotDetermined)
				return;

			/* 
			 * `SKCloudServiceController.RequestAuthorizationAsync ()`
			 * triggers a prompt for the user asking if they wish to
			 * allow the application that requested authorization 
			 * access to the device's cloud services information. 
			 * This allows the application to query information such
			 * as the what capabilities the currently authenticated 
			 * iTunes Store account has and if the account is 
			 * eligible for an Apple Music Subscription Trial.
			 * 
			 * This prompt will also include the value provided in 
			 * the application's Info.plist for the 
			 * `NSAppleMusicUsageDescription` key. This usage 
			 * description should reflect what the application 
			 * intends to use this access for.
			 */
			var authorizationStatus = await SKCloudServiceController.RequestAuthorizationAsync ();

			switch (authorizationStatus) {
			case SKCloudServiceAuthorizationStatus.Authorized:
				await RequestCloudServiceCapabilitiesAsync ();
				await RequestUserTokenAsync ();
				break;
			default:
				break;
			}

			InvokeOnMainThread (() => NSNotificationCenter.DefaultCenter.PostNotificationName (AuthorizationDidUpdateNotification, null));
		}

		public async Task RequestMediaLibraryAuthorizationAsync ()
		{
			/* 
			 * An application should only ever call 
			 * `MPMediaLibrary.AuthorizationStatus` when their current 
			 * authorization is `MPMediaLibraryAuthorizationStatus.NotDetermined`
			 */
			if (MPMediaLibrary.AuthorizationStatus != MPMediaLibraryAuthorizationStatus.NotDetermined)
				return;

			/* 
			 * `MPMediaLibrary.RequestAuthorizationAsync ()` triggers a 
			 * prompt for the user asking if they wish to allow the 
			 * application that requested authorization access to 
			 * the device's media library.
			 * 
			 * This prompt will also include the value provided in 
			 * the application's Info.plist for the 
			 * `NSAppleMusicUsageDescription` key. This usage 
			 * description should reflect what the application 
			 * intends to use this access for.
			 */
			await MPMediaLibrary.RequestAuthorizationAsync ();
			InvokeOnMainThread (() => NSNotificationCenter.DefaultCenter.PostNotificationName (CloudServiceDidUpdateNotification, null));
		}

		#endregion

		#region `SKCloudServiceController` Related Methods

		async Task RequestCloudServiceCapabilitiesAsync ()
		{
			CloudServiceCapabilities = await CloudServiceController.RequestCapabilitiesAsync ();
			InvokeOnMainThread (() => NSNotificationCenter.DefaultCenter.PostNotificationName (CloudServiceDidUpdateNotification, null));
		}

		async Task RequestStorefrontCountryCodeAsync ()
		{
			string countryCode;

			if (SKCloudServiceController.AuthorizationStatus == SKCloudServiceAuthorizationStatus.Authorized) {
				if (UIDevice.CurrentDevice.CheckSystemVersion (11, 0)) {
					/*
					 * On iOS 11.0 or later, if the 
					 * `SKCloudServiceController.authorizationStatus()` 
					 * is `.authorized` then you can request the 
					 * storefront country code.
					 */
					countryCode = (await CloudServiceController.RequestStorefrontCountryCodeAsync ()).ToString ();
				} else {
					countryCode = await AppleMusicManager.PerformAppleMusicGetUserStorefrontAsync (UserToken);
				}
			} else {
				countryCode = await DetermineRegion ();
			}

			if (string.IsNullOrWhiteSpace (countryCode))
				throw new ArgumentNullException (nameof (countryCode), "Unexpected value from SKCloudServiceController for storefront country code.");

			CloudServiceStorefrontCountryCode = countryCode;

			InvokeOnMainThread (() => NSNotificationCenter.DefaultCenter.PostNotificationName (CloudServiceDidUpdateNotification, null));
		}

		async Task RequestUserTokenAsync ()
		{
			var developerToken = AppleMusicManager.FetchDeveloperToken ();
			if (developerToken == null)
				throw new ArgumentNullException (nameof (developerToken), "Developer Token not configured. See README for more details.");

			if (SKCloudServiceController.AuthorizationStatus != SKCloudServiceAuthorizationStatus.Authorized)
				return;

			string token;

			if (UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
				token = await CloudServiceController.RequestUserTokenAsync (developerToken);
			else
				token = await CloudServiceController.RequestPersonalizationTokenAsync (developerToken);

			if (string.IsNullOrWhiteSpace (token)) {
				Console.WriteLine ("Unexpected value from SKCloudServiceController for user token.");
				return;
			}

			UserToken = token;

			// Store the Music User Token for future use in your application.
			var userDefaults = NSUserDefaults.StandardUserDefaults;
			userDefaults.SetString (token, UserTokenUserDefaultsKey);
			userDefaults.Synchronize ();

			if (string.IsNullOrWhiteSpace (CloudServiceStorefrontCountryCode))
				await RequestStorefrontCountryCodeAsync ();

			InvokeOnMainThread (() => NSNotificationCenter.DefaultCenter.PostNotificationName (CloudServiceDidUpdateNotification, null));
		}

		async Task<string> DetermineRegion ()
		{
			/*
			 * On other versions of iOS or when 
			 * `SKCloudServiceController.AuthorizationStatus` is not 
			 * `SKCloudServiceAuthorizationStatus.Authorized`, your 
			 * application should use a combination of the device's 
			 * `Locale.current.regionCode` and the Apple Music API 
			 * to make an approximation of the storefront to use.
			 */
			var currentRegionCode = NSLocale.CurrentLocale.CountryCode.ToLower ();
			return await AppleMusicManager.PerformAppleMusicStorefrontsLookupAsync (currentRegionCode);
		}

		#endregion

		#endregion
	}
}
