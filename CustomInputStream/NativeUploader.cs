using System;

using System.Collections.Generic;
using Foundation;

namespace InputStreamTest
{
	public class NativeUploader : NSObject
	{
		NSMutableUrlRequest request;
		NSUrlConnection url_connection;
		ZInputStream random_input_stream;
		Dictionary<string,string> headers;

		protected override void Dispose (bool disposing)
		{
			if (url_connection != null) {
				url_connection.Dispose ();
				url_connection = null;
			}
			
			if (request != null) {
				if(request.BodyStream!=null)
					request.BodyStream = null;
				request.Dispose ();
				request = null;
			}
			
			if (random_input_stream != null) {
				random_input_stream.Dispose ();
				random_input_stream = null;	
			}
			
			base.Dispose (disposing);
		}

		public void AddHeader (string key, string value)
		{
			if (headers == null)
				headers = new Dictionary<string, string> ();
			
			headers.Add (key, value);
		}

		public void UploadStream (string url, long content_length, Action completed)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			
			AddHeader ("Expect", "100-continue");
			AddHeader ("Content-Type", "application/octet-stream");
			AddHeader ("Content-Length", content_length.ToString ());
			
			InvokeOnMainThread (delegate {
				try {
					request = CreateNativePostRequest (url, content_length);
				} catch (Exception e) {
					Console.WriteLine ("Exception uploading stream");
					Console.WriteLine (e);
					completed ();
					return;
				}
				
				url_connection = NSUrlConnection.FromRequest (request, new NativeUrlDelegate ((body) => {
					completed ();
					request.Dispose ();
				}, (reason) => {
					Console.WriteLine ("upload failed: " + reason);
					completed ();
				}));
			});
		}

		NSMutableUrlRequest CreateNativePostRequest (string url, long content_length)
		{
			NSUrl nsurl = NSUrl.FromString (url);
			
			if (nsurl == null)
				throw new Exception ("Invalid upload URL, could not create NSUrl from: '" + url + "'.");
			
			NSMutableUrlRequest request = new NSMutableUrlRequest (nsurl);
					
			request.HttpMethod = "POST";

			random_input_stream = new ZInputStream (content_length);
			 
			request.BodyStream = random_input_stream;
					
			if (headers != null) {
				foreach (var header in headers) 
					request [header.Key] = header.Value;
			}

			return request;
		}
		
		private class NativeUrlDelegate : NSUrlConnectionDelegate {
			Action<string> success_callback;
			Action<string> failure_callback;
			NSMutableData data;
			nint status_code;

			public NativeUrlDelegate (Action<string> success, Action<string> failure)
			{
				success_callback = success;
				failure_callback = failure;
				data = new NSMutableData();
			}
	
			public override void ReceivedData (NSUrlConnection connection, NSData d)
			{
				data.AppendData (d);
			}
		
			public override void ReceivedResponse (NSUrlConnection connection, NSUrlResponse response)
			{
				var http_response = response as NSHttpUrlResponse;
				if (http_response == null) {
					Console.WriteLine ("Received non HTTP url response: '{0}'", response);
					status_code = -1;
					return;
				}
				
				status_code = http_response.StatusCode;
				Console.WriteLine ("Status code of result:   '{0}'", status_code);
			}
			
			public override void FailedWithError (NSUrlConnection connection, NSError error)
			{
				if (failure_callback != null)
					failure_callback (error.LocalizedDescription);
			}

			public override void FinishedLoading (NSUrlConnection connection)
			{
				if (status_code != 200) {
					failure_callback (string.Format ("Did not receive a 200 HTTP status code, received '{0}'", status_code));
					return;
				}

				success_callback (data.ToString ());
			}
		}
	}
}

