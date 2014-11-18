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
		readonly AVAssetReaderOutput readerOutput;
		readonly AssetWriterInput assetWriter;
		readonly DispatchQueue serializationQueue;

		bool finished;
		TaskCompletionSource<object> completionSource;

		bool IsStarted {
			get {
				return completionSource != null;
			}
		}

		public ReadWriteSampleBufferChannel (AVAssetReaderOutput readerOutput, AssetWriterInput writerInput)
		{
			if (readerOutput == null)
				throw new ArgumentNullException ("readerOutput");
			if (writerInput == null)
				throw new ArgumentNullException ("writerInput");

			this.readerOutput = readerOutput;
			this.assetWriter = writerInput;
			serializationQueue = new DispatchQueue ("ReadWriteSampleBufferChannel queue");
		}

		public Task StartTransformationAsync ()
		{
			if (IsStarted)
				throw new InvalidProgramException ();

			completionSource = new TaskCompletionSource<object> ();
			AdjustMediaData ();

			return completionSource.Task;
		}

		void AdjustMediaData ()
		{
			assetWriter.Input.RequestMediaData (serializationQueue, () => {
				// Read samples in a loop as long as the asset writer input is ready
				while (assetWriter.Input.ReadyForMoreMediaData) {
					using (CMSampleBuffer sampleBuffer = readerOutput.CopyNextSampleBuffer ()) {
						// TODO: https://trello.com/c/4YM7lofd
						if (sampleBuffer == null || !sampleBuffer.IsValid)
							break;

						bool success = assetWriter.Append (sampleBuffer);
						if (!success)
							break;
					}
				}

				CompleteTask ();
			});
		}

		public void Cancel ()
		{
			if (IsStarted)
				CompleteTask ();
		}

		private void CompleteTask ()
		{
			if (!finished) {
				assetWriter.Input.MarkAsFinished ();  // let the asset writer know that we will not be appending any more samples to this input
				completionSource.SetResult (null);
			}
			finished = true;
		}
	}
}