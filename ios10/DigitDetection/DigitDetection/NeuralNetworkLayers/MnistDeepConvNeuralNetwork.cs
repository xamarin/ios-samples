using Metal;
using MetalPerformanceShaders;

namespace DigitDetection {
	public class MnistDeepConvNeuralNetwork : MnistFullLayerNeuralNetwork {
		// MPSImageDescriptors for different layers outputs to be put in
		readonly MPSImageDescriptor c1id = MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float16, 28, 28, 32);
		readonly MPSImageDescriptor p1id = MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float16, 14, 14, 32);
		readonly MPSImageDescriptor c2id = MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float16, 14, 14, 64);
		readonly MPSImageDescriptor p2id = MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float16, 7, 7, 64);
		readonly MPSImageDescriptor fc1id = MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float16, 1, 1, 1024);

		// MPSImages and layers declared
		readonly MPSImage c2Image, p1Image, p2Image, fc1Image;

		// MPSImages and layers declared
		readonly MPSImage c1Image;
		readonly MPSCnnConvolution conv1, conv2;
		readonly MPSCnnFullyConnected fc1, fc2;
		readonly MPSCnnPoolingMax pool;
		readonly MPSCnnNeuronReLU relu;

		public MnistDeepConvNeuralNetwork (IMTLCommandQueue commandQueueIn)
			: base (commandQueueIn)
		{
			// use device for a little while to initialize
			var device = commandQueueIn.Device;

			pool = new MPSCnnPoolingMax (device, 2, 2, 2, 2) {
				Offset = new MPSOffset { X = 1, Y = 1, Z = 0 },
				EdgeMode = MPSImageEdgeMode.Clamp
			};
			relu = new MPSCnnNeuronReLU (device, 0);

			// Initialize MPSImage from descriptors
			c1Image = new MPSImage (device, c1id);
			p1Image = new MPSImage (device, p1id);
			c2Image = new MPSImage (device, c2id);
			p2Image = new MPSImage (device, p2id);
			fc1Image = new MPSImage (device, fc1id);

			// setup convolution layers
			conv1 = SlimMPSCnnConvolution.Create (kernelWidth: 5,
												  kernelHeight: 5,
												  inputFeatureChannels: 1,
												  outputFeatureChannels: 32,
												  neuronFilter: relu,
												  device: device,
												  kernelParamsBinaryName: "conv1",
												  padding: true,
												  strideX: 1,
												  strideY: 1,
												  destinationFeatureChannelOffset: 0,
												  groupNum: 1);

			conv2 = SlimMPSCnnConvolution.Create (kernelWidth: 5,
												  kernelHeight: 5,
												  inputFeatureChannels: 32,
												  outputFeatureChannels: 64,
												  neuronFilter: relu,
												  device: device,
												  kernelParamsBinaryName: "conv2",
												  padding: true,
												  strideX: 1,
												  strideY: 1,
												  destinationFeatureChannelOffset: 0,
												  groupNum: 1);

			// same as a 1x1 convolution filter to produce 1x1x10 from 1x1x1024
			fc1 = SlimMPSCnnFullyConnected.Create (kernelWidth: 7,
												   kernelHeight: 7,
												   inputFeatureChannels: 64,
												   outputFeatureChannels: 1024,
												   neuronFilter: null,
												   device: device,
												   kernelParamsBinaryName: "fc1",
												   destinationFeatureChannelOffset: 0);

			fc2 = SlimMPSCnnFullyConnected.Create (kernelWidth: 1,
												   kernelHeight: 1,
												   inputFeatureChannels: 1024,
												   outputFeatureChannels: 10,
												   neuronFilter: null,
												   device: device,
												   kernelParamsBinaryName: "fc2");
		}

		/// <summary>
		/// This function encodes all the layers of the network into given commandBuffer, it calls subroutines for each piece of the network
		/// Returns: Guess of the network as to what the digit is as uint
		/// </summary>
		/// <param name="inputImage">Image coming in on which the network will run</param>
		/// <param name="imageNum">If the test set is being used we will get a value between 0 and 9999 for which of the 10,000 images is being evaluated</param>
		/// <param name="correctLabel">The correct label for the inputImage while testing</param>
		public override uint Forward (MPSImage inputImage = null, int imageNum = 9999, int correctLabel = 10)
		{
			uint label = 99;

			// Get command buffer to use in MetalPerformanceShaders.
			using (var commandBuffer = commandQueue.CommandBuffer ()) {
				// output will be stored in this image
				var finalLayer = new MPSImage (commandBuffer.Device, DID);

				// encode layers to metal commandBuffer
				if (inputImage == null)
					conv1.EncodeToCommandBuffer (commandBuffer, SrcImage, c1Image);
				else
					conv1.EncodeToCommandBuffer (commandBuffer, inputImage, c1Image);

				pool.EncodeToCommandBuffer (commandBuffer, c1Image, p1Image);
				conv2.EncodeToCommandBuffer (commandBuffer, p1Image, c2Image);
				pool.EncodeToCommandBuffer (commandBuffer, c2Image, p2Image);
				fc1.EncodeToCommandBuffer (commandBuffer, p2Image, fc1Image);
				fc2.EncodeToCommandBuffer (commandBuffer, fc1Image, dstImage);
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
	}
}
