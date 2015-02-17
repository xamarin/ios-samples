using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;
using CoreFoundation;

namespace ListerKit
{
	// TODO: add comments
	public class ListsController: IListCoordinatorDelegate
	{
		IListCoordinator listCoordinator;
		readonly NSOperationQueue delegateQueue;
		readonly IComparer<ListInfo> comparer;
		readonly DispatchQueue listInfoQueue;
		readonly List<ListInfo> listInfos;

		public int Count {
			get {
				return listInfos.Count;
			}
		}

		public IListCoordinator ListCoordinator {
			get {
				return listCoordinator;
			}
			set {
				if (listCoordinator == value)
					return;

				IListCoordinator oldListCoordinator = listCoordinator;
				listCoordinator = value;

				oldListCoordinator.StopQuery();

				// Map the listInfo objects protected by listInfoQueue.
				IEnumerable<NSUrl> allUrls = null;
				listInfoQueue.DispatchSync(()=> {
					allUrls = listInfos.Select(item => item.Url);
				});
				ProcessContentChangesWithInsertedURLs (new NSUrl[0], allUrls, new NSUrl[0]);

				listCoordinator.Delegate = this;
				oldListCoordinator.Delegate = null;

				listCoordinator.StartQuery ();
			}
		}

		public IListsControllerDelegate Delegate { get; set; }

		public ListsController (IListCoordinator listCoordinator, NSOperationQueue delegateQueue, IComparer<ListInfo> comparer)
		{
			this.listCoordinator = listCoordinator;
			this.delegateQueue = delegateQueue ?? NSOperationQueue.MainQueue;
			this.comparer = comparer;
			listInfoQueue = new DispatchQueue ("list info queue");
			listInfos = new List<ListInfo> ();
			listCoordinator.Delegate = this;
		}

		public void StartSearching ()
		{
			listCoordinator.StartQuery ();
		}

		public void StopSearching ()
		{
			listCoordinator.StopQuery ();
		}

		public ListInfo ObjectAtIndexedSubscript(int index)
		{
			// Fetch the appropriate list info protected by listInfoQueue.
			ListInfo listInfo = null;
			listInfoQueue.DispatchSync (() => {
				listInfo = listInfos[index];
			});

			return listInfo;
		}

		#region Inserting / Removing / Managing / Updating AAPLListInfo Objects

		public void RemoveListInfo(ListInfo listInfo)
		{
			listCoordinator.RemoveListAtUrl (listInfo.Url);
		}

		public void CreateListInfoForList(List list, string name)
		{
			listCoordinator.CreateUrlForList (list, name);
		}

		public bool CanCreateListInfoWithName(string name)
		{
			return listCoordinator.CanCreateListWithName (name);
		}

		public void SetListInfoHasNewContents(ListInfo listInfo)
		{
			listInfoQueue.DispatchAsync (() => {
				// Remove the old list info and replace it with the new one.
				int indexOfListInfo = listInfos.IndexOf (listInfo);
				listInfos [indexOfListInfo] = listInfo;

				delegateQueue.AddOperation (() => {
					Delegate.WillChangeContent (this);
					Delegate.DidUpdateListInfo (this, listInfo, indexOfListInfo);
					Delegate.DidChangeContent (this);
				});
			});
		}

		public void DidUpdateContentsWithInsertedURLs (IEnumerable<NSUrl> insertedURLs, IEnumerable<NSUrl> removedURLs, IEnumerable<NSUrl> updatedURLs)
		{
			ProcessContentChangesWithInsertedURLs (insertedURLs, removedURLs, updatedURLs);
		}

		public void DidFailRemovingListAtURL (NSUrl url, NSError error)
		{
			ListInfo listInfo = new ListInfo (url);
			delegateQueue.AddOperation (() => {
				Delegate.DidFailCreatingListInfo(this, listInfo, error);
			});
		}

		public void DidFailCreatingListAtURL (NSUrl url, NSError error)
		{
			ListInfo listInfo = new ListInfo (url);
			delegateQueue.AddOperation (() => {
				Delegate.DidFailRemovingListInfo (this, listInfo, error);
			});
		}

		#endregion

		void ProcessContentChangesWithInsertedURLs(IEnumerable<NSUrl> insertedURLs, IEnumerable<NSUrl> removedURLs, IEnumerable<NSUrl> updatedURLs)
		{
			var insertedListInfos = ListInfosByMappingURLs (insertedURLs);
			var removedListInfos = ListInfosByMappingURLs (removedURLs);
			var updatedListInfos = ListInfosByMappingURLs (updatedURLs);

			delegateQueue.AddOperation (() => {
				// Filter out all lists that are already included in the tracked lists.
				IEnumerable<ListInfo> trackedRemovedListInfos = null;
				IEnumerable<ListInfo> untrackedInsertedListInfos = null;

				listInfoQueue.DispatchSync(()=> {
					var hs = new HashSet<ListInfo>(listInfos);
					trackedRemovedListInfos = removedListInfos.Where(item => hs.Contains(item));
					untrackedInsertedListInfos = insertedListInfos.Where(item => !hs.Contains(item));
				});

				if (!untrackedInsertedListInfos.Any()  && !trackedRemovedListInfos.Any() && !updatedListInfos.Any())
					return;

				Delegate.WillChangeContent(this);

				// Remove
				foreach (var trackedRemovedListInfo in trackedRemovedListInfos) {
					int trackedRemovedListInfoIndex = -1;
					listInfoQueue.DispatchSync(()=> {
						trackedRemovedListInfoIndex = listInfos.IndexOf(trackedRemovedListInfo);
						listInfos.RemoveAt(trackedRemovedListInfoIndex);
					});

					Delegate.DidRemoveListInfo(this, trackedRemovedListInfo, trackedRemovedListInfoIndex);
				}

				// Sort the untracked inserted list infos
				untrackedInsertedListInfos = untrackedInsertedListInfos.OrderBy(item => item, comparer);

				// Insert
				foreach (var untrackedInsertedListInfo in untrackedInsertedListInfos) {
					int untrackedInsertedListInfoIndex = -1;

					listInfoQueue.DispatchSync(()=> {
						listInfos.Add(untrackedInsertedListInfo);
						listInfos.Sort(comparer);
						untrackedInsertedListInfoIndex = listInfos.IndexOf(untrackedInsertedListInfo);
					});

					Delegate.DidInsertListInfo(this, untrackedInsertedListInfo, untrackedInsertedListInfoIndex);
				}

				// Update
				foreach (var updatedListInfo in updatedListInfos) {
					int updatedListInfoIndex = -1;

					listInfoQueue.DispatchSync(()=> {
						updatedListInfoIndex = listInfos.IndexOf(updatedListInfo);

						// Track the new list info instead of the old one.
						if (updatedListInfoIndex != -1)
							listInfos[updatedListInfoIndex] = updatedListInfo;
					});

					if (updatedListInfoIndex != -1 )
						Delegate.DidUpdateListInfo(this, updatedListInfo, updatedListInfoIndex);
				}

				Delegate.DidChangeContent(this);
			});
		}

		#region Convinience

		IEnumerable<ListInfo> ListInfosByMappingURLs(IEnumerable<NSUrl> urls)
		{
			List<ListInfo> result = urls.Select (url => new ListInfo (url)).ToList ();
			return result;
		}

		#endregion
	}
}

