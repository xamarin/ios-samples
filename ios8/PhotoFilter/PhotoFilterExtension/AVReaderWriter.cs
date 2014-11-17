using System;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using AVFoundation;
using Foundation;
using CoreMedia;
using CoreVideo;
using CoreFoundation;

namespace PhotoFilterExtension
{
	public class AVReaderWriter
	{
		// In production – your should extract interface from PhotoEditingViewController (ex IAVReaderWriterDelegate)
		public PhotoEditingViewController Delegate { get; set; }

		AVAsset asset;
		CMTimeRange timeRange;
		NSUrl outputURL;

		Action<float> progressProc;
		Action<NSError> completionProc;

		CancellationTokenSource cancellationTokenSrc;

		AVAssetReader assetReader;
		AVAssetWriter assetWriter;
		ReadWriteSampleBufferChannel audioSampleBufferChannel;
		ReadWriteSampleBufferChannel videoSampleBufferChannel;

		public AVReaderWriter (AVAsset asset)
		{
			this.asset = asset;
			cancellationTokenSrc = new CancellationTokenSource ();
		}

		public void WriteToUrl(NSUrl localOutputURL, Action<float> progress, Action<NSError> completion)
		{
			outputURL = localOutputURL;

			AVAsset localAsset = asset;

			completionProc = completion;
			progressProc = progress;

			// Dispatch the setup work with _cancellationTokenSrc, to ensure this work can be cancelled
			localAsset.LoadValuesTaskAsync (new string[] { "tracks", "duration" }).ContinueWith(_ => {
				// Since we are doing these things asynchronously, the user may have already cancelled on the main thread.
				// In that case, simply return from this block
				cancellationTokenSrc.Token.ThrowIfCancellationRequested();

				bool success = true;
				NSError localError = null;

				success = localAsset.StatusOfValue("tracks", out localError) == AVKeyValueStatus.Loaded &&
				          localAsset.StatusOfValue("duration", out localError) == AVKeyValueStatus.Loaded;

				if(!success)
					throw new NSErrorException(localError);

				timeRange = new CMTimeRange {
					Start = CMTime.Zero,
					Duration = localAsset.Duration
				};

				// AVAssetWriter does not overwrite files for us, so remove the destination file if it already exists
				if (File.Exists(localOutputURL.Path))
					File.Delete(localOutputURL.Path);

				// Set up the AVAssetReader and AVAssetWriter, then begin writing samples or flag an error
				SetupReaderAndWriter();
				StartReadingAndWriting();

				return localError;
			}, cancellationTokenSrc.Token).ContinueWith(prevTask => {
				switch(prevTask.Status) {
					case TaskStatus.Canceled:
						ReadingAndWritingDidFinish(false, null);
						break;

					case TaskStatus.Faulted:
						ReadingAndWritingDidFinish(false, ((NSErrorException)prevTask.Exception.InnerException).Error);
						break;

					default:
						break;
				}
			});
		}

		private void SetupReaderAndWriter()
		{
			AVAsset localAsset = asset;
			NSUrl localOutputURL = outputURL;
			NSError error = null;

			// Create asset reader and asset writer
			assetReader = new AVAssetReader (localAsset, out error);
			if (assetReader == null)
				throw new NSErrorException(error);

			assetWriter = new AVAssetWriter (localOutputURL, AVFileType.QuickTimeMovie, out error);
			if (assetWriter == null)
				throw new NSErrorException(error);

			// Create asset reader outputs and asset writer inputs for the first audio track and first video track of the asset
			// Grab first audio track and first video track, if the asset has them
			AVAssetTrack audioTrack = localAsset.TracksWithMediaType (AVMediaType.Audio).FirstOrDefault ();
			AVAssetTrack videoTrack = localAsset.TracksWithMediaType (AVMediaType.Video).FirstOrDefault ();

			SetupAssetReaderWriterForAudio (audioTrack);
			SetupAssetReaserWriterForVideo (videoTrack);
		}

