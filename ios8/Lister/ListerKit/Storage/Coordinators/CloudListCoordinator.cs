using System;
using Foundation;
using CoreFoundation;
using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;

namespace ListerKit
{
	public class CloudListCoordinator : IListCoordinator
	{
		NSMetadataQuery metadataQuery;
		DispatchQueue documentsDirectoryQueue;

		NSObject finishGatheringToken;
		NSObject didUpdateNotificationToken;

		// Closure executed after the first update provided by the coordinator regarding tracked Urls.
		Action firstQueryUpdateHandler;

		// Get value only from documentsDirectoryQueue
		NSUrl documentsDirectory;
		NSUrl DocumentsDirectory {
			get {
				NSUrl docDir;
				documentsDirectoryQueue.DispatchSync(() => {
					docDir = documentsDirectory;
				});
				return docDir;
			}
		}

		public CloudListCoordinator (NSPredicate predicate, Action firstQueryUpdateHandler)
		{
			this.firstQueryUpdateHandler = firstQueryUpdateHandler;
			documentsDirectoryQueue = new DispatchQueue (string.Format ("{0}.cloudlistcoordinator.documentsDirectory", NSBundle.MainBundle.BundleIdentifier));

			metadataQuery = new NSMetadataQuery ();
			metadataQuery.SearchScopes = new NSObject[] {
				NSMetadataQuery.UbiquitousDocumentsScope,
				NSMetadataQuery.AccessibleUbiquitousExternalDocumentsScope
			};

			metadataQuery.Predicate = predicate;
			metadataQuery.OperationQueue = new NSOperationQueue ();
			metadataQuery.OperationQueue.Name = string.Format ("{0}.cloudlistcoordinator.metadataQuery", NSBundle.MainBundle.BundleIdentifier);

			documentsDirectoryQueue.DispatchAsync (()=> {
				NSUrl cloudContainerURL = NSFileManager.DefaultManager.GetUrlForUbiquityContainer(null);
				documentsDirectory = cloudContainerURL.Append("Documents", true);
			});

			// Observe the query.
			NSNotificationCenter notificationCenter = NSNotificationCenter.DefaultCenter;

			finishGatheringToken = notificationCenter.AddObserver (NSMetadataQuery.DidFinishGatheringNotification,
				HandleMetadataQueryUpdates, metadataQuery);

			didUpdateNotificationToken = notificationCenter.AddObserver (NSMetadataQuery.DidUpdateNotification,
				HandleMetadataQueryUpdates, metadataQuery);
		}

		public static CloudListCoordinator CreateFromPathExtension (string pathExtenstion, Action firstQueryUpdateHandler)
		{
			var predicate = NSPredicate.FromFormat("(%K.pathExtension = %@)", NSMetadataQuery.ItemURLKey, (NSString)pathExtenstion);
			return new CloudListCoordinator (predicate, firstQueryUpdateHandler);
		}

		public static CloudListCoordinator CreateFromLastPathComponent (string lastPathComponent, Action firstQueryUpdateHandler)
		{
			var predicate = NSPredicate.FromFormat ("(%K.lastPathComponent = %@)", NSMetadataQuery.ItemURLKey, lastPathComponent);
			return new CloudListCoordinator (predicate, firstQueryUpdateHandler);
		}

		#region IListCoordinator implementation

		public void StartQuery ()
		{
			// NSMetadataQuery should always be started on the main thread.
			DispatchQueue.MainQueue.DispatchAsync (metadataQuery.StartQuery);
		}

		public void StopQuery ()
		{
			// NSMetadataQuery should always be stopped on the main thread.
			DispatchQueue.MainQueue.DispatchAsync (metadataQuery.StopQuery);
		}

		public void RemoveListAtUrl (Foundation.NSUrl url)
		{
			ListUtilities.ReadListAtUrl (url, error => {
				if (error != null)
					Delegate.DidFailRemovingListAtURL (url, error);
				else
					Delegate.DidUpdateContentsWithInsertedURLs (null, new NSUrl[]{ url }, null);
			});
		}

		public void CreateUrlForList (List list, string name)
		{
			NSUrl documentURL = DocumentURLForName (name);

			ListUtilities.CreateList (list, documentURL, error => {
				if (error != null)
					Delegate.DidFailCreatingListAtURL (documentURL, error);
				else
					Delegate.DidUpdateContentsWithInsertedURLs (new NSUrl[]{ documentURL }, null, null);
			});
		}

