using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using CloudKit;
using Foundation;
using CoreFoundation;

namespace CloudCaptions
{
	public class PostManager
	{
		readonly nuint updateBy = 10;

		Action reloadHandler;
		Post lastPostSeenOnServer;
		string[] tagArray;
		CKQueryCursor postCursor;
		readonly NSOperationQueue fetchRecordQueue;		// Allows for us to cancel loadBatch operation when the tag string has changed
		readonly DispatchQueue updateCellArrayQueue;	// Synchronous dispatch queue to synchronously add objects to postCells collection
		bool isLoadingBatch;	// Flags when we're loading a batch so we don't try loading a second batch while this one is running
		bool haveOldestPost;	// Flags when we've loaded the earliest post
		readonly object locker = new object ();
		readonly string[] desiredKeys;
		readonly List<Post> downloadingBatchStorage; // we use this storage to hold post during downloading from server

		public UIRefreshControl RefreshControl { get; set; }
		public List<Post> PostCells { get; private set; }

		CKDatabase PublicDB {
			get {
				return CKContainer.DefaultContainer.PublicCloudDatabase;
			}
		}

		public PostManager (Action reloadHandler)
		{
			this.reloadHandler = reloadHandler;
			PostCells = new List<Post> ();
			downloadingBatchStorage = new List<Post> ();
			updateCellArrayQueue = new DispatchQueue ("UpdateCellQueue");
			fetchRecordQueue = new NSOperationQueue ();
			tagArray = new string[0];

			desiredKeys = new string[] {
				Post.ImageRefKey,
				Post.FontKey,
				Post.TextKey
			};
		}

		#region LoadBatch

		public void LoadBatch()
		{
			lock (locker) {
				// Quickly returns if another loadNextBatch is running or we have the oldest post
				if (isLoadingBatch || haveOldestPost)
					return;
				else
					isLoadingBatch = true;
			}

			CKQueryOperation queryOp = CreateLoadBatchOperation ();
			fetchRecordQueue.AddOperation (queryOp);
		}

		CKQueryOperation CreateLoadBatchOperation()
		{
			CKQueryOperation queryOp = null;
			if (postCursor != null) {
				// If we have a cursor, go ahead and just continue from where we left off
				queryOp = new CKQueryOperation (postCursor);
			} else {
				CKQuery postQuery = CreatePostQuery ();
				queryOp = new CKQueryOperation (postQuery);
			}

			downloadingBatchStorage.Clear ();

			// This query should only fetch so many records and only retrieve the information we need
			queryOp.ResultsLimit = updateBy;
			queryOp.DesiredKeys = desiredKeys;
			queryOp.RecordFetched = OnPostFetched;
			queryOp.Completed = OnPostLoadingCompleted;
			queryOp.Database = PublicDB;

			return queryOp;
		}

		CKQuery CreatePostQuery()
		{
			// Create predicate out of tags. If self.tagArray is empty we should get every post
			NSPredicate[] subPredicates = new NSPredicate[tagArray.Length];
			for (int i = 0; i < tagArray.Length; i++)
				subPredicates [i] = Utils.CreateTagPredicate(tagArray [i]);

			// If our tagArray is empty, create a true predicate (as opposed to a predicate containing "Tags CONTAINS ''"
			NSPredicate finalPredicate = tagArray.Length == 0
				? NSPredicate.FromValue (true)
				: NSCompoundPredicate.CreateAndPredicate (subPredicates);

			CKQuery postQuery = new CKQuery (Post.RecordType, finalPredicate);
			// lastest post on the top
			postQuery.SortDescriptors = Utils.CreateCreationDateDescriptor (ascending: false);

			return postQuery;
		}

		void OnPostFetched(CKRecord record)
		{
			// When we get a record, use it to create an Post
			Post fetchedPost = new Post (record);
			downloadingBatchStorage.Add (fetchedPost);

			fetchedPost.LoadImage (new string[]{ Image.FullsizeKey }, () => {
				// Once image is loaded, tell the tableview to reload
				DispatchQueue.MainQueue.DispatchAsync (reloadHandler);
			});
		}

		void OnPostLoadingCompleted(CKQueryCursor cursor, NSError operationError)
		{
			Error error = HandleError (operationError);

			switch (error) {
				case Error.Success:
					postCursor = cursor;
					haveOldestPost = cursor == null;
					isLoadingBatch = false;
					PostCells.AddRange (downloadingBatchStorage);

					if (lastPostSeenOnServer == null && PostCells.Count > 0) {
						lastPostSeenOnServer = PostCells [0];
						DispatchQueue.MainQueue.DispatchAsync(RefreshControl.EndRefreshing);
					}

					DispatchQueue.MainQueue.DispatchAsync (reloadHandler);
					break;

				case Error.Retry:
					Utils.Retry (() => {
						isLoadingBatch = false;
						LoadBatch ();
					}, operationError);
					break;

				case Error.Ignore:
					isLoadingBatch = false;
					DispatchQueue.MainQueue.DispatchAsync (RefreshControl.EndRefreshing);
					Console.WriteLine ("Error: {0}", operationError.Description);
					break;

				default:
					break;
			}
		}

