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
using CoreGraphics;

namespace PhotoFilterExtension
{
	public class AVReaderWriter
	{
		readonly IVideoTransformer transformer;
		readonly AVAsset asset;

		CMTimeRange timeRange;
		NSUrl outputURL;

		Action<NSError> completionProc;

		CancellationTokenSource cancellationTokenSrc;

		AVAssetReader assetReader;
		AVAssetWriter assetWriter;
		ReadWriteSampleBufferChannel audioSampleBufferChannel;
		ReadWriteSampleBufferChannel videoSampleBufferChannel;

		public AVReaderWriter (AVAsset asset, IVideoTransformer transformer)
		{
			if (asset == null)
				throw new ArgumentNullException ("asset");
			if (transformer == null)
				throw new ArgumentNullException ("transformer");

			this.asset = asset;
			this.transformer = transformer;
			cancellationTokenSrc = new CancellationTokenSource ();
		}

		public void WriteToUrl(NSUrl localOutputURL, Action<NSError> completion)
		{
			outputURL = localOutputURL;

			AVAsset localAsset = asset;

			completionProc = completion;

			// Dispatch the setup work with cancellationTokenSrc, to ensure this work can be cancelled
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
			audioSampleBufferChannel = new ReadWriteSampleBufferChannel (output, new AudioWriter(input));
		}

		private void SetupAssetReaserWriterForVideo (AVAssetTrack videoTrack)
		{
			if (videoTrack == null)
				return;

			// Decompress to ARGB with the asset reader
			var decompSettings = new AVVideoSettingsUncompressed {
				PixelFormatType = CVPixelFormatType.CV32BGRA,
				AllocateWithIOSurface = null
			};
			AVAssetReaderOutput output = new AVAssetReaderTrackOutput(videoTrack, decompSettings);
			assetReader.AddOutput (output);

			// Get the format description of the track, to fill in attributes of the video stream that we don't want to change
			var formatDescription = (CMVideoFormatDescription)videoTrack.FormatDescriptions.FirstOrDefault ();
			// Grab track dimensions from format description
			CGSize trackDimensions = formatDescription != null
				? formatDescription.GetPresentationDimensions(false, false)
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
			AVAssetWriterInput input = new AVAssetWriterInput (videoTrack.MediaType, videoSettings);
			input.Transform = videoTrack.PreferredTransform;
			assetWriter.AddInput (input);

			// Create and save an instance of ReadWriteSampleBufferChannel,
			// which will coordinate the work of reading and writing sample buffers
			videoSampleBufferChannel = new ReadWriteSampleBufferChannel (output, new VideoWriter (input, transformer));
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

			Task audioTask = StartReadingAsync (audioSampleBufferChannel);
			Task videoTask = StartReadingAsync (videoSampleBufferChannel);

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

		private Task StartReadingAsync(ReadWriteSampleBufferChannel channel)
		{
			if (channel == null)
				return Task.FromResult<object> (null);
			else
				return channel.StartTransformationAsync ();
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
	}
}