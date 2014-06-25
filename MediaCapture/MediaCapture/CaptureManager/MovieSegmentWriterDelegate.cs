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
using System.IO;
using System.Collections.Generic;
using CoreGraphics;
using System.Text;
using Foundation;
using UIKit;
using AVFoundation;
using CoreVideo;
using CoreMedia;

using CoreFoundation;
using System.Runtime.InteropServices;

namespace MediaCapture
{
	public class MovieSegmentWriterDelegate : AVCaptureFileOutputRecordingDelegate 
	{ 	
		#region events
		public EventHandler<MovieSegmentRecordingStartedEventArgs> MovieSegmentRecordingStarted;
		private void onMovieSegmentRecordingStarted( string path )
		{
			if (  MovieSegmentRecordingStarted != null )
			{
				try
				{
					MovieSegmentRecordingStartedEventArgs args = new MovieSegmentRecordingStartedEventArgs();
					args.Path = path;
					MovieSegmentRecordingStarted( this, args );
				}
				catch
				{
				}
			}
		}

		public EventHandler<MovieSegmentRecordingCompleteEventArgs> MovieSegmentRecordingComplete;
		private void onMovieSegmentRecordingComplete( string path, int length, bool errorOccurred )
		{
			if ( MovieSegmentRecordingComplete != null )
			{
				try
				{
					MovieSegmentRecordingCompleteEventArgs args = new MovieSegmentRecordingCompleteEventArgs();	
					args.Path = path;
					args.Length = length;
					args.ErrorOccured = errorOccurred;
					MovieSegmentRecordingComplete(this, args);
				}
				catch
				{
				}
			}
		}
		
		public EventHandler<CaptureErrorEventArgs> CaptureError;
		private void onCaptureError( string errorMessage )
		{
			if ( CaptureError != null )
			{
				try
				{
					CaptureErrorEventArgs args = new CaptureErrorEventArgs();	
					args.ErrorMessage = errorMessage;
					CaptureError(this, args);
				}
				catch
				{
				}
			}
		}
		#endregion
		
		public override void DidStartRecording
		(
			AVCaptureFileOutput captureOutput, 
			NSUrl outputFileUrl, 
			NSObject[] connections
		)
		{
			string path = outputFileUrl.Path;
			onMovieSegmentRecordingStarted( path );
		}

		public override void FinishedRecording
		(
			AVCaptureFileOutput captureOutput, 
			NSUrl outputFileUrl, 
			NSObject[] connections, 
			NSError nsError
		)
		{
			try
			{
				finishedRecordingInternal( captureOutput, outputFileUrl, connections, nsError );
			}
			catch (Exception ex)
			{
				string errorMessage = string.Format( "Exception during movie recording finish handler: {0}", ErrorHandling.GetExceptionDetailedText(ex) );
				onCaptureError( errorMessage );
			}
		}
		
		private void finishedRecordingInternal
		(
			AVCaptureFileOutput captureOutput, 
			NSUrl outputFileUrl, 
			NSObject[] connections, 
			NSError nsError
		)
		{
			if ( nsError != null )
			{
				// we are hoping for an 'error' object that contains three items
				// this error is expected because we have specified a file size limit and the streaming
				// session has filled it up so we need to let the world know by raising an event.
				// note that a correctly recorded file calls the delegate with an error code even
				// though this is not (always) an actual error.  We have to check the code to know for sure.
				var userInfo = nsError.UserInfo;
				
				// handle recording stoppage due to file size limit
				if ( ( userInfo.Keys.Length == 3 ) && ( nsError.Code == -11811 ) )
				{
					var k0 = userInfo.Keys[0];
					var v0 = userInfo.Values[0];
					
					int segmentLength;
					if ( ( k0.ToString() != "AVErrorFileSizeKey" ) || 
						 ( int.TryParse( v0.ToString(), out segmentLength ) == false )  || 
						 ( segmentLength == 0) )
					{
						onMovieSegmentRecordingComplete( null, 0, true );
					}
					else
					{
						string path = outputFileUrl.Path;
						onMovieSegmentRecordingComplete( path, segmentLength, false );
					}
				}
				// handle recording stoppage due to file duration limit
				else if ( ( userInfo.Keys.Length == 3 ) && ( nsError.Code == -11810 ) )
				{
					var k0 = userInfo.Keys[0];
					
					if ( k0.ToString() != "AVErrorTimeKey" )
					{
						onMovieSegmentRecordingComplete( null, 0, true );
					}
					else
					{
						string path = outputFileUrl.Path;
						onMovieSegmentRecordingComplete( path, 0, false );
					}
				}
				else
				{
					// unexpected error code indicates a real error
					string errorMessage = string.Format( "Error during movie recording finish handler: {0}", ErrorHandling.GetNSErrorString(nsError) );
					onCaptureError( errorMessage );
				}
			}
			else
			{
				string path = outputFileUrl.Path;
				if ( File.Exists(path) == true )
				{
					onMovieSegmentRecordingComplete( path, 0, false );
				}
				else
				{
					string errorMessage = string.Format( "file not written: {0}", path );
					onCaptureError( errorMessage );
				}
			}
		}
		
		
		
		
	}
	
		
}

