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
using System.Text;
using System.IO;
using UIKit;
using Foundation;

namespace MediaCapture
{
	public static class Utilities
	{
		private static string debugDirectory = null;
		public static string DebugDirectory
		{
			get
			{
				if ( debugDirectory == null )
				{
					string path = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs" );
					if ( Directory.Exists(path) == false )
					{
						Directory.CreateDirectory( path );
					}
					debugDirectory = path;
				}
				return debugDirectory;
			}
		}
		
		public static string ExceptionLogFilePath
		{
			get
			{
				return Path.Combine( DebugDirectory, "ExceptionLog.txt" );
			}
		}
		
		public static void ShowLastErrorLog()
		{
			if ( File.Exists( ExceptionLogFilePath ) == true )
			{
				string exceptionText = File.ReadAllText( ExceptionLogFilePath );
				File.Delete( ExceptionLogFilePath );
				ShowMessage( "Logged Exception", exceptionText );
			}
		}
		
		public static void ShowMessage( String title, string message )
		{
			using(var alert = new UIAlertView(title, message, null, "OK", null))
			{
				alert.Show();  
			}
		}		
		
		// this utility API walks the stack of inner exceptions and builds a string containing everything known about the exception and then writes
		// it to the log file
		public static void LogUnhandledException( Exception ex)
		{
			StringBuilder sb = new StringBuilder();
			Exception currentException = ex;
			while ( currentException != null )
			{
				sb.AppendFormat("{0}\r\n", currentException.Message);
				sb.AppendFormat("{0}\r\n", currentException.StackTrace);
				sb.AppendFormat("-- Source: {0}\r\n", currentException.Source);
				if ( currentException.TargetSite != null )
				{
					sb.AppendFormat("-- Target Site: {0}\r\n", currentException.TargetSite.Name);
				}
				sb.Append("----------------------\r\n");
				currentException = currentException.InnerException;
			}
			string text = sb.ToString();
			if ( File.Exists(ExceptionLogFilePath) == true )
			{
				File.Delete(ExceptionLogFilePath);
			}
			File.WriteAllText( ExceptionLogFilePath, text );
		}
		
		
	}
}

