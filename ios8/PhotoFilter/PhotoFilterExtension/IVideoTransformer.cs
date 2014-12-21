using System;
using CoreVideo;

namespace PhotoFilterExtension
{
	public interface IVideoTransformer
	{
		void AdjustPixelBuffer (CVPixelBuffer inputBuffer, CVPixelBuffer outputBuffer);
	}
}

