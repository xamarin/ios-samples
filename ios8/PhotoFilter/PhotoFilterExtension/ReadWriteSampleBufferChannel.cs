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
		bool _useAdaptor;
		bool _finished;
		AVAssetWriterInput _assetWriterInput;
		AVAssetReaderOutput _assetReaderOutput;
		AVAssetWriterInputPixelBufferAdaptor _adaptor;
		DispatchQueue _serializationQueue;

		TaskCompletionSource<object> _completionSource;

		public ReadWriteSampleBufferChannel (AVAssetReaderOutput localAssetReaderOutput,
			AVAssetWriterInput localAssetWriterInput,
			bool useAdaptor)
		{
			_assetReaderOutput = localAssetReaderOutput;
			_assetWriterInput = localAssetWriterInput;
			_useAdaptor = useAdaptor;

			if (_useAdaptor) {
				var adaptorAttrs = new CVPixelBufferAttributes {
					PixelFormatType = CVPixelFormatType.CV32BGRA
				};
				_adaptor = AVAssetWriterInputPixelBufferAdaptor.FromInput (localAssetWriterInput, adaptorAttrs.Dictionary);
			}

			_serializationQueue = new DispatchQueue ("ReadWriteSampleBufferChannel queue");
		}

		private void CompleteTaskIfNecessary()
		{
			// Set state to mark that we no longer need to call the completion handler, grab the completion handler, and clear out the ivar
			bool oldFinished = _finished;
			_finished = true;

			if (!oldFinished) {
				_assetWriterInput.MarkAsFinished ();  // let the asset writer know that we will not be appending any more samples to this input
				_completionSource.SetResult (null);
			}
		}

		public void StartWithAsync (TaskCompletionSource<object> completionSource, AVReaderWriter handler)
		{
			if (_completionSource != null)
				throw new InvalidProgramException ();

			_completionSource = completionSource;

			_assetWriterInput.RequestMediaData (_serializationQueue, () => {
				if (_finished)
					return;

				bool completedOrFailed = false;

				// Read samples in a loop as long as the asset writer input is ready
				while (_assetWriterInput.ReadyForMoreMediaData && !completedOrFailed) {
					bool success;
					using (CMSampleBuffer sampleBuffer = _assetReaderOutput.CopyNextSampleBuffer ()) {
						if (sampleBuffer == null) {
							completedOrFailed = true;
							continue;
						}

						if (_adaptor != null) {
							CMTime presentationTime = sampleBuffer.PresentationTimeStamp;
							using (CVPixelBuffer writerBuffer = _adaptor.PixelBufferPool.CreatePixelBuffer ()) {
								handler.DidReadAndWriteSampleBuffer (this, sampleBuffer, writerBuffer);
								success = _adaptor.AppendPixelBufferWithPresentationTime (writerBuffer, presentationTime);
							}
						} else {
							handler.DidReadSampleBuffer (this, sampleBuffer);
							success = _assetWriterInput.AppendSampleBuffer (sampleBuffer);
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
			_completionSource.Task.ContinueWith (_ => CompleteTaskIfNecessary ());
		}
	}
}

