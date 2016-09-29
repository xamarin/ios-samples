using System;
using System.Linq;
using Accelerate;
using Metal;
using MetalPerformanceShaders;

namespace DigitDetection
{
	// This class has our entire network with all layers to getting the final label
	// Resources
	// https://www.tensorflow.org/versions/r0.8/tutorials/mnist/beginners/index.html#mnist-for-ml-beginners to run this network on TensorFlow.
	public class MnistFullLayerNeuralNetwork
	{
		// MPSImageDescriptors for different layers outputs to be put in
		MPSImageDescriptor sid = MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Unorm8, 28, 28, 1);
		MPSImageDescriptor did = MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float16, 1, 1, 10);

		// MPSImages and layers declared
		MPSImage srcImage;
		MPSImage dstImage;
		MPSCnnFullyConnected layer;
		MPSCnnSoftMax softmax;
		readonly IMTLCommandQueue commandQueue;
		readonly IMTLDevice device;

		public MnistFullLayerNeuralNetwork (IMTLCommandQueue commandQueueIn)
		{
			// CommandQueue to be kept around
			commandQueue = commandQueueIn;
			device = commandQueueIn.Device;

			// Initialize MPSImage from descriptors
			srcImage = new MPSImage (device, sid);
			dstImage = new MPSImage (device, did);

			// setup convolution layer (which is a fully-connected layer)
			// cliprect, offset is automatically set
			layer = SlimMPSCNNFullyConnected.Create (kernelWidth: 28, kernelHeight: 28,
													 inputFeatureChannels: 1, outputFeatureChannels: 10,
													 neuronFilter: null, device: device,
													 kernelParamsBinaryName: "NN");

			// prepare softmax layer to be applied at the end to get a clear label
			softmax = new MPSCnnSoftMax (device);
		}

		/// <summary>
		/// This function encodes all the layers of the network into given commandBuffer, it calls subroutines for each piece of the network
		/// Returns: Guess of the network as to what the digit is as UInt
		/// </summary>
		/// <param name="inputImage">Image coming in on which the network will run</param>
		/// <param name="imageNum">If the test set is being used we will get a value between 0 and 9999 for which of the 10,000 images is being evaluated</param>
		/// <param name="correctLabel">The correct label for the inputImage while testing</param>
		public uint Forward (MPSImage inputImage = null, int imageNum = 9999, uint correctLabel = 10)
		{
			uint label = 99;

			// Get command buffer to use in MetalPerformanceShaders.
			using (var commandBuffer = commandQueue.CommandBuffer ()) {
				// output will be stored in this image
				var finalLayer = new MPSImage (commandBuffer.Device, did);

				// encode layers to metal commandBuffer
				if (inputImage == null)
					layer.EncodeToCommandBuffer (commandBuffer, srcImage, dstImage);
				else
					layer.EncodeToCommandBuffer (commandBuffer, inputImage, dstImage);

				softmax.EncodeToCommandBuffer (commandBuffer, dstImage, finalLayer);

				// add a completion handler to get the correct label the moment GPU is done and compare it to the correct output or return it
				commandBuffer.AddCompletedHandler (buffer => {
					label = GetLabel (finalLayer);

					if (correctLabel == label)
						Atomics.Increment ();
				});

				// commit commandbuffer to run on GPU and wait for completion
				commandBuffer.Commit ();
				if (imageNum == 9999 || inputImage == null)
					commandBuffer.WaitUntilCompleted ();
			}

			return label;
		}

		/// <summary>
		/// This function reads the output probabilities from finalLayer to CPU, sorts them and gets the label with heighest probability
		/// </summary>
		/// <param name="finalLayer">output image of the network this has probabilities of each digit</param>
		/// <returns>Guess of the network as to what the digit is as uint</returns>
		public uint GetLabel (MPSImage finalLayer)
		{
			// even though we have 10 labels outputed the MTLTexture format used is RGBAFloat16 thus 3 slices will have 3*4 = 12 outputs
			var result_half_array = Enumerable.Repeat ((UInt16)6, 12).ToArray ();
			var result_float_array = Enumerable.Repeat (0.3f, 10).ToArray ();

			for (uint i = 0; i <= 2; i++) {
				finalLayer.Texture.GetBytes (IntPtr.Zero,
											sizeof (UInt16) * 1 * 4, sizeof (UInt16) * 1 * 1 * 4,
											new MTLRegion (new MTLOrigin (0, 0, 0), new MTLSize (1, 1, 1)),
											0, i);
			}

			// we use vImage to convert our data to float16, Metal GPUs use float16 and swift float is 32-bit
			var fullResultVImagebuf = new vImageBuffer {
				Data = IntPtr.Zero, //&result_float_array
				Height = 1,
				Width = 10,
				BytesPerRow = 10 * 4
			};

			var halfResultVImagebuf = new vImageBuffer {
				Data = IntPtr.Zero, // &result_half_array, height: 1, width: 10, rowBytes: 10 * 2)
				Height = 1,
				Width = 10,
				BytesPerRow = 10 * 2
			};

			//if vImageConvert_Planar16FtoPlanarF (&halfResultVImagebuf, &fullResultVImagebuf, 0) != kvImageNoError {
			//	print ("Error in vImage")
			//}

			// poll all labels for probability and choose the one with max probability to return
			float max = 0f;
			uint mostProbableDigit = 10;

			for (uint i = 0; i <= 9; i++) {
				if (max < result_float_array [i]) {
					max = result_float_array [i];
					mostProbableDigit = i;
				}
			}

			return mostProbableDigit;
		}
	}
}
