using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Foundation;
using UIKit;
using ObjCRuntime;
using System.IO;

namespace DocPicker
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		#region Static Properties
		public const string TestFilename = "test.txt"; 
		#endregion

		#region Computed Properties
		public override UIWindow Window { get; set; }
		public bool HasiCloud { get; set; }
		public bool CheckingForiCloud { get; set; }
		public NSUrl iCloudUrl { get; set; }

		public GenericTextDocument Document { get; set; }
		public NSMetadataQuery Query { get; set; }
		public NSData Bookmark { get; set; }
		#endregion

		#region Private Methods
		/// <summary>
		/// Starts a query to look for the sample Test File.
		/// </summary>
		private void FindDocument () {
			Console.WriteLine ("Finding Document...");

			// Create a new query and set it's scope
			Query = new NSMetadataQuery();
			Query.SearchScopes = new NSObject [] {
				NSMetadataQuery.UbiquitousDocumentsScope,
				NSMetadataQuery.UbiquitousDataScope,
				NSMetadataQuery.AccessibleUbiquitousExternalDocumentsScope
			};

			// Build a predicate to locate the file by name and attach it to the query
			var pred = NSPredicate.FromFormat ("%K == %@"
				, new NSObject[] {
				NSMetadataQuery.ItemFSNameKey
				, new NSString(TestFilename)});
			Query.Predicate = pred;

			// Register a notification for when the query returns
			NSNotificationCenter.DefaultCenter.AddObserver (this
				, new Selector("queryDidFinishGathering:")
				, NSMetadataQuery.DidFinishGatheringNotification
				, Query);

			// Start looking for the file
			Query.StartQuery ();
			Console.WriteLine ("Querying: {0}", Query.IsGathering);
		}

		/// <summary>
		/// Callback for when the query finishs gathering documents.
		/// </summary>
		/// <param name="notification">Notification.</param>
		[Export("queryDidFinishGathering:")]
		public void DidFinishGathering (NSNotification notification) {
			Console.WriteLine ("Finish Gathering Documents.");

			// Access the query and stop it from running
			var query = (NSMetadataQuery)notification.Object;
			query.DisableUpdates();
			query.StopQuery();

			// Release the notification
			NSNotificationCenter.DefaultCenter.RemoveObserver (this
				, NSMetadataQuery.DidFinishGatheringNotification
				, query);

			// Load the document that the query returned
			LoadDocument(query);
		}

		/// <summary>
		/// Loads the document.
		/// </summary>
		/// <param name="query">Query.</param>
		private void LoadDocument (NSMetadataQuery query) {
			Console.WriteLine ("Loading Document...");	

			// Take action based on the returned record count
			switch (query.ResultCount) {
			case 0:
				// Create a new document
				CreateNewDocument ();
				break;
			case 1:
				// Gain access to the url and create a new document from
				// that instance
				NSMetadataItem item = (NSMetadataItem)query.ResultAtIndex (0);
				var url = (NSUrl)item.ValueForAttribute (NSMetadataQuery.ItemURLKey);

				// Load the document
				OpenDocument (url);
				break;
			default:
				// There has been an issue
				Console.WriteLine ("Issue: More than one document found...");
				break;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Opens the document.
		/// </summary>
		/// <param name="url">URL.</param>
		public void OpenDocument(NSUrl url) {

			Console.WriteLine ("Attempting to open: {0}", url);
			Document = new GenericTextDocument (url);

			// Open the document
			Document.Open ( (success) => {
				if (success) {
					Console.WriteLine ("Document Opened");
				} else
					Console.WriteLine ("Failed to Open Document");
			});

			// Inform caller
			RaiseDocumentLoaded (Document);
		}

		/// <summary>
		/// Creates the new document.
		/// </summary>
		public void CreateNewDocument() {
			// Create path to new file
			// var docsFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var docsFolder = Path.Combine(iCloudUrl.Path, "Documents");
			var docPath = Path.Combine (docsFolder, TestFilename);
			var ubiq = new NSUrl (docPath, false);

			// Create new document at path 
			Console.WriteLine ("Creating Document at:" + ubiq.AbsoluteString);
			Document = new GenericTextDocument (ubiq);

			// Set the default value
			Document.Contents = "(default value)";

			// Save document to path
			Document.Save (Document.FileUrl, UIDocumentSaveOperation.ForCreating, (saveSuccess) => {
				Console.WriteLine ("Save completion:" + saveSuccess);
				if (saveSuccess) {
					Console.WriteLine ("Document Saved");
				} else {
					Console.WriteLine ("Unable to Save Document");
				}
			});

			// Inform caller
			RaiseDocumentLoaded (Document);
		}

		/// <summary>
		/// Saves the document.
		/// </summary>
		/// <returns><c>true</c>, if document was saved, <c>false</c> otherwise.</returns>
		public bool SaveDocument() {
			bool successful = false;

			// Save document to path
			Document.Save (Document.FileUrl, UIDocumentSaveOperation.ForOverwriting, (saveSuccess) => {
				Console.WriteLine ("Save completion: " + saveSuccess);
				if (saveSuccess) {
					Console.WriteLine ("Document Saved");
					successful = true;
				} else {
					Console.WriteLine ("Unable to Save Document");
					successful=false;
				}
			});

			// Return results
			return successful;
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Finisheds the launching.
		/// </summary>
		/// <param name="application">Application.</param>
		public override void FinishedLaunching (UIApplication application)
		{

			// Start a new thread to check and see if the user has iCloud
			// enabled.
			new Thread(new ThreadStart(() => {
				// Inform caller that we are checking for iCloud
				CheckingForiCloud = true;

				// Checks to see if the user of this device has iCloud
				// enabled
				var uburl = NSFileManager.DefaultManager.GetUrlForUbiquityContainer(null);

				// Connected to iCloud?
				if (uburl == null)
				{
					// No, inform caller
					HasiCloud = false;
					iCloudUrl =null;
					Console.WriteLine("Unable to connect to iCloud");
					InvokeOnMainThread(()=>{
						var okAlertController = UIAlertController.Create ("iCloud Not Available", "Developer, please check your Entitlements.plist, Bundle ID and Provisioning Profiles.", UIAlertControllerStyle.Alert);
						okAlertController.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));
						Window.RootViewController.PresentViewController (okAlertController, true, null);
					});
				}
				else
				{	
					// Yes, inform caller and save location the the Application Container
					HasiCloud = true;
					iCloudUrl = uburl;
					Console.WriteLine("Connected to iCloud");

					// If we have made the connection with iCloud, start looking for documents
					InvokeOnMainThread(()=>{
						// Search for the default document
						FindDocument ();
					});
				}

				// Inform caller that we are no longer looking for iCloud
				CheckingForiCloud = false;

			})).Start();
				
		}
		
		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
			// Trap all errors
			try {
				// Values to include in the bookmark packet
				var resources = new string[] {
					NSUrl.FileSecurityKey,
					NSUrl.ContentModificationDateKey,
					NSUrl.FileResourceIdentifierKey,
					NSUrl.FileResourceTypeKey,
					NSUrl.LocalizedNameKey
				};

				// Create the bookmark
				NSError err;
				Bookmark = Document.FileUrl.CreateBookmarkData (NSUrlBookmarkCreationOptions.WithSecurityScope, resources, iCloudUrl, out err);

				// Was there an error?
				if (err != null) {
					// Yes, report it
					Console.WriteLine ("Error Creating Bookmark: {0}", err.LocalizedDescription);
				}
			}
			catch (Exception e) {
				// Report error
				Console.WriteLine ("Error: {0}", e.Message);
			}
		}
		
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
			// Is there any bookmark data?
			if (Bookmark != null) {
				// Trap all errors
				try {
					// Yes, attempt to restore it
					bool isBookmarkStale;
					NSError err;
					var srcUrl = new NSUrl (Bookmark, NSUrlBookmarkResolutionOptions.WithSecurityScope, iCloudUrl, out isBookmarkStale, out err);

					// Was there an error?
					if (err != null) {
						// Yes, report it
						Console.WriteLine ("Error Loading Bookmark: {0}", err.LocalizedDescription);
					} else {
						// Load document from bookmark
						OpenDocument (srcUrl);
					}
				}
				catch (Exception e) {
					// Report error
					Console.WriteLine ("Error: {0}", e.Message);
				}
			}

		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}
		#endregion

		#region Events
		/// <summary>
		/// Document loaded delegate.
		/// </summary>
		public delegate void DocumentLoadedDelegate(GenericTextDocument document);
		public event DocumentLoadedDelegate DocumentLoaded;

		/// <summary>
		/// Raises the document loaded event.
		/// </summary>
		/// <param name="document">Document.</param>
		internal void RaiseDocumentLoaded(GenericTextDocument document) {
			// Inform caller
			if (this.DocumentLoaded != null) {
				this.DocumentLoaded (document);
			}
		}
		#endregion
	}
}

