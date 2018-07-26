using System;
using StoreKit;
using System.Collections.Generic;
using Foundation;
using MediaPlayer;
using ObjCRuntime;
namespace MusicKitSample.Controllers
{
	public class AuthorizationDataSource
	{
		[Native]
		enum SectionTypes : long
		{
			MediaLibraryAuthorizationStatus = 0,
			CloudServiceAuthorizationStatus,
			RequestCapabilities
		}

		#region Properties

		public AuthorizationManager AuthorizationManager { get; private set; }
		public SKCloudServiceCapability [] Capabilities { get; private set; }

		#endregion

		#region Constructors

		public AuthorizationDataSource (AuthorizationManager authorizationManager)
		{
			AuthorizationManager = authorizationManager;
		}

		#endregion

		#region Public Functionality

		#region Data Source Methods

		public int NumberOfSections ()
		{
			// There is always a section for the displaying 
			// AuthorizationStatus from `SKCloudServiceController` 
			// and `MPMediaLibrary`.
			var section = 2;

			// If we have capabilities to display from 
			// RequestCapabilities from SKCloudServiceController.
			if (SKCloudServiceController.AuthorizationStatus != SKCloudServiceAuthorizationStatus.Authorized)
				return section;

			var capabilities = new List<SKCloudServiceCapability> ();
			var cloudServiceCapabilities = AuthorizationManager.CloudServiceCapabilities;

			if (cloudServiceCapabilities.HasFlag (SKCloudServiceCapability.AddToCloudMusicLibrary))
				capabilities.Add (SKCloudServiceCapability.AddToCloudMusicLibrary);

			if (cloudServiceCapabilities.HasFlag (SKCloudServiceCapability.MusicCatalogPlayback))
				capabilities.Add (SKCloudServiceCapability.MusicCatalogPlayback);

			if (cloudServiceCapabilities.HasFlag (SKCloudServiceCapability.MusicCatalogSubscriptionEligible))
				capabilities.Add (SKCloudServiceCapability.MusicCatalogSubscriptionEligible);

			Capabilities = capabilities.ToArray ();

			return ++section;
		}

		public int NumberOfItems (long section)
		{
			var sectionType = (SectionTypes)section;

			switch (sectionType) {
			case SectionTypes.MediaLibraryAuthorizationStatus:
			case SectionTypes.CloudServiceAuthorizationStatus:
				return 1;
			case SectionTypes.RequestCapabilities:
				return Capabilities.Length;
			default:
				return 0;
			}
		}

		public string SectionTitle (long section)
		{
			var sectionType = (SectionTypes)section;

			switch (sectionType) {
			case SectionTypes.MediaLibraryAuthorizationStatus:
				return "MPMediaLibrary";
			case SectionTypes.CloudServiceAuthorizationStatus:
				return "SKCloudServiceController";
			case SectionTypes.RequestCapabilities:
				return "Capabilities";
			default:
				return string.Empty;
			}
		}

		public string StringForItem (NSIndexPath indexPath)
		{
			var sectionType = (SectionTypes)indexPath.Section;

			switch (sectionType) {
			case SectionTypes.MediaLibraryAuthorizationStatus:
				return GetStringValue (MPMediaLibrary.AuthorizationStatus);
			case SectionTypes.CloudServiceAuthorizationStatus:
				return GetStringValue (SKCloudServiceController.AuthorizationStatus);
			case SectionTypes.RequestCapabilities:
				return GetStringValue (Capabilities [indexPath.Row]);
			default:
				return string.Empty;
			}
		}

		#endregion

		#endregion

		#region Private Functionality

		string GetStringValue (SKCloudServiceAuthorizationStatus authorizationStatus)
		{
			switch (authorizationStatus) {
			case SKCloudServiceAuthorizationStatus.NotDetermined:
				return "Not Determined";
			case SKCloudServiceAuthorizationStatus.Denied:
			case SKCloudServiceAuthorizationStatus.Restricted:
			case SKCloudServiceAuthorizationStatus.Authorized:
				return authorizationStatus.ToString ("f");
			default:
				return string.Empty;
			}
		}

		string GetStringValue (MPMediaLibraryAuthorizationStatus authorizationStatus)
		{
			switch (authorizationStatus) {
			case MPMediaLibraryAuthorizationStatus.NotDetermined:
				return "Not Determined";
			case MPMediaLibraryAuthorizationStatus.Denied:
			case MPMediaLibraryAuthorizationStatus.Restricted:
			case MPMediaLibraryAuthorizationStatus.Authorized:
				return authorizationStatus.ToString ("f");
			default:
				return string.Empty;
			}
		}

		string GetStringValue (SKCloudServiceCapability capability)
		{
			switch (capability) {
			case SKCloudServiceCapability.MusicCatalogPlayback:
				return "Music Catalog Playback";
			case SKCloudServiceCapability.MusicCatalogSubscriptionEligible:
				return "Music Catalog Subscription Eligible";
			case SKCloudServiceCapability.AddToCloudMusicLibrary:
				return "Add To Cloud Music Library";
			default:
				return string.Empty;
			}
		}

		#endregion
	}
}
