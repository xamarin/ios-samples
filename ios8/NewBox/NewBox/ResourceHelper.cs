using System;
using System.Threading;
using System.Threading.Tasks;

using Foundation;
using System.IO;

namespace NewBox
{
	public static class ResourceHelper
	{
		public static Task<NSUrl> CopyToDocumentsDirectoryAsync(NSUrl fromUrl)
		{
			var tcs = new TaskCompletionSource<NSUrl>();

			NSUrl localDocDir = GetDocumentDirectoryUrl ();
			NSUrl toURL = localDocDir.Append (fromUrl.LastPathComponent, false);

			bool success = false;
			NSError coordinationError, copyError = null;
			NSFileCoordinator fileCoordinator = new NSFileCoordinator ();

			ThreadPool.QueueUserWorkItem (_ => {
				fileCoordinator.CoordinateReadWrite (fromUrl, 0, toURL, NSFileCoordinatorWritingOptions.ForReplacing, out coordinationError, (src, dst) => {
					NSFileManager fileManager = new NSFileManager();
					success = fileManager.Copy(src, dst, out copyError);

					if (success) {
						var attributes = new NSFileAttributes {
							ExtensionHidden = true
						};
						fileManager.SetAttributes (attributes, dst.Path);
						Console.WriteLine ("Copied file: {0} to: {1}.", src.AbsoluteString, dst.AbsoluteString);
					}
				});

				// In your app, handle this gracefully.
				if (!success)
					Console.WriteLine ("Couldn't copy file: {0} to: {1}. Error: {2}.", fromUrl.AbsoluteString,
						toURL.AbsoluteString, (coordinationError ?? copyError).Description);

				tcs.SetResult(toURL);
			});

			return tcs.Task;
		}

		static NSUrl GetDocumentDirectoryUrl()
		{
			var defaultManager = NSFileManager.DefaultManager;
			return defaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
		}
	}
}

