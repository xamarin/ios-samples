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
using Foundation;
using UIKit;
using System.Runtime.InteropServices;

namespace HttpClient
{
	
	public class Cocoa : NSUrlConnectionDelegate
	{
		AppDelegate ad;
		byte [] result;
		
		public Cocoa (AppDelegate ad)
		{
			this.ad = ad;
			result = new byte [0];
		}
		
		public void HttpSample ()
		{
			var req = new NSUrlRequest (new NSUrl (Application.WisdomUrl), NSUrlRequestCachePolicy.ReloadIgnoringCacheData, 10);
			NSUrlConnection.FromRequest (req, this);
		}

		// Collect all the data
		public override void ReceivedData (NSUrlConnection connection, NSData data)
		{
			byte [] nb = new byte [(int)result.Length + (int)data.Length];
			result.CopyTo (nb, 0);
			Marshal.Copy (data.Bytes, nb, result.Length, (int) data.Length);
			result = nb;
		}
		
		public override void FinishedLoading (NSUrlConnection connection)
		{
			Application.Done ();
			var ms = new MemoryStream (result);
			ad.RenderStream (ms);
		}

		public override void FailedWithError (NSUrlConnection connection, NSError error)
		{
			Application.Done ();
		}

	}
}