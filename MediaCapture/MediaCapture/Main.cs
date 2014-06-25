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
	public class Application
	{
		static void Main (string[] args)
		{
			try
			{
				// register an event handler for unhandled exceptions.  This little trick handles cases where something has gone so wrong that
				// you can't even show an error dialog.
				AppDomain.CurrentDomain.UnhandledException += logUnhandledException;
				UIApplication.Main (args, null, "AppDelegate");
			}
			catch (Exception ex)
			{
				Utilities.LogUnhandledException(ex);
			}
		}

		static void logUnhandledException (object sender, UnhandledExceptionEventArgs e)
		{
			// log the unhandled exception.  It will be shown to the user the next time the app runs.
			Utilities.LogUnhandledException(e.ExceptionObject as Exception);
		}
		
		
		
	}
}
