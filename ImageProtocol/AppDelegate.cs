// 
// AppDelegate.cs
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
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UIWebView web;
		UIViewController controller;

		public override bool FinishedLaunching (UIApplication application, NSDictionary options)
		{
			// Register our custom url protocol
			NSUrlProtocol.RegisterClass (new ObjCRuntime.Class (typeof (ImageProtocol)));

			controller = new UIViewController ();

			web = new UIWebView () {
				BackgroundColor = UIColor.White,
				ScalesPageToFit = true,
				AutoresizingMask = UIViewAutoresizing.All
			};
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0))
				web.Frame = new CGRect (0, 20, 
				                            UIScreen.MainScreen.Bounds.Width,
				                            UIScreen.MainScreen.Bounds.Height - 20);
			else
				web.Frame = UIScreen.MainScreen.Bounds;
			controller.NavigationItem.Title = "Test case";

			controller.View.AutosizesSubviews = true;
			controller.View.AddSubview (web);
			
			web.LoadRequest (NSUrlRequest.FromUrl (NSUrl.FromFilename (NSBundle.MainBundle.PathForResource ("test", "html"))));

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.BackgroundColor = UIColor.White;
			window.MakeKeyAndVisible ();
			window.RootViewController = controller;

			return true;
		}
	}
}

