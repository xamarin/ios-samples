using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Foundation;
using Metal;
using MetalPerformanceShaders;
using ObjCRuntime;

namespace DigitDetection
{
	/// <summary>
	/// The SlimMPSCNNConvolution is a wrapper class around MPSCNNConvolution used to encapsulate:
	/// - making an MPSCNNConvolutionDescriptor,
	/// - adding network parameters (weights and bias binaries by memory mapping the binaries)
	/// - getting our convolution layer
	/// </summary>
	public class SlimMPSCnnConvolution : MPSCnnConvolution
	{
		unsafe SlimMPSCnnConvolution (IMTLDevice device, MPSCnnConvolutionDescriptor convolutionDescriptor, IntPtr kernelWeights, IntPtr biasTerms, MPSCnnConvolutionFlags flags)
			: base (NSObjectFlag.Empty)
		{
		}

		/// <summary>
		/// Initializes a fully connected kernel.
		/// Returns: A valid SlimMPSCnnConvolution object or null, if failure.
		/// </summary>
		/// <param name="kernelWidth">Kernel Width</param>
		/// <param name="kernelHeight">Kernel Height</param>
		/// <param name="inputFeatureChannels">Number feature channels in input of this layer</param>
		/// <param name="outputFeatureChannels">Number feature channels from output of this layer</param>
		/// <param name="neuronFilter">A neuronFilter to add at the end as activation (could be null)</param>
		/// <param name="device">The IMTLDevice on which this SlimMPSCnnConvolution filter will be used</param>
		/// <param name="kernelParamsBinaryName">name of the layer to fetch kernelParameters by adding a prefix "weights_" or "bias_"</param>
		/// <param name="padding">Bool value whether to use padding or not</param>
		/// <param name="strideXY">Stride of the filter</param>
		/// <param name="destinationFeatureChannelOffset">FeatureChannel no. in the destination MPSImage to start writing from, helps with concat operations</param>
		/// <param name="groupNum">if grouping is used, default value is 1 meaning no groups</param>
		public static SlimMPSCnnConvolution Create (uint kernelWidth, uint kernelHeight,
													uint inputFeatureChannels, uint outputFeatureChannels,
													MPSCnnNeuron neuronFilter, IMTLDevice device,
													string kernelParamsBinaryName, bool padding,
													Tuple<int, int> strideXY,
													uint destinationFeatureChannelOffset, uint groupNum)
		{
			throw new NotImplementedException ();
		}
	}
}
