using System;
using System.Linq;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using UIKit;
using Vision;
using CoreVideo;
using Foundation;
using CoreMedia;
using CoreML;
using CoreImage;

namespace CustomVision
{
	public partial class ViewController : UIViewController, IAVCaptureVideoDataOutputSampleBufferDelegate
	{

		// controlling the pace of the machine vision analysis
		DateTime lastAnalysis = DateTime.Now;
		TimeSpan pace = new TimeSpan(0, 0, 0, 0, 800); // in milliseconds, classification will not repeat faster than this value

		// performance tracking
		bool trackPerformance = false; // use "true" for performance logging
		int frameCount = 0;
		int framesPerSample = 10;
		DateTime startDate = DateTime.Now;

		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		AVCaptureVideoPreviewLayer previewLayer;
		BubbleLayer bubbleLayer = new BubbleLayer("INITIALIZING");

		UIView topBlurView;
		UIView bottomBlurView;

		DispatchQueue queue = new DispatchQueue("videoQueue");
		AVCaptureSession captureSession = new AVCaptureSession();
		AVCaptureDevice captureDevice;
		AVCaptureVideoDataOutput videoOutput = new AVCaptureVideoDataOutput();
		int unknownCounter = 0; // used to track how many unclassified images in a row
		float confidence = 0.01f;

		// MARK: Load the Model

		CGSize targetImageSize = new CGSize(227, 227); // must match model data input

		VNCoreMLModel model;
		VNRequest[] classificationRequestAry;
		public VNRequest[] ClassificationRequest
		{
			get
			{
				if (model == null)
				{
					var modelPath = NSBundle.MainBundle.GetUrlForResource("Fruit", "mlmodelc");
                    NSError createErr, mlErr;
                    var mlModel = MLModel.Create(modelPath, out createErr);
                    model = VNCoreMLModel.FromMLModel(mlModel, out mlErr);
				}
				if (classificationRequestAry == null)
				{
					var classificationRequest = new VNCoreMLRequest(model, handleClassification);
					classificationRequestAry = new VNRequest[] { classificationRequest };
				}
				return classificationRequestAry;
			}
		}

