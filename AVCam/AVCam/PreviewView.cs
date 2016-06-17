using System;

using UIKit;
using Foundation;
using AVFoundation;
using ObjCRuntime;

namespace AVCam
{
	[Register("AAPLPreviewView")]
	public class PreviewView : UIView
	{
		static Class layerClass;

		public static Class LayerClass
		{
			[Export("layerClass")]
			get {
				return layerClass = layerClass ?? new Class (typeof (AVCaptureVideoPreviewLayer));
			}
		}

		public AVCaptureSession Session {
			get {
				return PreviewLayer.Session;
			}
			set {
				PreviewLayer.Session = value;
			}
		}

		AVCaptureVideoPreviewLayer PreviewLayer {
			get {
				return (AVCaptureVideoPreviewLayer)Layer;
			}
		}

		public PreviewView (IntPtr handle)
			: base (handle)
		{
		}
	}
}