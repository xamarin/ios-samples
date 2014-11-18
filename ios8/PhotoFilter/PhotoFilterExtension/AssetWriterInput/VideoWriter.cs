using System;
using CoreMedia;
using CoreVideo;
using AVFoundation;

namespace PhotoFilterExtension
{
	public class VideoWriter : AssetWriterInput
	{
		readonly IVideoTransformer transformer;
		readonly AVAssetWriterInputPixelBufferAdaptor adaptor;

		public VideoWriter (AVAssetWriterInput writerInput, IVideoTransformer transformer)
			: base(writerInput)
		{
			if (transformer == null)
				throw new ArgumentNullException ("transformer");

			this.transformer = transformer;

			var adaptorAttrs = new CVPixelBufferAttributes {
				PixelFormatType = CVPixelFormatType.CV32BGRA
			};
			adaptor = new AVAssetWriterInputPixelBufferAdaptor (writerInput, adaptorAttrs);
		}

		public override bool Append (CMSampleBuffer sampleBuffer)
		{
			CMTime presentationTime = sampleBuffer.PresentationTimeStamp;

			using (CVPixelBuffer writerBuffer = adaptor.PixelBufferPool.CreatePixelBuffer ()) {
				// Grab the pixel buffer from the sample buffer, if possible
				using (CVImageBuffer imageBuffer = sampleBuffer.GetImageBuffer ()) {
					var pixelBuffer = (CVPixelBuffer)imageBuffer;
					if (pixelBuffer != null)
						transformer.AdjustPixelBuffer (pixelBuffer, writerBuffer);
				}

				return adaptor.AppendPixelBufferWithPresentationTime (writerBuffer, presentationTime);
			}
		}
	}
}