		// MARK: Handle image classification results
		public void handleClassification(VNRequest request, NSError error)
		{

			var observations = request.GetResults<VNClassificationObservation>();
			if (observations == null)
			{
				Console.WriteLine("Error: no results");
			}

			var best = observations[0];
			if (best == null)
			{
				Console.WriteLine("Error: no observations");
			}

			// Use results to update user interface (includes basic filtering)
			Console.WriteLine($"{best.Identifier}: {best.Confidence:P0}");
			if (best.Identifier.IndexOf("Unknown", StringComparison.InvariantCulture) > -1
				|| best.Confidence < confidence)
			{
				if (unknownCounter < 3)
				{   // a bit of a low-pass filter to avoid flickering
					unknownCounter++;
				}
				else
				{
					unknownCounter = 0;
					DispatchQueue.MainQueue.DispatchAsync(() =>
					{
						bubbleLayer.String = "";
					});
				}
			}
			else
			{
				unknownCounter = 0;
				Console.WriteLine("Identified: " + best.Identifier.Trim());
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					bubbleLayer.String = $"{best.Identifier.Trim()} : {best.Confidence:P0}";
				});
			}
		}

		// MARK: Lifecycle

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			previewLayer = new AVCaptureVideoPreviewLayer(captureSession);
			//previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			previewView.Layer.AddSublayer(previewLayer);
			ConfigureBlurViews();

		}

		public override void ViewDidAppear(bool animated)
		{
			bubbleLayer.String = "Custom View: Fruit";
			bubbleLayer.Opacity = 0.0f;
			bubbleLayer.Position = new CGPoint(bottomBlurView.Frame.Width / 2.0, bottomBlurView.Frame.Height / 2);

			//lowerView.Layer.AddSublayer(bubbleLayer);
			bottomBlurView.Layer.AddSublayer(bubbleLayer);

			SetupCamera();
   		}

		private void ConfigureBlurViews()
		{
			var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Regular);
		 	topBlurView = new UIVisualEffectView(blur);
			this.View.AddSubview(topBlurView);
			bottomBlurView = new UIVisualEffectView(blur);
			View.AddSubview(bottomBlurView);
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			previewLayer.Frame = previewView.Bounds;
			topBlurView.Frame = new CGRect(previewLayer.Frame.Left, previewLayer.Frame.Top, previewLayer.Frame.Right, 150);
			bottomBlurView.Frame = new CGRect(previewLayer.Frame.Left, previewLayer.Frame.Bottom - 150, previewLayer.Frame.Right, 150);
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			var subviews = View.Subviews;
			foreach(var vw in subviews)
			{
				Console.WriteLine(vw.Frame);
			}
		}

		public void SetupCamera()
		{
			var deviceDiscovery = AVCaptureDeviceDiscoverySession.Create(
				new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInWideAngleCamera }, AVMediaType.Video, AVCaptureDevicePosition.Back);
			var device = deviceDiscovery.Devices.LastOrDefault();
			if (device != null)
			{
				captureDevice = device;
				BeginSession();
			}
		}

		public void BeginSession()
		{
			try
			{
				captureSession.BeginConfiguration();

				var settings = new CVPixelBufferAttributes
				{
					PixelFormatType = CVPixelFormatType.CV32BGRA
				};
				videoOutput.WeakVideoSettings = settings.Dictionary;
				videoOutput.AlwaysDiscardsLateVideoFrames = true;
				videoOutput.SetSampleBufferDelegateQueue(this, queue);

				captureSession.SessionPreset = AVCaptureSession.Preset1920x1080;
				captureSession.AddOutput(videoOutput);

                NSError err;
				var input = new AVCaptureDeviceInput(captureDevice, out err);
				if (err != null)
				{
					Console.WriteLine("AVCapture error: " + err);
				}
				captureSession.AddInput(input);

				videoConnection = videoOutput.ConnectionFromMediaType(AVMediaType.Video);

				captureSession.CommitConfiguration();
				captureSession.StartRunning();
				Console.WriteLine("started AV capture session");
			}
			catch
			{
				Console.WriteLine("error connecting to the capture device");
			}
		}


		// MARK: Video Data Delegate
		[Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
		public void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			try
			{
				var currentDate = DateTime.Now;
				//Console.WriteLine("DidOutputSampleBuffer: " + currentDate + " " + lastAnalysis + " " + currentDate.Subtract(lastAnalysis).Milliseconds);
				// control the pace of the machine vision to protect battery life
				if (currentDate - lastAnalysis >= pace)
				{
					lastAnalysis = currentDate;
				}
				else
				{
					//Console.WriteLine("-- skip --");
					return; // don't run the classifier more often than we need
				}
				// keep track of performance and log the frame rate
				if (trackPerformance)
				{
					frameCount = frameCount + 1;
					if (frameCount % framesPerSample == 0)
					{
						var diff = currentDate.Subtract(startDate);
						if (diff.Seconds > 0)
						{
							if (pace > TimeSpan.Zero)
							{
								Console.WriteLine("WARNING: Frame rate of image classification is being limited by \"pace\" setting. Set to 0.0 for fastest possible rate.");
							}
						}
						Console.WriteLine($"{diff.Seconds / framesPerSample}s per frame (average");
					}
					startDate = currentDate;
				}

				// Crop and resize the image data.
				// Note, this uses a Core Image pipeline that could be appended with other pre-processing.
				// If we don't want to do anything custom, we can remove this step and let the Vision framework handle
				// crop and resize as long as we are careful to pass the orientation properly.
				using (var croppedBuffer = CroppedSampleBuffer(sampleBuffer, targetImageSize))
				{
					if (croppedBuffer == null)
					{
						return;
					}
					try
					{
						VNImageOptions options = new VNImageOptions();
						classifierRequestHandler = new VNImageRequestHandler(croppedBuffer, options);
                        NSError err;
						classifierRequestHandler.Perform(ClassificationRequest, out err);
						if (err != null)
						{
							Console.WriteLine(err);
						}
					}
					catch (Exception error)
					{
						Console.WriteLine(error);
					}
				}
			}
			finally
			{
				sampleBuffer.Dispose();
			}
		}


		CIContext context = CIContext.Create();
        bool alreadySet = false;
		CIAffineTransform rotateTransform;
		CIAffineTransform scaleTransform;
		CIAffineTransform cropTransform;

		CVPixelBuffer resultBuffer;
		private VNImageRequestHandler classifierRequestHandler;
		private AVCaptureConnection videoConnection;

		public CVPixelBuffer CroppedSampleBuffer(CMSampleBuffer sampleBuffer, CGSize targetSize)
		{
			var imageBuffer = sampleBuffer.GetImageBuffer();
			if (imageBuffer == null)
			{
				throw new ArgumentException("Cannot convert to CVImageBuffer");
			}

			// Only doing these calculations once for efficiency.
			// If the incoming images could change orientation or size during a session, this would need to be reset when that happens.
			if (!alreadySet)
			{
				alreadySet = true;
				var imageSize = imageBuffer.EncodedSize;
				var rotatedSize = new CGSize(imageSize.Height, imageSize.Width);

				if (targetSize.Width > rotatedSize.Width || targetSize.Height > rotatedSize.Height)
				{
					throw new NotSupportedException("Captured image is smaller than image size for model.");
				}

				var shorterSide = rotatedSize.Width < rotatedSize.Height ? rotatedSize.Width : rotatedSize.Height;

				rotateTransform = new CIAffineTransform
				{
					Transform = new CGAffineTransform(0, -1, 1, 0, 0, shorterSide)
					//Transform = CGAffineTransform.MakeIdentity()
				};
		
				var scale = targetSize.Width / shorterSide;
				scaleTransform = new CIAffineTransform
				{
					Transform = CGAffineTransform.MakeScale(scale, scale),
				};

				var xDiff = rotatedSize.Width * scale - targetSize.Width;
				var yDiff = rotatedSize.Height * scale - targetSize.Height;

				cropTransform = new CIAffineTransform
				{
					//Transform = CGAffineTransform.MakeTranslation(xDiff / 2.0f, yDiff / 2.0f),
					Transform = CGAffineTransform.MakeIdentity()
				};

			}

			// Convert to CIImage because it is easier to manipulate
			var ciImage = CIImage.FromImageBuffer(imageBuffer);

			rotateTransform.Image = ciImage;
			scaleTransform.Image = rotateTransform.OutputImage;
			cropTransform.Image = scaleTransform.OutputImage;
			var cropped = cropTransform.OutputImage;

			
            // Note that the above pipeline could be easily appended with other image manipulations.
			// For example, to change the image contrast. It would be most efficient to handle all of
			// the image manipulation in a single Core Image pipeline because it can be hardware optimized.

			// Only need to create this buffer one time and then we can reuse it for every frame
			if (resultBuffer == null || resultBuffer.Handle == IntPtr.Zero)
			{
				byte[] data = new byte[(int)targetSize.Height * 4 * (int)targetSize.Width];

				resultBuffer = CVPixelBuffer.Create((nint)targetSize.Width, (nint)targetSize.Height, CVPixelFormatType.CV32BGRA, data, 4 * (nint)targetSize.Width, null); // HACK

				if (resultBuffer == null) throw new Exception("Can't allocate pixel buffer.");
			}

			context.Render(cropped, resultBuffer);

            //  For debugging
            //var image = ImageBufferToUIImage(resultBuffer);
            //Console.WriteLine("Image size: " + image.Size); // set breakpoint to see image being provided to CoreML

			return resultBuffer;
		}

        /// <summary>
        /// for debugging...
        /// </summary>
        public UIImage ImageBufferToUIImage(CVPixelBuffer imageBuffer)
		{
			//imageBuffer
			imageBuffer.Lock(CVPixelBufferLock.None);

			var baseAddress = imageBuffer.BaseAddress;
			var bytesPerRow = imageBuffer.BytesPerRow;

			var width = imageBuffer.Width;
			var height = imageBuffer.Height;

			var colorSpace = CGColorSpace.CreateDeviceRGB();
			var bitmapInfo = (uint)CGImageAlphaInfo.NoneSkipFirst | (uint)CGBitmapFlags.ByteOrder32Little;

			var context = new CGBitmapContext(baseAddress, width, height, 8, bytesPerRow, colorSpace, (CGImageAlphaInfo)bitmapInfo);

			var quartzImage = context?.ToImage();
			imageBuffer.Unlock(CVPixelBufferLock.None);

			var image = new UIImage(quartzImage, 1.0f, UIImageOrientation.Right);

            // HACK: save as file
			//var documentsDirectory = Environment.GetFolderPath
			//			 (Environment.SpecialFolder.Personal);
			//string jpgFilename = System.IO.Path.Combine(documentsDirectory, "Photo.jpg"); // hardcoded filename, overwritten each time
			//NSData imgData = image.AsJPEG();
			//NSError err = null;
			//if (imgData.Save(jpgFilename, false, out err))
			//{
			//	Console.WriteLine("saved as " + jpgFilename);
			//}
			//else
			//{
			//	Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
			//}

			return image;
		}
	}
}