		#endregion

		#region LoadNewPosts

		// Called when users pulls to refresh
		// This adds new items to the beginning of the table
		public void LoadNewPosts(Post post = null)
		{
			// If we don't have any posts on our table yet,
			// fetch the first batch instead (we make assumptions in this method that we have other posts to compare to)
			if (TryLoadFirstBatch ())
				return;

			// We want to strip all posts we have that haven't been seen on the server yet from tableview (order isn't guaranteed)
			var loc = PostCells.IndexOf (lastPostSeenOnServer);
			List<Post> newPosts = PostCells.Take (loc).ToList ();
			PostCells.RemoveRange (0, loc);

			// If we had a post passed in and it matches our tags, we should put that in the array too
			if (MatchesTags (post))
				newPosts.Add (post);
			else
				post = null;

			// The last record we see will be the most recent we see on the server, we'll set the property to this in the completion handler
			Post lastRecordInOperation = null;

			var postQuery = CreateLoadNewPostQuery ();
			var queryOp = new CKQueryOperation (postQuery);
			queryOp.DesiredKeys = desiredKeys;
			queryOp.RecordFetched = (CKRecord record) => OnLoadNewPostFetchRecord (record, newPosts, ref lastRecordInOperation);
			queryOp.Completed = (CKQueryCursor cursor, NSError operationError) => OnLoadNewPostComplted (cursor, operationError, newPosts, lastPostSeenOnServer, post);
			queryOp.Database = PublicDB;

			fetchRecordQueue.AddOperation (queryOp);
		}

		bool TryLoadFirstBatch()
		{
			bool haveNoPost = PostCells.Count == 0 || lastPostSeenOnServer == null;

			if(haveNoPost) {
				// We dispatch it after two seconds to give the server time to index the new post
				// TODO: https://trello.com/c/cWWQXKYY
				var dispatchTime = new DispatchTime (DispatchTime.Now, 2 * 1000000000);
				DispatchQueue.MainQueue.DispatchAfter (dispatchTime, () => {
					// If we get here, we must have no posts. That must mean that last time we tried loading a batch nothing came through so we locked the method. Let's unlock it
					haveOldestPost = false;
					LoadBatch ();
				});
			}

			return haveNoPost;
		}

		bool MatchesTags(Post post)
		{
			if (post == null)
				return false;

			var weakTags = post.PostRecord [Post.TagsKey];
			string[] postTags = NSArray.StringArrayFromHandle (weakTags.Handle);

			// All tags from seach field (tagArray) must be presented inside post
			foreach (string tag in tagArray) {
				if (!postTags.Contains (tag))
					return false;
			}

			// if we are here => post contains all tags from search field
			return true;
		}

		CKQuery CreateLoadNewPostQuery()
		{
			// Creates predicate based on tag string and most recent post from server
			NSPredicate predicate = CreateLoadNewPostPredicate ();

			var postQuery = new CKQuery (Post.RecordType, predicate);
			postQuery.SortDescriptors = Utils.CreateCreationDateDescriptor (ascending: true);

			return postQuery;
		}

		NSPredicate CreateLoadNewPostPredicate()
		{
			var len = tagArray.Length + 1;
			var subPredicates = new NSPredicate[len];
			subPredicates [0] = Utils.CreateAfterPredicate(lastPostSeenOnServer.PostRecord.CreationDate);

			for (int i = 0; i < tagArray.Length; i++)
				subPredicates [i + 1] = Utils.CreateTagPredicate(tagArray [i]);

			// ANDs all subpredicates to make a final predicate
			NSPredicate finalPredicate = NSCompoundPredicate.CreateAndPredicate (subPredicates);
			return finalPredicate;
		}

		void OnLoadNewPostFetchRecord(CKRecord record, List<Post> newPosts, ref Post lastRecordInOperation)
		{
			// If the record we just fetched doesn't match recordIDs to any item in our newPosts array, let's make an Post and add it
			var matchingRecord = newPosts.FindIndex (p => p.PostRecord.Id.Equals(record.Id));
			if (matchingRecord == -1) {
				Post fetchedPost = new Post (record);
				newPosts.Add (fetchedPost);
				fetchedPost.LoadImage(new string[] { Image.FullsizeKey }, reloadHandler);
				lastRecordInOperation = fetchedPost;
			} else {
				// If we already have this record we don't have to fetch. We'll still update lastRecordInOperation because we did see it on the server
				lastRecordInOperation = newPosts[matchingRecord];
			}
		}

