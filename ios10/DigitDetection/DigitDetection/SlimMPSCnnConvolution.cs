using System;
using Metal;
using MetalPerformanceShaders;

namespace DigitDetection
{
	public class SlimMPSCnnConvolution : MPSCnnConvolution
	{
		public SlimMPSCnnConvolution (uint kernelWidth, uint kernelHeight, uint inputFeatureChannels, uint outputFeatureChannels,
		                              MPSCnnNeuron neuronFilter, IMTLDevice device, string kernelParamsBinaryName, bool padding,
		                              Tuple<int, int> strideXY, uint destinationFeatureChannelOffset, uint groupNum)
			: base (device, null, null, null, MPSCnnConvolutionFlags.None)
		{
			throw new NotImplementedException ();
		}


	}
}
