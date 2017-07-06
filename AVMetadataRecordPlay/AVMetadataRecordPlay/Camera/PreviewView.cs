using System;

using UIKit;
using Foundation;
using AVFoundation;
using ObjCRuntime;

namespace AVMetadataRecordPlay.Camera
{
	[Register("PreviewView")]
	public class PreviewView : UIView
	{
		static Class layerClass;

		public static Class LayerClass
		{
			[Export("layerClass")]
			get
			{
				return layerClass = layerClass ?? new Class(typeof(AVCaptureVideoPreviewLayer));
			}
		}

		public AVCaptureSession Session
		{
			get
			{
				return VideoPreviewLayer.Session;
			}
			set
			{
				VideoPreviewLayer.Session = value;
			}
		}

		public AVCaptureVideoPreviewLayer VideoPreviewLayer
		{
			get
			{
				return (AVCaptureVideoPreviewLayer)Layer;
			}
		}

		public PreviewView(IntPtr handle)
			: base(handle)
		{
		}
	}
}