		void OnLoadNewPostComplted(CKQueryCursor cursor, NSError operationError, List<Post> newPosts, Post lastRecordInOperation, Post retryPost)
		{
			Error error = HandleError (operationError);
			switch (error) {
				case Error.Success:
					// lastRecordCreationDate is the most recent record we've seen on server, let's set our property to that for next time we get a push
					lastPostSeenOnServer = lastRecordInOperation ?? lastPostSeenOnServer;
					// This sorts the newPosts array in ascending order
					newPosts.Sort (PostComparison);
					// Takes our newPosts array and inserts the items into the table array one at a time
					foreach (Post p in newPosts) {
						updateCellArrayQueue.DispatchAsync (() => {
							PostCells.Insert (0, p);
							DispatchQueue.MainQueue.DispatchAsync (reloadHandler);
						});
					}
					DispatchQueue.MainQueue.DispatchAsync (RefreshControl.EndRefreshing);
					break;

				case Error.Retry:
					Utils.Retry(()=> LoadNewPosts(retryPost), operationError);
					break;

				case Error.Ignore:
					Console.WriteLine ("Error: {0}", operationError.Description);
					DispatchQueue.MainQueue.DispatchAsync(RefreshControl.EndRefreshing);
					break;

				default:
					throw new NotImplementedException ();
			}
		}

		#endregion

		public void ResetWithTagString(string tags)
		{
			// Reloads table with new tag settings
			// First, anything the table is updating with now is potentially invalid, cancel any current updates
			fetchRecordQueue.CancelAllOperations ();
			// This should only be filled with array add operations, best to just wait for it to finish
			updateCellArrayQueue.DispatchSync (() => {
			});

			// Resets the table to be empty
			PostCells.Clear ();
			lastPostSeenOnServer = null;
			reloadHandler ();

			// Sets tag array and prepares table for initial update
			tagArray = string.IsNullOrWhiteSpace (tags)
				? new string[0]
				: tags.ToLower ().Split (new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);

			postCursor = null;
			isLoadingBatch = false;
			haveOldestPost = false;
			LoadBatch ();
		}

		int PostComparison(Post first, Post second)
		{
			var firstDate = (DateTime)first.PostRecord.CreationDate;
			var secondDate = (DateTime)second.PostRecord.CreationDate;
			return firstDate.CompareTo (secondDate);
		}

		Error HandleError (NSError error)
		{
			if (error == null)
				return Error.Success;

			switch ((CKErrorCode)(long)error.Code) {
				case CKErrorCode.NetworkUnavailable:
				case CKErrorCode.NetworkFailure:
					// A reachability check might be appropriate here so we don't just keep retrying if the user has no service
				case CKErrorCode.ServiceUnavailable:
				case CKErrorCode.RequestRateLimited:
					return Error.Retry;

				case CKErrorCode.UnknownItem:
					Console.WriteLine ("If a post has never been made, CKErrorCode.UnknownItem will be returned in PostManager because it has never seen the Post record type");
					return Error.Ignore;

				case CKErrorCode.InvalidArguments:
					Console.WriteLine ("If invalid arguments is returned in PostManager with a message about not being marked indexable or sortable, go into CloudKit dashboard and set the Post record type as sortable on date created (under metadata index)");
					return Error.Ignore;

				case CKErrorCode.IncompatibleVersion:
				case CKErrorCode.BadContainer:
				case CKErrorCode.MissingEntitlement:
				case CKErrorCode.PermissionFailure:
				case CKErrorCode.BadDatabase:
					// This app uses the publicDB with default world readable permissions
				case CKErrorCode.AssetFileNotFound:
				case CKErrorCode.PartialFailure:
					// These shouldn't occur during a query operation
				case CKErrorCode.QuotaExceeded:
					// We should not retry if it'll exceed our quota
				case CKErrorCode.OperationCancelled:
					// Nothing to do here, we intentionally cancelled
				case CKErrorCode.NotAuthenticated:
				case CKErrorCode.ResultsTruncated:
				case CKErrorCode.ServerRecordChanged:
				case CKErrorCode.AssetFileModified:
				case CKErrorCode.ChangeTokenExpired:
				case CKErrorCode.BatchRequestFailed:
				case CKErrorCode.ZoneBusy:
				case CKErrorCode.ZoneNotFound:
				case CKErrorCode.LimitExceeded:
				case CKErrorCode.UserDeletedZone:
					// All of these errors are irrelevant for this query operation
				case CKErrorCode.InternalError:
				case CKErrorCode.ServerRejectedRequest:
				case CKErrorCode.ConstraintViolation:
					//Non-recoverable, should not retry
				default:
					return Error.Ignore;
			}
		}
	}
}
