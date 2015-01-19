using System;
using System.IO;

using Foundation;
using UIKit;

namespace NBoxFileProvider
{
	[Register ("FileProvider")]
	public class FileProvider : NSFileProviderExtension
	{
		public FileProvider ()
		{
			Console.WriteLine ("FileProvider ctor");

			// ensure that the DocumentStorageUrl actually exists
			NSError error;
			NSFileManager.DefaultManager.CreateDirectory (DocumentStorageUrl, true, null, out error);
		}

		public override void ProvidePlaceholderAtUrl (NSUrl url, Action<NSError> completionHandler)
		{
			Console.WriteLine ("FileProvider ProvidePlaceholderAtUrl");

			var fileName = Path.GetFileName (url.Path);
			var placeholder = NSFileProviderExtension.GetPlaceholderUrl (DocumentStorageUrl.Append (fileName, false));

			// get file size for file at <url> from model
			NSError err = null;

			var metadata = new NSDictionary (NSUrl.FileSizeKey, 0);
			NSFileProviderExtension.WritePlaceholder (placeholder, metadata, ref err);

			if (completionHandler != null)
				completionHandler (null);
		}

		public override void StartProvidingItemAtUrl (NSUrl url, Action<NSError> completionHandler)
		{
			Console.WriteLine ("FileProvider StartProvidingItemAtUrl {0}", url);

			// When this method is called, your extension should begin to download,
			// create, or otherwise make a local file ready for use.
			// As soon as the file is available, call the provided completion handler.
			// If any errors occur during this process, pass the error to the completion handler.
			// The system then passes the error back to the original coordinated read.

			string str = "These are the contents of the file";
			NSData fileData = ((NSString)str).Encode (NSStringEncoding.UTF8);

			NSError error = null;
			fileData.Save (url, NSDataWritingOptions.Atomic, out error);
			if(error != null)
				Console.WriteLine ("FileProvider atomic save error {0}", error);

			completionHandler (error);
		}

		public override void ItemChangedAtUrl (NSUrl url)
		{
			Console.WriteLine ("FileProvider ItemChangedAtUrl {0}", url);
			// Called at some point after the file has changed; the provider may then trigger an upload
			// mark file at <url> as needing an update in the model; kick off update process
		}

		public override void StopProvidingItemAtUrl (NSUrl url)
		{
			Console.WriteLine ("FileProvider StopProvidingItemAtUrl {0}", url);
			// Called after the last claim to the file has been released. At this point, it is safe for the file provider to remove the content file.
			// Care should be taken that the corresponding placeholder file stays behind after the content file has been deleted.

			NSError err;
			NSFileManager.DefaultManager.Remove (url, out err);
			ProvidePlaceholderAtUrl (url, null);
		}
	}
}
