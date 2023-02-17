using System;
using System.IO;

using Foundation;
using NewsstandKit;

namespace Newsstand {
	public class NewsstandUrlDelegate1 : NSUrlConnectionDownloadDelegate {
		public Action OnDownloadingFinished { get; set; }

		NKIssue Issue { get; set; }

		/// <summary>
		/// Pass the issue into the delegate to determine the target file location
		/// </summary>
		public NewsstandUrlDelegate1 (string name, NKIssue issue)
		{
			Issue = issue;
		}

		/// <summary>
		/// Delivers progress information for the download
		/// </summary>
		public override void WroteData (NSUrlConnection connection, long bytesWritten, long totalBytesWritten, long expectedTotalBytes)
		{
			Console.WriteLine ("WroteData");
		}
		/// <summary>
		/// Sent when a connection resumes after being suspended
		/// </summary>
		public override void ResumedDownloading (NSUrlConnection connection, long totalBytesWritten, long expectedTotalBytes)
		{
			Console.WriteLine ("ResumedDownloading");
		}
		/// <summary>
		/// Connection has successfully downloaded the asset to the destinationUrl file location.
		/// You must copy/move this file to a more persisten/appropriate location
		/// </summary>
		public override void FinishedDownloading (NSUrlConnection connection, NSUrl destinationUrl)
		{
			Console.WriteLine ($"Downloaded file: {destinationUrl.Path}");
			Console.WriteLine ($"Target issue location: {Issue.ContentUrl.Path}");

			var saveToFilename = Path.Combine (Issue.ContentUrl.Path, "default.html");

			if (!File.Exists (saveToFilename))
				File.Move (destinationUrl.Path, saveToFilename);

			Console.WriteLine ($"File moved for issue: {Issue.Name}");

			if (OnDownloadingFinished != null)
				OnDownloadingFinished ();
		}
	}
}
