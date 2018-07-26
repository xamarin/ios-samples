using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreImage;
using System.Linq;
using System.Threading.Tasks;
using CoreFoundation;

namespace CoreMLImageRecognition
{
	public partial class ViewController : UIViewController
	{
		ImagePickerControllerDelegate imagePickerControllerDelegate;
		MachineLearningModel model;

		protected ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Configure UI
			CameraButton.Enabled = UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera);
			GalleryButton.Enabled = UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.SavedPhotosAlbum);


			// Configure Behavior
			imagePickerControllerDelegate = new ImagePickerControllerDelegate();
			imagePickerControllerDelegate.ErrorOccurred += (s, e) => ShowAlert("Processing Error", e.Value);
			imagePickerControllerDelegate.MessageUpdated += (s, e) => ShowMessage(e.Value);
			imagePickerControllerDelegate.ImagePicked += (s, e) =>
			{
				var img = UIImage.FromImage(e.Value);
				ShowImage(img);
				ClassifyImageAsync(img);
			};

			CameraButton.Clicked += ShowCamera;
			GalleryButton.Clicked += ShowGallery;
			ModelSwapButton.Clicked += SwapModels;

			// Configure Model
			model = new MachineLearningModel();
			model.PredictionsUpdated += (s, e) => ShowPrediction(e.Value);
			model.ErrorOccurred += (s, e) => ShowAlert("Processing Error", e.Value);
			model.MessageUpdated += (s, e) => ShowMessage(e.Value);
		}

		private void SwapModels(object sender, EventArgs e)
		{
			var currentModelName = ModelSwapButton.Title;
			var newModelName = currentModelName == "VGG16" ? "SqueezeNet" : "VGG16";
			model.SwitchToModel(newModelName);
			ModelSwapButton.Title = newModelName;
			//If there's an image, see what the new model thinks...
			if (ImageView.Image != null)
			{
				ClassifyImageAsync(ImageView.Image);
			}
		}

		private void ShowAlert(string title, string message)
		{
			var okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
			okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (_) => { }));
			InvokeOnMainThread(() => PresentViewController(okAlertController, true, () => { }));
		}

		void ShowMessage(string msg)
		{
			InvokeOnMainThread(() => ClassificationLabel.Text = msg);
		}

		private void ShowImage(UIImage img)
		{
			InvokeOnMainThread(() => ImageView.Image = img);
		}

		void ClassifyImageAsync(UIImage img)
		{
			Task.Run(() => model.Classify(img));
		}

		void ShowPrediction(ImageDescriptionPrediction imageDescriptionPrediction)
		{
			//Grab the first 5 predictions, format them for display, and show 'em
			InvokeOnMainThread(() =>
			{
				var message = $"{imageDescriptionPrediction.ModelName} thinks:\n";
				var topFive = imageDescriptionPrediction.predictions.Take(5);
				foreach (var prediction in topFive)
				{
					var prob = prediction.Item1;
					var desc = prediction.Item2;
					message += $"{desc} : {prob.ToString("P") }\n";
				}

				ShowMessage(message);
			});
		}

		void ShowCamera(object sender, EventArgs e)
		{
			// Is the camera available?
			if (!UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
			{
				// No report to user and cancel
				ShowAlert("Not Supported", "Sorry but the saved photos album is not available on this iOS device.");
				return;
			}

			// Create a picker to get the camera image
			var picker = new UIImagePickerController()
			{
				Delegate = imagePickerControllerDelegate,
				SourceType = UIImagePickerControllerSourceType.Camera,
				CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo
			};

			// Display the picker
			PresentViewController(picker, true, null);
		}

		void ShowGallery(object sender, EventArgs e)
		{
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
				Delegate = imagePickerControllerDelegate,
				SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum
			};

			// Display the picker
			PresentViewController(picker, true, null);
		}
	}

	class ImagePickerControllerDelegate : UIImagePickerControllerDelegate
	{
		public event EventHandler<EventArgsT<String>> MessageUpdated = delegate { };
		public event EventHandler<EventArgsT<String>> ErrorOccurred = delegate { };
		public event EventHandler<EventArgsT<CIImage>> ImagePicked = delegate { };

		public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
		{
			// Close the picker
			picker.DismissViewController(true, null);

			MessageUpdated(this, new EventArgsT<string>("Analyzing image..."));

			// Read Image from returned data
			var uiImage = info[UIImagePickerController.OriginalImage] as UIImage;
			if (uiImage == null)
			{
				ErrorOccurred(this, new EventArgsT<string>("Unable to read image from picker."));
				return;
			}

			// Convert to CIImage
			var ciImage = new CIImage(uiImage);
			if (ciImage == null)
			{
				ErrorOccurred(this, new EventArgsT<string>("Unable to create required CIImage from UIImage."));
				return;
			}
			var inputImage = ciImage.CreateWithOrientation(uiImage.Orientation.ToCIImageOrientation());

			ImagePicked(this, new EventArgsT<CIImage>(inputImage));

		}
	}
}
