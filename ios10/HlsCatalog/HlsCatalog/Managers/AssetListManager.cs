using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using CoreFoundation;
using AVFoundation;
using ObjCRuntime;

namespace HlsCatalog
{
	public class AssetListManager : IDisposable
	{
		public static event EventHandler Loaded;

		// A singleton instance of AssetListManager.
		public static readonly AssetListManager SharedManager = new AssetListManager ();

		// The internal list of Asset structs.
		readonly List<Asset> assets = new List<Asset> ();

		public AssetListManager ()
		{
			// Do not setup the AssetListManager.assets until AssetPersistenceManager has
			// finished restoring.  This prevents race conditions where the `AssetListManager`
			// creates a list of `Asset`s that doesn't reuse already existing `AVURLAssets`
			// from existng `AVAssetDownloadTasks.
			var notificationCenter = NSNotificationCenter.DefaultCenter;

			AssetPersistenceManager.StateRestored += StateRestoredHandler;
		}

		public void Dispose ()
		{
			AssetPersistenceManager.StateRestored -= StateRestoredHandler;
		}

		// Returns the number of Assets.
		public int NumberOfAssets {
			get {
				return assets.Count;
			}
		}

		// Returns an Asset for a given IndexPath.
		public Asset AssetAt (int index)
		{
			return assets [index];
		}

		void StateRestoredHandler (object sender, EventArgs e)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				// Get the file path of the Streams.plist from the application bundle.
				var streamsFilepath = NSBundle.MainBundle.PathForResource ("Streams", "plist");
				if (string.IsNullOrWhiteSpace (streamsFilepath))
					return;

				var arrayOfStreams = LoadAssets (streamsFilepath);

				// Iterate over each dictionary in the array.
				foreach (var entry in arrayOfStreams) {
					// To ensure that we are reusing AVUrlAssets we first find out if there is one available for an already active download.
					var asset = AssetPersistenceManager.SharedManager.AssetForStream (entry.Name);

					// If an existing AVUrlAsset is not available for an active
					// download we then see if there is a file URL available to create an asset from.
					asset = asset ?? AssetPersistenceManager.SharedManager.LocalAssetForStream (entry.Name);

					// No instance of AVUrlAsset exists for this stream, use deserialized one.
					asset = asset ?? entry;

					assets.Add (asset);
				}
				Loaded?.Invoke (this, EventArgs.Empty);
			});
		}

		static Asset[] LoadAssets (string path)
		{
			var array = NSArray.FromFile (path);
			var items = new NSArrayIterator (array);
			return items.Cast<NSDictionary> ()
						.Select (CreateAsset)
						.ToArray ();
		}

		static Asset CreateAsset (NSDictionary dictionary)
		{
			return new Asset {
				Name = dictionary.GetString("AssetNameKey"),
				UrlAsset = new AVUrlAsset (NSUrl.FromString (dictionary.GetString ("StreamPlaylistUrl")))
			};
		}
	}

	class NSArrayIterator : IEnumerable<INativeObject>
	{
		readonly NSArray array;

		public NSArrayIterator (NSArray array)
		{
			this.array = array;
		}

		public IEnumerator<INativeObject> GetEnumerator ()
		{
			for (nuint i = 0; i < array.Count; i++)
				yield return array.GetItem<INativeObject> (i);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}

	static class NSDictionaryExtensions
	{
		public static string GetString (this NSDictionary dictionary, string key)
		{
			return (NSString)dictionary.ObjectForKey ((NSString)key);
		}
	}
}
