/*
Abstract:
`Artwork` represents a `Artwork` object from the Apple Music Web Services.
*/

using System;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;
namespace MusicKitSample.Models
{
	public class Artwork
	{
		#region Types

		// The various keys needed for serializing an instance of `Artwork` using a JSON response from the Apple Music Web Service.
		struct JsonKeys
		{
			public static readonly string Height = "height";
			public static readonly string Width = "width";
			public static readonly string Url = "url";
		}

		#endregion

		#region Properties

		/// The maximum height available for the image.
		public int Height { get; set; }
		/// The maximum width available for the image.
		public int Width { get; set; }

		/*
		 * The string representation of the URL to request the image asset. This template should be used to create the URL for the correctly sized image 
		 * your application wishes to use.  See `Artwork.imageURL(size:)` for additional information.
		 */
		public string UrlTemplate { get; set; }

		#endregion

		#region Constructors

		Artwork ()
		{
		}

		#endregion

		#region Static Constructors

		public static Artwork Create () => From (0, 0, string.Empty);

		public static Artwork From (int height, int width, string urlTemplate)
		{
			return new Artwork {
				Height = height,
				Width = width,
				UrlTemplate = urlTemplate
			};
		}

		public static Artwork From (NSDictionary json)
		{
			var heightString = json [JsonKeys.Height]?.ToString () ?? throw new SerializationException (JsonKeys.Height);
			var widthString = json [JsonKeys.Width]?.ToString () ?? throw new SerializationException (JsonKeys.Width);
			var urlTemplate = json [JsonKeys.Url]?.ToString () ?? throw new SerializationException (JsonKeys.Url);

			int.TryParse (json [JsonKeys.Height]?.ToString () ?? string.Empty, out int height);
			int.TryParse (json [JsonKeys.Width]?.ToString () ?? string.Empty, out int width);

			return From (height, width, urlTemplate);
		}

		#endregion

		#region Public Functionality

		public NSUrl GenerateImageUrl () => GenerateImageUrl (new CGSize (Width, Height));

		public NSUrl GenerateImageUrl (CGSize size)
		{
			/*
			 * There are three pieces of information needed to create
			 * the Url for the image we want for a given size.
			 * This information is the width, height and image format.
			 * We can use this information in addition to the `urlTemplate`
			 * to create the Url for the image we wish to use.
			 */

			// 1) Replace the "{w}" placeholder with the desired width as an integer value.
			var imageUrl = UrlTemplate.Replace ("{w}", size.Width.ToString ());

			// 2) Replace the "{h}" placeholder with the desired height as an integer value.
			imageUrl = imageUrl.Replace ("{h}", size.Height.ToString ());

			// 3) Replace the "{f}" placeholder with the desired image format.
			imageUrl = imageUrl.Replace ("{f}", "png");

			return NSUrl.FromString (imageUrl);
		}

		#endregion
	}
}
