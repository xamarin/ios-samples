using System;
using System.Collections.Generic;
using System.Threading;

using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using MobileCoreServices;
using ObjCRuntime;
using UIKit;
using VideoToolbox;

namespace VideoTimeLine
{
	public partial class ViewController : UIViewController, IUIImagePickerControllerDelegate, IUIPopoverControllerDelegate, IUINavigationControllerDelegate 
	{
		DispatchQueue backgroundQueue;

		List<CVPixelBuffer> outputFrames;
		List<double> presentationTimes;

		double lastCallbackTime;
		CADisplayLink displayLink;
		SemaphoreSlim bufferSemaphore;
		Object thisLock;
		CGAffineTransform videoPreferredTransform;
		AVAssetReader assetReader;
		UIPopoverController popover;
		VTDecompressionSession decompressionSession;

		public ViewController (IntPtr handle) : base (handle)
		{
			thisLock = new Object ();
			presentationTimes = new List<double> ();
			outputFrames = new List<CVPixelBuffer> ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			backgroundQueue = new DispatchQueue ("com.videotimeline.backgroundqueue", false);
			displayLink = CADisplayLink.Create (DisplayLinkCallback);
			displayLink.AddToRunLoop (NSRunLoop.Current, NSRunLoopMode.Default);
			displayLink.Paused = true;
			lastCallbackTime = 0.0;
			bufferSemaphore = new SemaphoreSlim (0);
		}

		public void DisplayLinkCallback ()
		{
			if (lastCallbackTime == 0.0)
				lastCallbackTime = displayLink.Timestamp;

			double timeSinceLastCallback = displayLink.Timestamp - lastCallbackTime;

			if (outputFrames.Count > 0 && presentationTimes.Count > 0) {
				CVPixelBuffer pixelBuffer = null;
				double framePTS;

				lock (thisLock) {
					framePTS = presentationTimes [0];
					pixelBuffer = outputFrames [0];
				}

				if (timeSinceLastCallback >= framePTS) {
					lock (thisLock) {
						if (pixelBuffer != null)
							outputFrames.RemoveAt (0);

						presentationTimes.RemoveAt (0);

						if (presentationTimes.Count == 3)
							bufferSemaphore.Release ();
					}
				}

				if (pixelBuffer != null) {
					DisplayPixelBuffer (pixelBuffer, framePTS);
					MoveTimeLine ();
				}
			}
		}

		partial void ChooseVideoTapped (UIBarButtonItem sender)
		{
			var videoPicker = new UIImagePickerController {
				ModalPresentationStyle = UIModalPresentationStyle.CurrentContext,
				SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum,
				MediaTypes = new string[] { UTType.Movie }
			};
			videoPicker.FinishedPickingMedia += (object s, UIImagePickerMediaPickedEventArgs e) => {
				displayLink.Paused = true;
				playButton.Title = "Play";
				popover.Dismiss (true);

				outputFrames.Clear ();
				presentationTimes.Clear ();

				lastCallbackTime = 0.0;
				var asset = AVAsset.FromUrl (e.MediaUrl);

				if (assetReader != null && assetReader.Status == AVAssetReaderStatus.Reading) {
					bufferSemaphore.Release ();
					assetReader.CancelReading ();
				}

				backgroundQueue.DispatchAsync (() => ReadSampleBuffers (asset));
			};
			videoPicker.Canceled += (object s, EventArgs e) => DismissViewController (true, null);

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				popover = new UIPopoverController (videoPicker);
				popover.PresentFromBarButtonItem (sender, UIPopoverArrowDirection.Down, true);
			}
		}

		partial void PlayButtonTapped (UIBarButtonItem sender)
		{
			if (!displayLink.Paused) {
				displayLink.Paused = true;
				sender.Title = "Play";
			} else {
				displayLink.Paused = false;
				sender.Title = "Pause";
			}
		}

		void DisplayPixelBuffer (CVPixelBuffer pixelBuffer, double framePTS)
		{
			nint width = pixelBuffer.Width;
			nint height = pixelBuffer.Height;
			var f = View.Frame;
			if (width > f.Width || height > f.Height) {
				width /= 2;
				height /= 2;
			}

			var layer = new EAGLLayer ();
			if (videoPreferredTransform.xx == -1f)
				layer.AffineTransform.Rotate (NMath.PI);
			else if (videoPreferredTransform.yy == 0f)
				layer.AffineTransform.Rotate (NMath.PI / 2);

			layer.Frame = new CGRect (0f, View.Frame.Height - 50f - height, width, height);
			layer.PresentationRect = new CGSize (width, height);

			layer.TimeCode = framePTS.ToString ("0.000");
			layer.SetupGL ();

			View.Layer.AddSublayer (layer);
			layer.DisplayPixelBuffer (pixelBuffer);
		}

