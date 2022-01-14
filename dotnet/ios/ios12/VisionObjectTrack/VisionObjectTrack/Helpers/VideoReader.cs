
namespace VisionObjectTrack;

/// <summary>
/// Contains the video reader implementation using AVCapture.
/// </summary>
public class VideoReader
{
        const float MillisecondsInSecond = 1000f;

        AVAsset? videoAsset;
        AVAssetTrack? videoTrack;
        AVAssetReader? assetReader;
        AVAssetReaderTrackOutput? videoAssetReaderOutput;
        static NSString videoDeviceName = AVMediaTypes.Video.GetConstant ()!;

        protected float FrameRateInMilliseconds => videoTrack!.NominalFrameRate;

        public float FrameRateInSeconds => FrameRateInMilliseconds * MillisecondsInSecond;

        public CGAffineTransform AffineTransform => videoTrack!.PreferredTransform.Invert ();

        public CGImagePropertyOrientation Orientation
        {
                get
                {
                        var orientation = 1;
                        var angleInDegrees = Math.Atan2 (AffineTransform.yx, AffineTransform.xx) * 180 / Math.PI;
                        switch (angleInDegrees)
                        {
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

                        return (CGImagePropertyOrientation)orientation;
                }
        }

        public static VideoReader? Create (AVAsset videoAsset)
        {
                var result = new VideoReader { videoAsset = videoAsset };
                var array = result.videoAsset.TracksWithMediaType (videoDeviceName);
                result.videoTrack = array [0];

                return result.RestartReading () ? result : null;
        }

        public bool RestartReading ()
        {
                var result = false;

                assetReader = AVAssetReader.FromAsset (videoAsset!, out NSError error);
                if (error is null && assetReader is not null)
                {
                        var settings = new AVVideoSettingsUncompressed { PixelFormatType = CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange };
                        videoAssetReaderOutput = new AVAssetReaderTrackOutput (videoTrack!, settings);
                        videoAssetReaderOutput.AlwaysCopiesSampleData = true;

                        if (assetReader.CanAddOutput (videoAssetReaderOutput))
                        {
                                assetReader.AddOutput (videoAssetReaderOutput);
                                result = assetReader.StartReading ();
                        }
                }
                else
                {
                        Console.WriteLine ($"Failed to create AVAssetReader object: {error}");
                }

                return result;
        }

        public CVPixelBuffer? NextFrame ()
        {
                var sampleBuffer = videoAssetReaderOutput!.CopyNextSampleBuffer ();
                return sampleBuffer?.GetImageBuffer () as CVPixelBuffer;
        }
}
