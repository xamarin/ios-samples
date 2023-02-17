using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BackgroundTasks;
using Foundation;
using UIKit;

namespace ColorFeed {
	public partial class FeedTableViewController : UITableViewController {
		List<Post> posts;

		Operations operations;
		public Operations Operations {
			get => operations;
			set {
				if (operations == value)
					return;

				if (operations != null)
					operations.PostsFetched -= Operations_PostsFetched;

				operations = value;

				if (operations != null)
					operations.PostsFetched += Operations_PostsFetched;
			}
		}

		public FeedTableViewController (IntPtr handle) : base (handle)
		{
		}

		private void Operations_PostsFetched (object sender, PostsFetchedEventArgs e)
		{
			InsertPostsIntoTable (e.Posts, 1, e.CancellationToken, e.Task);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			posts = new List<Post> ();

			var refreshControl = new UIRefreshControl ();
			refreshControl.AttributedTitle = new NSAttributedString ("Initializing Database");
			refreshControl.ValueChanged += FetchLatestEntries;

			RefreshControl = refreshControl;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
		}

		public override void ViewDidAppear (bool animated)
		{
			LoadPosts ();
		}

		#region TableView Data Source

		public override nint NumberOfSections (UITableView tableView) => 1;
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return posts.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell ("postCell", indexPath) as FeedEntryTableViewCell;
			cell.Post = posts [indexPath.Row];

			return cell;
		}

		#endregion

		private void FetchLatestEntries (object sender, EventArgs e)
		{
			var refreshControl = sender as UIRefreshControl;
			refreshControl.BeginRefreshing ();

			Operations.FetchLatestPosts ();
		}

		partial void ShowActions (UIBarButtonItem sender)
		{
			var alertController = UIAlertController.Create (null, null, UIAlertControllerStyle.ActionSheet);

			if (alertController.PopoverPresentationController != null)
				alertController.PopoverPresentationController.BarButtonItem = sender;

			alertController.AddAction (UIAlertAction.Create ("Reset Feed Data", UIAlertActionStyle.Destructive, (action) => {
				DeletePostsFromTable ();
				RefreshControl.AttributedTitle = new NSAttributedString ("Initializing Database");
				DBManager.SharedInstance.LoadInitialData (false);
				LoadPosts ();
			}));
			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));

			PresentViewController (alertController, true, null);
		}

		Task LoadPosts ()
		{
			RefreshControl.BeginRefreshing ();

			return Task.Factory.StartNew (async () => {
				var insertedPostCount = 0;
				// Let's load posts while the database is being generated
				while (true) {
					await Task.Delay (1500);
					posts = await DBManager.SharedInstance.GetPosts ();

					if (DBManager.SharedInstance.IsReady)
						break;

					InvokeOnMainThread (() => {
						InsertPostsIntoTable (insertedPostCount);
						insertedPostCount = posts.Count;
					});
				}

				posts = await DBManager.SharedInstance.GetPosts ();
				InvokeOnMainThread (() => {
					InsertPostsIntoTable (insertedPostCount);
					RefreshControl.EndRefreshing ();
					RefreshControl.AttributedTitle = new NSAttributedString ("Pull to fetch new Posts");
				});
			});
		}

		void InsertPostsIntoTable (int insertedPosts)
		{
			TableView.BeginUpdates ();

			var total = posts.Count - insertedPosts;
			var indexPaths = new NSIndexPath [total];
			for (int i = total - 1; i >= 0; i--)
				indexPaths [i] = NSIndexPath.FromRowSection (i, 0);

			TableView.InsertRows (indexPaths, UITableViewRowAnimation.Right);
			TableView.EndUpdates ();
		}

		void InsertPostsIntoTable (Post [] posts, int delayInSeconds, CancellationToken cancellationToken, BGAppRefreshTask task = null)
		{
			Task.Factory.StartNew (() => {
				var totalPosts = posts.Length;
				for (int i = 0; i < totalPosts; i++) {
					if (cancellationToken.IsCancellationRequested)
						return;

					Task.Delay (delayInSeconds * 1000).Wait ();

					this.posts.Insert (0, posts [i]);

					InvokeOnMainThread (() => {
						TableView.BeginUpdates ();
						TableView.InsertRows (new [] { NSIndexPath.FromRowSection (0, 0) }, UITableViewRowAnimation.Right);
						TableView.EndUpdates ();
					});
				}

				InvokeOnMainThread (() => RefreshControl.EndRefreshing ());

				if (task != null)
					NSNotificationCenter.DefaultCenter.PostNotificationName (AppDelegate.RefreshSuccessNotificationName, task);
			}, cancellationToken);
		}

		void DeletePostsFromTable ()
		{
			TableView.BeginUpdates ();

			posts = new List<Post> ();

			var indexSet = NSIndexSet.FromIndex (0);
			TableView.ReloadSections (indexSet, UITableViewRowAnimation.Right);

			TableView.EndUpdates ();
		}
	}
}

