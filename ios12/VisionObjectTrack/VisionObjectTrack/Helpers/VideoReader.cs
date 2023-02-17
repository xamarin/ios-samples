
namespace VisionObjectTrack {
	using AVFoundation;
	using CoreGraphics;
	using CoreVideo;
	using Foundation;
	using ImageIO;
	using System;

	/// <summary>
	/// Contains the video reader implementation using AVCapture.
	/// </summary>
	public class VideoReader {
		private const float MillisecondsInSecond = 1000f;

		private AVAsset videoAsset;
		private AVAssetTrack videoTrack;
		private AVAssetReader assetReader;
		private AVAssetReaderTrackOutput videoAssetReaderOutput;

		protected float FrameRateInMilliseconds => this.videoTrack.NominalFrameRate;

		public float FrameRateInSeconds => this.FrameRateInMilliseconds * MillisecondsInSecond;

		public CGAffineTransform AffineTransform => this.videoTrack.PreferredTransform.Invert ();

		public CGImagePropertyOrientation Orientation {
			get {
				var orientation = 1;
				var angleInDegrees = Math.Atan2 (this.AffineTransform.yx, this.AffineTransform.xx) * 180 / Math.PI;
				switch (angleInDegrees) {
				case 0:
					orientation = 1; // Recording button is on the right
					break;

				case 180:
					orientation = 3; // abs(180) degree rotation recording button is on the right
					break;

				case -180:
					orientation = 3; // abs(180) degree rotation recording button is on the right
					break;

				case 90:
					orientation = 8; // 90 degree CW rotation recording button is on the top
					break;

				case -90:
					orientation = 6; // 90 degree CCW rotation recording button is on the bottom
					break;

				default:
					orientation = 1;
					break;
				}

				return (CGImagePropertyOrientation) orientation;
			}
		}

		public static VideoReader Create (AVAsset videoAsset)
		{
			var result = new VideoReader { videoAsset = videoAsset };
			var array = result.videoAsset.TracksWithMediaType (AVMediaType.Video);
			result.videoTrack = array [0];

			if (!result.RestartReading ()) {
				result = null;
			}

			return result;
		}

		public bool RestartReading ()
		{
			var result = false;

			this.assetReader = AVAssetReader.FromAsset (this.videoAsset, out NSError error);
			if (error == null) {
				var settings = new AVVideoSettingsUncompressed { PixelFormatType = CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange };
				this.videoAssetReaderOutput = new AVAssetReaderTrackOutput (this.videoTrack, settings);
				if (this.videoAssetReaderOutput != null) {
					this.videoAssetReaderOutput.AlwaysCopiesSampleData = true;

					if (this.assetReader.CanAddOutput (this.videoAssetReaderOutput)) {
						this.assetReader.AddOutput (this.videoAssetReaderOutput);
						result = this.assetReader.StartReading ();
					}
				}
			} else {
				Console.WriteLine ($"Failed to create AVAssetReader object: {error}");
			}

			return result;
		}

		public CVPixelBuffer NextFrame ()
		{
			CVPixelBuffer result = null;

			var sampleBuffer = this.videoAssetReaderOutput.CopyNextSampleBuffer ();
			if (sampleBuffer != null) {
				result = sampleBuffer.GetImageBuffer () as CVPixelBuffer;
			}

			return result;
		}
	}
}
