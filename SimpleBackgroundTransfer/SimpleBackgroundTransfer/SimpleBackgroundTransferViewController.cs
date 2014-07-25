using System;
using System.Drawing;
using Foundation;
using UIKit;

namespace SimpleBackgroundTransfer {

	public partial class SimpleBackgroundTransferViewController : UIViewController {

		const string Identifier = "com.SimpleBackgroundTransfer.BackgroundSession";
		const string DownloadUrlString = "https://atmire.com/dspace-labs3/bitstream/handle/123456789/7618/earth-map-huge.jpg";

		public NSUrlSessionDownloadTask downloadTask;
		public NSUrlSession session;

		public SimpleBackgroundTransferViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (session == null)
				session = InitBackgroundSession ();

			// Perform any additional setup after loading the view, typically from a nib.
			progressView.Progress = 0;
			imageView.Hidden = false;
			progressView.Hidden = true;

			startButton.Clicked += Start;

			// Force the app to crash
			crashButton.Clicked += delegate {
				string s = null;
				s.ToString ();
			};
		}

		void Start (object sender, EventArgs e)
		{
			if (downloadTask != null)
				return;

			using (var url = NSUrl.FromString (DownloadUrlString))
			using (var request = NSUrlRequest.FromUrl (url)) {
				downloadTask = session.CreateDownloadTask (request);
				downloadTask.Resume ();
			}

			imageView.Hidden = true;
			progressView.Hidden = false;
		}

		public NSUrlSession InitBackgroundSession ()
		{
			Console.WriteLine ("InitBackgroundSession");
			using (var configuration = NSUrlSessionConfiguration.BackgroundSessionConfiguration (Identifier)) {
				return NSUrlSession.FromConfiguration (configuration, new UrlSessionDelegate (this), null);
			}
		}

		public UIProgressView ProgressView {
			get { return progressView; }
		}

		public UIImageView ImageView {
			get { return imageView; }
		}
	}

	public class UrlSessionDelegate : NSUrlSessionDownloadDelegate
	{
		public SimpleBackgroundTransferViewController controller;

		public UrlSessionDelegate (SimpleBackgroundTransferViewController controller)
		{
			this.controller = controller;
		}

		public override void DidWriteData (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite)
		{
			Console.WriteLine ("Set Progress");
			if (downloadTask == controller.downloadTask) {
				float progress = totalBytesWritten / (float)totalBytesExpectedToWrite;
				Console.WriteLine (string.Format ("DownloadTask: {0}  progress: {1}", downloadTask, progress));
				InvokeOnMainThread( () => {
					controller.ProgressView.Progress = progress;
				});
			}
		}

		public override void DidFinishDownloading (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
		{
			Console.WriteLine ("Finished");
			Console.WriteLine ("File downloaded in : {0}", location);
			NSFileManager fileManager = NSFileManager.DefaultManager;

			var URLs = fileManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
			NSUrl documentsDictionry = URLs [0];

			NSUrl originalURL = downloadTask.OriginalRequest.Url;
			NSUrl destinationURL = documentsDictionry.Append ("image1.png", false);
			NSError removeCopy;
			NSError errorCopy;

			fileManager.Remove (destinationURL, out removeCopy);
			bool success = fileManager.Copy (location, destinationURL, out errorCopy);

			if (success) {
				// we do not need to be on the main/UI thread to load the UIImage
				UIImage image = UIImage.FromFile (destinationURL.Path);
				InvokeOnMainThread (() => {
					controller.ImageView.Image = image;
					controller.ImageView.Hidden = false;
					controller.ProgressView.Hidden = true;
				});
			} else {
				Console.WriteLine ("Error during the copy: {0}", errorCopy.LocalizedDescription);
			}
		}

		public override void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error)
		{
			Console.WriteLine ("DidComplete");
			if (error == null)
				Console.WriteLine ("Task: {0} completed successfully", task);
			else
				Console.WriteLine ("Task: {0} completed with error: {1}", task, error.LocalizedDescription);

			float progress = task.BytesReceived / (float)task.BytesExpectedToReceive;
			InvokeOnMainThread (() => {
				controller.ProgressView.Progress = progress;
			});

			controller.downloadTask = null;
		}

		public override void DidResume (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long resumeFileOffset, long expectedTotalBytes)
		{
			Console.WriteLine ("DidResume");
		}

		public override void DidFinishEventsForBackgroundSession (NSUrlSession session)
		{
			AppDelegate appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			var handler = appDelegate.BackgroundSessionCompletionHandler;
			if (handler != null) {
				appDelegate.BackgroundSessionCompletionHandler = null;
				handler ();
			}
			Console.WriteLine ("All tasks are finished");
		}
	}
}