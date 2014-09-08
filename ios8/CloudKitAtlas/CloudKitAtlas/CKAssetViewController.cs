using System;
using Foundation;
using UIKit;
using CoreFoundation;
using System.Drawing;
using CloudKit;

namespace CloudKitAtlas
{
	public partial class CKAssetViewController : UIViewController, ICloudViewController
	{
		public CloudManager CloudManager { get; set; }

		string assetRecordName;

		public CKAssetViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		async partial void DownloadPhoto (NSObject sender)
		{
			if (string.IsNullOrEmpty (assetRecordName)) {
				var alert = UIAlertController.Create ("CloudKitAtlas", "Upload an asset to retrieve it.", UIAlertControllerStyle.Alert);
				alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, (act) => {
					DismissViewController (true, null);
				}));

				await PresentViewControllerAsync (alert, true);
			} else {
				var record = await CloudManager.FetchRecordAsync (assetRecordName);

				var photoAsset = record ["photo"] as CKAsset;
				var image = UIImage.FromFile (photoAsset.FileUrl.Path);
				assetPreview.Image = image;
			}
		}

		partial void TakePhoto (NSObject sender)
		{
			var imagePicker = new UIImagePickerController ();
			var sourceType = UIImagePickerControllerSourceType.PhotoLibrary;

			if (UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera))
				sourceType = UIImagePickerControllerSourceType.Camera;

			imagePicker.SourceType = sourceType;
			imagePicker.FinishedPickingMedia += async (picker, e) => {
				UIImage image = e.Info [UIImagePickerController.OriginalImage] as UIImage;
				var newSize = new SizeF (512, 512);

				if (image.Size.Width > image.Size.Height)
					newSize.Height = (int)(newSize.Width * image.Size.Height / image.Size.Width);
				else
					newSize.Width = (int)(newSize.Height * image.Size.Width / image.Size.Height);

				UIGraphics.BeginImageContext (newSize);
				image.Draw (new RectangleF (0, 0, newSize.Width, newSize.Height));
				var data = UIGraphics.GetImageFromCurrentImageContext ().AsJPEG (0.75f);
				UIGraphics.EndImageContext ();

				NSError error;
				var cachesDirectory = NSFileManager.DefaultManager.GetUrl (NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User, null, true, out error);
				var temporaryName = new NSUuid ().AsString () + "jpeg";
				var localUrl = cachesDirectory.Append (temporaryName, false);
				data.Save (localUrl, true, out error);

				CKRecord record = await CloudManager.UploadAssetAsync (localUrl);
				if (record == null)
					return;

				assetRecordName = record.RecordId.RecordName;

				var alert = new UIAlertView ( "CloudKitAtlas", "Successfully Uploaded", null, "OK", null);
				alert.Show ();

				DismissViewController (true, null);
			};

			imagePicker.Canceled += (picker, e) => DismissViewController (true, null);

			PresentViewController (imagePicker, true, null);
		}
	}
}
