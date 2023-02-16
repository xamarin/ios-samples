/*
Abstract:
`MediaItem` represents a `Resource` object from the Apple Music Web Services.
*/

using System;
using Foundation;
namespace MusicKitSample.Models {
	public class MediaItem {
		#region Types

		// The various keys needed for serializing an instance of `MediaItem`
		// using a Json response from the Apple Music Web Service.
		struct JsonKeys {
			public static readonly string Id = "id";
			public static readonly string Type = "type";
			public static readonly string Attributes = "attributes";
			public static readonly string Name = "name";
			public static readonly string ArtistName = "artistName";
			public static readonly string Artwork = "artwork";
		}

		#endregion

		#region Properties

		// The persistent identifier of the resource which is used 
		// to add the item to the playlist or trigger playback.
		public string Id { get; set; }

		// The localized name of the album or song.
		public string Name { get; set; }

		// The artist’s name.
		public string ArtistName { get; set; }

		// The album artwork associated with the song or album.
		public Artwork Artwork { get; set; }

		// The type of the `MediaItem` which in this application can be either `songs` or `albums`.
		public MediaType Type { get; set; }

		#endregion

		#region Constructors

		MediaItem ()
		{
		}

		#endregion

		#region Static Functionality

		public static MediaItem Create () => From (string.Empty, string.Empty, string.Empty, Artwork.Create (), MediaType.Songs);

		public static MediaItem From (string id, string name, string artistName, Artwork artwork, MediaType type)
		{
			return new MediaItem {
				Id = id,
				Name = name,
				ArtistName = artistName,
				Artwork = artwork,
				Type = type
			};
		}

		public static MediaItem From (NSDictionary json)
		{
			var id = json [JsonKeys.Id]?.ToString () ?? throw new SerializationException (JsonKeys.Id);

			var typeString = json [JsonKeys.Type]?.ToString () ?? throw new SerializationException (JsonKeys.Type);
			var type = (MediaType) Enum.Parse (typeof (MediaType), typeString, true);

			var attributes = json [JsonKeys.Attributes] as NSDictionary ??
				throw new SerializationException (JsonKeys.Attributes);
			var name = attributes [JsonKeys.Name]?.ToString () ?? throw new SerializationException (JsonKeys.Name);

			var artworkJson = attributes [JsonKeys.Artwork] as NSDictionary ??
				throw new SerializationException (JsonKeys.Artwork);
			var artwork = Artwork.From (artworkJson);

			var artistName = attributes [JsonKeys.ArtistName]?.ToString () ?? string.Empty;

			return From (id, name, artistName, artwork, type);
		}

		#endregion
	}
}
