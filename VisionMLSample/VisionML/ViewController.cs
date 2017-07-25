using System;
using Foundation;
using CoreFoundation;
using UIKit;
using CoreML;
using ImageIO;
using CoreImage;
using CoreGraphics;
using Vision;


namespace VisionML
{
    public partial class ViewController : UIViewController, IUIImagePickerControllerDelegate, IUINavigationControllerDelegate
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        partial void ChooseImage(UIBarButtonItem sender)
        {
            throw new NotImplementedException();
        }

        partial void TakePicture(UIBarButtonItem sender)
        {
            var picker = new UIImagePickerController();
            picker.Delegate = this;
            picker.SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum;
            PresentViewController(picker, true, ()=>{});
        }

        CIImage inputImage;

		[Foundation.Export("imagePickerController:didFinishPickingMediaWithInfo:")]
        public virtual void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info){

            picker.DismissViewController(true, ()=>{});
            classificationLabel.Text = "Analyzing Image...";
            correctedImageView.Image = null;
            var originalImg = new NSString("UIImagePickerControllerOriginalImage");
            var uiImage = (UIImage)info[originalImg];
            var ciImage = new CIImage(uiImage);
            var orientation = UIImageOrientationToCGImagePropertyOrientation(uiImage.Orientation);

            //inputImage = ciImage.CreateWithOrientation(orientation); TO DO WHEN BINDINGS ARE IMPLEMENTED
            inputImage = ciImage; //WORKAROUND WITHOUT APPLYING ORIENTATION

            imageView.Image = uiImage;

            var handler = new VNImageRequestHandler(ciImage, orientation, NSDictionary.FromDictionary(null));

            //PROBABLY NEED TO IMPLEMENT/BIND QOS MODIFIER IN GLOBALQUEUE
			DispatchQueue.DefaultGlobalQueue.DispatchAsync(() =>
			{
                var error = new NSError();
                handler.Perform(new VNRequest[] { this.RectanglesRequest }, out error);
			});

        }

        public VNCoreMLRequest ClassificationRequest
		{
			get
			{

				var bundle = NSBundle.MainBundle;
				var assetPath = bundle.GetUrlForResource("MNISTClassifier", "mlmodel");
				NSError err = null;
				var modelFromBundleModel = MLModel.FromUrl(assetPath, out err);
				if (err != null)
				{
					throw new Exception(err.ToString());
				}

                var model = VNCoreMLModel.FromMLModel(modelFromBundleModel, out err);

				if (err != null)
				{
					throw new Exception(err.ToString());
				}

                return new VNCoreMLRequest(model, this.HandleClassification);

			}

		}

        public void HandleClassification(VNRequest request, NSError error){
            var observations = (VNClassificationObservation[])request.GetResults<VNClassificationObservation>();
			if (observations.Length == 0)
			{
				this.classificationLabel.Text = "No results";
				return;
			}
            var best = observations[0];
            DispatchQueue.MainQueue.DispatchAsync(()=>{
                this.classificationLabel.Text = $"Classification: \"{(best.Identifier)}\" Confidence: {best.Confidence}";
            });
        }

		public VNDetectRectanglesRequest RectanglesRequest
		{
			get
			{
                //return new VNDetectRectanglesRequest((request, error) => ;)
                return new VNDetectRectanglesRequest(this.HandleRectangles);
                    
			}

		}


        public void HandleRectangles(VNRequest request, NSError error)
        {
            var observations = (VNRectangleObservation[])request.GetResults<VNRectangleObservation>();
            if (observations.Length == 0)
            {
                this.classificationLabel.Text = "No rectangles Detected";
                return;
            }
            var detectedRectangle = observations[0];
            var imageSize = inputImage.Extent.Size;

            // Verify detected rectangle is valid.
            var boundingBox = ScaleCGRectToSize(detectedRectangle.BoundingBox, imageSize);

            if (!inputImage.Extent.Contains(boundingBox))
            {
                Console.WriteLine("Invalid rectangle detected");
                return;
            }

            // Rectify the detected image and reduce it to inverted grayscale for applying model.
            var topLeft = ScaleCGPointToSize(detectedRectangle.TopLeft, imageSize);
            var topRight = ScaleCGPointToSize(detectedRectangle.TopRight, imageSize);
            var bottomLeft = ScaleCGPointToSize(detectedRectangle.BottomLeft, imageSize);
            var bottomRight = ScaleCGPointToSize(detectedRectangle.BottomRight, imageSize);

            NSDictionary inputParametersPerspectiveCorrection = NSDictionary.FromObjectsAndKeys(new CIVector[] { CIVector.Create(topLeft), CIVector.Create(topRight), CIVector.Create(bottomLeft), CIVector.Create(bottomRight) },
                                                                           new string[] { "inputTopLeft", "inputTopRight", "inputBottomLeft", "inputBottomright" });


            NSDictionary inputParametersColorControls = NSDictionary.FromObjectsAndKeys(new NSNumber[]{NSNumber.FromNUInt(0), NSNumber.FromNUInt(32) }, new NSString[] { CIFilterInputKey.Saturation, CIFilterInputKey.Contrast});



            var correctedImage = inputImage
                                .ImageByCroppingToRect(boundingBox)
                                .CreateByFiltering("CIPerspectiveCorrection", inputParametersPerspectiveCorrection)
                                .CreateByFiltering("CIColorControls", inputParametersColorControls)
                                .CreateByFiltering("CIColorInvert", null);


            // Show the pre-processed image
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.correctedImageView.Image = UIImage.FromImage(correctedImage);
            });

            // Run the Core ML MNIST classifier -- results in handleClassification method
            var handler = new VNImageRequestHandler(correctedImage, NSDictionary.FromDictionary(null));
            NSError err = null;
            handler.Perform(new VNRequest[]{this.ClassificationRequest}, out err);
			if (err != null)
			{
				throw new Exception(err.ToString());
			}
		}


		

		public CGImagePropertyOrientation UIImageOrientationToCGImagePropertyOrientation(UIImageOrientation orientation){
            switch (orientation){
                case UIImageOrientation.Down:
                    return CGImagePropertyOrientation.Down;
                case UIImageOrientation.DownMirrored:
					return CGImagePropertyOrientation.DownMirrored;
				case UIImageOrientation.Up:
					return CGImagePropertyOrientation.Up;
				case UIImageOrientation.UpMirrored:
					return CGImagePropertyOrientation.UpMirrored;
				case UIImageOrientation.Left:
					return CGImagePropertyOrientation.Left;
				case UIImageOrientation.LeftMirrored:
					return CGImagePropertyOrientation.LeftMirrored;
				case UIImageOrientation.Right:
					return CGImagePropertyOrientation.Right;
				case UIImageOrientation.RightMirrored:
					return CGImagePropertyOrientation.RightMirrored;
                default:
                    return CGImagePropertyOrientation.Up;    
            }
        }

        public CGRect ScaleCGRectToSize(CGRect original, CGSize toSize){
            return new CGRect(original.Location.X * toSize.Width,
                              original.Location.Y * toSize.Height,
                              original.Size.Width * toSize.Width,
                              original.Size.Height * toSize.Height
                             );
            
        }

		public CGPoint ScaleCGPointToSize(CGPoint original, CGSize toSize)
		{
			return new CGPoint(original.X * toSize.Width,
                               original.Y * toSize.Height);

		}


		public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
