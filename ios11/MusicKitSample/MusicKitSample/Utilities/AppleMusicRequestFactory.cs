/*
Abstract:
The `AppleMusicRequestFactory` type is used to build the various Apple Music API calls used by the sample.
*/

using System;
using Foundation;
using System.Collections.Generic;
namespace MusicKitSample.Utilities {
	public struct AppleMusicRequestFactory {
		#region Types

		// The base URL for all Apple Music API network calls.
		static readonly string appleMusicApiBaseUrl = "api.music.apple.com";

		// The Apple Music API endpoint for requesting a list of recently played items.
		static readonly string recentlyPlayedPathUrl = "/v1/me/recent/played";

		// The Apple Music API endpoint for requesting a the storefront of the currently logged in iTunes Store account.
		static readonly string userStorefrontPathUrl = "/v1/me/storefront";

		public static NSUrlRequest CreateSearchRequest (string term, string countryCode, string developerToken)
		{
			// Create the URL components for the network call.
			var urlComponents = CreateUrlComponents ($"/v1/catalog/{countryCode}/search");

			var expectedTerms = term.Replace (" ", "+");
			var urlParameters = new Dictionary<string, string> {
				{ "term", expectedTerms },
				{ "limit", "10" },
				{ "types", "songs,albums" }
			};

			var queryItems = new List<NSUrlQueryItem> ();
			foreach (var urlParameter in urlParameters)
				queryItems.Add (new NSUrlQueryItem (urlParameter.Key, urlParameter.Value));

			urlComponents.QueryItems = queryItems.ToArray ();

			// Create and configure the `URLRequest`.
			var urlRequest = new NSMutableUrlRequest (urlComponents.Url) { HttpMethod = "GET" };
			urlRequest.Headers = new NSDictionary ("Authorization", $"Bearer {developerToken}");

			return urlRequest;
		}

		#endregion

		#region Public Functionality

		public static NSUrlRequest CreateStorefrontsRequest (string regionCode, string developerToken)
		{
			// Create the URL components for the network call.
			var urlComponents = CreateUrlComponents ($"/v1/storefronts/{regionCode}");

			// Create and configure the `URLRequest`.
			var urlRequest = new NSMutableUrlRequest (urlComponents.Url) { HttpMethod = "GET" };
			urlRequest.Headers = new NSDictionary ("Authorization", $"Bearer {developerToken}");

			return urlRequest;
		}

		public static NSUrlRequest CreateRecentlyPlayedRequest (string developerToken, string userToken)
		{
			// Create the URL components for the network call.
			var urlComponents = CreateUrlComponents (recentlyPlayedPathUrl);

			// Create and configure the `URLRequest`.
			var urlRequest = new NSMutableUrlRequest (urlComponents.Url) { HttpMethod = "GET" };
			urlRequest.Headers = new NSDictionary ("Authorization", $"Bearer {developerToken}",
								   "Music-User-Token", userToken);

			return urlRequest;
		}

		public static NSUrlRequest CreateGetUserStorefrontRequest (string developerToken, string userToken)
		{
			// Create the URL components for the network call.
			var urlComponents = CreateUrlComponents (userStorefrontPathUrl);

			// Create and configure the `URLRequest`.
			var urlRequest = new NSMutableUrlRequest (urlComponents.Url) { HttpMethod = "GET" };
			urlRequest.Headers = new NSDictionary ("Authorization", $"Bearer {developerToken}",
								   "Music-User-Token", userToken);

			return urlRequest;
		}

		#endregion

		#region Private Functionality

		static NSUrlComponents CreateUrlComponents (string fromPath)
		{
			return new NSUrlComponents {
				Scheme = "https",
				Host = appleMusicApiBaseUrl,
				Path = fromPath
			};
		}

		#endregion
	}
}
