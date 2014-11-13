using System;
using System.Threading;
using System.IO;

using Foundation;
using Common;

namespace ListerKit
{
	public class ListCoordinator : NSObject
	{
		public event EventHandler StorageChoiceChanged;

		static readonly ListCoordinator sharedListCoordinator;
		public static ListCoordinator SharedListCoordinator {
			get {
				return sharedListCoordinator;
			}
		}

		public NSUrl DocumentsDirectory { get; private set; }
		public NSUrl TodayDocumentURL {
			get {
				string todayFileName = AppConfig.SharedAppConfiguration.TodayDocumentNameAndExtension;
				return DocumentsDirectory.Append (todayFileName, false);
			}
		}

		static ListCoordinator()
		{
			sharedListCoordinator = new ListCoordinator();
			sharedListCoordinator.DocumentsDirectory = GetFirstDocumentDirectoryUrlForUserDomain ();
		}

		ListCoordinator ()
		{
			AppConfig.SharedAppConfiguration.StorageOptionChanged += UpdateDocumentStorageContainerURL;
		}

		static NSUrl GetFirstDocumentDirectoryUrlForUserDomain()
		{
			var defaultManager = NSFileManager.DefaultManager;
			return defaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
		}

		#region Convenience

		void CopyFileToDocumentsDirectory(NSUrl fromUrl)
		{
			NSUrl toURL = DocumentsDirectory.Append (fromUrl.LastPathComponent, false);

			bool success = false;
			NSError error;
			NSFileCoordinator fileCoordinator = new NSFileCoordinator ();

			fileCoordinator.CoordinateWriteWrite (fromUrl, NSFileCoordinatorWritingOptions.ForMoving, toURL, NSFileCoordinatorWritingOptions.ForReplacing, out error, (src, dst) => {
				NSFileManager fileManager = new NSFileManager();
				success = fileManager.Copy(src, dst, out error);

				if (success) {
					var attributes = new NSFileAttributes {
						ExtensionHidden = true
					};
					fileManager.SetAttributes (attributes, dst.Path);
					Console.WriteLine ("Moved file: {0} to: {1}.", src.AbsoluteString, dst.AbsoluteString);
				}
			});

			// In your app, handle this gracefully.
			if (!success)
				Console.WriteLine ("Couldn't move file: {0} to: {1}. Error: {3}.", fromUrl.AbsoluteString,
					toURL.AbsoluteString, error.Description);
		}

		public void DeleteFileAtURL(NSUrl fileURL)
		{
			var fileCoordinator = new NSFileCoordinator ();
			NSError error;
			bool success = false;

			fileCoordinator.CoordinateWrite (fileURL, NSFileCoordinatorWritingOptions.ForDeleting, out error,
				writingURL => {
					NSFileManager fileManager = new NSFileManager();
					success = fileManager.Remove(writingURL, out error);
			});

				// In your app, handle this gracefully.
			if (!success) {
				string msg = string.Format("Couldn't delete file at URL {0}. Error: {1}.", fileURL.AbsoluteString, error.Description);
				throw new InvalidProgramException (msg);
			}
		}

		#endregion

		#region Document Management

		public void CopyInitialDocuments()
		{
			var bundle = NSBundle.MainBundle;
			NSUrl[] defaultListPaths = bundle.GetUrlsForResourcesWithExtension(AppConfig.ListerFileExtension, string.Empty);

			foreach (var p in defaultListPaths)
				CopyFileToDocumentsDirectory (p);
		}

		void UpdateDocumentStorageContainerURL(object sender, EventArgs e)
		{
			NSUrl oldDocumentsDirectory = DocumentsDirectory;

			NSFileManager fileManager = NSFileManager.DefaultManager;
			NSNotificationCenter defaultCenter = NSNotificationCenter.DefaultCenter;

			if (AppConfig.SharedAppConfiguration.StorageOption != StorageType.Cloud) {
				DocumentsDirectory = GetFirstDocumentDirectoryUrlForUserDomain ();
				RaiseStorageChoiceChanged ();
			} else {
				ThreadPool.QueueUserWorkItem (_ => {
					// The call to GetUrlForUbiquityContainer should be on a background thread.
					// You can pass null to retrieve the URL for the first container in the list
					// For more information visit https://developer.apple.com/library/ios/documentation/General/Conceptual/iCloudDesignGuide/Chapters/iCloudFundametals.html#//apple_ref/doc/uid/TP40012094-CH6-SW1
					NSUrl cloudDirectory = fileManager.GetUrlForUbiquityContainer(null);

					InvokeOnMainThread(()=> {
						DocumentsDirectory = cloudDirectory.Append("Documents", true);

						NSError error;
						NSUrl[] localDocuments = fileManager.GetDirectoryContent(oldDocumentsDirectory,
							null, NSDirectoryEnumerationOptions.SkipsPackageDescendants, out error);

						foreach (NSUrl url in localDocuments) {
							string ext = Path.GetExtension(url.AbsoluteString).Replace(".", string.Empty);
							if (ext == AppConfig.ListerFileExtension)
								MakeItemUbiquitousAtURL(url);
						}

						RaiseStorageChoiceChanged();
					});
				});
			}
		}

		void RaiseStorageChoiceChanged()
		{
			var handler = StorageChoiceChanged;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		void MakeItemUbiquitousAtURL(NSUrl sourceURL)
		{
			string destinationFileName = sourceURL.LastPathComponent;
			NSUrl destinationURL = DocumentsDirectory.Append (destinationFileName, false);

			// Upload the file to iCloud on a background queue.
			ThreadPool.QueueUserWorkItem (_ => {
				NSFileManager fileManager = new NSFileManager();
				NSError error;
				bool success = fileManager.SetUbiquitous(true, sourceURL, destinationURL, out error);

				// If the move wasn't successful, try removing the item locally since the document may already exist in the cloud.
				if (!success)
					fileManager.Remove(sourceURL, out error);
			});
		}

		#endregion

		#region Document Name Helper Methods

		public NSUrl DocumentURLForName(string name)
		{
			var url = DocumentsDirectory.Append (name, false);
			url = url.AppendPathExtension (AppConfig.ListerFileExtension);
			return url;
		}

		public bool IsValidDocumentName(string name)
		{
			if (string.IsNullOrWhiteSpace (name))
				return false;

			string proposedDocumentPath = DocumentURLForName (name).Path;
			return !File.Exists (proposedDocumentPath);
		}

		#endregion
	}
}

