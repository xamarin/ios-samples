using System;

using AVFoundation;

namespace HlsCatalog
{
	public enum DownloadState
	{
		// The asset is not downloaded at all.
		NotDownloaded,

		// The asset has a download in progress.
		Downloading,

		// The asset is downloaded and saved on disk.
		Downloaded
	}

	public class Asset : IEquatable<Asset>
	{
		// Key for the Asset name, used for `AssetDownloadProgressNotification` and
		// `AssetDownloadStateChangedNotification` Notifications as well as
		// AssetListManager.
		public static readonly string NameKey = "AssetNameKey";

		// Key for the Asset download percentage, used for
		// `AssetDownloadProgressNotification` Notification.
		public static readonly string PercentDownloadedKey = "AssetPercentDownloadedKey";

         // Key for the Asset download state, used for
         // `AssetDownloadStateChangedNotification` Notification.
		public static readonly string DownloadStateKey = "AssetDownloadStateKey";

         // Key for the Asset download AVMediaSelection display Name, used for
         // `AssetDownloadStateChangedNotification` Notification.
		public static readonly string DownloadSelectionDisplayNameKey = "AssetDownloadSelectionDisplayNameKey";

		public string Name { get; set; }

		public AVUrlAsset UrlAsset { get; set; }

		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;

			if (obj.GetType () != typeof (Asset))
				return false;

			var asset = (Asset)obj;
			return Equals (asset);
		}

		public bool Equals (Asset other)
		{
			return Name == other.Name && UrlAsset.Equals (other.UrlAsset);
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode () ^ UrlAsset.GetHashCode ();
		}
	}
}
