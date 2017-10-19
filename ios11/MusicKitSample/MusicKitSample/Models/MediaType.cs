using System;
namespace MusicKitSample.Models
{
	// - Songs: This indicates that the `MediaItem` is a song from the Apple Music Catalog.
	// - Albums: This indicates that the `MediaItem` is an album from the Apple Music Catalog.
	public enum MediaType
	{
		Songs,
		Albums,
		Stations,
		Playlists,
		Unknown
	}
}
