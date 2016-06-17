using AVFoundation;

namespace MediaCapture {
	public static class MediaDevices {
		static AVCaptureDevice frontCamera = null;
		public static AVCaptureDevice FrontCamera {
			get {
				frontCamera = frontCamera ?? GetCamera ("Front Camera");
				return frontCamera;
			}
		}

		static AVCaptureDevice backCamera = null;
		public static AVCaptureDevice BackCamera {
			get {
				backCamera = backCamera ?? GetCamera ("Back Camera");
				return backCamera;
			}
		}

		static AVCaptureDevice microphone = null;
		public static AVCaptureDevice Microphone {
			get {
				microphone = microphone ?? GetMicrophone ();
				return microphone;
			}
		}

		static AVCaptureDevice GetCamera (string localizedDeviceName)
		{
			var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video);
			foreach (AVCaptureDevice device in devices) {
				if (string.Compare (device.LocalizedName, localizedDeviceName, true) == 0)
					return device;
			}
			return null;
		}

		static AVCaptureDevice GetMicrophone()
		{
			var devices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Audio);
			foreach (AVCaptureDevice device in devices) {
				if (device.LocalizedName.ToLower().Contains ("microphone") == true)
					return device;
			}
			return null;
		}
	}
}

