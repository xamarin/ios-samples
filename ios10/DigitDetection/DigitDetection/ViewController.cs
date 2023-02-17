using System;

using UIKit;
using Foundation;
using Metal;
using MetalPerformanceShaders;

namespace DigitDetection {
	public partial class ViewController : UIViewController {
		// some properties used to control the app and store appropriate values
		// we will start with the simple 1 layer
		bool deep;

		IMTLCommandQueue commandQueue;
		IMTLDevice device;

		// Networks we have
		MnistFullLayerNeuralNetwork neuralNetwork;
		MnistDeepConvNeuralNetwork neuralNetworkDeep;
		MnistFullLayerNeuralNetwork runningNet;

		// loading MNIST Test Set here
		readonly MNISTData Mnistdata = new MNISTData ();

		// MNIST dataset image parameters
		nuint mnistInputWidth = 28;
		int mnistInputHeight = 28;
		int mnistInputNumPixels = 784;

		public ViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Load default device.
			device = MTLDevice.SystemDefault;

			// Make sure the current device supports MetalPerformanceShaders.
			if (!MPSKernel.Supports (device)) {
				Console.WriteLine ("Metal Performance Shaders not Supported on current Device");
				return;
			}

			// Create new command queue.
			commandQueue = device.CreateCommandQueue ();

			// initialize the networks we shall use to detect digits
			neuralNetwork = new MnistFullLayerNeuralNetwork (commandQueue);
			neuralNetworkDeep = new MnistDeepConvNeuralNetwork (commandQueue);

			runningNet = neuralNetwork;
		}

		partial void TappedDeepButton (UIButton sender)
		{
			// switch network to be used between the deep and the single layered
			if (deep) {
				sender.SetTitle ("Use Deep Net", UIControlState.Normal);
				runningNet = neuralNetwork;
			} else {
				sender.SetTitle ("Use Single Layer", UIControlState.Normal);
				runningNet = neuralNetworkDeep;
			}

			deep = !deep;
		}

		partial void TappedClear (UIButton sender)
		{
			// clear the digitview
			DigitView.Lines.Clear ();
			DigitView.SetNeedsDisplay ();
			PredictionLabel.Hidden = true;
		}

		partial void TappedTestSet (UIButton sender)
		{
			// placeholder to count number of correct detections on the test set
			var correctDetections = 0;
			var total = 10000f;
			AccuracyLabel.Hidden = false;

			Atomics.Reset ();

			// validate NeuralNetwork was initialized properly
			if (runningNet == null)
				throw new InvalidProgramException ();

			for (int i = 0; i < total; i++) {
				Inference (i, Mnistdata.LabelsCount);

				if (i % 100 == 0) {
					AccuracyLabel.Text = $"{i / 100}% Done";
					// this command helps update the UI in the loop regularly
					NSRunLoop.Current.RunUntil (NSRunLoopMode.Default, NSDate.DistantPast);
				}
			}
			// display accuracy of the network on the MNIST test set
			correctDetections = Atomics.GetCount ();

			AccuracyLabel.Hidden = false;
			AccuracyLabel.Text = $"Accuracy = {(correctDetections * 100) / total}%";
		}

		partial void TappedDetectDigit (UIButton sender)
		{
			// get the digitView context so we can get the pixel values from it to intput to network
			var context = DigitView.GetViewContext ();

			// validate NeuralNetwork was initialized properly
			if (runningNet == null)
				throw new InvalidProgramException ();

			// putting input into MTLTexture in the MPSImage
			var region = new MTLRegion (new MTLOrigin (0, 0, 0), new MTLSize ((nint) mnistInputWidth, mnistInputHeight, 1));
			runningNet.SrcImage.Texture.ReplaceRegion (region,
													   level: 0,
													   slice: 0,
													   pixelBytes: context.Data,
													   bytesPerRow: mnistInputWidth,
													   bytesPerImage: 0);
			// run the network forward pass
			var label = runningNet.Forward ();

			// show the prediction
			PredictionLabel.Text = $"{label}";
			PredictionLabel.Hidden = false;
		}

		/// <summary>
		/// This function runs the inference network on the test set
		/// </summary>
		/// <param name="imageNum">If the test set is being used we will get a value between 0 and 9999 for which of the 10,000 images is being evaluated</param>
		/// <param name="correctLabel">The correct label for the inputImage while testing</param>
		void Inference (int imageNum, int correctLabel)
		{
			// get the correct image pixels from the test set
			int startIndex = imageNum * mnistInputNumPixels;

			// create a source image for the network to forward
			var inputImage = new MPSImage (device, runningNet.SID);

			// put image in source texture (input layer)
			inputImage.Texture.ReplaceRegion (region: new MTLRegion (new MTLOrigin (0, 0, 0), new MTLSize ((nint) mnistInputWidth, mnistInputHeight, 1)),
											  level: 0,
											  slice: 0,
											  pixelBytes: Mnistdata.Images + startIndex,
											  bytesPerRow: mnistInputWidth,
											  bytesPerImage: 0);

			// run the network forward pass
			runningNet.Forward (inputImage, imageNum, correctLabel);
		}
	}
}
