//
// how to capture still images, video and audio using iOS AVFoundation and the AVCAptureSession
//
// This sample handles all of the low-level AVFoundation and capture graph setup required to capture and save media.  This code also exposes the
// capture, configuration and notification capabilities in a more '.Netish' way of programming.  The client code will not need to deal with threads, delegate classes
// buffer management, or objective-C data types but instead will create .NET objects and handle standard .NET events.  The underlying iOS concepts and classes are detailed in
// the iOS developer online help (TP40010188-CH5-SW2).
//
// https://developer.apple.com/library/mac/#documentation/AudioVideo/Conceptual/AVFoundationPG/Articles/04_MediaCapture.html#//apple_ref/doc/uid/TP40010188-CH5-SW2
//
// Enhancements, suggestions and bug reports can be sent to steve.millar@infinitekdev.com
//
using System;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using Foundation;

namespace MediaCapture
{
	public static class MediaDevices
	{
		static AVCaptureDevice frontCamera = null;
		public static AVCaptureDevice FrontCamera {
			get {
				if ( frontCamera == null )
					frontCamera = GetCamera("Front Camera");
				return frontCamera;
			}
		}

		static AVCaptureDevice backCamera = null;
		public static AVCaptureDevice BackCamera
		{
			get {
				if ( backCamera == null )
					backCamera = GetCamera("Back Camera");
				return backCamera;
			}
		}

		static AVCaptureDevice microphone = null;
		public static AVCaptureDevice Microphone
		{
			get {
				if ( microphone == null )
					microphone = GetMicrophone();
				return microphone;
			}
		}

		// TODO - need better method of device detection than localized string
		static AVCaptureDevice GetCamera( string localizedDeviceName )
		{
			var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
			foreach ( AVCaptureDevice device in devices )
			{
				if ( string.Compare( device.LocalizedName, localizedDeviceName, true ) == 0 )
					return device;
			}
			return null;
		}

		// TODO - need better method of device detection than localized string
		static AVCaptureDevice GetMicrophone()
		{
			var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Audio);
			foreach ( AVCaptureDevice device in devices )
			{
				if ( device.LocalizedName.ToLower().Contains("microphone") == true )
					return device;
			}
			return null;
		}
	}
}