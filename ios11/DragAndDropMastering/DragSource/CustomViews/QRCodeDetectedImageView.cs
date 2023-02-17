using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Foundation;

namespace DragSource {
	/**
	A QRCodeDetectedImageView represents an image view that
	contains QR codes. Each QR code is determined by a cropped
	image of the source image, and the rect indicating where in
	the source image the QR code appears.
	*/
	public class QRCodeDetectedImageView : UIImageView {
		#region Computed Properties
		public List<QRDetectionResult> QRCodes { get; set; } = new List<QRDetectionResult> ();
		#endregion

		#region Constructors
		public QRCodeDetectedImageView ()
		{
		}

		public QRCodeDetectedImageView (NSCoder coder) : base (coder)
		{
		}

		public QRCodeDetectedImageView (UIImage image) : base (image)
		{
			if (image != null) {
				var codes = image.ComputeQRCodeRectsAndMessages ();
				foreach ((CGRect rect, string message) in codes) {
					var croppedImage = image.CroppedImageInRect (rect);
					if (croppedImage != null) {
						var result = new QRDetectionResult (croppedImage, rect, message);
						QRCodes.Add (result);
					}
				}
			}
		}
		#endregion
	}
}
