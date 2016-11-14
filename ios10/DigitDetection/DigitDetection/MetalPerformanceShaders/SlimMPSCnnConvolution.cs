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
		// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=44938
		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_UInt64 (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, ulong arg5);

		// A field to keep info from init time whether we will pad input image or not for use during encode call
		bool padding = true;

		unsafe SlimMPSCnnConvolution (IMTLDevice device, MPSCnnConvolutionDescriptor convolutionDescriptor, IntPtr kernelWeights, IntPtr biasTerms, MPSCnnConvolutionFlags flags)
			: base (NSObjectFlag.Empty)
		{
			const string sel = "initWithDevice:convolutionDescriptor:kernelWeights:biasTerms:flags:";
			InitializeHandle (IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_UInt64 (Handle, Selector.GetHandle (sel), device.Handle, convolutionDescriptor.Handle, kernelWeights, biasTerms, (ulong)flags), sel);
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
		/// <param name="strideX">Stride of the filter</param>
		/// <param name="strideY">Stride of the filter</param>
		/// <param name="destinationFeatureChannelOffset">FeatureChannel no. in the destination MPSImage to start writing from, helps with concat operations</param>
		/// <param name="groupNum">if grouping is used, default value is 1 meaning no groups</param>
		public static SlimMPSCnnConvolution Create (uint kernelWidth, uint kernelHeight,
													uint inputFeatureChannels, uint outputFeatureChannels,
													MPSCnnNeuron neuronFilter, IMTLDevice device,
													string kernelParamsBinaryName, bool padding,
		                                            uint strideX, uint strideY,
													uint destinationFeatureChannelOffset, uint groupNum)
		{
			// get the url to this layer's weights and bias
			var wtPath = NSBundle.MainBundle.PathForResource ($"weights_{kernelParamsBinaryName}", "dat");
			var bsPath = NSBundle.MainBundle.PathForResource ($"bias_{kernelParamsBinaryName}", "dat");

			// create appropriate convolution descriptor with appropriate stride
			var convDesc = MPSCnnConvolutionDescriptor.GetConvolutionDescriptor (
				kernelWidth, kernelHeight,
				inputFeatureChannels, outputFeatureChannels,
				neuronFilter);
			convDesc.StrideInPixelsX = strideX;
			convDesc.StrideInPixelsY = strideY;

			if (groupNum <= 0)
				throw new ArgumentException ("Group size can't be less than 1");
			convDesc.Groups = groupNum;

			unsafe
			{
				using (var mmfW = MemoryMappedFile.CreateFromFile (wtPath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
				using (var mmfB = MemoryMappedFile.CreateFromFile (bsPath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
				using (var wView = mmfW.CreateViewAccessor (0, 0, MemoryMappedFileAccess.Read))
				using (var bView = mmfB.CreateViewAccessor (0, 0, MemoryMappedFileAccess.Read)) {
					byte* w = null;
					wView.SafeMemoryMappedViewHandle.AcquirePointer (ref w);

					byte* b = null;
					bView.SafeMemoryMappedViewHandle.AcquirePointer (ref b);

					return new SlimMPSCnnConvolution (device, convDesc, (IntPtr)w, (IntPtr)b, MPSCnnConvolutionFlags.None) {
						DestinationFeatureChannelOffset = destinationFeatureChannelOffset,
						padding = padding
					};
				}
			}
		}

		/// <summary>
		/// Encode a MPSCnnKernel into a command Buffer. The operation shall proceed out-of-place.
		/// We calculate the appropriate offset as per how TensorFlow calculates its padding using input image size and stride here.
		/// This [Link](https://github.com/tensorflow/tensorflow/blob/master/tensorflow/python/ops/nn.py) has an explanation in header comments how tensorFlow pads its convolution input images.
		/// </summary>
		/// <param name="commandBuffer">A valid MTLCommandBuffer to receive the encoded filter</param>
		/// <param name="sourceImage">A valid MPSImage object containing the source image.</param>
		/// <param name="destinationImage">A valid MPSImage to be overwritten by result image. destinationImage may not alias sourceImage</param>
		public override void EncodeToCommandBuffer (IMTLCommandBuffer commandBuffer, MPSImage sourceImage, MPSImage destinationImage)
		{
			// select offset according to padding being used or not
			if (padding) {
				var pad_along_height = ((destinationImage.Height - 1) * StrideInPixelsY + KernelHeight - sourceImage.Height);
				var pad_along_width = ((destinationImage.Width - 1) * StrideInPixelsX + KernelWidth - sourceImage.Width);
				var pad_top = pad_along_height / 2;
				var pad_left = pad_along_width / 2;

				Offset = new MPSOffset {
					X = (nint)(KernelWidth / 2 - pad_left),
					Y = (nint)(KernelHeight / 2 - pad_top),
					Z = 0
				};
			} else {
				Offset = new MPSOffset {
					X = (nint)(KernelWidth / 2),
					Y = (nint)(KernelHeight / 2),
					Z = 0
				};
			}
			base.EncodeToCommandBuffer (commandBuffer, sourceImage, destinationImage);
		}
	}
}