		private void SetupAssetReaderWriterForAudio(AVAssetTrack audioTrack)
		{
			if (audioTrack == null)
				return;

			// Decompress to Linear PCM with the asset reader
			AVAssetReaderOutput output = AVAssetReaderTrackOutput.Create (audioTrack, (AudioSettings)null);
			assetReader.AddOutput (output);

			AVAssetWriterInput input = AVAssetWriterInput.Create (audioTrack.MediaType, (AudioSettings)null);
			assetWriter.AddInput (input);

			// Create and save an instance of ReadWriteSampleBufferChannel,
			// which will coordinate the work of reading and writing sample buffers
			audioSampleBufferChannel = new ReadWriteSampleBufferChannel (output, input, false);
		}

		private void SetupAssetReaserWriterForVideo (AVAssetTrack videoTrack)
		{
			throw new NotImplementedException("64 bit");
			/*
			if (videoTrack == null)
				return;

			// Decompress to ARGB with the asset reader
			var decompSettings = new AVVideoSettingsUncompressed {
				PixelFormatType = CVPixelFormatType.CV32BGRA,
				AllocateWithIOSurface = null
			};
			AVAssetReaderOutput output = new AVAssetReaderTrackOutput(videoTrack, decompSettings);
			_assetReader.AddOutput (output);

			// Get the format description of the track, to fill in attributes of the video stream that we don't want to change
			CMFormatDescription formatDescription = videoTrack.FormatDescriptions.FirstOrDefault ();

			// Grab track dimensions from format description
			SizeF trackDimensions = formatDescription != null
				? formatDescription.GetVideoPresentationDimensions (false, false)
				: videoTrack.NaturalSize;

			// Grab clean aperture, pixel aspect ratio from format description
			AVVideoCodecSettings compressionSettings = null;
			if (formatDescription != null) {
				var cleanApertureDescr = (NSDictionary)formatDescription.GetExtension (CVImageBuffer.CleanApertureKey);
				var pixelAspectRatioDescr = (NSDictionary)formatDescription.GetExtension (CVImageBuffer.PixelAspectRatioKey);
				compressionSettings = CreateCodecSettingsFor (cleanApertureDescr, pixelAspectRatioDescr);
			}

			// Compress to H.264 with the asset writer
			var videoSettings = new AVVideoSettingsCompressed {
				Codec = AVVideoCodec.H264,
				Width = (int)trackDimensions.Width,
				Height = (int)trackDimensions.Height,
				CodecSettings = compressionSettings
			};
			AVAssetWriterInput input = AVAssetWriterInput.FromType (videoTrack.MediaType, videoSettings.Dictionary);
			input.Transform = videoTrack.PreferredTransform;
			_assetWriter.AddInput (input);

			// Create and save an instance of ReadWriteSampleBufferChannel,
			// which will coordinate the work of reading and writing sample buffers
			_videoSampleBufferChannel = new ReadWriteSampleBufferChannel (output, input, true);
			*/
		}

		private AVVideoCodecSettings CreateCodecSettingsFor(NSDictionary cleanAperture, NSDictionary aspectRatio)
		{
			if (cleanAperture == null && aspectRatio == null)
				return null;

			var compressionSettings = new AVVideoCodecSettings
			{
				VideoCleanAperture = cleanAperture != null ? new AVVideoCleanApertureSettings (cleanAperture) : null,
				PixelAspectRatio = aspectRatio != null ? new AVVideoPixelAspectRatioSettings (aspectRatio): null
			};

			return compressionSettings;
		}

