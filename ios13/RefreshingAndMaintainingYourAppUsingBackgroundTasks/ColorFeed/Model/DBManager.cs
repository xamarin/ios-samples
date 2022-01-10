using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Foundation;

using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace ColorFeed {
	public class DBManager {
		#region Shared Instance

		static Lazy<DBManager> lazy = new Lazy<DBManager> (() => new DBManager ());
		public static DBManager SharedInstance { get => lazy.Value; }

		#endregion

		#region Class Variables

		object locked = new object ();
		static readonly string lastCleanedKey = $"{nameof (ColorFeed)}_{nameof (LastCleaned)}";

		#endregion

		#region Properties

		public SQLiteAsyncConnection Connection { get; private set; }
		public bool IsReady { get; set; }
		public DateTime? LastCleaned {
			get {
				if (!(NSUserDefaults.StandardUserDefaults.StringForKey (lastCleanedKey) is string stringDate))
					return null;

				return DateTime.Parse (stringDate);
			}
			set {
				if (value != null) NSUserDefaults.StandardUserDefaults.SetString (value.ToString (), lastCleanedKey);
				else NSUserDefaults.StandardUserDefaults.RemoveObject (lastCleanedKey);
			}
		}

		#endregion

		DBManager ()
		{
			InitializeDatabase ();
		}

		#region Public Functionality

		// Fills the database with initial fake data
		// If onlyIfNeeded is true, only does so if the store is empty
		public Task LoadInitialData (bool onlyIfNeeded = true)
		{
			lock (locked) {
				IsReady = false;
			}

			return Task.Factory.StartNew (async () => await LoadInitialDataAsync ());

			async Task LoadInitialDataAsync ()
			{
				var oldPosts = Connection.GetAllWithChildrenAsync<Post> ().Result;

				if (!onlyIfNeeded) {
					await Connection.DropTableAsync<Post> ();
					await Connection.DropTableAsync<Color> ();

					await Connection.CreateTableAsync<Color> ();
					await Connection.CreateTableAsync<Post> ();
				}

				if (!onlyIfNeeded || oldPosts.Count == 0) {
					var now = DateTime.Now;
					var start = now.AddSeconds (-(7 * 24 * 60 * 60));
					var end = now.AddSeconds (-(60 * 60));

					var newPosts = await ServerUtils.GenerateFakePosts (start, end);
					foreach (var post in newPosts)
						await Connection.InsertWithChildrenAsync (post);

					lock (this) {
						LastCleaned = null;
					}
				}

				lock (locked) {
					IsReady = true;
				}
			}
		}

		public async Task<List<Post>> GetPosts ()
		{
			var posts = await Connection.GetAllWithChildrenAsync<Post> ();
			return posts.OrderByDescending (p => p.Timestamp).ToList ();
		}

		public async Task SavePosts (List<Post> posts)
		{
			foreach (var post in posts) {
				await Connection.InsertAllAsync (new object [] { post.FirstColor, post.SecondColor, post });
				await Connection.UpdateWithChildrenAsync (post);
			}
		}

		public async Task DeletePost (Post post)
		{
			await Connection.DeleteAsync (post, true);
		}

		public async Task DeletePosts (List<Post> posts)
		{
			await Connection.DeleteAllAsync (posts, true);
		}

		#endregion

		#region Internal Functionality

		void InitializeDatabase ()
		{
			try {
				var dbFile = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "ColorFeed");
				Connection = new SQLiteAsyncConnection (dbFile);
				_ = Connection.CreateTableAsync<Color> ().Result;
				_ = Connection.CreateTableAsync<Post> ().Result;
			} catch (Exception ex) {
				Debug.WriteLine (ex);
			}
		}

		#endregion
	}
}
