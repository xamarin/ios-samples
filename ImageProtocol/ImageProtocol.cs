// 
// ImageProtocol.cs
//  
// Author:
//       Rolf Bjarne Kvinge (rolf@xamarin.com)
// 
// Copyright 2012, Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;

namespace ImageProtocol
{
	public class ImageProtocol : NSUrlProtocol
	{
		[Export ("canInitWithRequest:")]
		public static bool canInitWithRequest (NSUrlRequest request)
		{
			return request.Url.Scheme == "custom";
		}

		[Export ("canonicalRequestForRequest:")]
		public static new NSUrlRequest GetCanonicalRequest (NSUrlRequest forRequest)
		{
			return forRequest;
		}

		[Export ("initWithRequest:cachedResponse:client:")]
		public ImageProtocol (NSUrlRequest request, NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client) 
			: base (request, cachedResponse, client)
		{
		}
		public override void StartLoading ()
		{
			var value = Request.Url.Path.Substring (1);
			using (var image = Render (value)) {
				using (var response = new NSUrlResponse (Request.Url, "image/jpeg", -1, null)) {
					Client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.NotAllowed);
					this.InvokeOnMainThread (delegate {
						using (var data = image.AsJPEG ()) {
							Client.DataLoaded (this, data);
						}
						Client.FinishedLoading (this);
					});
				}
			}
		}

		public override void StopLoading ()
		{
		}

		static UIImage Render (string value)
		{
			NSString text = new NSString (string.IsNullOrEmpty (value) ? " " : value);
			UIFont font = UIFont.SystemFontOfSize (20);
			CGSize size = text.StringSize (font);
			UIGraphics.BeginImageContextWithOptions (size, false, 0.0f);
			UIColor.Red.SetColor ();
			text.DrawString (new CGPoint (0, 0), font);
			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			
			return image;
		}
	}
}

