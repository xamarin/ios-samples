using System;

using Foundation;
using CoreFoundation;

namespace PhotoProgress {
	class DownloadState {
		
		public DispatchQueue Queue { get; set; }

		public DispatchSource.Timer DownloadTimer { get; set; }

		public NSError DownloadError { get; set; }

		public bool IsPaused { get; set; }
	}

	public class PhotoDownload : NSObject, INSProgressReporting {

		const long NanosecondsPerSecond = 1000000000;
		const long NanosecondsPerMillisecond = 1000000;
		const double TimerInterval = 1.0;
		const double BatchSize = 5000.0;

		NSUrl downloadURL;
		DownloadState downloadState;
		Action<NSData, NSError> completionHandler;

		public PhotoProgress Progress { get; private set; }

		public PhotoDownload (NSUrl url)
		{
			downloadURL = url;
			downloadState = new DownloadState ();
			Progress = new PhotoProgress {
				TotalUnitCount = -1,
				Kind = NSProgress.KindFile
			};
			Progress.SetUserInfo (NSProgress.FileOperationKindDownloading, NSProgress.FileOperationKindKey);
		}

		public void Start (Action<NSData, NSError> completionHandler)
		{
			this.completionHandler = completionHandler;
			downloadState.Queue = new DispatchQueue ("download_queue");
			downloadState.Queue.DispatchAsync (() => {
				try {
					var data = NSData.FromUrl (downloadURL);

					downloadState.DownloadTimer = new DispatchSource.Timer (downloadState.Queue);
					int downloadedBytes = 0;
					int randomMilliseconds = new Random ().Next (0, 500);
					var delay = new DispatchTime (DispatchTime.Now, (long)(TimerInterval * NanosecondsPerSecond - randomMilliseconds * NanosecondsPerMillisecond));
					downloadState.DownloadTimer.SetTimer (delay, (long)TimerInterval * NanosecondsPerSecond, 0);

					downloadState.DownloadTimer.SetEventHandler (() => {
						downloadedBytes += (int)(BatchSize * TimerInterval);

						if (downloadedBytes >= (int)data.Length) {
							downloadState.DownloadTimer.Cancel ();
							return;
						}

						DidDownloadData (downloadedBytes);
					});

					downloadState.DownloadTimer.SetCancelHandler (() => {
						if (downloadedBytes >= (int)data.Length)
							DidFinishDownload (data);
						else
							DidFailDownloadWithError (downloadState.DownloadError);

						downloadState.DownloadTimer = null;
					});

					WillBeginDownload ((int)data.Length);
					downloadState.DownloadTimer.Resume ();
				} catch (Exception) {
					var error = new NSError (NSError.CocoaErrorDomain, 0, null);
					DidFailDownloadWithError (error);
				}
			});
		}

		void FailDownloadWithError (NSError error)
		{
			downloadState.Queue.DispatchAsync (() => {
				if (downloadState.DownloadTimer == null)
					return;
				
				downloadState.DownloadError = error;

				if (downloadState.IsPaused)
					downloadState.DownloadTimer.Resume ();

				downloadState.DownloadTimer.Cancel ();
			});
		}

		void ChangeDownloadStatus (bool pause)
		{
			downloadState.Queue.DispatchAsync (() => {
				if ((downloadState.DownloadTimer == null) ||
					(downloadState.IsPaused == pause && !downloadState.DownloadTimer.IsCanceled))
					return;
				
				downloadState.IsPaused = pause;

				if (pause)
					downloadState.DownloadTimer.Suspend ();
				else
					downloadState.DownloadTimer.Resume ();
			});
		}

		void WillBeginDownload (int downloadLength)
		{
			Progress.TotalUnitCount = downloadLength;
			Progress.Cancellable = true;
			Progress.SetCancellationHandler (() => {
				var error = new NSError (NSError.CocoaErrorDomain, (nint)(int)NSCocoaError.UserCancelled, null);
				FailDownloadWithError (error);
			});

			Progress.Pausable = true;
			Progress.SetPauseHandler (() => ChangeDownloadStatus (true));
			Progress.SetResumingHandler (() => ChangeDownloadStatus (false));
		}

		void DidDownloadData (int numberOfBytes)
		{
			Progress.CompletedUnitCount = numberOfBytes;
		}

		void DidFinishDownload (NSData downloadedData)
		{
			Progress.CompletedUnitCount = (long)downloadedData.Length;
			completionHandler?.Invoke (downloadedData, null);
			completionHandler = null;
		}

		void DidFailDownloadWithError (NSError error)
		{
			completionHandler?.Invoke (null, error);
			completionHandler = null;
		}
	}
}

