using System;

using Foundation;
using CoreFoundation;

namespace ListerKit
{
	public static class ListUtilities
	{
		static readonly NSOperationQueue queue = new NSOperationQueue();

		public static NSUrl SharedApplicationGroupContainer {
			get {
				throw new NotImplementedException ();
				NSUrl containerURL = NSFileManager.DefaultManager.GetContainerUrl (AppConfig.ApplicationGroupsPrimary);

				if (containerURL == null)
					throw new InvalidProgramException ("The shared application group container is unavailable. Check your entitlements and provisioning profiles for this target. Details on proper setup can be found in the README.");

				return containerURL;
			}
		}

		public static NSUrl LocalDocumentsDirectory {
			get {
				NSUrl documentsURL = SharedApplicationGroupContainer.Append ("Documents", true);

				NSError error;
				// This will return `true` for success if the directory is successfully created, or already exists.
				bool success = NSFileManager.DefaultManager.CreateDirectory(documentsURL, true, null, out error);
				if (success)
					return documentsURL;
				else
					throw new InvalidProgramException (string.Format ("The shared application group documents directory doesn't exist and could not be created. Error: {0}", error.LocalizedDescription));
			}
		}

		public static void CopyInitialLists()
		{
			NSUrl[] defaultListURLs = NSBundle.MainBundle.GetUrlsForResourcesWithExtension (AppConfig.ListerFileExtension, string.Empty);
			foreach (var url in defaultListURLs)
				CopyUrlToDocumentsDirectory(url);
		}

		public static void CopyTodayList()
		{
			string localizedTodayListName = AppConfig.SharedAppConfiguration.TodayDocumentName;
			NSUrl url = NSBundle.MainBundle.GetUrlForResource (localizedTodayListName, AppConfig.ListerFileExtension);
			CopyUrlToDocumentsDirectory (url);
		}

		public static void MigrateLocalListsToCloud ()
		{
			var defaultQueue = DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Default);

			defaultQueue.DispatchAsync (() => {
				NSFileManager fileManager = NSFileManager.DefaultManager;

				// Note the call to GetUrlForUbiquityContainer should be on a background queue.
				NSUrl cloudDirectoryURL = fileManager.GetUrlForUbiquityContainer(null);

				NSUrl documentsDirectoryURL = cloudDirectoryURL.Append("Documents", false);

				NSError error;
				NSUrl[] localDocumentURLs = fileManager.GetDirectoryContent(LocalDocumentsDirectory, null, NSDirectoryEnumerationOptions.SkipsNone, out error);

				foreach (var url in localDocumentURLs) {
					if (url.GetPathExt() == AppConfig.ListerFileExtension)
						MakeItemUbiquitousAtURL(url, documentsDirectoryURL);
				}
			});
		}

		static string GetPathExt(this NSUrl url)
		{
			string ext = System.IO.Path.GetExtension (url.Path);
			if (string.IsNullOrEmpty (ext))
				return string.Empty;

			// remove lead '.' Ex: ".txt" -> "txt"
			return ext.Substring (1);
		}

		static void MakeItemUbiquitousAtURL (NSUrl sourceURL, NSUrl documentsDirectoryURL)
		{
			string destinationFileName = sourceURL.LastPathComponent;

			var fileManager = new NSFileManager ();
			NSUrl destinationURL = documentsDirectoryURL.Append (destinationFileName, false);

			if (fileManager.IsUbiquitous(destinationURL) ||
				fileManager.FileExists(destinationURL.Path)) {
				// If the file already exists in the cloud, remove the local version and return.
				RemoveListAtURL(sourceURL, null);
				return;
			}

			var defaultQueue = DispatchQueue.GetGlobalQueue (DispatchQueuePriority.Default);
			defaultQueue.DispatchAsync (() => {
				NSError error;
				fileManager.SetUbiquitous(true, sourceURL, destinationURL, out error);
			});
		}

		public static void ReadListAtUrl(NSUrl url, Action<List, NSError> completionHandler)
		{
			var fileCoordinator = new NSFileCoordinator ();

			// `url` may be a security scoped resource.
			bool successfulSecurityScopedResourceAccess = url.StartAccessingSecurityScopedResource ();

			NSFileAccessIntent readingIntent = NSFileAccessIntent.CreateReadingIntent (url, NSFileCoordinatorReadingOptions.WithoutChanges);
			fileCoordinator.CoordinateAccess (new NSFileAccessIntent[]{ readingIntent }, queue, accessError => {
				if (accessError != null) {
					if (successfulSecurityScopedResourceAccess)
						url.StopAccessingSecurityScopedResource ();

					if (completionHandler != null)
						completionHandler (null, accessError);

					return;
				}

				// Local variables that will be used as parameters to `completionHandler`.
				NSError readError;
				List deserializedList = null;

				NSData contents = NSData.FromUrl (readingIntent.Url, NSDataReadingOptions.Uncached, out readError);

				if (contents != null) {
					deserializedList = (List)NSKeyedUnarchiver.UnarchiveObject (contents);
					if (deserializedList == null)
						throw new InvalidProgramException ("The provided URL must correspond to an List object.");
				}

				if (successfulSecurityScopedResourceAccess)
					url.StopAccessingSecurityScopedResource ();

				if (completionHandler != null)
					completionHandler (deserializedList, readError);
			});
		}

