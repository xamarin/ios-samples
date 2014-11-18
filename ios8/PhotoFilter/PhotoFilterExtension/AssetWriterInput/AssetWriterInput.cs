using System;
using CoreMedia;
using AVFoundation;

namespace PhotoFilterExtension
{
	public abstract class AssetWriterInput
	{
		public AVAssetWriterInput Input { get; private set; }

		public AssetWriterInput(AVAssetWriterInput writerInput)
		{
			if (writerInput == null)
				throw new ArgumentNullException ("writerInput");

			Input = writerInput;
		}

		public abstract bool Append (CMSampleBuffer sampleBuffer);
	}
}

