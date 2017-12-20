using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace DragSource
{
	public class QRDetectionResult : NSObject
	{
		#region Computed Properties
		public UIImage CroppedImage { get; set; }
		public CGRect RectInOriginalImage { get; set; } = CGRect.Empty;
		public string Message { get; set; } = "";
		#endregion

		#region Constructors
		public QRDetectionResult()
		{
		}

		public QRDetectionResult(UIImage croppedImage, CGRect rectInOriginalImage, string message)
		{
			// Initialize
			CroppedImage = croppedImage;
			RectInOriginalImage = rectInOriginalImage;
			Message = message;
		}
		#endregion
	}
}
