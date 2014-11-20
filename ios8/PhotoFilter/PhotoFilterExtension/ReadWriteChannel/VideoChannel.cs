using System;
using AVFoundation;
using CoreVideo;
using CoreMedia;

namespace PhotoFilterExtension
{
	public class VideoChannel : ReadWriteSampleBufferChannel
	{
		readonly IVideoTransformer transformer;
		readonly AVAssetWriterInputPixelBufferAdaptor adaptor;

		public VideoChannel (AVAssetReaderOutput readerOutput, AVAssetWriterInput writerInput, IVideoTransformer transformer)
			: base(readerOutput, writerInput)
		{
			if (transformer == null)
				throw new ArgumentNullException ("transformer");

			this.transformer = transformer;

			var adaptorAttrs = new CVPixelBufferAttributes {
				PixelFormatType = CVPixelFormatType.CV32BGRA
			};
			adaptor = new AVAssetWriterInputPixelBufferAdaptor (WriterInput, adaptorAttrs);
		}

		protected override bool Append (CMSampleBuffer sampleBuffer)
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