		void MoveTimeLine ()
		{
			var layersForRemoval = new List<CALayer> ();
			foreach (CALayer layer in View.Layer.Sublayers) {
				if (layer is EAGLLayer) {
					CGRect frame = layer.Frame;
					var newFrame = new CGRect (frame.Location.X + 20f, frame.Location.Y - 20f, frame.Width, frame.Height);
					layer.Frame = newFrame;
					CGRect screenBounds = UIScreen.MainScreen.Bounds;

					if ((newFrame.Location.X >= screenBounds.Location.X + screenBounds.Width) ||
						newFrame.Location.Y >= (screenBounds.Location.Y + screenBounds.Height)) {
						layersForRemoval.Add (layer);
					}
				}
			}

			foreach (var layer in layersForRemoval) {
				layer.RemoveFromSuperLayer ();
				layer.Dispose ();
			}
		}

		void ReadSampleBuffers (AVAsset asset)
		{
			NSError error;
			assetReader = AVAssetReader.FromAsset (asset, out error);

			if (error != null)
				Console.WriteLine ("Error creating Asset Reader: {0}", error.Description);

			AVAssetTrack[] videoTracks = asset.TracksWithMediaType (AVMediaType.Video);
			AVAssetTrack videoTrack = videoTracks [0];
			CreateDecompressionSession (videoTrack);
			var videoTrackOutput = AVAssetReaderTrackOutput.Create (videoTrack, (AVVideoSettingsUncompressed)null);

			if (assetReader.CanAddOutput (videoTrackOutput))
				assetReader.AddOutput (videoTrackOutput);

			if (!assetReader.StartReading ())
				return;

			while (assetReader.Status == AVAssetReaderStatus.Reading) {
				CMSampleBuffer sampleBuffer = videoTrackOutput.CopyNextSampleBuffer ();
				if (sampleBuffer != null) {
					VTDecodeFrameFlags flags = VTDecodeFrameFlags.EnableAsynchronousDecompression;
					VTDecodeInfoFlags flagOut;
					decompressionSession.DecodeFrame (sampleBuffer, flags, IntPtr.Zero, out flagOut);

					sampleBuffer.Dispose ();
					if (presentationTimes.Count >= 5)
						bufferSemaphore.Wait ();

				} else if (assetReader.Status == AVAssetReaderStatus.Failed) {
					Console.WriteLine ("Asset Reader failed with error: {0}", assetReader.Error.Description);
				} else if (assetReader.Status == AVAssetReaderStatus.Completed) {
					Console.WriteLine ("Reached the end of the video.");
				}
			}
		}

		void CreateDecompressionSession (AVAssetTrack videoTrack)
		{
			CMFormatDescription[] formatDescriptions = videoTrack.FormatDescriptions;
			var formatDescription = (CMVideoFormatDescription)formatDescriptions [0];
			videoPreferredTransform = videoTrack.PreferredTransform;
			decompressionSession = VTDecompressionSession.Create (DidDecompress, formatDescription);
		}

		void DidDecompress (IntPtr sourceFrame, VTStatus status, VTDecodeInfoFlags flags, CVImageBuffer buffer, CMTime presentationTimeStamp, CMTime presentationDuration)
		{
			if (status != VTStatus.Ok) {
				Console.WriteLine ("Error decompresssing frame at time: {0:#.###} error: {1} infoFlags: {2}", 
					(float)presentationTimeStamp.Value / presentationTimeStamp.TimeScale, (int)status, flags);
				return;
			}

			if (buffer == null)
				return;

			// Find the correct position for this frame in the output frames array
			if (presentationTimeStamp.IsInvalid) {
				Console.WriteLine ("Not a valid time for image buffer");
				return;
			}

			var framePTS = presentationTimeStamp.Seconds;

			lock (thisLock) {
				// since we want to keep the managed `pixelBuffer` alive outside the execution 
				// of the callback we need to create our own (managed) instance from the handle
				var pixelBuffer = Runtime.GetINativeObject<CVPixelBuffer> (buffer.Handle, false);

				int insertionIndex = presentationTimes.Count - 1;
				while (insertionIndex >= 0) {
					var aNumber = presentationTimes [insertionIndex];
					if (aNumber <= framePTS)
						break;
					insertionIndex--;
				}

				if (insertionIndex + 1 == presentationTimes.Count) {
					presentationTimes.Add (framePTS);
					outputFrames.Add (pixelBuffer);
				} else {
					presentationTimes.Insert (insertionIndex + 1, framePTS);
					outputFrames.Insert (insertionIndex + 1, pixelBuffer);
				}
			}
		}
	}
}