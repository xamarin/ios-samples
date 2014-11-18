using System;
using CoreMedia;
using AVFoundation;

namespace PhotoFilterExtension
{
	public class AudioWriter : AssetWriterInput
	{
		public AudioWriter (AVAssetWriterInput writerInput)
			: base (writerInput)
		{
		}

		public override bool Append (CMSampleBuffer sampleBuffer)
		{
			return Input.AppendSampleBuffer (sampleBuffer);
		}
	}
}