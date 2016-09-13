using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Foundation;
using AVFoundation;
using ObjCRuntime;
using CoreMedia;

namespace HlsCatalog
{
	public class DownloadStateEventArgs : EventArgs
	{
		public string AssetName { get; set; }
		public string DisplayName { get; set; }
		public DownloadState State { get; set; }
	}

	public class DownloadProgressEventArgs : EventArgs
	{
		public string AssetName { get; set; }
		public double PercentDownloaded { get; set; }
	}

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
		readonly Dictionary<AVAssetDownloadTask, AVMediaSelection> mediaSelectionMap = new Dictionary<AVAssetDownloadTask, AVMediaSelection> ();

		// The URL to the Library directory of the application's data container.
		NSUrl baseDownloadURL;

		public static event EventHandler StateRestored;
		public static event EventHandler<DownloadStateEventArgs> DownloadStateChanged;
		public static event EventHandler<DownloadProgressEventArgs> DownloadingProgressChanged;

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

				StateRestored?.Invoke (this, EventArgs.Empty);
			});
		}

		// Triggers the initial AVAssetDownloadTask for a given Asset.
		public void DownloadStream (Asset asset)
		{
			// For the initial download, we ask the URLSession for an AVAssetDownloadTask
			// with a minimum bitrate corresponding with one of the lower bitrate variants
			// in the asset.

			// TODO: method is not bound
			// AVAssetDownloadTaskKeys.MinimumRequiredMediaBitrateKey;
			// options – [AVAssetDownloadTaskMinimumRequiredMediaBitrateKey: 265000]
			var task = assetDownloadURLSession.GetAssetDownloadTask (asset.UrlAsset, asset.Name, null, (AVAssetDownloadOptions)null);
			if (task == null)
				return;

			// To better track the AVAssetDownloadTask we set the taskDescription to something unique for our sample.
			task.TaskDescription = asset.Name;

			activeDownloadsMap [task] = asset;

			task.Resume ();

			DownloadStateChanged?.Invoke (this, new DownloadStateEventArgs {
				AssetName = asset.Name,
				State = DownloadState.Downloading
			});
		}

		// Returns an Asset given a specific name if that Asset is asasociated with an active download.
		public Asset AssetForStream (string name)
		{
			return activeDownloadsMap.Values.FirstOrDefault (v => v.Name == name);
		}

		// Returns an Asset pointing to a file on disk if it exists.
		public Asset LocalAssetForStream (string name)
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;
			var localFileLocation = userDefaults.StringForKey (name);
			if (string.IsNullOrWhiteSpace(localFileLocation))
				return null;

			var url = baseDownloadURL.Append (localFileLocation, false);
			return new Asset { Name = name, UrlAsset = new AVUrlAsset (url) };
		}

		// Returns the current download state for a given Asset.
		public DownloadState GetDownloadState (Asset asset)
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
		public void DeleteAsset (Asset asset)
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;

			var localFileLocation = userDefaults.StringForKey (asset.Name);
			if (!string.IsNullOrWhiteSpace (localFileLocation)) {
				var url = baseDownloadURL.Append (localFileLocation, false);
				NSError error;
				if (NSFileManager.DefaultManager.Remove (url, out error)) {
					userDefaults.RemoveObject (asset.Name);
					DownloadStateChanged?.Invoke (this, new DownloadStateEventArgs {
						AssetName = asset.Name,
						State = DownloadState.NotDownloaded
					});
				} else {
					Console.WriteLine ($"An error occured deleting the file: {error}");
				}
			}
		}

		// Cancels an AVAssetDownloadTask given an Asset.
		public void CancelDownload (Asset asset)
		{
			activeDownloadsMap.FirstOrDefault (kvp => kvp.Value.Equals (asset)).Key?.Cancel ();
		}

		// This function demonstrates returns the next AVMediaSelectionGroup and
		// AVMediaSelectionOption that should be downloaded if needed. This is done
		// by querying an AVURLAsset's AVAssetCache for its available `AVMediaSelection`
		// and comparing it to the remote versions.
		Tuple<AVMediaSelectionGroup, AVMediaSelectionOption> NextMediaSelection (AVUrlAsset asset)
		{
			var assetCache = asset.Cache;
			if (assetCache == null)
				return Tuple(null, null);

			NSString [] mediaCharacteristics = {
				AVMediaCharacteristic.Audible,
				AVMediaCharacteristic.Legible
			};
			foreach (var mediaCharacteristic in mediaCharacteristics) {
				// TODO: request strong typed API
				AVMediaSelectionGroup mediaSelectionGroup = asset.MediaSelectionGroupForMediaCharacteristic (mediaCharacteristic);
				if (mediaSelectionGroup != null) {
					var savedOptions = assetCache.GetMediaSelectionOptions (mediaSelectionGroup);
					if (savedOptions.Length < mediaSelectionGroup.Options.Length) {
						// There are still media options left to download.
						foreach (var option in mediaSelectionGroup.Options) {
							// This option has not been download.
							if (!savedOptions.Contains (option))
								return Tuple (mediaSelectionGroup, option);
						}
					}
				}
			}

			// At this point all media options have been downloaded.
			return Tuple (null, null);
		}

		static Tuple<AVMediaSelectionGroup, AVMediaSelectionOption> Tuple (AVMediaSelectionGroup group, AVMediaSelectionOption option)
		{
			return new Tuple<AVMediaSelectionGroup, AVMediaSelectionOption> (group, option);
		}

		void UrlSession (NSUrlSession session, NSUrlSessionTask task, NSError error)
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;

			// This is the ideal place to begin downloading additional media selections
			// once the asset itself has finished downloading.
			var downloadTask = task as AVAssetDownloadTask;
			if (downloadTask == null)
				return;

			Asset asset;
			if (!activeDownloadsMap.TryGetValue (downloadTask, out asset))
				return;
			activeDownloadsMap.Remove (downloadTask);

			// Prepare the basic event arg that will be posted as part of our notification.
			var eventArgs = new DownloadStateEventArgs {
				AssetName = asset.Name
			};

			if (error != null) {
				if (Match (error, NSUrlError.Cancelled)) {
					// TODO: fix comment
					// This task was canceled, you should perform cleanup using the
					// URL saved from AVAssetDownloadDelegate.urlSession (_: assetDownloadTask:didFinishDownloadingTo:).
					var localFileLocation = userDefaults.StringForKey (asset.Name);
					if (string.IsNullOrWhiteSpace (localFileLocation))
						return;

					var fileURL = baseDownloadURL.Append (localFileLocation, false);

					NSError err;
					if (NSFileManager.DefaultManager.Remove (fileURL, out err))
						userDefaults.RemoveObject (asset.Name);
					else
						Console.WriteLine ($"An error occured trying to delete the contents on disk for {asset.Name}: {error}");

					// TODO: port
					//userInfo [Asset.Keys.downloadState] = Asset.DownloadState.notDownloaded.rawValue
				} else if (Match (error, NSUrlError.Unknown)) {
					throw new InvalidProgramException ("Downloading HLS streams is not supported in the simulator.");
				} else {
					Console.WriteLine ($"An unexpected error occured {error.Domain}");
				}
			} else {
				var mediaSelectionPair = NextMediaSelection (downloadTask.UrlAsset);
				var mediaSelectionGroup = mediaSelectionPair.Item1;
				if (mediaSelectionGroup != null) {
					// TODO: fix comment
					// This task did complete sucessfully.At this point the application
					// can download additional media selections if needed.
					// To download additional `AVMediaSelection`s, you should use the
					// `AVMediaSelection` reference saved in `AVAssetDownloadDelegate.urlSession (_: assetDownloadTask:didResolve:)`.
					AVMediaSelection originalMediaSelection;
					if (!mediaSelectionMap.TryGetValue (downloadTask, out originalMediaSelection))
						return;

					// There are still media selections to download.
					// Create a mutable copy of the AVMediaSelection reference saved in
					// `AVAssetDownloadDelegate.urlSession (_: assetDownloadTask:didResolve:)`.
					var mediaSelection = (AVMutableMediaSelection)originalMediaSelection.MutableCopy ();

					// Select the AVMediaSelectionOption in the AVMediaSelectionGroup we found earlier.
					mediaSelection.SelectMediaOption (mediaSelectionPair.Item2, mediaSelectionPair.Item1);

					// Ask the NSUrlSession to vend a new `AVAssetDownloadTask` using
					// the same `AVURLAsset` and assetTitle as before.
					// This time, the application includes the specific `AVMediaSelection`
					// to download as well as a higher bitrate.

					// TODO: provide own implementations for AVAssetDownloadOptions with setters https://bugzilla.xamarin.com/show_bug.cgi?id=44201
					var t = assetDownloadURLSession.GetAssetDownloadTask (downloadTask.UrlAsset, asset.Name, null, new AVAssetDownloadOptions {
						//media
					});

					t.TaskDescription = asset.Name;
					activeDownloadsMap.Add (t, asset);
					t.Resume ();

					eventArgs.DisplayName = mediaSelectionPair.Item2.DisplayName;
					eventArgs.State = DownloadState.Downloading;
					DownloadStateChanged?.Invoke (this, eventArgs);
				} else {
					// All additional media selections have been downloaded.
					eventArgs.State = DownloadState.Downloaded;
				}
			}
			DownloadStateChanged?.Invoke (this, eventArgs);
		}

		bool Match (NSError error, NSUrlError expectedCode)
		{
			return error.Domain == NSError.NSUrlErrorDomain && error.Code == (int)NSUrlError.Cancelled;
		}

		void UrlSession (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, NSUrl location)
		{
			var userDefaults = NSUserDefaults.StandardUserDefaults;

			// TODO: fix comment
			// This delegate callback should only be used to save the location URL
			// somewhere in your application. Any additional work should be done in
			// `URLSessionTaskDelegate.urlSession (_: task:didCompleteWithError:)`.
			Asset asset;
			if (activeDownloadsMap.TryGetValue (assetDownloadTask, out asset))
				userDefaults.SetString (location.RelativePath, asset.Name);
		}

		void UrlSession (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, CMTimeRange timeRange, NSValue [] loadedTimeRanges, CMTimeRange timeRangeExpectedToLoad)
		{
			// This delegate callback should be used to provide download progress for your AVAssetDownloadTask.
			Asset asset;
			if (!activeDownloadsMap.TryGetValue (assetDownloadTask, out asset))
				return;

			double percentComplete = 0;

			foreach (var value in loadedTimeRanges) {
				var loadedTimeRange = value.CMTimeRangeValue;
				percentComplete += loadedTimeRange.Duration.Seconds / timeRangeExpectedToLoad.Duration.Seconds;
			}

			DownloadingProgressChanged?.Invoke (this, new DownloadProgressEventArgs {
				AssetName = asset.Name,
				PercentDownloaded = percentComplete
			});
		}

		void UrlSession (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, AVMediaSelection resolvedMediaSelection)
		{
			// You should be sure to use this delegate callback to keep a reference
			// to `resolvedMediaSelection` so that in the future you can use it to
			// download additional media selections.
			mediaSelectionMap [assetDownloadTask] = resolvedMediaSelection;
		}
	}
}
