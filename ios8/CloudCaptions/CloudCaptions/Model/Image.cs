using System;
using System.IO;

using UIKit;
using CloudKit;
using Foundation;
using CoreGraphics;

namespace CloudCaptions
{
	public class Image : NSObject
	{
		public static readonly string RecordType = "Image";
		public static readonly string ThumbnailKey = "Thumb";
		public static readonly string FullsizeKey = "Full";

		public bool IsOnServer { get; set; }

		public UIImage FullImage { get; private set; }
		public CKRecord Record { get; private set; }
		public UIImage Thumbnail { get; private set; }

		// Creates an Image from a UIImage (photo was taken from camera or photo library)
		public Image (UIImage image)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			CGImage rawImage = image.CGImage;
			using (CGImage cropped = Crop (rawImage)) {
				// Resizes image to be 1500 x 1500 px and saves it to a temporary file
				UIImage fullImage = Resize (cropped, image.CurrentScale, image.Orientation, new CGSize (1500, 1500));
				var fullUrl = SaveToTmp (fullImage, "toUploadFull.tmp");

				// Resizes thumbnail to be 200 x 200 px and then saves to different temp file
				UIImage thumbImage = Resize (cropped, image.CurrentScale, image.Orientation, new CGSize (200, 200));
				var thumbURL = SaveToTmp (thumbImage, "toUploadThumb.tmp");

				// Creates Image record type with two assets, full sized image and thumbnail sized image
				CKRecord newImageRecord = new CKRecord (RecordType);
				newImageRecord [FullsizeKey] = new CKAsset (fullUrl);
				newImageRecord [ThumbnailKey] = new CKAsset (thumbURL);

				// Calls designated initalizer, this is a new image so it is not on the server
				Init (newImageRecord, false);
			}
		}

		// Creates an Image from a CKRecord that has been fetched
		public Image(CKRecord record, bool onServer = true)
		{
			if (record == null)
				throw new ArgumentNullException ("record");

			if (record.RecordType != RecordType)
				throw new ArgumentException ("Wrong type for image record");

			Init (record, onServer);
		}

		void Init (CKRecord record, bool onServer)
		{
			IsOnServer = onServer;
			Record = record;
			Thumbnail = LoadImage (ThumbnailKey);
			FullImage = LoadImage (FullsizeKey);
		}

		UIImage LoadImage (string key)
		{
			var asset = (CKAsset)Record [key];
			if (asset == null)
				return null;

			NSUrl url = asset.FileUrl;
			NSData data = NSData.FromUrl (url);

			return new UIImage (data);
		}

		static CGImage Crop (CGImage rawImage)
		{
			// Crops from top and bottom evenly
			var h = rawImage.Height;
			var w = rawImage.Width;

			if (h > w) {
				var offset = (h - w) / 2;
				return rawImage.WithImageInRect (new CGRect (0, offset, w, w));
			} else {
				var offset = (w - h) / 2;
				return rawImage.WithImageInRect (new CGRect (offset, 0, h, h));
			}
		}

		static UIImage Resize(CGImage original, nfloat scale, UIImageOrientation orientation, CGSize newSize)
		{
			UIGraphics.BeginImageContext(newSize);

			var rect = new CGRect (CGPoint.Empty, newSize);
			UIImage.FromImage (original, scale, orientation).Draw (rect);
			UIImage resized = UIGraphics.GetImageFromCurrentImageContext ();

			UIGraphics.EndImageContext ();

			return resized;
		}

		static NSUrl SaveToTmp (UIImage img, string fileName)
		{
			var tmp = Path.GetTempPath ();
			string path = Path.Combine (tmp, fileName);

			NSData imageData = img.AsJPEG (0.5f);
			imageData.Save (path, true);

			var url = new NSUrl (path, false);
			return url;
		}
	}
}

