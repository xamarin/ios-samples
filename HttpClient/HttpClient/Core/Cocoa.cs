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

namespace HttpClient.Core
{

    public class Cocoa : NetworkProvider, INSUrlConnectionDataDelegate
    {
        private TaskCompletionSource<Stream> taskCompletionSource;

        private byte[] result;

        public Cocoa()
        {
            result = new byte[0];
        }

        public IntPtr Handle => throw new NotImplementedException();

        public override async Task<Stream> ExecuteAsync()
        {
            Busy();
            taskCompletionSource = new TaskCompletionSource<Stream>();

            var req = new NSUrlRequest(new NSUrl(WisdomUrl), NSUrlRequestCachePolicy.ReloadIgnoringCacheData, 10);
            NSUrlConnection.FromRequest(req, this);


            return await taskCompletionSource.Task;
        }

        [Export("connection:didReceiveData:")]
        public void ReceivedData(NSUrlConnection connection, NSData data)
        {
            var nb = new byte[result.Length + (int)data.Length];
            result.CopyTo(nb, 0);
            Marshal.Copy(data.Bytes, nb, result.Length, (int)data.Length);
            result = nb;
        }

        [Export("connectionDidFinishLoading:")]
        public void FinishedLoading(NSUrlConnection connection)
        {
            Done();
            taskCompletionSource.TrySetResult(new MemoryStream(result));
        }

        [Export("connection:didFailWithError:")]
        public void FailedWithError(NSUrlConnection connection, NSError error)
        {
            Done();
            taskCompletionSource.TrySetResult(null);
        }

        public void Dispose() { }
    }
}