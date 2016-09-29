using System;

using Metal;
using MetalPerformanceShaders;

namespace DigitDetection
{
	public class SlimMPSCnnFullyConnected : MPSCnnFullyConnected
	{
		SlimMPSCnnFullyConnected ()
			: base (null, null, null, null, MPSCnnConvolutionFlags.None)
		{
			throw new NotImplementedException ();
		}

		public static SlimMPSCnnFullyConnected Create (uint kernelWidth, uint kernelHeight,
										 uint inputFeatureChannels, uint outputFeatureChannels,
										 MPSCnnNeuron neuronFilter, IMTLDevice device,
										 string kernelParamsBinaryName,
										 uint destinationFeatureChannelOffset = 0)
		{
			throw new NotImplementedException ();
		}
	}
}
