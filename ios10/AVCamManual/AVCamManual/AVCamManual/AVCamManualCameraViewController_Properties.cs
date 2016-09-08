using UIKit;
using Foundation;
using AVFoundation;

namespace AVCamManual
{
	public partial class AVCamManualCameraViewController
	{
		// Higher numbers will give the slider more sensitivity at shorter durations
		const float ExposureDurationPower = 5;

		// Limit exposure duration to a useful range
		const float ExposureMinDuration = 1f / 1000;

		[Outlet ("previewView")]
		AVCamManualPreviewView PreviewView { get; set; }

		[Outlet ("captureModeControl")]
		UISegmentedControl CaptureModeControl { get; set; }

		[Outlet ("cameraUnavailableLabel")]
		UILabel CameraUnavailableLabel { get; set; }

		[Outlet ("resumeButton")]
		UIButton ResumeButton { get; set; }

		[Outlet ("recordButton")]
		UIButton RecordButton { get; set; }

		[Outlet ("cameraButton")]
		UIButton CameraButton { get; set; }

		[Outlet ("photoButton")]
		UIButton PhotoButton { get; set; }

		[Outlet ("HUDButton")]
		UIButton HUDButton { get; set; }

		[Outlet ("manualHUD")]
		UIView ManualHUD { get; set; }

		[Outlet ("manualHUDFocusView")]
		UIView ManualHUDFocusView { get; set; }

		[Outlet ("focusModeControl")]
		UISegmentedControl FocusModeControl { get; set; }

		[Outlet ("lensPositionSlider")]
		UISlider LensPositionSlider { get; set; }

		[Outlet ("lensPositionNameLabel")]
		UILabel LensPositionNameLabel { get; set; }

		[Outlet ("lensPositionValueLabel")]
		UILabel LensPositionValueLabel { get; set; }

		[Outlet ("manualHUDExposureView")]
		UIView ManualHUDExposureView { get; set; }

		[Outlet ("exposureModeControl")]
		UISegmentedControl ExposureModeControl { get; set; }

		[Outlet ("exposureDurationSlider")]
		UISlider ExposureDurationSlider { get; set; }

		[Outlet ("exposureDurationNameLabel")]
		UILabel ExposureDurationNameLabel { get; set; }

		[Outlet ("exposureDurationValueLabel")]
		UILabel ExposureDurationValueLabel { get; set; }

		[Outlet ("ISOSlider")]
		UISlider ISOSlider { get; set; }

		[Outlet ("ISONameLabel")]
		UILabel IsoNameLabel { get; set; }

		[Outlet ("ISOValueLabel")]
		UILabel ISOValueLabel { get; set; }

		[Outlet ("exposureTargetBiasSlider")]
		UISlider ExposureTargetBiasSlider { get; set; }

		[Outlet ("exposureTargetBiasNameLabel")]
		UILabel ExposureTargetBiasNameLabel { get; set; }

		[Outlet ("exposureTargetBiasValueLabel")]
		UILabel ExposureTargetBiasValueLabel { get; set; }

		[Outlet ("exposureTargetOffsetSlider")]
		UISlider ExposureTargetOffsetSlider { get; set; }

		[Outlet ("exposureTargetOffsetNameLabel")]
		UILabel ExposureTargetOffsetNameLabel { get; set; }

		[Outlet ("exposureTargetOffsetValueLabel")]
		UILabel ExposureTargetOffsetValueLabel { get; set; }

		[Outlet ("manualHUDWhiteBalanceView")]
		UIView ManualHUDWhiteBalanceView { get; set; }

		[Outlet ("whiteBalanceModeControl")]
		UISegmentedControl WhiteBalanceModeControl { get; set; }

		[Outlet ("temperatureSlider")]
		UISlider TemperatureSlider { get; set; }

		[Outlet ("temperatureNameLabel")]
		UILabel TemperatureNameLabel { get; set; }

		[Outlet ("temperatureValueLabel")]
		UILabel TemperatureValueLabel { get; set; }

		[Outlet ("tintSlider")]
		UISlider TintSlider { get; set; }

		[Outlet ("tintNameLabel")]
		UILabel TintNameLabel { get; set; }

		[Outlet ("tintValueLabel")]
		UILabel TintValueLabel { get; set; }

		[Outlet ("manualHUDLensStabilizationView")]
		UIView ManualHUDLensStabilizationView { get; set; }

		[Outlet ("lensStabilizationControl")]
		UISegmentedControl LensStabilizationControl { get; set; }

		[Outlet ("manualHUDPhotoView")]
		UIView ManualHUDPhotoView { get; set; }

		[Outlet ("rawControl")]
		UISegmentedControl RawControl { get; set; }

		public AVCaptureSession Session {
			[Export("session")]
			get;
			[Export("setSession:")]
			set;
		}

		AVCaptureDeviceInput videoDeviceInput;

		public AVCaptureDeviceInput VideoDeviceInput {
			[Export("videoDeviceInput")]
			get {
				return videoDeviceInput;
			}
			[Export("setVideoDeviceInput:")]
			set {
				WillChangeValue ("videoDeviceInput");
				videoDeviceInput = value;
				DidChangeValue ("videoDeviceInput");
			}
		}

		AVCaptureDevice videoDevice;
		AVCaptureDevice VideoDevice {
			[Export ("videoDevice")]
			get {
				return videoDevice;
			}
			[Export ("setVideoDevice:")]
			set {
				WillChangeValue ("videoDevice");
				videoDevice = value;
				DidChangeValue ("videoDevice");
			}
		}

		AVCaptureVideoPreviewLayer PreviewLayer {
			get {
				return  (AVCaptureVideoPreviewLayer)PreviewView.Layer;
			}
		}
	}
}

