using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BackgroundTasks;
using Foundation;

namespace ColorFeed {
	public class Operations {
		public event EventHandler<PostsFetchedEventArgs> PostsFetched;
		public event EventHandler<PostsDeletedEventArgs> PostsDeleted;

		CancellationTokenSource cancellationTokenSource;
		IDownloadTask downloadTask;
		BGAppRefreshTask refreshTask;
		BGProcessingTask processingTask;

		IServer server;
		public IServer Server {
			get => server;
			set {
				if (server == value)
					return;

				if (server != null) {
					server.Success -= FetchPostsSucceeded;
					server.Cancelled -= FetchPostCancelled;
				}

				server = value;

				if (server != null) {
					server.Success += FetchPostsSucceeded;
					server.Cancelled += FetchPostCancelled;
				}
			}
		}

		public Operations () : this (null) { }

		public Operations (IServer server)
		{
			Server = server;

			cancellationTokenSource = new CancellationTokenSource ();
		}

		~Operations ()
		{
			if (Server != null) {
				Server.Success -= FetchPostsSucceeded;
				Server.Cancelled -= FetchPostCancelled;
			}
		}

		// Returns an array of operations for fetching the latest entries and then adding them to the Database
		public void FetchLatestPosts (BGAppRefreshTask task = null)
		{
			if (Server == null)
				throw new ArgumentNullException (nameof (Server));

			refreshTask = task;

			Task.Factory.StartNew (async () => {
				try {
					var post = await FetchMostRecentPost ();
					DownloadPostsFromServer (post.Timestamp, cancellationTokenSource.Token);
				} catch (OperationCanceledException) {
					Debug.WriteLine ("Fetch latest entries was cancelled.");
				} catch (Exception) {
					Debug.WriteLine ("Something went wrong trying to fech latest posts.");
				}
			});
		}

		public void CancelOperations ()
		{
			if (cancellationTokenSource.IsCancellationRequested)
				return;

			cancellationTokenSource.Cancel ();
			cancellationTokenSource = new CancellationTokenSource ();
			downloadTask?.Cancel ();

			NSNotificationCenter.DefaultCenter.RemoveObserver (AppDelegate.RefreshSuccessNotificationName);
			refreshTask?.SetTaskCompleted (false);
			processingTask?.SetTaskCompleted (false);
		}

		async Task<Post> FetchMostRecentPost ()
		{
			return (await DBManager.SharedInstance.GetPosts ()).FirstOrDefault ();
		}

		void DownloadPostsFromServer (DateTime sinceDate, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested ();
			downloadTask = Server.GetFetchEntriesSinceTask (sinceDate, cancellationToken);
			cancellationToken.ThrowIfCancellationRequested ();

			downloadTask.Start ();
		}

		public void DeletePosts (DateTime beforeDate, BGProcessingTask task = null)
		{
			processingTask = task;
			var cancellationToken = cancellationTokenSource.Token;

			Task.Factory.StartNew (async () => {
				var posts = (await DBManager.SharedInstance.GetPosts ()).Where (p => p.Timestamp < beforeDate).ToList ();

				foreach (var post in posts) {
					if (cancellationToken.IsCancellationRequested)
						return;

					Debug.WriteLine ($"Deleting post with timestamp: {post.Timestamp.ToString ("M/dd h:mm tt")}");
					await DBManager.SharedInstance.DeletePost (post);
					await Task.Delay (5);
				}

				PostsDeleted?.Invoke (this, new PostsDeletedEventArgs (task));
			}, cancellationToken);
		}

		void FetchPostsSucceeded (object sender, ServerSuccessEventArgs e)
		{
			DBManager.SharedInstance.SavePosts (e.Posts.ToList ()).Wait ();
			PostsFetched?.Invoke (this, new PostsFetchedEventArgs (e.Posts, cancellationTokenSource.Token, refreshTask));
		}

		void FetchPostCancelled (object sender, EventArgs e)
		{
			Debug.WriteLine ("The fetch was cancelled");
			refreshTask?.SetTaskCompleted (false);
		}
	}

	public sealed class PostsFetchedEventArgs : EventArgs {
		public Post [] Posts { get; private set; }
		public CancellationToken CancellationToken { get; set; }
		public BGAppRefreshTask Task { get; private set; }

		public PostsFetchedEventArgs (Post [] posts, CancellationToken cancellationToken, BGAppRefreshTask task)
		{
			Posts = posts;
			CancellationToken = cancellationToken;
			Task = task;
		}
	}

	public sealed class PostsDeletedEventArgs : EventArgs {
		public BGProcessingTask Task { get; private set; }

		public PostsDeletedEventArgs (BGProcessingTask task) => Task = task;
	}
}
