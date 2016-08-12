using System;
using System.Collections.Generic;
using Foundation;

namespace HlsCatalog
{
	public class AssetListManager
	{
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
			// TODO:
			//notificationCenter.addObserver(self, selector: #selector(handleAssetPersistenceManagerDidRestoreStateNotification(_:)), name: AssetPersistenceManagerDidRestoreStateNotification, object: nil)
		}
	}
}
