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
				adaptor = AVAssetWriterInputPixelBufferAdaptor.FromInput (localAssetWriterInput, adaptorAttrs.Dictionary);
			}

			serializationQueue = new DispatchQueue ("ReadWriteSampleBufferChannel queue");
		}

		private void CompleteTaskIfNecessary()
		{
			// Set state to mark that we no longer need to call the completion handler, grab the completion handler, and clear out the ivar
			bool oldFinished = finished;
			finished = true;

			if (!oldFinished) {
				assetWriterInput.MarkAsFinished ();  // let the asset writer know that we will not be appending any more samples to this input
				completionSource.SetResult (null);
			}
		}

		public void StartWithAsync (TaskCompletionSource<object> completionSource, AVReaderWriter handler)
		{
			if (this.completionSource != null) {
				Console.WriteLine ("this.completionSource is not null");
				throw new InvalidProgramException ();
			}

			this.completionSource = completionSource;

			assetWriterInput.RequestMediaData (serializationQueue, () => {
				if (finished)
					return;

				bool completedOrFailed = false;

				// Read samples in a loop as long as the asset writer input is ready
				while (assetWriterInput.ReadyForMoreMediaData && !completedOrFailed) {
					bool success;
					using (CMSampleBuffer sampleBuffer = assetReaderOutput.CopyNextSampleBuffer ()) {
						// TODO: https://trello.com/c/4YM7lofd
						if (sampleBuffer == null || !sampleBuffer.IsValid || handler == null) {
							completedOrFailed = true;
							break;
						}

						if (adaptor != null) {
							CMTime presentationTime = sampleBuffer.PresentationTimeStamp;
							using (CVPixelBuffer writerBuffer = adaptor.PixelBufferPool.CreatePixelBuffer ()) {
								handler.DidReadAndWriteSampleBuffer (this, sampleBuffer, writerBuffer);
								success = adaptor.AppendPixelBufferWithPresentationTime (writerBuffer, presentationTime);
							}
						} else {
							handler.DidReadSampleBuffer (this, sampleBuffer);
							success = assetWriterInput.AppendSampleBuffer (sampleBuffer);
						}
					}
					completedOrFailed = !success;
				}

				if (completedOrFailed)
					CompleteTaskIfNecessary ();
			});
		}

		public void Cancel()
		{
			completionSource.Task.ContinueWith (_ => CompleteTaskIfNecessary ());
		}
	}
}

