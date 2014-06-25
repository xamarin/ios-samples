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
using Foundation;
using System.Text;

namespace MediaCapture
{
	public static class ErrorHandling
	{
		public static string GetExceptionDetailedText( Exception ex )
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
			return sb.ToString();

		}
		
		public static string GetNSErrorString (NSError nsError)
		{
			if ( nsError == null )
			{
				return "No Error Info Available";
			}
			
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Error Code:  {0}\r\n", nsError.Code.ToString());
				sb.AppendFormat("Description: {0}\r\n", nsError.LocalizedDescription);
				var userInfo = nsError.UserInfo;
				for ( int i = 0; i < userInfo.Keys.Length; i++ )
				{
					sb.AppendFormat("[{0}]: {1}\r\n", userInfo.Keys[i].ToString(), userInfo.Values[i].ToString() );
				}
				return sb.ToString();
			}
			catch
			{
				return "Error parsing NSError object. Ironic, is it not ?";		
			}
		}
		
	}
}

