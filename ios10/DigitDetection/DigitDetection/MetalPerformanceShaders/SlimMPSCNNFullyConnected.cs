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
	/// The SlimMPSCnnFullyConnected is a wrapper class around MPSCnnFullyConnected used to encapsulate:
	/// - making an MPSCnnConvolutionDescriptor,
	/// - adding network parameters (weights and bias binaries by memory mapping the binaries)
	/// - getting our fullyConnected layer
	/// </summary>
	public class SlimMPSCnnFullyConnected : MPSCnnFullyConnected
	{
		// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=44938
		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_UInt64 (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, ulong arg5);

		SlimMPSCnnFullyConnected (IMTLDevice device, MPSCnnConvolutionDescriptor convolutionDescriptor, IntPtr kernelWeights, IntPtr biasTerms, MPSCnnConvolutionFlags flags)
			: base (NSObjectFlag.Empty)
		{
			var sel = "initWithDevice:convolutionDescriptor:kernelWeights:biasTerms:flags:";
			var selector = Selector.GetHandle (sel);
			InitializeHandle (IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_UInt64 (Handle, selector, device.Handle, convolutionDescriptor.Handle, kernelWeights, biasTerms, (ulong)flags), sel);
		}

		public static SlimMPSCnnFullyConnected Create (uint kernelWidth, uint kernelHeight,
										 uint inputFeatureChannels, uint outputFeatureChannels,
										 MPSCnnNeuron neuronFilter, IMTLDevice device,
										 string kernelParamsBinaryName,
										 uint destinationFeatureChannelOffset = 0)
		{
			// get the url to this layer's weights and bias
			var wtPath = NSBundle.MainBundle.PathForResource ($"weights_{kernelParamsBinaryName}", "dat");
			var bsPath = NSBundle.MainBundle.PathForResource ($"bias_{kernelParamsBinaryName}", "dat");

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

					var convDesc = MPSCnnConvolutionDescriptor.GetConvolutionDescriptor (
						kernelWidth, kernelHeight,
						inputFeatureChannels, outputFeatureChannels,
						neuronFilter);

					return new SlimMPSCnnFullyConnected (device, convDesc, (IntPtr)w, (IntPtr)b, MPSCnnConvolutionFlags.None) {
						DestinationFeatureChannelOffset = destinationFeatureChannelOffset
					};
				}
			}
		}
	}
}
