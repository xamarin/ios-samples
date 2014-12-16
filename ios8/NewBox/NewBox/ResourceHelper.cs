using System;
using System.Threading;
using System.Threading.Tasks;

using Foundation;
using System.IO;

namespace NewBox
{
	public static class ResourceHelper
	{
		public static async Task<NSUrl> CopyResourceToCloud()
		{
			var bundleFileUrl = NSBundle.MainBundle.GetUrlForResource ("TextDocument", "txt");
			var cloudFileUrl = GetCloudUrlFor (bundleFileUrl);

			// We can't just move file from bunlde folder to iCloud container (will produce error: operation is not permited)
			// We need copy file from bunlde to somewhere (e.x. Documents folder) and then
			// move it from that location to iCloud
			var docsFileUrl = await CopyToDocumentsDirectoryAsync (bundleFileUrl);
			await MoveToiCloud (docsFileUrl, cloudFileUrl);

			return cloudFileUrl;
		}

		static Task<NSUrl> CopyToDocumentsDirectoryAsync(NSUrl fromUrl)
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

		static Task MoveToiCloud(NSUrl sourceUrl, NSUrl destinationURL)
		{
			if (NSFileManager.DefaultManager.UbiquityIdentityToken == null)
				return Task.FromResult<NSUrl> (null);

			var tcs = new TaskCompletionSource<object>();

			// Upload the file to iCloud on a background queue.
			ThreadPool.QueueUserWorkItem (_ => {
				NSError error;
				NSFileManager fileManager = new NSFileManager();
				bool success = fileManager.SetUbiquitous(true, sourceUrl, destinationURL, out error);

				if (!success)
					Console.WriteLine (error);

				tcs.SetResult(null);
			});

			return tcs.Task;
		}

		static NSUrl GetDocumentDirectoryUrl()
		{
			var defaultManager = NSFileManager.DefaultManager;
			return defaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
		}

		static NSUrl GetCloudUrlFor(NSUrl localUrl)
		{
			string destinationFileName = localUrl.LastPathComponent;
			NSUrl cloudDirectory = NSFileManager.DefaultManager.GetUrlForUbiquityContainer(null);
			NSUrl destinationURL = cloudDirectory.Append (destinationFileName, false);

			return destinationURL;
		}
	}
}

