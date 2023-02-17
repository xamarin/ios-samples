
namespace SimpleWatchConnectivity {
	using Foundation;
	using System;
	using System.Collections.Generic;
	using WatchConnectivity;

	/// <summary>
	/// Manage the observation of file transfers.
	/// </summary>
	public class FileTransferObservers {
		// Hold the observations and file transfers.
		// KVO will be removed automatically after observations are released.
		private readonly List<WCSessionFileTransfer> fileTransfers = new List<WCSessionFileTransfer> ();
		private readonly List<IDisposable> observations = new List<IDisposable> ();

		~FileTransferObservers ()
		{
			// Dispose all the observations.
			foreach (var observation in this.observations) {
				observation.Dispose ();
			}

			this.observations.Clear ();
		}

		/// <summary>
		/// Observe a file transfer, hold the observation.
		/// </summary>
		public void Observe (WCSessionFileTransfer fileTransfer, Action<NSProgress> handler)
		{
			var observation = fileTransfer.Progress.AddObserver ("fractionCompleted", NSKeyValueObservingOptions.New, (_) => {
				handler (fileTransfer.Progress);
			});

			this.observations.Add (observation);
			this.fileTransfers.Add (fileTransfer);
		}

		/// <summary>
		/// Unobserve a file transfer, invalidate the observation.
		/// </summary>
		public void Unobserve (WCSessionFileTransfer fileTransfer)
		{
			var index = this.fileTransfers.IndexOf (fileTransfer);
			if (index != -1) {
				var observation = this.observations [index];
				observation.Dispose ();

				this.observations.RemoveAt (index);
				this.fileTransfers.RemoveAt (index);
			}
		}
	}
}
