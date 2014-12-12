using System;
using AVFoundation;
using CoreMedia;

namespace PhotoFilterExtension
{
	public class AudioChannel : ReadWriteSampleBufferChannel
	{
		public AudioChannel (AVAssetReaderOutput readerOutput, AVAssetWriterInput writerInput)
			: base(readerOutput, writerInput)
		{
		}

		protected override bool Append (CMSampleBuffer sampleBuffer)
		{
			// append audio data without modification
			return WriterInput.AppendSampleBuffer (sampleBuffer);
		}
	}
}

