//
// This file contains the sample code to use System.Net.WebRequest
// on the iPhone to communicate with HTTP and HTTPS servers
//
// Author:
//   Miguel de Icaza
//

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HttpClient.Core
{
    public class DotNetProvider : NetworkProvider
    {
        private TaskCompletionSource<Stream> taskCompletionSource;

        private readonly bool secure;

        public DotNetProvider(bool secure)
        {
            this.secure = secure;
        }

        public override async Task<Stream> ExecuteAsync()
        {
            Busy();
            taskCompletionSource = new TaskCompletionSource<Stream>();

            if (!secure)
            { 
                // http
                var request = WebRequest.Create(WisdomUrl);
                request.BeginGetResponse(FeedDownloaded, request);
            }
            else
            {
                // https
                var request = (HttpWebRequest)WebRequest.Create("https://gmail.com");

                //
                // To not depend on the root certficates, we will
                // accept any certificates:
                //
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, ssl) => true;

                request.BeginGetResponse(GmailDownloaded, request);
            }

            return await taskCompletionSource.Task;
        }

        /// <summary>
        /// Invoked when we get the stream back from the twitter feed
        /// We parse the RSS feed and push the data into a table.
        /// </summary>
        private void FeedDownloaded(IAsyncResult result)
        {
            Done();
            var request = result.AsyncState as HttpWebRequest;

            try
            {
                var response = request.EndGetResponse(result);
                taskCompletionSource.TrySetResult(response.GetResponseStream());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                taskCompletionSource.TrySetResult(null);
            }
        }

        /// <summary>
        /// This sample just gets the result from calling https://gmail.com, an HTTPS secure connection,
        /// we do not attempt to parse the output, but merely dump it as text
        /// </summary>
        private void GmailDownloaded(IAsyncResult result)
        {
            Done();
            var request = result.AsyncState as HttpWebRequest;

            try
            {
                var response = request.EndGetResponse(result);
                taskCompletionSource.TrySetResult(response.GetResponseStream());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                taskCompletionSource.TrySetResult(null);
            }
        }
    }
}