using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UIKit;
using Foundation;
using NewsstandKit;

namespace Newsstand
{
	/// <summary>
	/// This class contains the code shown in the article 
	/// </summary>
	public class SampleCode
	{
		// -gcc_flags "-weak_framework NewsstandKit"
		public static void GetLibrary (UITextView display)
		{
			var library = NKLibrary.SharedLibrary;
			
//			var ptr = new MonoTouch.ObjCRuntime.Selector("sharedLibrary");
//			var hndl = ptr.Handle;
//			var nkl = MonoTouch.ObjCRuntime.Class.GetHandle ("NKLibrary");
//var nso = MonoTouch.ObjCRuntime.Runtime.GetNSObject (
//		MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend(
//			nkl, hndl
//			)
//		);

			var issues = library.Issues;
			
			if (issues.Length == 0)
				display.Text = "No issues in library yet\n\nTry the Populate button.";
			else
			{
				display.Text = "";
				foreach (var issue in library.Issues)
				{
					display.Text += String.Format ("{0} {1} {2}\n", issue.Name, issue.Date, issue.Status);
				}
			}
		}
		
		/// <summary>
		/// Use NKLibrary.SharedLibrary.AddIssue() to tell newsstand
		/// about issues you want to keep track of. Code can't download
		/// an issue until it's in the library, since the library keeps
		/// track of downloads and file locations for you.
		/// </summary>
		public static void PopulateLibrary (UITextView display)
		{
			var library = NKLibrary.SharedLibrary;
			var weekSeconds = 60 * 60 * 24 * 7;

			display.Text = "Adding issues to library...";

			if (library.GetIssue ("Las Vegas") == null)
				library.AddIssue("Las Vegas", NSDate.Now.AddSeconds(-1 * weekSeconds));
			else	
				display.Text += "\nLas Vegas already added";
				

			if (library.GetIssue ("New York") == null)
				library.AddIssue("New York", NSDate.Now.AddSeconds(-2 * weekSeconds));
			else
				display.Text += "\nNew York already added";
				

			if (library.GetIssue ("San Francisco") == null)
				library.AddIssue("San Francisco", NSDate.Now.AddSeconds(-3 * weekSeconds));
			else
				display.Text += "\nSan Francisco already added";
	
			display.Text += "\n\nLibrary populated!";
		}
		
		/// <summary>
		/// When the user reads an issue, tell NewsstandKit about it so it can keep track
		/// </summary>
		public static void SetReading (UITextView display)
		{
			var library = NKLibrary.SharedLibrary;
			var issues = library.Issues;
			
			if (library.CurrentlyReadingIssue == null)
				display.Text = "No issue is currently being read";
			else
			{
				display.Text = "Currently reading: " + library.CurrentlyReadingIssue.Name;
				display.Text += "\nDate: " + library.CurrentlyReadingIssue.Date;
				display.Text += "\nStatus: " + library.CurrentlyReadingIssue.Status;
				display.Text += "\nContentURL: " + library.CurrentlyReadingIssue.ContentUrl.Path;
				display.Text += "\n\nNotice how the ContentURL changes for each issue - NewsstandKit manages where your issue's files are stored";
			}

			var randomIssueNumber = new Random().Next(0,3);
			// Note: it is possible to set this to an issue with Status=None
			// (content hasn't been downloaded yet).
			library.CurrentlyReadingIssue = issues[randomIssueNumber]; // New York
			
			// Set the Badge to zero to remove the 'New' banner
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}
		

		static NewsstandUrlDelegate1 newsstandDelegate;
		/// <summary>
		/// Assets are attached to an issue and then downloaded.
		/// The DownloadDelegate can call UIApplication.SharedApplication.BeginBackgroundTask()
		/// to 'process' the download, AND set a new icon
		/// </summary>
		public static void Download (UITextView display)
		{
			var library = NKLibrary.SharedLibrary;
			var issues = library.Issues;
			var issue = issues[1] ; // New York

			NKAssetDownload asset = issue.AddAsset (new NSUrlRequest(new NSUrl("http://xamarin.com/")));
			
			newsstandDelegate = new NewsstandUrlDelegate1("NewYorkContent", issue);
		
			//you do not have background download privileges: add 'newsstand-content' to mainBundle.infoDictionary.UIBackgroundModes	
			asset.DownloadWithDelegate (newsstandDelegate);
			
			display.Text = String.Format("Issue {0} downloading has started", issues[1].Name);
			display.Text += "\n\nPress the Read button quickly to see the 'downloading' status detected";
		}

		/// <summary>
		/// Tell NewsstandKit which issue is currently being read. 
		/// Prevents that issue being 'cleaned up' if diskspace is required.
		/// </summary>
		public static void Read (UITextView display)
		{	
			var library = NKLibrary.SharedLibrary;
			var issues = library.Issues;
			var issue = issues[1]; // New York

			switch (issue.Status)
			{
				case NKIssueContentStatus.Available:
					display.Text = "Just download some random HTML to simulate downloading an issue.\n\n";
					display.Text += String.Format ("Issue '{0}' content has been downloaded to \n\t{1}"
												, issue.Name
												, issue.ContentUrl.Path);
					display.Text += "\n\n\n------------------------------------\n\n\n"
									+ System.IO.File.ReadAllText(
												Path.Combine(issue.ContentUrl.Path, "default.html"));
					
					// set this whenever user reads a different issue
					library.CurrentlyReadingIssue = issue; 
	
					UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					break;
				case NKIssueContentStatus.Downloading:
					display.Text = string.Format ("Issue '{0}' is still downloading...\n\nPress Read again until the content appears"
												, issue.Name);
					break;
				default:
					display.Text = string.Format ("Issue '{0}' has not been downloaded"
												, issue.Name);
					break;
			}
		}
		
		/// <summary>
		/// Change the icon that appears in Newsstand - you would do this
		/// when you download a new issue (via a notification or otherwise).
		/// Can be done in the background.
		/// </summary>
		public static void UpdateIcon (UITextView display)
		{
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 1;
			
			display.Text = string.Format ("Newsstand application badge has been set to 1");

			string coverFile = "cover_lasvegas.jpg";
			var randomIssueNumber = new Random().Next(0,4);
			switch(randomIssueNumber)
			{
				case 1:
					coverFile = "cover_newyork.jpg";
					break;
				case 2: 
					coverFile = "cover_sanfrancisco.jpg";
					break;
				case 3: 
					coverFile = "cover_peru.jpg";
					break;
			}
			
			// We can use a downloaded image here - it doesn't have to exist in the 
			// application bundle
			UIImage newcover = new UIImage(coverFile);
			// Setting the Newsstand Icon in code doesn't seem to have any effect
			// if your Info.plist doesn't already define the UINewsstandIcon key
			UIApplication.SharedApplication.SetNewsstandIconImage (newcover);
			

			display.Text += string.Format ("\n\nNewsstand Icon has been set to '{0}'\n\n\nClose the app to see the new cover image in Newsstand"
												, coverFile);
		}
	}
}