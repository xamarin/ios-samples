/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A set of protocols and a struct that represent an interface to a remote server.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Foundation;

namespace ColorFeed {
	public interface IServer {
		event EventHandler<ServerSuccessEventArgs> Success;
		event EventHandler Cancelled;
		// Fetch any entries on the server that are more recent than the start date.
		IDownloadTask GetFetchEntriesSinceTask (DateTime startDate, CancellationToken cancellationToken);
	}

	// A cancellable download task.
	public interface IDownloadTask {
		CancellationToken DownloadToken { get; }
		void Start ();
		void Cancel ();
	}

	public sealed class ServerSuccessEventArgs : EventArgs {
		public Post [] Posts { get; private set; }

		public ServerSuccessEventArgs (Post [] posts)
		{
			Posts = posts;
		}
	}

	public static class ServerUtils
	{
		static Random random = new Random (DateTime.Now.Millisecond);

		public static Color CreateRandomColor ()
		{
			var red = random.NextDouble ();
			var green = random.NextDouble ();
			var blue = random.NextDouble ();

			return new Color (red, green, blue);
		}

		public static Post CreateRandomPost (DateTime timestamp) => new Post (timestamp, CreateRandomColor (), CreateRandomColor (), random.Next (0, 360));

		public static Task<Post []> GenerateFakePosts (DateTime startDate, DateTime endDate, double interval = 60 * 10, int variation = 5 * 60)
		{
			return Task<Post []>.Factory.StartNew (() => {
				var entries = new List<Post> ();
				var unixStart = (startDate - new DateTime (1970, 1, 1)).TotalSeconds;
				var unixEnd = (endDate - new DateTime (1970, 1, 1)).TotalSeconds;

				for (double i = unixStart; i < unixEnd; i += interval) {
					var randomVariation = random.Next (-variation, variation);
					var fakeTime = Math.Max (unixStart, Math.Min (i + randomVariation, unixEnd));
					entries.Add (CreateRandomPost (new DateTime (1970, 1, 1).AddSeconds (fakeTime)));
				}

				return entries.ToArray ();
			});
		}
	}
}
