//
// This sample shows how to use the Cocoa
// NS URL connection APIs for doing http
// transfers.
//
// It does not show all of the methods that could be
// overwritten for finer control though.
//
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Foundation;

namespace HttpClient.Core {
	public class CocoaProvider : NetworkProvider {
		public override async Task<Stream> ExecuteAsync ()
		{
			Busy ();

			Stream result = null;
			using (var cocoa = new Cocoa ()) {
				result = await cocoa.ExecuteAsync ();
			}

			Done ();

			return result;
		}
	}

	public class Cocoa : NSUrlConnectionDataDelegate {
		private TaskCompletionSource<Stream> taskCompletionSource;

		private byte [] result;

		public Cocoa ()
		{
			result = new byte [0];
		}

		public async Task<Stream> ExecuteAsync ()
		{
			taskCompletionSource = new TaskCompletionSource<Stream> ();

			var req = new NSUrlRequest (new NSUrl (NetworkProvider.WisdomUrl), NSUrlRequestCachePolicy.ReloadIgnoringCacheData, 10);
			NSUrlConnection.FromRequest (req, this);

			return await taskCompletionSource.Task;
		}

		public override void ReceivedData (NSUrlConnection connection, NSData data)
		{
			var nb = new byte [result.Length + (int) data.Length];
			result.CopyTo (nb, 0);
			Marshal.Copy (data.Bytes, nb, result.Length, (int) data.Length);
			result = nb;
		}

		public override void FinishedLoading (NSUrlConnection connection)
		{
			taskCompletionSource.TrySetResult (new MemoryStream (result));
		}

		public override void FailedWithError (NSUrlConnection connection, NSError error)
		{
			taskCompletionSource.TrySetResult (null);
		}
	}
}