		public bool CanCreateListWithName (string name)
		{
			if (string.IsNullOrWhiteSpace (name))
				return false;

			NSUrl documentURL = DocumentURLForName (name);
			return !NSFileManager.DefaultManager.FileExists (documentURL.Path);
		}

		public IListCoordinatorDelegate Delegate { get; set; }

		#endregion

		#region NSMetadataQuery Notifications

		void HandleMetadataQueryUpdates(NSNotification notification)
		{
			metadataQuery.DisableUpdates ();

			var insertedURLs = metadataQuery.Results.Select (item => item.Url);
			Delegate.DidUpdateContentsWithInsertedURLs (insertedURLs, null, null);

			metadataQuery.EnableUpdates ();
		}

		void HandleMetadataQueryDidUpdate(NSNotification notification)
		{
			metadataQuery.DisableUpdates ();

			NSUrl[] insertedURLs;
			NSUrl[] removedURLs;
			NSUrl[] updatedURLs;

			// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=27550
			IntPtr foundationHandle = Dlfcn.dlopen ("/System/Library/Frameworks/Foundation.framework/Foundation", 0);
			NSString NSMetadataQueryUpdateAddedItemsKey = Dlfcn.GetStringConstant (foundationHandle, "NSMetadataQueryUpdateAddedItemsKey");
			NSString NSMetadataQueryUpdateChangedItemsKey = Dlfcn.GetStringConstant (foundationHandle, "NSMetadataQueryUpdateChangedItemsKey");
			NSString NSMetadataQueryUpdateRemovedItemsKey = Dlfcn.GetStringConstant (foundationHandle, "NSMetadataQueryUpdateRemovedItemsKey");

			var insertedMetadataItemsOrNull = (NSArray)notification.UserInfo [NSMetadataQueryUpdateAddedItemsKey];
			if (insertedMetadataItemsOrNull != null) {
				var insertedMetadataItems = NSArray.FromArray<NSMetadataItem> (insertedMetadataItemsOrNull);
				insertedURLs = URLsByMappingMetadataItems (insertedMetadataItems);
			}

			var removedMetadataItemsOrNull = (NSArray)notification.UserInfo[NSMetadataQueryUpdateRemovedItemsKey];
			if (removedMetadataItemsOrNull) {
				var removedMetadataItems = NSArray.FromArray<NSMetadataItem> (removedMetadataItemsOrNull);
				removedURLs = URLsByMappingMetadataItems (removedMetadataItemsOrNull);
			}

			var updatedMetadataItemsOrNil = (NSArray)notification.UserInfo[NSMetadataQueryUpdateChangedItemsKey];
			if (updatedMetadataItemsOrNil != null) {
				var updatedMetadataItems = NSArray.FromArray<NSMetadataItem> (updatedMetadataItemsOrNil);
				var completelyDownloadedUpdatedMetadataItems = updatedMetadataItems.Where (item => {
					item.DownloadingStatus == NSItemDownloadingStatus.Current;
				}).ToArray ();
				updatedURLs = URLsByMappingMetadataItems (completelyDownloadedUpdatedMetadataItems);
			}

			Delegate.DidUpdateContentsWithInsertedURLs(insertedURLs, removedURLs, updatedURLs);

			metadataQuery.EnableUpdates();

			if (firstQueryUpdateHandler != null) {
				// Execute the `firstQueryUpdateHandler`, it will contain the closure from initialization on first update.
				firstQueryUpdateHandler();

				// Set `firstQueryUpdateHandler` to an empty closure so that the handler provided is only run on first update.
				firstQueryUpdateHandler = null;
			}
		}

		#endregion

		#region Convenience

		NSUrl DocumentURLForName(string name)
		{
			NSUrl documentURLWithoutExtension = documentsDirectory.Append (name, false);
			return documentURLWithoutExtension.AppendPathExtension (AppConfig.ListerFileExtension);
		}

		NSUrl[] URLsByMappingMetadataItems(NSMetadataItem[] metadataItems)
		{
			return metadataItems.Select (item => item.Url).ToArray ();
		}

		#endregion

	}
}