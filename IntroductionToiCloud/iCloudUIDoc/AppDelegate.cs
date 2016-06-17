using System;
using System.IO;
using System.Threading;

using Foundation;
using UIKit;
using ObjCRuntime;

namespace Cloud {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {

		// just a single doc in iCloud for this example
		public const string MonkeyDocFilename = "test.txt";

		MonkeyDocument doc;
		NSMetadataQuery query;
		MonkeyDocumentViewController viewController;

		public override UIWindow Window { get; set; }

		bool HasiCloud { get; set; }

		bool CheckingForiCloud { get; set; }

		NSUrl iCloudUrl { get; set; }

		public AppDelegate ()
		{
			CheckingForiCloud = true;
		}

		public override void FinishedLaunching (UIApplication application)
		{
			viewController = new MonkeyDocumentViewController ();

			Window = new UIWindow (UIScreen.MainScreen.Bounds) {
				BackgroundColor = UIColor.White,
				Bounds = UIScreen.MainScreen.Bounds,
				RootViewController = viewController
			};
			Window.MakeKeyAndVisible ();

			// GetUrlForUbiquityContainer is blocking, Apple recommends background thread or your UI will freeze
			ThreadPool.QueueUserWorkItem (_ => {
				CheckingForiCloud = true;
				Console.WriteLine ("Checking for iCloud");
				var uburl = NSFileManager.DefaultManager.GetUrlForUbiquityContainer (null);
				// OR instead of null you can specify "TEAMID.com.your-company.ApplicationName"

				if (uburl == null) {
					HasiCloud = false;
					Console.WriteLine ("Can't find iCloud container, check your provisioning profile and entitlements");

					InvokeOnMainThread (() => {
						var alertController = UIAlertController.Create ("No \uE049 available",
						"Check your Entitlements.plist, BundleId, TeamId and Provisioning Profile!", UIAlertControllerStyle.Alert);
						alertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Destructive, null));
						viewController.PresentViewController (alertController, false, null);
					});
				} else {
					HasiCloud = true;
					iCloudUrl = uburl;
					Console.WriteLine ("yyy Yes iCloud! {0}", uburl.AbsoluteUrl);
				}
				CheckingForiCloud = false;
			});

			FindDocument ();
		}

		void FindDocument ()
		{
			Console.WriteLine ("FindDocument");
			query = new NSMetadataQuery {
				SearchScopes = new NSObject [] { NSMetadataQuery.UbiquitousDocumentsScope }
			};

			var pred = NSPredicate.FromFormat ("%K == %@", new NSObject[] {
				NSMetadataQuery.ItemFSNameKey, new NSString (MonkeyDocFilename)
			});

			Console.WriteLine ("Predicate:{0}", pred.PredicateFormat);
			query.Predicate = pred;

			NSNotificationCenter.DefaultCenter.AddObserver (
				this,
				new Selector ("queryDidFinishGathering:"),
				NSMetadataQuery.DidFinishGatheringNotification,
				query
			);
			
			query.StartQuery ();
		}

		[Export ("queryDidFinishGathering:")]
		void DidFinishGathering (NSNotification notification)
		{
			Console.WriteLine ("DidFinishGathering");
			var metadataQuery = (NSMetadataQuery)notification.Object;
			metadataQuery.DisableUpdates ();
			metadataQuery.StopQuery ();

			NSNotificationCenter.DefaultCenter.RemoveObserver (this, NSMetadataQuery.DidFinishGatheringNotification, metadataQuery);
			LoadDocument (metadataQuery);
		}

		void LoadDocument (NSMetadataQuery metadataQuery)
		{
			Console.WriteLine ("LoadDocument");	

			if (metadataQuery.ResultCount == 1) {
				var item = (NSMetadataItem)metadataQuery.ResultAtIndex (0);
				var url = (NSUrl)item.ValueForAttribute (NSMetadataQuery.ItemURLKey);
				doc = new MonkeyDocument (url);
				
				doc.Open (success => {
					if (success) {
						Console.WriteLine ("iCloud document opened");
						Console.WriteLine (" -- {0}", doc.DocumentString);
						viewController.DisplayDocument (doc);
					} else {
						Console.WriteLine ("failed to open iCloud document");
					}
				});

			} else if (metadataQuery.ResultCount == 0) {
				// no document exists, CREATE the first one
				// for a more realistic iCloud application the user will probably 
				// create documents as needed, so this bit of code wouldn't be necessary
				var docsFolder = Path.Combine (iCloudUrl.Path, "Documents"); // NOTE: Documents folder is user-accessible in Settings
				var docPath = Path.Combine (docsFolder, MonkeyDocFilename);
				var ubiq = new NSUrl (docPath, false);
				
				Console.WriteLine ("ubiq:" + ubiq.AbsoluteString);

				var monkeyDoc = new MonkeyDocument (ubiq);
				
				monkeyDoc.Save (monkeyDoc.FileUrl, UIDocumentSaveOperation.ForCreating, saveSuccess => {
					Console.WriteLine ("Save completion:" + saveSuccess);
					if (saveSuccess) {
						monkeyDoc.Open (openSuccess => {
							Console.WriteLine ("Open completion:" + openSuccess);
							if (openSuccess) {
								Console.WriteLine ("new document for iCloud");
								Console.WriteLine (" == " + monkeyDoc.DocumentString);
								viewController.DisplayDocument (monkeyDoc);
							} else {
								Console.WriteLine ("couldn't open");
							}
						});
					} else {
						Console.WriteLine ("couldn't save");
					}
				});
			} else {
				Console.WriteLine ("Who put all these other UIDocuments here?");
			}
		}
	}
}