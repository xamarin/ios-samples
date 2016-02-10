using System;
using System.IO;

using Foundation;
using AVFoundation;

namespace MediaCapture {
	public class MovieSegmentWriterDelegate : AVCaptureFileOutputRecordingDelegate {
		#region events
		public EventHandler<MovieSegmentRecordingStartedEventArgs> MovieSegmentRecordingStarted;

		void OnMovieSegmentRecordingStarted (string path)
		{
			if (MovieSegmentRecordingStarted != null) {
				try {
					var args = new MovieSegmentRecordingStartedEventArgs {
						Path = path
					};
					MovieSegmentRecordingStarted (this, args);
				} catch (Exception e) {
					Console.WriteLine (e.Message);
				}
			}
		}

		public EventHandler<MovieSegmentRecordingCompleteEventArgs> MovieSegmentRecordingComplete;
		void OnMovieSegmentRecordingComplete (string path, int length, bool errorOccurred)
		{
			if ( MovieSegmentRecordingComplete != null ) {
				try {
					var args = new MovieSegmentRecordingCompleteEventArgs {
						Path = path,
						Length = length,
						ErrorOccured = errorOccurred
					};

					MovieSegmentRecordingComplete (this, args);
				} catch (Exception e) {
					Console.WriteLine (e.Message);
				}
			}
		}

		public EventHandler<CaptureErrorEventArgs> CaptureError;
		void OnCaptureError( string errorMessage )
		{
			if (CaptureError != null) {
				try {
					var args = new CaptureErrorEventArgs {
						ErrorMessage = errorMessage
					};
					CaptureError (this, args);
				} catch (Exception e) {
					Console.WriteLine (e.Message);
				}
			}
		}
		#endregion

		public override void DidStartRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections)
		{
			string path = outputFileUrl.Path;
			OnMovieSegmentRecordingStarted (path);
		}

		public override void FinishedRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			try {
				FinishedRecordingInternal (captureOutput, outputFileUrl, connections, error);
			} catch (Exception ex) {
				string errorMessage = string.Format ("Exception during movie recording finish handler: {0}", ErrorHandling.GetExceptionDetailedText (ex) );
				OnCaptureError (errorMessage);
			}
		}

		void FinishedRecordingInternal (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError nsError)
		{
			if ( nsError != null ) {
				// we are hoping for an 'error' object that contains three items
				// this error is expected because we have specified a file size limit and the streaming
				// session has filled it up so we need to let the world know by raising an event.
				// note that a correctly recorded file calls the delegate with an error code even
				// though this is not (always) an actual error.  We have to check the code to know for sure.
				var userInfo = nsError.UserInfo;

				// handle recording stoppage due to file size limit
				if ((userInfo.Keys.Length == 3) && (nsError.Code == -11811)) {
					var k0 = userInfo.Keys[0];
					var v0 = userInfo.Values[0];

					int segmentLength;
					if (k0.ToString () != "AVErrorFileSizeKey" ||
						(int.TryParse (v0.ToString (), out segmentLength) == false ) ||
						segmentLength == 0) {
						OnMovieSegmentRecordingComplete (null, 0, true);
					} else {
						string path = outputFileUrl.Path;
						OnMovieSegmentRecordingComplete (path, segmentLength, false);
					}
				} else if (userInfo.Keys.Length == 3 && nsError.Code == -11810) {
					var k0 = userInfo.Keys[0];

					if (k0.ToString () != "AVErrorTimeKey") {
						OnMovieSegmentRecordingComplete (null, 0, true);
					} else {
						string path = outputFileUrl.Path;
						OnMovieSegmentRecordingComplete (path, 0, false);
					}
				} else {
					// unexpected error code indicates a real error
					OnCaptureError ($"\"Error during movie recording finish handler: {ErrorHandling.GetNSErrorString (nsError)}");
				}
			} else {
				string path = outputFileUrl.Path;
				if ( File.Exists(path) == true ) {
					OnMovieSegmentRecordingComplete( path, 0, false );
				} else {
					OnCaptureError ($"file not written: {path}");
				}
			}
		}
	}

}

