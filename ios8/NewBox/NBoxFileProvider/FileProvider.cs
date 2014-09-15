using System;
using System.IO;

using Foundation;
using UIKit;

namespace NBoxFileProvider
{
	[Register ("FileProvider")]
	public class FileProvider : NSFileProviderExtension
	{
		public NSFileCoordinator FileCoordinator {
			get;
			private set;
		}

		public FileProvider ()
		{
			Console.WriteLine ("FileProvider ctor");
			NSError error;

			FileCoordinator = new NSFileCoordinator ();
			FileCoordinator.PurposeIdentifier = ProviderIdentifier;

			FileCoordinator.CoordinateWrite (DocumentStorageUrl, 0, out error, (newUrl) => {
				NSError err;

				// ensure that the DocumentStorageUrl actually exists
				NSFileManager.DefaultManager.CreateDirectory (newUrl, true, null, out err);
			});
		}

		public override void ProvidePlaceholderAtUrl (NSUrl url, Action<NSError> completionHandler)
		{
			Console.WriteLine ("FileProvider ProvidePlaceholderAtUrl");

			var fileName = Path.GetFileName (url.Path);
			var placeholder = NSFileProviderExtension.GetPlaceholderUrl (DocumentStorageUrl.Append (fileName, false));
			NSError error;

			// TODO: get file size for file at <url> from model

			FileCoordinator.CoordinateWrite (placeholder, 0, out error, (newUrl) => {
				NSError err = null;

				var metadata = new NSMutableDictionary ();
				metadata.Add (NSUrl.FileSizeKey, new NSNumber (0));

				NSFileProviderExtension.WritePlaceholder (placeholder, metadata, ref err);
			});

			if (completionHandler != null)
				completionHandler (null);
		}

		public override void StartProvidingItemAtUrl (NSUrl url, Action<NSError> completionHandler)
		{
			Console.WriteLine ("FileProvider StartProvidingItemAtUrl");

			// When this method is called, your extension should begin to download,
			// create, or otherwise make a local file ready for use.
			// As soon as the file is available, call the provided completion handler.
			// If any errors occur during this process, pass the error to the completion handler.
			// The system then passes the error back to the original coordinated read.

			NSError error, fileError = null;

			string str = "These are the contents of the file";
			NSData fileData = ((NSString)str).Encode (NSStringEncoding.UTF8);

			Console.WriteLine ("FileProvider before CoordinateWrite url {0}", url);
			FileCoordinator.CoordinateWrite (url, 0, out error, (newUrl) => {
				Console.WriteLine ("before data save");
				fileData.Save (newUrl, 0, out fileError);
				Console.WriteLine ("data saved");
			});
			Console.WriteLine ("FileProvider after CoordinateWrite");
			Console.WriteLine ("FileProvider CoordinateWrite error {0}", error);

			completionHandler (error ?? fileError);
		}

		public override void ItemChangedAtUrl (NSUrl url)
		{
			Console.WriteLine ("FileProvider ItemChangedAtUrl");
			// Called at some point after the file has changed; the provider may then trigger an upload

			// TODO: mark file at <url> as needing an update in the model; kick off update process
			Console.WriteLine ("Item changed at URL {0}", url);
		}

		public override void StopProvidingItemAtUrl (NSUrl url)
		{
			Console.WriteLine ("FileProvider StopProvidingItemAtUrl");
			// Called after the last claim to the file has been released. At this point, it is safe for the file provider to remove the content file.
			// Care should be taken that the corresponding placeholder file stays behind after the content file has been deleted.
			NSError err;

			FileCoordinator.CoordinateWrite (url, NSFileCoordinatorWritingOptions.ForDeleting, out err, (newUrl) => {
				NSError error;
				NSFileManager.DefaultManager.Remove (newUrl, out error);
			});

			ProvidePlaceholderAtUrl (url, null);
		}
	}
}
