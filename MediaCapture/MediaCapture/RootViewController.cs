//
// how to capture still images, video and audio using iOS AVFoundation and the AVCAptureSession
// 
// This sample handles all of the low-level AVFoundation and capture graph setup required to capture and save media.  This code also exposes the
// capture, configuration and notification capabilities in a more '.Netish' way of programming.  The client code will not need to deal with threads, delegate classes
// buffer management, or objective-C data types but instead will create .NET objects and handle standard .NET events.  The underlying iOS concepts and classes are detailed in 
// the iOS developer online help (TP40010188-CH5-SW2).
//
// https://developer.apple.com/library/mac/#documentation/AudioVideo/Conceptual/AVFoundationPG/Articles/04_MediaCapture.html#//apple_ref/doc/uid/TP40010188-CH5-SW2
//
// Enhancements, suggestions and bug reports can be sent to steve.millar@infinitekdev.com
//
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace MediaCapture
{
	public class RootViewController : UINavigationController
	{
		private MediaCaptureViewController mediaCaptureViewController;

		#region events
		public class ViewControllerPoppedEventArgs : EventArgs
		{
			public UIViewController Controller = null;	
		}
		public event EventHandler<ViewControllerPoppedEventArgs> ViewControllerPopped;
		private void onViewControllerPopped( UIViewController controller )
		{
			if ( ViewControllerPopped != null )
			{
				ViewControllerPoppedEventArgs args = new ViewControllerPoppedEventArgs();
				args.Controller = controller;
				ViewControllerPopped( this, args );
			}
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad();
			
			Utilities.ShowLastErrorLog();
			
			mediaCaptureViewController = new MediaCaptureViewController();
			PushViewController( mediaCaptureViewController, true );
		}
		
		public override UIViewController PopViewController (bool animated)
		{
			UIViewController controller = base.PopViewController (animated);
			onViewControllerPopped( controller );
			return controller;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		
	}
}

