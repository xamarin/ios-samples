using System;
using Foundation;
using UIKit;
using Vision;
using CoreImage;
using CoreFoundation;
using CoreGraphics;

namespace VisionFramework
{
	public partial class ViewController : UIViewController, IUIImagePickerControllerDelegate, IUINavigationControllerDelegate
	{
		CIImage InputImage;
        UIImage RawImage;
		VNDetectRectanglesRequest RectangleRequest;
		
        protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		
        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Configure UI
			CameraButton.Enabled = UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera);
			GalleryButton.Enabled = UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.SavedPhotosAlbum);

			// Setup Vision Rectangles
			RectangleRequest = new VNDetectRectanglesRequest(HandleRectangles);
            RectangleRequest.MaximumObservations = 10; // limit on the number of rectangles to look for - can increase "thinking time"

		}

		void HandleRectangles(VNRequest request, NSError error){

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

            var summary = "";
            var imageSize = InputImage.Extent.Size;
            bool atLeastOneValid = false;
            Console.WriteLine("Rectangles:");
            summary += "Rectangles:" + Environment.NewLine;
            foreach (var o in observations){
                
				// Verify detected rectangle is valid.
				var boundingBox = o.BoundingBox.Scaled(imageSize);
				if (!InputImage.Extent.Contains(boundingBox))
				{
                    Console.WriteLine(" --- Rectangle out of bounds: " + boundingBox);
                    summary += " --- Rectangle out of bounds:" + boundingBox + Environment.NewLine;
                } else {
					Console.WriteLine(o.BoundingBox);
					summary += o.BoundingBox + Environment.NewLine;
                    atLeastOneValid |= true;
                }
            }

            if (!atLeastOneValid)
            {
				DispatchQueue.MainQueue.DispatchAsync(() => {
					ClassificationLabel.Text = "No _valid_ rectangles detected." + Environment.NewLine + summary;
				});
                return;
            }
			
            // Show the pre-processed image
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
                ClassificationLabel.Text = summary;
                ClassificationLabel.Lines = 0;
                ImageView.Image = OverlayRectangles(RawImage, imageSize, observations); 
			});
            	
		}


		public static UIImage OverlayRectangles(UIImage uiImage, CGSize imageSize, VNRectangleObservation[] observations)
		{
			nfloat fWidth = uiImage.Size.Width;
			nfloat fHeight = uiImage.Size.Height;

			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();

			using (CGBitmapContext ctx = new CGBitmapContext(IntPtr.Zero, (nint)fWidth, (nint)fHeight, 8, 4 * (nint)fWidth, CGColorSpace.CreateDeviceRGB(), CGImageAlphaInfo.PremultipliedFirst))
			{
                Console.WriteLine("Orientation:" +uiImage.Orientation);
                if (uiImage.Orientation == UIImageOrientation.Up)
                {   // correct orientation
                    ctx.DrawImage(new CGRect(0, 0, (double)fWidth, (double)fHeight), uiImage.CGImage);
                } 
                else 
                {   // need to rotate image so that rectangle overlays match
                    UIGraphics.BeginImageContextWithOptions(uiImage.Size, false, 0);
                    uiImage.Draw(new CGRect(0, 0, (double)fWidth, (double)fHeight));
                    var img = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();
                    ctx.DrawImage(new CGRect(0, 0, (double)fWidth, (double)fHeight), img.CGImage);
                }

				var count = 0;
                foreach (var o in observations)
                {
					// Draw rectangle
					var text = "Rectangle: " + count++.ToString();
                    Console.WriteLine(o.BoundingBox + " " + o.Confidence);

					var topLeft = o.TopLeft.Scaled(imageSize);
					var topRight = o.TopRight.Scaled(imageSize);
					var bottomLeft = o.BottomLeft.Scaled(imageSize);
					var bottomRight = o.BottomRight.Scaled(imageSize);

                    //set up drawing attributes
                    ctx.SetStrokeColor(UIColor.Red.CGColor);
                    ctx.SetLineWidth(10);

                    //create geometry
                    var path = new CGPath();

                    path.AddLines(new CGPoint[]{
                        topLeft, topRight, bottomRight, bottomLeft
                        });

                    path.CloseSubpath();

                    //add geometry to graphics context and draw it
                    ctx.AddPath(path);
                    ctx.DrawPath(CGPathDrawingMode.Stroke);


                    // Draw text
					ctx.SelectFont("Helvetica", 60, CGTextEncoding.MacRoman);
					//Measure the text's width - This involves drawing an invisible string to calculate the X position difference
					float start, end, textWidth;
					//Get the texts current position
					start = (float)ctx.TextPosition.X;
					//Set the drawing mode to invisible
					ctx.SetTextDrawingMode(CGTextDrawingMode.Invisible);
					//Draw the text at the current position
					ctx.ShowText(text);
					//Get the end position
					end = (float)ctx.TextPosition.X;
					//Subtract start from end to get the text's width
					textWidth = end - start;
					ctx.SetFillColor (UIColor.Red.CGColor);

					//Set the drawing mode back to something that will actually draw Fill for example
					ctx.SetTextDrawingMode(CGTextDrawingMode.Fill);

					//Draw the text at given coords.
                    ctx.ShowTextAtPoint (topLeft.X + 60, topLeft.Y - 60, text);


				}
				return UIImage.FromImage(ctx.ToImage());
			}
		}

		void ShowAlert(string title, string message)
		{
			//Create Alert
			var okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

			//Add Action
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

			// Present Alert
			PresentViewController(okAlertController, true, null);
		}

		#region UIImagePickerControllerDelegate
		[Export("imagePickerController:didFinishPickingMediaWithInfo:")]
		public void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
		{
			// Close the picker
			picker.DismissViewController(true, null);

			// Update UI
            ClassificationLabel.Text = "Analyzing Image (please wait)...";
		
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
            RawImage = uiImage;
			// Run the rectangle detector
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