		private void StartReadingAndWriting()
		{
			// Instruct the asset reader and asset writer to get ready to do work
			if (!assetReader.StartReading ())
				throw new NSErrorException (assetReader.Error);

			if (!assetWriter.StartWriting())
				throw new NSErrorException (assetWriter.Error);

			// Start a sample-writing session
			assetWriter.StartSessionAtSourceTime (timeRange.Start);

			// Only set audio handler(obj-c delegate) for audio-only assets, else let the video channel drive progress
			AVReaderWriter audioHandler = videoSampleBufferChannel == null ? this : null;
			var audioTask = StartReadingAsync (audioSampleBufferChannel, audioHandler);
			var videoTask = StartReadingAsync (videoSampleBufferChannel, this);

			// Set up a callback for when the sample writing is finished
			Task.WhenAll (audioTask, videoTask).ContinueWith (_ => {
				if (cancellationTokenSrc.Token.IsCancellationRequested) {
					assetReader.CancelReading ();
					assetWriter.CancelWriting ();
					throw new OperationCanceledException();
				}

				if (assetReader.Status != AVAssetReaderStatus.Failed) {
					assetWriter.FinishWriting (() => {
						bool success = assetWriter.Status == AVAssetWriterStatus.Completed;
						ReadingAndWritingDidFinish (success, assetWriter.Error);
					});
				}
			}, cancellationTokenSrc.Token);
		}

		// TODO: where called in original sample
		// - (void)cancel:(id)sender

		private Task StartReadingAsync(ReadWriteSampleBufferChannel channel, AVReaderWriter handler)
		{
			var completionSrc = new TaskCompletionSource<object> ();

			if (channel == null)
				completionSrc.SetResult (null);
			else
				channel.StartWithAsync (completionSrc, handler);

			return completionSrc.Task;
		}

		private void ReadingAndWritingDidFinish(bool success, NSError error)
		{
			if (!success)
			{
				assetReader.CancelReading ();
				assetWriter.CancelWriting ();
			}

			// Tear down ivars
			assetReader.Dispose ();
			assetReader = null;

			assetWriter.Dispose ();
			assetWriter = null;

			audioSampleBufferChannel = null;
			videoSampleBufferChannel = null;
			cancellationTokenSrc = null;

			completionProc(error);
		}

		public void DidReadSampleBuffer (ReadWriteSampleBufferChannel sampleBufferChannel, CMSampleBuffer sampleBuffer)
		{
			// Calculate progress (scale of 0.0 to 1.0)
			double progress = AVReaderWriter.ProgressOfSampleBufferInTimeRange(sampleBuffer, timeRange);
			progressProc((float)progress * 100);

			// Grab the pixel buffer from the sample buffer, if possible
			CVImageBuffer imageBuffer = sampleBuffer.GetImageBuffer ();

			var pixelBuffer = imageBuffer as CVPixelBuffer;
			if (pixelBuffer != null)
				Delegate.AdjustPixelBuffer (pixelBuffer, null); // TODO: problem in original sample. No method
		}

		public void DidReadAndWriteSampleBuffer (ReadWriteSampleBufferChannel sampleBufferChannel,
			CMSampleBuffer sampleBuffer,
			CVPixelBuffer sampleBufferForWrite)
		{
			// Calculate progress (scale of 0.0 to 1.0)
			double progress = AVReaderWriter.ProgressOfSampleBufferInTimeRange(sampleBuffer, timeRange);
			progressProc((float)progress * 100);

			// Grab the pixel buffer from the sample buffer, if possible
			CVImageBuffer imageBuffer = sampleBuffer.GetImageBuffer ();
			var pixelBuffer = imageBuffer as CVPixelBuffer;

			if (pixelBuffer != null)
				Delegate.AdjustPixelBuffer (pixelBuffer, sampleBufferForWrite);
		}

		private static double ProgressOfSampleBufferInTimeRange(CMSampleBuffer sampleBuffer, CMTimeRange timeRange)
		{
			CMTime progressTime = sampleBuffer.PresentationTimeStamp;
			progressTime = progressTime - timeRange.Start;
			CMTime sampleDuration = sampleBuffer.Duration;
			if (sampleDuration.IsNumeric)
				progressTime = progressTime + sampleDuration;
			return progressTime.Seconds / timeRange.Duration.Seconds;
		}
	}
}

