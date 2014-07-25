
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreGraphics;

namespace Example_SharedResources.Screens.iPhone.Photos
{
	public partial class ImagePickerScreen : UIViewController
	{
		protected UIImagePickerController imagePicker;
		protected FeaturesTableDataSource tableSource;
		protected PickerDelegate pickerDelegate;	
		
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public ImagePickerScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ImagePickerScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ImagePickerScreen () : base("ImagePickerScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Image Picker";
			
			// populate the features table
			PopulateFeaturesTable ();
			
			btnChoosePhoto.TouchUpInside += (s, e) => {
				// create a new picker controller
				imagePicker = new UIImagePickerController ();
				
				// set our source to the photo library
				imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
								
				// set what media types
				imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary);
				
				// As with most controls, you can either handle the events directly, or 
				// wire up a delegate. in this first one, i wire up the events. in the other two,
				// i use a delegate
				imagePicker.FinishedPickingMedia += Handle_imagePickerhandleFinishedPickingMedia;
				imagePicker.Canceled += (sender,evt) => {
					Console.WriteLine ("picker cancelled");
					imagePicker.DismissModalViewController (true);
				};
				
				// show the picker
				NavigationController.PresentModalViewController (imagePicker, true);
			};
			
			btnTakePhoto.TouchUpInside += (s, e) => {

				try {
					// create a new picker controller
					imagePicker = new UIImagePickerController ();
				
					// set our source to the camera
					imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
								
					// set what media types
					imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.Camera);
				
					// show the camera controls
					imagePicker.ShowsCameraControls = true;

					// attach the delegate
					pickerDelegate = new ImagePickerScreen.PickerDelegate ();
					imagePicker.Delegate = pickerDelegate;
				
					// show the picker
					NavigationController.PresentModalViewController (imagePicker, true);

				} catch (Exception ex) {
					UIAlertView alert = new UIAlertView ("No Camera", "No Camera Detected!", null, "OK", null);
					alert.Show ();
				}
			};

					
			btnPhotoRoll.TouchUpInside += (s, e) => {
				// create a new picker controller
				imagePicker = new UIImagePickerController ();
				
				// set our source to the camera
				imagePicker.SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum;
								
				// set what media types
				imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.SavedPhotosAlbum);

				// attach the delegate
				pickerDelegate = new ImagePickerScreen.PickerDelegate ();
				imagePicker.Delegate = pickerDelegate;

