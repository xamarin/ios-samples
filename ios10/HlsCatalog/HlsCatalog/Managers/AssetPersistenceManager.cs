using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Foundation;
using AVFoundation;
using ObjCRuntime;
using System.Linq;

namespace HlsCatalog
{
	public class AssetPersistenceManager : NSObject, IAVAssetDownloadDelegate
	{
		[DllImport (Constants.FoundationLibrary)]
		public static extern IntPtr NSHomeDirectory ();

		// TODO: Environment.GetFolderPath(Environment.SpecialFolder.Personal)
		public static string ContainerDirectory {
			get {
				return ((NSString)Runtime.GetNSObject (NSHomeDirectory ())).ToString ();
			}
		}

		// Singleton for AssetPersistenceManager.
		public static readonly AssetPersistenceManager SharedManager = new AssetPersistenceManager ();

		// Internal Bool used to track if the AssetPersistenceManager finished restoring its state.
		bool didRestorePersistenceManager;

		// The AVAssetDownloadURLSession to use for managing AVAssetDownloadTasks.
		AVAssetDownloadUrlSession assetDownloadURLSession;

		// Internal map of AVAssetDownloadTask to its corresponding Asset.
		Dictionary<AVAssetDownloadTask, Asset> activeDownloadsMap = new Dictionary<AVAssetDownloadTask, Asset> ();

		// Internal map of AVAssetDownloadTask to its resoled AVMediaSelection
		Dictionary<AVAssetDownloadTask, AVMediaSelection> mediaSelectionMap = new Dictionary<AVAssetDownloadTask, AVMediaSelection> ();

		// The URL to the Library directory of the application's data container.
		NSUrl baseDownloadURL;

		public AssetPersistenceManager ()
		{
			baseDownloadURL = NSUrl.FromFilename (ContainerDirectory);

			// TODO: fix identifier
			// Create the configuration for the AVAssetDownloadURLSession.
			var backgroundConfiguration = NSUrlSessionConfiguration.BackgroundSessionConfiguration ("AAPL-Identifier");

			// Create the AVAssetDownloadURLSession using the configuration.
			assetDownloadURLSession = AVAssetDownloadUrlSession.CreateSession (backgroundConfiguration, this, NSOperationQueue.MainQueue);
		}

		// Restores the Application state by getting all the AVAssetDownloadTasks and restoring their Asset structs.
		public void RestorePersistenceManager ()
		{
			if (!didRestorePersistenceManager)
				return;

			didRestorePersistenceManager = true;

			// Grab all the tasks associated with the assetDownloadURLSession
			// TODO: await ?
			assetDownloadURLSession.GetAllTasks ((NSUrlSessionTask [] tasks) => {
				// For each task, restore the state in the app by recreating Asset structs and reusing existing AVURLAsset objects.
				foreach (var task in tasks) {
					var assetDownloadTask = task as AVAssetDownloadTask;
					if (assetDownloadTask == null)
						return;
					var assetName = assetDownloadTask?.Description;
					if (string.IsNullOrWhiteSpace (assetName))
						return;

					var asset = new Asset { Name = assetName, UrlAsset = assetDownloadTask.UrlAsset };
					activeDownloadsMap [assetDownloadTask] = asset;
				}

				// TODO: raise event
				// NotificationCenter.default.post (name: AssetPersistenceManagerDidRestoreStateNotification, object: nil)
			});
		}

		// Triggers the initial AVAssetDownloadTask for a given Asset.
		void DownloadStream (Asset asset)
		{
			// For the initial download, we ask the URLSession for an AVAssetDownloadTask
			// with a minimum bitrate corresponding with one of the lower bitrate variants
			// in the asset.

			// TODO: method is not bound
			// AVAssetDownloadTaskKeys.MinimumRequiredMediaBitrateKey;
			// options – [AVAssetDownloadTaskMinimumRequiredMediaBitrateKey: 265000]
			var task = assetDownloadURLSession.CreateDownloadTask (asset.UrlAsset, asset.Name, null, null);
			if (task == null)
				return;

			// To better track the AVAssetDownloadTask we set the taskDescription to something unique for our sample.
			task.TaskDescription = asset.Name;

			activeDownloadsMap [task] = asset;

			task.Resume ();

			// TODO: raise event
			// var userInfo = [String: AnyObject] ()
			// userInfo [Asset.Keys.name] = asset.name
			// userInfo [Asset.Keys.downloadState] = Asset.DownloadState.downloading.rawValue
			// NotificationCenter.default.post (name: AssetDownloadStateChangedNotification, object: nil, userInfo: userInfo)
		}

		// Returns an Asset given a specific name if that Asset is asasociated with an active download.
		public Asset AssetForStream (string name)
		{
			return activeDownloadsMap.Values.FirstOrDefault (v => v.Name == name);
		}

		// Returns an Asset pointing to a file on disk if it exists.
		Asset LocalAssetForStream (string name)
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;
			var localFileLocation = userDefaults.StringForKey (name);
			if (string.IsNullOrWhiteSpace(localFileLocation))
				return null;

			var url = baseDownloadURL.Append (localFileLocation, false);
			return new Asset { Name = name, UrlAsset = new AVUrlAsset (url) };
		}

		// Returns the current download state for a given Asset.
		DownloadState GetDownloadState (Asset asset)
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;

			// Check if there is a file URL stored for this asset.
			var localFileLocation = userDefaults.StringForKey (asset.Name);
			if (!string.IsNullOrWhiteSpace (localFileLocation)) {
				// Check if the file exists on disk
				var localFilePath = baseDownloadURL.Append (localFileLocation, false).Path;
				return NSFileManager.DefaultManager.FileExists (localFilePath)
									? DownloadState.Downloaded
									: DownloadState.NotDownloaded;
			}

			// Check if there are any active downloads in flight.
			return activeDownloadsMap.Values.Any (v => v.Name == asset.Name)
									 ? DownloadState.Downloading
									 : DownloadState.NotDownloaded;
		}

		// Deletes an Asset on disk if possible.
		void DeleteAsset (Asset asset)
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;

			var localFileLocation = userDefaults.StringForKey (asset.Name);
			if (!string.IsNullOrWhiteSpace (localFileLocation)) {
				var url = baseDownloadURL.Append (localFileLocation, false);
				NSError error;
				if (NSFileManager.DefaultManager.Remove (url, out error)) {
					userDefaults.RemoveObject (asset.Name);

					// TODO: raise event
					// var userInfo = [String: AnyObject]()
					// userInfo [Asset.Keys.name] = asset.name
					// userInfo [Asset.Keys.downloadState] = Asset.DownloadState.notDownloaded.rawValue
					// NotificationCenter.default.post (name: AssetDownloadStateChangedNotification, object: nil, userInfo: userInfo)
				} else {
					Console.WriteLine ($"An error occured deleting the file: {error}");
				}
			}
		}
	}

	public static class AVAssetDownloadUrlSessionExtensions
	{
		// TODO:
		// assetDownloadTaskWithURLAsset:assetTitle:assetArtworkData:options:
		// !missing-selector! AVAssetDownloadURLSession::assetDownloadTaskWithURLAsset:assetTitle:assetArtworkData:options: not bound
		public static AVAssetDownloadTask CreateDownloadTask (this AVAssetDownloadUrlSession session, AVUrlAsset asset, string title, NSData artworkData, NSDictionary options)
		{
			throw new NotImplementedException ();
		}
	}
}
