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

		public void WriteToUrl (NSUrl localOutputURL, Action<NSError> completion)
		{
			outputURL = localOutputURL;
			completionProc = completion;

			// Dispatch the setup work with cancellationTokenSrc, to ensure this work can be cancelled
			asset.LoadValuesTaskAsync (new string[] { "tracks", "duration" }).ContinueWith (_ => {
				// Since we are doing these things asynchronously, the user may have already cancelled on the main thread.
				// In that case, simply return from this block
				cancellationTokenSrc.Token.ThrowIfCancellationRequested ();

				NSError localError = null;

				if(asset.StatusOfValue ("tracks", out localError) != AVKeyValueStatus.Loaded)
					throw new NSErrorException (localError);

				if(asset.StatusOfValue ("duration", out localError) != AVKeyValueStatus.Loaded)
					throw new NSErrorException (localError);

				var timeRange = new CMTimeRange {
					Start = CMTime.Zero,
					Duration = asset.Duration
				};

				// AVAssetWriter does not overwrite files for us, so remove the destination file if it already exists
				if (File.Exists (localOutputURL.Path))
					File.Delete (localOutputURL.Path);

				// Set up the AVAssetReader and AVAssetWriter, then begin writing samples or flag an error
				SetupReaderAndWriter ();
				StartReadingAndWriting (timeRange);
			}, cancellationTokenSrc.Token).ContinueWith (prevTask => {
				switch (prevTask.Status) {
					case TaskStatus.Canceled:
						ReadingAndWritingDidFinish (false, null);
						break;

					case TaskStatus.Faulted:
						ReadingAndWritingDidFinish (false, ((NSErrorException)prevTask.Exception.InnerException).Error);
						break;

					default:
						break;
				}
			});
		}

		void SetupReaderAndWriter ()
		{
			NSError error = null;

			// Create asset reader and asset writer
			assetReader = AVAssetReader.FromAsset (asset, out error);
			if (assetReader == null)
				throw new NSErrorException (error);

			assetWriter = AVAssetWriter.FromUrl (outputURL, AVFileType.QuickTimeMovie, out error);
			if (assetWriter == null)
				throw new NSErrorException (error);

			// Create asset reader outputs and asset writer inputs for the first audio track and first video track of the asset
			// Grab first audio track and first video track, if the asset has them
			AVAssetTrack audioTrack = asset.TracksWithMediaType (AVMediaType.Audio).FirstOrDefault ();
			AVAssetTrack videoTrack = asset.TracksWithMediaType (AVMediaType.Video).FirstOrDefault ();

			SetupAssetReaderWriterForAudio (audioTrack);
			SetupAssetReaserWriterForVideo (videoTrack);
		}

		void SetupAssetReaderWriterForAudio (AVAssetTrack audioTrack)
		{
			if (audioTrack == null)
				return;

			// Decompress to Linear PCM with the asset reader
			// To read the media data from a specific asset track in the format in which it was stored, pass null to the settings parameter.
			AVAssetReaderOutput output = AVAssetReaderTrackOutput.Create (audioTrack, (AudioSettings)null);
			if (assetReader.CanAddOutput (output))
				assetReader.AddOutput (output);

			AVAssetWriterInput input = AVAssetWriterInput.Create (audioTrack.MediaType, (AudioSettings)null);
			if (assetWriter.CanAddInput (input))
				assetWriter.AddInput (input);

			// Create and save an instance of ReadWriteSampleBufferChannel,
			// which will coordinate the work of reading and writing sample buffers
			audioSampleBufferChannel = new AudioChannel(output, input);
		}

		void SetupAssetReaserWriterForVideo (AVAssetTrack videoTrack)
		{
			if (videoTrack == null)
				return;

			// Decompress to ARGB with the asset reader
			var decompSettings = new AVVideoSettingsUncompressed {
				PixelFormatType = CVPixelFormatType.CV32BGRA,
				AllocateWithIOSurface = null
			};
			AVAssetReaderOutput output = new AVAssetReaderTrackOutput (videoTrack, decompSettings);
			assetReader.AddOutput (output);

			// Get the format description of the track, to fill in attributes of the video stream that we don't want to change
			var formatDescription = (CMVideoFormatDescription)videoTrack.FormatDescriptions.FirstOrDefault ();
			// Grab track dimensions from format description
			CGSize trackDimensions = formatDescription != null
				? formatDescription.GetPresentationDimensions (false, false)
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
			videoSampleBufferChannel = new VideoChannel (output, input, transformer);
		}

		AVVideoCodecSettings CreateCodecSettingsFor (NSDictionary cleanAperture, NSDictionary aspectRatio)
		{
			if (cleanAperture == null && aspectRatio == null)
				return null;

			var compressionSettings = new AVVideoCodecSettings {
				VideoCleanAperture = cleanAperture != null ? new AVVideoCleanApertureSettings (cleanAperture) : null,
				PixelAspectRatio = aspectRatio != null ? new AVVideoPixelAspectRatioSettings (aspectRatio) : null
			};

			return compressionSettings;
		}

		void StartReadingAndWriting (CMTimeRange timeRange)
		{
			// Instruct the asset reader and asset writer to get ready to do work
			if (!assetReader.StartReading ())
				throw new NSErrorException (assetReader.Error);

			if (!assetWriter.StartWriting ())
				throw new NSErrorException (assetWriter.Error);

			// Start a sample-writing session
			assetWriter.StartSessionAtSourceTime (timeRange.Start);

			Task audioTask = Start (audioSampleBufferChannel);
			Task videoTask = Start (videoSampleBufferChannel);

			// Set up a callback for when the sample writing is finished
			Task.WhenAll (audioTask, videoTask).ContinueWith (_ => {
				if (cancellationTokenSrc.Token.IsCancellationRequested) {
					assetReader.CancelReading ();
					assetWriter.CancelWriting ();
					throw new OperationCanceledException ();
				}

				if (assetReader.Status != AVAssetReaderStatus.Failed) {
					assetWriter.FinishWriting (() => {
						bool success = assetWriter.Status == AVAssetWriterStatus.Completed;
						ReadingAndWritingDidFinish (success, assetWriter.Error);
					});
				}
			}, cancellationTokenSrc.Token);
		}

		Task Start (ReadWriteSampleBufferChannel channel)
		{
			if (channel == null)
				return Task.FromResult<object> (null);
			else
				return channel.StartAsync ();
		}

		void ReadingAndWritingDidFinish (bool success, NSError error)
		{
			if (!success) {
				assetReader.CancelReading ();
				assetWriter.CancelWriting ();
			}

			// Tear down
			assetReader.Dispose ();
			assetReader = null;

			assetWriter.Dispose ();
			assetWriter = null;

			audioSampleBufferChannel = null;
			videoSampleBufferChannel = null;
			cancellationTokenSrc = null;

			completionProc (error);
		}
	}
}