				// show the picker
				NavigationController.PresentModalViewController (imagePicker, true);
				
			};
		}
		
		// This is a sample method that handles the FinishedPickingMediaEvent
		protected void Handle_imagePickerhandleFinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// determine what was selected, video or image
			bool isImage = false;
			switch (e.Info [UIImagePickerController.MediaType].ToString ()) {
			case "public.image":
				Console.WriteLine ("Image selected");
				isImage = true;
				break;
				Console.WriteLine ("Video selected");
			case "public.video":
				break;
			}
			
			Console.Write ("Reference URL: [" + UIImagePickerController.ReferenceUrl + "]");
			
			// get common info (shared between images and video)
			//NSUrl referenceURL = info[UIImagePickerController.ReferenceUrl] as NSUrl;
			NSUrl referenceURL = e.Info [new NSString ("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null) 
				Console.WriteLine (referenceURL.ToString ());
			
			// if it was an image, get the other image info
			if (isImage) {
				
				// get the original image
				UIImage originalImage = e.Info [UIImagePickerController.OriginalImage] as UIImage;
				if (originalImage != null) {
					// do something with the image
					Console.WriteLine ("got the original image");
				}
				
				// get the edited image
				UIImage editedImage = e.Info [UIImagePickerController.EditedImage] as UIImage;
				if (editedImage != null) {
					// do something with the image
					Console.WriteLine ("got the edited image");
				}
				
				//- get the image metadata
				NSDictionary imageMetadata = e.Info [UIImagePickerController.MediaMetadata] as NSDictionary;
				if (imageMetadata != null) {
					// do something with the metadata
					Console.WriteLine ("got image metadata");
				}
				
			}
			// if it's a video
			else {
				// get video url
				NSUrl mediaURL = e.Info [UIImagePickerController.MediaURL] as NSUrl;
				if (mediaURL != null) {
					//
					Console.WriteLine (mediaURL.ToString ());
				}
			}
			
			// dismiss the picker
			imagePicker.DismissModalViewController (true);
		}
		
		// Fills the table with a list of available features
		protected void PopulateFeaturesTable ()
		{
			// declare vars
			List<FeatureGroup> features = new List<FeatureGroup> ();
			FeatureGroup featGroup;
			string[] mediaTypes;
			
			// Sources
			featGroup = new FeatureGroup () { Name = "Sources" };

			try {

				featGroup.Features.Add (new Feature () { Name = "Camera", IsAvailable = UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera) });
			
			} catch (System.NullReferenceException ex) {
				UIAlertView alert = new UIAlertView ("No Camera", "No Camera Detected!", null, "OK", null);
				alert.Show ();
			}

			featGroup.Features.Add (new Feature () { Name = "Photo Library", IsAvailable = UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.PhotoLibrary) });
			featGroup.Features.Add (new Feature () { Name = "Saved Photos Album", IsAvailable = UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.SavedPhotosAlbum) });			
			features.Add (featGroup);
			
			// Camera and Flash
			featGroup = new FeatureGroup () { Name = "Camera and Flash" };
			try {

				featGroup.Features.Add (new Feature () { Name = "Front Camera", IsAvailable = UIImagePickerController.IsCameraDeviceAvailable (UIImagePickerControllerCameraDevice.Front) });			
				featGroup.Features.Add (new Feature () { Name = "Front Flash", IsAvailable = UIImagePickerController.IsFlashAvailableForCameraDevice (UIImagePickerControllerCameraDevice.Front) });			
				featGroup.Features.Add (new Feature () { Name = "Rear Camera", IsAvailable = UIImagePickerController.IsCameraDeviceAvailable (UIImagePickerControllerCameraDevice.Rear) });			
				featGroup.Features.Add (new Feature () { Name = "Rear Flash", IsAvailable = UIImagePickerController.IsFlashAvailableForCameraDevice (UIImagePickerControllerCameraDevice.Rear) });
				features.Add (featGroup);

			} catch (System.NullReferenceException ex) {
				UIAlertView alert = new UIAlertView ("No Camera", "No Camera Detected!", null, "OK", null);
				alert.Show ();
			}

			// Camera Media Types
			featGroup = new FeatureGroup () { Name = "Camera Media Types" };
			try {

				mediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.Camera);
				foreach (var mediaType in mediaTypes) {
					featGroup.Features.Add (new Feature () { Name = mediaType, IsAvailable = true });
				}
				features.Add (featGroup);

			} catch (System.NullReferenceException ex) {
				UIAlertView alert = new UIAlertView ("No Camera", "No Camera Detected!", null, "OK", null);
				alert.Show ();
			}
			
			// Photo Library Media Types
			featGroup = new FeatureGroup () { Name = "Photo Library Media Types" };
			mediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary);
			foreach (var mediaType in mediaTypes) {
				featGroup.Features.Add (new Feature () { Name = mediaType, IsAvailable = true });
			}
			features.Add (featGroup);
			
			// Saved Photos Album Media Types
			featGroup = new FeatureGroup () { Name = "Saved Photos Album Media Types" };
			mediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.SavedPhotosAlbum);
			foreach (var mediaType in mediaTypes) {
				featGroup.Features.Add (new Feature () { Name = mediaType, IsAvailable = true });
			}
			features.Add (featGroup);
			
			// bind to the table
			tableSource = new ImagePickerScreen.FeaturesTableDataSource (features);
			tblFeatures.Source = tableSource;
		}

		// Our custom picker delegate. The events haven't been exposed so we have to use a 
		// delegate.
		protected class PickerDelegate : UIImagePickerControllerDelegate
		{
			public override void Canceled (UIImagePickerController picker)
			{
				Console.WriteLine ("picker cancelled");
				picker.DismissModalViewController (true);
			}
						
			public override void FinishedPickingMedia (UIImagePickerController picker, NSDictionary info)
			{
				// determine what was selected, video or image
				bool isImage = false;
				switch (info [UIImagePickerController.MediaType].ToString ()) {
				case "public.image":
					Console.WriteLine ("Image selected");
					isImage = true;
					break;
					Console.WriteLine ("Video selected");
				case "public.video":
					break;
				}
				
				Console.Write ("Reference URL: [" + UIImagePickerController.ReferenceUrl + "]");
				
				// get common info (shared between images and video)
				//NSUrl referenceURL = info[UIImagePickerController.ReferenceUrl] as NSUrl;
				NSUrl referenceURL = info [new NSString ("UIImagePickerControllerReferenceUrl")] as NSUrl;
				if (referenceURL != null) {
					//
					Console.WriteLine (referenceURL.ToString ());
				}
				
				// if it was an image, get the other image info
				if (isImage) {
					// get the original image
					UIImage originalImage = info [UIImagePickerController.OriginalImage] as UIImage;
					if (originalImage != null) {
						// do something with the image
						Console.WriteLine ("got the original image");
					}
					
					// get the edited image
					UIImage editedImage = info [UIImagePickerController.EditedImage] as UIImage;
					if (editedImage != null) {
						// do something with the image
						Console.WriteLine ("got the edited image");
						
						
					}
					
//					// get the cropping, if any
//					try
//					{
//						CGRect cropRectangle = info[UIImagePickerController.CropRect] as CGRect;
//						if(cropRectangle != null)
//						{
//							// do something with the crop rectangle
//							Console.WriteLine("Got the crop rectangle");
//						}
//					} finally {}
					
					//- get the image metadata
					NSDictionary imageMetadata = info [UIImagePickerController.MediaMetadata] as NSDictionary;
					if (imageMetadata != null) {
						// do something with the metadata
						Console.WriteLine ("got image metadata");
					}
					
					
				}
				// if it's a video
				else {
					// get video url
					NSUrl mediaURL = info [UIImagePickerController.MediaURL] as NSUrl;
					if (mediaURL != null) {
						//
						Console.WriteLine (mediaURL.ToString ());
					}
					
				}
				
				// dismiss the picker
				picker.DismissModalViewController (true);
			}			
		}

		#region -= table stuff =-
		
		/// <summary>
		/// Group that holds features available
		/// </summary>
		protected class FeatureGroup
		{
			public string Name { get; set; }

			public List<Feature> Features
			{ get { return features; } set { features = value; } }

			protected List<Feature> features = new List<Feature> ();
		}
		
		/// <summary>
		/// A feature, such as whether or not the front camera is available
		/// </summary>
		protected class Feature
		{
			public string Name { get; set; }

			public bool IsAvailable { get; set; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected class FeaturesTableDataSource : UITableViewSource
		{
			protected List<FeatureGroup> features { get; set; }
			
			public FeaturesTableDataSource (List<FeatureGroup> features)
			{
				this.features = features;
			}
			
			public override nint NumberOfSections (UITableView tableView)
			{
				return features.Count;
			}
			
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return features [(int)section].Features.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("FeatureCell");
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Value1, "FeatureCell");
				
				cell.TextLabel.Text = features [(int)indexPath.Section].Features [(int)indexPath.Row].Name;
				cell.DetailTextLabel.Text = features [(int)indexPath.Section].Features [(int)indexPath.Row].IsAvailable.ToString ();
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				return cell;
			}
			
			public override string TitleForHeader (UITableView tableView, nint section)
			{
				return features [(int)section].Name;
			}
			
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 35;
			}
		}
		
		#endregion
	}
}

