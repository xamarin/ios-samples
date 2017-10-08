using System;
using Foundation;
using MusicKitSample.Models;
using MusicKitSample.Utilities;
using System.Collections.Generic;
using UIKit;
using System.Threading.Tasks;
namespace MusicKitSample.Controllers
{
	public class AppleMusicManager
	{
		#region Properties

		// The instance of `URLSession` that is going to be used for making network calls.
		public NSUrlSession UrlSession { get; } = NSUrlSession.FromConfiguration (NSUrlSessionConfiguration.DefaultSessionConfiguration);

		// The storefront id that is used when making Apple Music API calls.
		public string StorefrontId { get; set; }

		#endregion

		#region Constructors

		public AppleMusicManager ()
		{
		}

		#endregion

		#region Public Functionality

		#region General Apple Music API Methods

		public string FetchDeveloperToken ()
		{
			// ADAPT: YOU MUST IMPLEMENT THIS METHOD
			string developerAuthenticationToken = null;
			return developerAuthenticationToken;
		}

		public async Task<MediaItem [] []> PerformAppleMusicCatalogSearchAsync (string term, string countryCode)
		{
			var developerToken = FetchDeveloperToken ();
			if (developerToken == null)
				throw new ArgumentNullException (nameof (developerToken), "Developer Token not configured. See README for more details.");

			var urlRequest = AppleMusicRequestFactory.CreateSearchRequest (term, countryCode, developerToken);
			var dataTaskRequest = await UrlSession.CreateDataTaskAsync (urlRequest);
			var urlResponse = dataTaskRequest.Response as NSHttpUrlResponse;

			if (urlResponse?.StatusCode != 200)
				return new MediaItem [0] [];

			return ProcessMediaItemSections (dataTaskRequest.Data);
		}

		public async Task<string> PerformAppleMusicStorefrontsLookupAsync (string regionCode)
		{
			var developerToken = FetchDeveloperToken ();
			if (developerToken == null)
				throw new ArgumentNullException (nameof (developerToken), "Developer Token not configured. See README for more details.");

			var urlRequest = AppleMusicRequestFactory.CreateStorefrontsRequest (regionCode, developerToken);
			var dataTaskRequest = await UrlSession.CreateDataTaskAsync (urlRequest);
			var urlResponse = dataTaskRequest.Response as NSHttpUrlResponse;

			if (urlResponse?.StatusCode != 200)
				return null;

			return ProcessStorefront (dataTaskRequest.Data);
		}

		#endregion

		#region Personalized Apple Music API Methods

		public async Task<MediaItem []> PerformAppleMusicGetRecentlyPlayedAsync (string userToken)
		{
			var developerToken = FetchDeveloperToken ();
			if (developerToken == null)
				throw new ArgumentNullException (nameof (developerToken), "Developer Token not configured. See README for more details.");

			var urlRequest = AppleMusicRequestFactory.CreateRecentlyPlayedRequest (developerToken, userToken);
			var dataTaskRequest = await UrlSession.CreateDataTaskAsync (urlRequest);
			var urlResponse = dataTaskRequest.Response as NSHttpUrlResponse;

			if (urlResponse?.StatusCode != 200)
				return new MediaItem [0];

			var jsonDictionary = NSJsonSerialization.Deserialize (dataTaskRequest.Data, NSJsonReadingOptions.AllowFragments, out NSError error) as NSDictionary;
			if (error != null)
				throw new NSErrorException (error);

			var results = jsonDictionary [ResponseRootJsonKeys.Data] as NSArray ??
				throw new SerializationException (ResponseRootJsonKeys.Data);

			return ProcessMediaItems (results);
		}

		public async Task<string> PerformAppleMusicGetUserStorefrontAsync (string userToken)
		{
			var developerToken = FetchDeveloperToken ();
			if (developerToken == null)
				throw new ArgumentNullException (nameof (developerToken), "Developer Token not configured. See README for more details.");

			var urlRequest = AppleMusicRequestFactory.CreateGetUserStorefrontRequest (developerToken, userToken);
			var dataTaskRequest = await UrlSession.CreateDataTaskAsync (urlRequest);
			var urlResponse = dataTaskRequest.Response as NSHttpUrlResponse;

			if (urlResponse?.StatusCode != 200)
				return null;

			return ProcessStorefront (dataTaskRequest.Data);
		}

		#endregion

		#endregion

		#region Private Functionality

		MediaItem [] [] ProcessMediaItemSections (NSData json)
		{
			var jsonDictionary = NSJsonSerialization.Deserialize (json, NSJsonReadingOptions.AllowFragments, out NSError error) as NSDictionary;

			if (error != null)
				throw new NSErrorException (error);
			
			var results = jsonDictionary [ResponseRootJsonKeys.Results] as NSDictionary ??
				throw new SerializationException (ResponseRootJsonKeys.Results);

			var mediaItems = new List<MediaItem []> ();
			var songsDictionary = results [ResourceTypeJsonKeys.Songs] as NSDictionary;
			var dataArray = songsDictionary? [ResponseRootJsonKeys.Data] as NSArray;

			if (dataArray != null)
				mediaItems.Add (ProcessMediaItems (dataArray));

			var albumsDictionary = results [ResourceTypeJsonKeys.Albums] as NSDictionary;
			dataArray = albumsDictionary? [ResponseRootJsonKeys.Data] as NSArray;

			if (dataArray != null)
				mediaItems.Add (ProcessMediaItems (dataArray));

			return mediaItems.ToArray ();
		}

		MediaItem [] ProcessMediaItems (NSArray json)
		{
			var mediaItems = new MediaItem [json.Count];

			for (nuint i = 0; i < json.Count; i++)
				mediaItems [i] = MediaItem.From (json.GetItem<NSDictionary> (i));

			return mediaItems;
		}

		string ProcessStorefront (NSData json)
		{
			var jsonDictionary = NSJsonSerialization.Deserialize (json, NSJsonReadingOptions.AllowFragments, out NSError error) as NSDictionary;

			if (error != null)
				throw new NSErrorException (error);

			var dataArray = jsonDictionary [ResponseRootJsonKeys.Data] as NSArray ??
				throw new SerializationException (ResponseRootJsonKeys.Data);
			var id = dataArray?.GetItem<NSDictionary> (0) [ResourceJsonKeys.Id]?.ToString () ??
					   throw new SerializationException (ResourceJsonKeys.Id);

			return id;
		}

		#endregion
	}
}
