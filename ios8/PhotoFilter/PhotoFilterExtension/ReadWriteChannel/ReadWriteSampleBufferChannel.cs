using System;
using System.Threading.Tasks;

using AVFoundation;
using CoreVideo;
using CoreFoundation;
using Foundation;
using CoreMedia;

namespace PhotoFilterExtension
{
	public abstract class ReadWriteSampleBufferChannel
	{
		readonly AVAssetReaderOutput readerOutput;
		protected AVAssetReaderOutput ReaderOutput {
			get {
				return readerOutput;
			}
		}

		readonly AVAssetWriterInput writerInput;
		protected AVAssetWriterInput WriterInput {
			get {
				return writerInput;
			}
		}

		readonly DispatchQueue serializationQueue;

		bool finished;
		TaskCompletionSource<object> completionSource;

		bool IsStarted {
			get {
				return completionSource != null;
			}
		}

		public ReadWriteSampleBufferChannel (AVAssetReaderOutput readerOutput, AVAssetWriterInput writerInput)
		{
			if (readerOutput == null)
				throw new ArgumentNullException ("readerOutput");
			if (writerInput == null)
				throw new ArgumentNullException ("writerInput");

			this.readerOutput = readerOutput;
			this.writerInput = writerInput;

			serializationQueue = new DispatchQueue ("ReadWriteSampleBufferChannel queue");
		}

		public Task StartAsync ()
		{
			if (IsStarted)
				throw new InvalidProgramException ();

			completionSource = new TaskCompletionSource<object> ();
			AdjustMediaData ();

			return completionSource.Task;
		}

		void AdjustMediaData ()
		{
			writerInput.RequestMediaData (serializationQueue, () => {
				if(finished)
					return;

				bool shouldContinue = true;

				// Read samples in a loop as long as the asset writer input is ready
				while (writerInput.ReadyForMoreMediaData && shouldContinue) {
					using (CMSampleBuffer sampleBuffer = readerOutput.CopyNextSampleBuffer ()) {
						// TODO: https://trello.com/c/4YM7lofd
						bool isBufferValid = sampleBuffer != null && sampleBuffer.IsValid;
						shouldContinue = isBufferValid ? Append (sampleBuffer) : false;
					}
				}

				if(!shouldContinue)
					CompleteTask ();
			});
		}

		protected abstract bool Append(CMSampleBuffer sampleBuffer);

		public void Cancel ()
		{
			if (IsStarted)
				CompleteTask ();
		}

		void CompleteTask ()
		{
			if (!finished) {
				writerInput.MarkAsFinished ();  // let the asset writer know that we will not be appending any more samples to this input
				completionSource.SetResult (null);
			}
			finished = true;
		}
	}
}