		static void CreateList(List list, NSUrl url, Action<NSError> completionHandler)
		{
			var fileCoordinator = new NSFileCoordinator ();

			NSFileAccessIntent writingIntent = NSFileAccessIntent.CreateWritingIntent (url, NSFileCoordinatorWritingOptions.ForReplacing);
			fileCoordinator.CoordinateAccess (new NSFileAccessIntent[]{ writingIntent }, queue, accessError => {
				if (accessError != null) {
					if (completionHandler != null)
						completionHandler (accessError);
					return;
				}

				NSError error;
				NSData serializedListData = NSKeyedArchiver.ArchivedDataWithRootObject (list);

				bool success = serializedListData.Save (writingIntent.Url, NSDataWritingOptions.Atomic, out error);
				if (success)
					NSFileManager.DefaultManager.SetAttributes (new NSFileAttributes{ ExtensionHidden = true }, writingIntent.Url.Path);

				if (completionHandler != null)
					completionHandler (error);
			});
		}

		static void RemoveListAtURL(NSUrl url, Action<NSError> completionHandler)
		{
			var fileCoordinator = new NSFileCoordinator ();

			// `url` may be a security scoped resource.
			bool successfulSecurityScopedResourceAccess = url.StartAccessingSecurityScopedResource ();

			NSFileAccessIntent writingIntent = NSFileAccessIntent.CreateWritingIntent (url, NSFileCoordinatorWritingOptions.ForDeleting);
			fileCoordinator.CoordinateAccess (new NSFileAccessIntent[]{ writingIntent }, queue, accessError => {
				if (accessError != null) {
					if (completionHandler != null)
						completionHandler (accessError);
					return;
				}

				var fileManager = new NSFileManager ();

				NSError error;

				fileManager.Remove (writingIntent.Url, out error);

				if (successfulSecurityScopedResourceAccess)
					url.StopAccessingSecurityScopedResource ();

				if (completionHandler != null)
					completionHandler (error);
			});
		}

		#region Convenience

		static void CopyUrlToDocumentsDirectory (NSUrl url)
		{
			NSUrl toURL = ListUtilities.LocalDocumentsDirectory.Append (url.LastPathComponent, false);

			// If the file already exists, don't attempt to copy the version from the bundle.
			if (NSFileManager.DefaultManager.FileExists (toURL.Path))
				return;

			var fileCoordinator = new NSFileCoordinator ();
			NSError error;

			bool successfulSecurityScopedResourceAccess = url.StartAccessingSecurityScopedResource ();

			NSFileAccessIntent movingIntent = NSFileAccessIntent.CreateWritingIntent (url, NSFileCoordinatorWritingOptions.ForMoving);
			NSFileAccessIntent replacingIntent = NSFileAccessIntent.CreateWritingIntent (toURL, NSFileCoordinatorWritingOptions.ForReplacing);
			fileCoordinator.CoordinateAccess (new NSFileAccessIntent[]{ movingIntent, replacingIntent }, queue, accessError => {
				if (accessError != null) {
					// An error occured when trying to coordinate moving URL to toURL. In your app, handle this gracefully.
					Console.WriteLine ("Couldn't move file: {0} to: {1} error: {2}.", url.AbsoluteString, toURL.AbsoluteString, accessError.LocalizedDescription);
					return;
				}

				var fileManager = new NSFileManager ();

				bool success = fileManager.Copy (movingIntent.Url, replacingIntent.Url, out error);
				if (success)
					fileManager.SetAttributes (new NSFileAttributes{ ExtensionHidden = true }, replacingIntent.Url.Path);

				if (successfulSecurityScopedResourceAccess)
					url.StopAccessingSecurityScopedResource ();

				if (!success) {
					// An error occured when moving URL to toURL. In your app, handle this gracefully.
					Console.WriteLine ("Couldn't move file: {0} to: {1}.", url.AbsoluteString, toURL.AbsoluteString);
				}
			});
		}

		static void RemoveListAtURL (NSUrl sourceURL, object o)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}