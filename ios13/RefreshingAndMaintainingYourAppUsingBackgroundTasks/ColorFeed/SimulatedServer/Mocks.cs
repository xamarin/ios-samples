/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A set of extensions, a class, and a function that simulate data coming from a server.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Foundation;

namespace ColorFeed {
	// Simulates a remote server by generating randomized ServerEntry results
	public class MockServer : IServer {
		public event EventHandler<ServerSuccessEventArgs> Success;
		public event EventHandler Cancelled;

		private class MockDownloadTask : IDownloadTask {
			TimeSpan delay;
			DateTime startDate;
			Action<Post []> success;
			Action cancelled;

			public CancellationToken DownloadToken { get; private set; }

			public MockDownloadTask (TimeSpan delay, DateTime startDate, Action<Post []> success, Action cancelled, CancellationToken cancellationToken)
			{
				this.delay = delay;
				this.startDate = startDate;
				this.success = success;
				this.cancelled = cancelled;
				DownloadToken = cancellationToken;
			}

			public void Start ()
			{	
				Task.Factory.StartNew (async () => {
					await Task.Delay (delay);
					var posts = await ServerUtils.GenerateFakePosts (startDate, DateTime.Now);
					if (!DownloadToken.IsCancellationRequested)
						success?.Invoke (posts);
				}, DownloadToken);
			}

			public void Cancel ()
			{
				cancelled?.Invoke ();
			}
		}

		public IDownloadTask GetFetchEntriesSinceTask (DateTime startDate, CancellationToken cancellationToken)
		{
			var random = new Random (DateTime.Now.Millisecond);
			var delay = random.Next (0, 26) / 10d;
			var downloadTask = new MockDownloadTask (TimeSpan.FromSeconds (delay), startDate, posts => {
				Success?.Invoke (this, new ServerSuccessEventArgs (posts));
			}, () => {
				Cancelled?.Invoke (this, new EventArgs ());
			}, cancellationToken);

			return downloadTask;
		}
	}

	public class DownloadCancelledException : Exception { }
}
