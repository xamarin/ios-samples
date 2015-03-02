using System;
using Foundation;
using CoreFoundation;

namespace ListerKit
{
	public class CloudListCoordinator : IListCoordinator
	{
		NSMetadataQuery metadataQuery;
		DispatchQueue documentsDirectoryQueue;
		NSUrl documentsDirectory;

		// Closure executed after the first update provided by the coordinator regarding tracked Urls.
		Action firstQueryUpdateHandler;

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

			documentsDirectoryQueue.DispatchAsync ();
//			dispatch_barrier_async(_documentsDirectoryQueue, ^{
//				NSURL *cloudContainerURL = [[NSFileManager defaultManager] URLForUbiquityContainerIdentifier:nil];
//
//				_documentsDirectory = [cloudContainerURL URLByAppendingPathComponent:@"Documents"];
//			});
//
//			// Observe the query.
//			NSNotificationCenter *notificationCenter = [NSNotificationCenter defaultCenter];
//
//			[notificationCenter addObserver:self selector:@selector(metadataQueryDidFinishGathering:) name:NSMetadataQueryDidFinishGatheringNotification object:_metadataQuery];
//
//			[notificationCenter addObserver:self selector:@selector(metadataQueryDidUpdate:) name:NSMetadataQueryDidUpdateNotification object:_metadataQuery];

		}

		#region IListCoordinator implementation

		public void StartQuery ()
		{
			throw new NotImplementedException ();
		}

		public void StopQuery ()
		{
			throw new NotImplementedException ();
		}

		public void RemoveListAtUrl (Foundation.NSUrl url)
		{
			throw new NotImplementedException ();
		}

		public void CreateUrlForList (List list, string name)
		{
			throw new NotImplementedException ();
		}

		public bool CanCreateListWithName (string name)
		{
			throw new NotImplementedException ();
		}

		public IListCoordinatorDelegate Delegate {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		#endregion
	}
}

