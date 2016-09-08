using System;

using UIKit;
using Foundation;
using AVFoundation;
using ObjCRuntime;

namespace AVCamManual
{
	[Register ("AVCamManualPreviewView")]
	public class AVCamManualPreviewView : UIView
	{
		public static Class LayerClass {
			[Export ("layerClass")]
			get {
				return new Class (typeof (AVCaptureVideoPreviewLayer));
			}
		}

		public AVCaptureSession Session {
			get {
				return ((AVCaptureVideoPreviewLayer)Layer).Session;
			}
			set {
				((AVCaptureVideoPreviewLayer)Layer).Session = value;
			}
		}

		public AVCamManualPreviewView (IntPtr handle)
			: base (handle)
		{
		}
	}
}
