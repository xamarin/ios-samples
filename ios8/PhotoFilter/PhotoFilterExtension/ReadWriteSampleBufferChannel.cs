using System;
using System.Threading.Tasks;

using AVFoundation;
using CoreVideo;
using CoreFoundation;
using Foundation;
using CoreMedia;

namespace PhotoFilterExtension
{
	public class ReadWriteSampleBufferChannel
	{
		bool finished;
		AVAssetWriterInput assetWriterInput;
		AVAssetReaderOutput assetReaderOutput;
		AVAssetWriterInputPixelBufferAdaptor adaptor;
		DispatchQueue serializationQueue;

		TaskCompletionSource<object> completionSource;

		bool IsStarted {
			get {
				return completionSource != null;
			}
		}

		public ReadWriteSampleBufferChannel (AVAssetReaderOutput localAssetReaderOutput,
			AVAssetWriterInput localAssetWriterInput,
			bool useAdaptor)
		{
			assetReaderOutput = localAssetReaderOutput;
			assetWriterInput = localAssetWriterInput;

			if (useAdaptor) {
				var adaptorAttrs = new CVPixelBufferAttributes {
					PixelFormatType = CVPixelFormatType.CV32BGRA
				};
				adaptor = new AVAssetWriterInputPixelBufferAdaptor (localAssetWriterInput, adaptorAttrs);
			}

			serializationQueue = new DispatchQueue ("ReadWriteSampleBufferChannel queue");
		}

		public Task Start (AVReaderWriter mediaTransformer)
		{
			if (mediaTransformer == null)
				throw new ArgumentNullException ("mediaTransformer");

			if (IsStarted)
				throw new InvalidProgramException ();

			completionSource = new TaskCompletionSource<object> ();
			AdjustMediaData (mediaTransformer);

			return completionSource.Task;
		}

		void AdjustMediaData(AVReaderWriter mediaTransformer)
		{
			assetWriterInput.RequestMediaData (serializationQueue, () => {
				// Read samples in a loop as long as the asset writer input is ready
				while (assetWriterInput.ReadyForMoreMediaData) {
					using (CMSampleBuffer sampleBuffer = assetReaderOutput.CopyNextSampleBuffer ()) {
						// TODO: https://trello.com/c/4YM7lofd
						if (sampleBuffer == null || !sampleBuffer.IsValid || mediaTransformer == null)
							break;

						bool success = (adaptor == null) ? Append(sampleBuffer) : Append(sampleBuffer, mediaTransformer);
						if(!success)
							break;
					}
				}

				CompleteTask ();
			});
		}

		bool Append(CMSampleBuffer sampleBuffer)
		{
			return assetWriterInput.AppendSampleBuffer (sampleBuffer);
		}

		bool Append(CMSampleBuffer sampleBuffer, AVReaderWriter mediaTransformer)
		{
			CMTime presentationTime = sampleBuffer.PresentationTimeStamp;

			using (CVPixelBuffer writerBuffer = adaptor.PixelBufferPool.CreatePixelBuffer ()) {
				mediaTransformer.Transform (sampleBuffer, writerBuffer);
				return adaptor.AppendPixelBufferWithPresentationTime (writerBuffer, presentationTime);
			}
		}

		public void Cancel()
		{
			if (IsStarted)
				CompleteTask ();
		}

		private void CompleteTask()
		{
			if (!finished) {
				assetWriterInput.MarkAsFinished ();  // let the asset writer know that we will not be appending any more samples to this input
				completionSource.SetResult (null);
			}
			finished = true;
		}
	}
}