using System;
using System.Collections.Generic;
using System.IO;
using Foundation;
using UIKit;
using CoreML;
using Vision;
using ImageIO;
using CoreImage;
using CoreFoundation;
using CoreGraphics;

namespace CoreMLVision
{
	public partial class ViewController : UIViewController, IUIImagePickerControllerDelegate, IUINavigationControllerDelegate
	{
		#region Private Variables
		private CIImage InputImage;
		private VNDetectRectanglesRequest RectangleRequest;
		private VNCoreMLRequest ClassificationRequest;
		#endregion

		#region Constructors
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Configure UI
			CameraButton.Enabled = UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera);
			GalleryButton.Enabled = UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.SavedPhotosAlbum);

			// Load the ML model
			var bundle = NSBundle.MainBundle;
			var assetPath = bundle.GetUrlForResource("MNISTClassifier", "mlmodelc");
			var mlModel = MLModel.FromUrl(assetPath, out NSError mlErr);
			var model = VNCoreMLModel.FromMLModel(mlModel, out NSError vnErr);

			// Initialize
			RectangleRequest = new VNDetectRectanglesRequest(HandleRectangles);
			ClassificationRequest = new VNCoreMLRequest(model, HandleClassification);
		}
		#endregion

		#region Private Methods
		private void ShowAlert(string title, string message) {
			//Create Alert
			var okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

			//Add Action
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

			// Present Alert
			PresentViewController(okAlertController, true, null);
		}

		private void HandleClassification(VNRequest request, NSError error){

			var observations = request.GetResults<VNClassificationObservation>();
			if (observations == null)
			{
				ShowAlert("Processing Error", "Unexpected result type from VNCoreMLRequest.");
				return;
			}
			if (observations.Length < 1)
			{
				DispatchQueue.MainQueue.DispatchAsync(() => {
					ClassificationLabel.Text = "Can't Get Best Result";
				});
				return;
			}
			var best = observations[0];

			DispatchQueue.MainQueue.DispatchAsync(()=>{
				ClassificationLabel.Text = $"Classification: {best.Identifier} Confidence: {best.Confidence * 100f:#.00}%";
			});
		}

		private void HandleRectangles(VNRequest request, NSError error){

			var observations = request.GetResults<VNRectangleObservation>();
			if (observations == null) {
				ShowAlert("Processing Error","Unexpected result type from VNDetectRectanglesRequest.");
				return;
			}
			if (observations.Length < 1) {
				DispatchQueue.MainQueue.DispatchAsync(()=>{
					ClassificationLabel.Text = "No rectangles detected.";
				});
				return;
			}
			var detectedRectangle = observations[0];
			var imageSize = InputImage.Extent.Size;

			// Verify detected rectangle is valid.
			var boundingBox = detectedRectangle.BoundingBox.Scaled(imageSize);
			if (!InputImage.Extent.Contains(boundingBox)) {
				DispatchQueue.MainQueue.DispatchAsync(() => {
					ClassificationLabel.Text = "Invalid rectangle detected.";
				});
				return;
			}

			// Rectify the detected image and reduce it to inverted grayscale for applying model.
			var topLeft = detectedRectangle.TopLeft.Scaled(imageSize);
			var topRight = detectedRectangle.TopRight.Scaled(imageSize);
			var bottomLeft = detectedRectangle.BottomLeft.Scaled(imageSize);
			var bottomRight = detectedRectangle.BottomRight.Scaled(imageSize);

			var correctedImage = InputImage.ImageByCroppingToRect(boundingBox);

			var fp1 = new Dictionary<string, CGPoint>() {
				{"inputTopLeft", topLeft},
				{"inputTopRight", topRight},
				{"inputBottomLeft", bottomLeft},
				{"inputBottomRight", bottomRight}
			};
			correctedImage = correctedImage.CreateByFiltering("CIPerspectiveCorrection", fp1.ToNSDictionary());

			var fp2 = new Dictionary<NSString, NSNumber>() {
				{CIFilterInputKey.Saturation, new NSNumber(0)},
				{CIFilterInputKey.Contrast, new NSNumber(32)}
			};
			correctedImage = correctedImage.CreateByFiltering("CIColorControls", fp2.ToNSDictionary());

			var fp3 = new Dictionary<NSString, NSNumber>();
			correctedImage = correctedImage.CreateByFiltering("CIColorInvert", fp3.ToNSDictionary());

			// Show the pre-processed image
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				ClassificationLabel.Text = "Selected First Rectangle";
				CorrectedImageView.Image = new UIImage(correctedImage);
			});

			// Run the Core ML MNIST classifier -- results in handleClassification method
			var handler = new VNImageRequestHandler(correctedImage, new VNImageOptions());
			DispatchQueue.DefaultGlobalQueue.DispatchAsync(() => {
				handler.Perform(new VNRequest[] { ClassificationRequest }, out NSError err);
			});
		}
		#endregion

		#region UIImagePickerControllerDelegate
		[Export("imagePickerController:didFinishPickingMediaWithInfo:")]
		public void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
		{
			// Close the picker
			picker.DismissViewController(true, null);

			// Update UI
			ClassificationLabel.Text = "Analyizing Image...";
			CorrectedImageView.Image = null;

			// Read Image from returned data
			var uiImage = info[UIImagePickerController.OriginalImage] as UIImage;
			if (uiImage == null) {
				ShowAlert("Processing Error","Unable to read image from picker.");
				return;
			}

			// Convert to CIImage
			var ciImage = new CIImage(uiImage);
			if (ciImage == null) {
				ShowAlert("Processing Error", "Unable to create required CIImage from UIImage.");
				return;
			}
			InputImage = ciImage.CreateWithOrientation(uiImage.Orientation.ToCIImageOrientation());

			// Show source image
			ImageView.Image = uiImage;

			// Run the rectangle detector, which upon completion runs the ML classifier.
			var handler = new VNImageRequestHandler(ciImage, uiImage.Orientation.ToCGImagePropertyOrientation(), new VNImageOptions());
			DispatchQueue.DefaultGlobalQueue.DispatchAsync(()=>{
				handler.Perform(new VNRequest[] {RectangleRequest}, out NSError error);
			});
		}
		#endregion

		#region Custom Actions
		[Action("TakePicture:")]
		public void TakePicture(NSObject sender) {

			// Is the camera available?
			if (!UIImagePickerController.IsSourceTypeAvailable( UIImagePickerControllerSourceType.Camera))
			{
				// No report to user and cancel
				ShowAlert("Not Supported", "Sorry but the saved photos album is not available on this iOS device.");
				return;
			}

			// Create a picker to get the camera image
			var picker = new UIImagePickerController()
			{
				Delegate = this,
				SourceType = UIImagePickerControllerSourceType.Camera,
				CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo
			};

			// Display the picker
			PresentViewController(picker, true, null);
		}

		[Action("ChooseImage:")]
		public void ChooseImage(NSObject senter) {

			// Is the camera available?
			if (!UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.SavedPhotosAlbum))
			{
				// No report to user and cancel
				ShowAlert("Not Supported", "Sorry but the camera is not available on this iOS device.");
				return;
			}

			// Create a picker to get the camera image
			var picker = new UIImagePickerController()
			{
				Delegate = this,
				SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum
			};

			// Display the picker
			PresentViewController(picker, true, null);
		}
		#endregion
	}
}
