using System;
using UIKit;
using Foundation;
using NewsstandKit;

namespace Newsstand
{
	public class NewsstandUrlDelegate1 : NSUrlConnectionDownloadDelegate {
		
		string _name;
		NKIssue _issue;
		
		/// <summary>
		/// Pass the issue into the delegate to determine the target file location
		/// </summary>
		public NewsstandUrlDelegate1(string name, NKIssue issue)
		{
			_name = name;
			_issue = issue;
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
			Console.WriteLine ("-- Downloaded file: " + destinationUrl.Path);
			Console.WriteLine ("---Target issue location: " + _issue.ContentUrl.Path);
		
			var saveToFilename = System.IO.Path.Combine(_issue.ContentUrl.Path, "default.html");
			if (!System.IO.File.Exists (saveToFilename))
				System.IO.File.Move (destinationUrl.Path, saveToFilename);
		
			Console.WriteLine ("---File moved for issue: " + _issue.Name);
			
			//TODO: If you download a ZIP or something, process it in the background
			//UIApplication.SharedApplication.BeginBackgroundTask ();
		}
		
//		// doesn't get called by NewsstandKit, see FinishedDownloading above instead... 
//		public override void FinishedLoading (NSUrlConnection connection)
//		{
//			
//		}

		public override void FailedWithError (NSUrlConnection connection, NSError error)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			// do stuff
		}
	}	
}