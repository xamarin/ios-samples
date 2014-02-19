using System;
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;
using System.Linq;
using System.Drawing;

namespace QRchestra
{
	public class SessionManager : NSObject
	{
		public Action<SessionManager, NSError> StoppedRunning;

		public AVCaptureSession CaptureSession { get; set; }
		public List<AVMetadataObject> Barcodes { get; set; }
		
		DispatchQueue delegateCallbackQueue;
		DispatchQueue sessionQueue;

		List<double> previousSecondTimestamps;

		AVCaptureDeviceInput videoInput;
		AVCaptureDevice videoDevice;
		AVCaptureConnection audioConnection;
		AVCaptureConnection videoConnection;

		bool running;
		bool startCaptureSessionOnEnteringForeground;

		NSObject applicationWillEnterForegroundNotificationObserver;

		int pipelineRunningTask;

		AVCaptureMetadataOutput metadataOutput;

		MetadataObjectsDelegate metadataObjectsDelegate;

		public SessionManager ()
		{
			previousSecondTimestamps = new List<double> ();
			sessionQueue = new DispatchQueue ("com.apple.sample.sessionmanager.capture");
			pipelineRunningTask = 0;
		}

		public void StartRunning ()
		{
			sessionQueue.DispatchSync (delegate {
				try {
					setupCaptureSession ();
					CaptureSession.StartRunning ();
					running = true;
					metadataOutput.MetadataObjectTypes = new [] { AVMetadataObject.TypeQRCode };
				} catch (Exception e) {
					Console.WriteLine (e.Message);
				}
			});
		}

		public void StopRunning ()
		{
			sessionQueue.DispatchSync (delegate {
				running = false;

				CaptureSession.StopRunning ();

				captureSessionStoppedRunning ();

				teardownCaptureSession ();
			});
		}

		void setupCaptureSession ()
		{
			if (CaptureSession != null)
				return;

			CaptureSession = new AVCaptureSession ();

			NSNotificationCenter.DefaultCenter.AddObserver (null, captureSessionNotification, CaptureSession);

			applicationWillEnterForegroundNotificationObserver = 
				NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillEnterForegroundNotification.ToString (),
			                                                    UIApplication.SharedApplication,
					NSOperationQueue.CurrentQueue, delegate(NSNotification notification) {
				applicationWillEnterForeground ();                                          	
			});

			videoDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);

			NSError error;
			videoInput = new AVCaptureDeviceInput (videoDevice, out error);
			if (CaptureSession.CanAddInput (videoInput))
				CaptureSession.AddInput (videoInput);

			metadataOutput = new AVCaptureMetadataOutput ();

			var metadataQueue = new DispatchQueue ("com.AVCam.metadata");
			metadataObjectsDelegate = new MetadataObjectsDelegate {
				DidOutputMetadataObjectsAction = DidOutputMetadataObjects
			};
			metadataOutput.SetDelegate (metadataObjectsDelegate, metadataQueue);

			if (CaptureSession.CanAddOutput (metadataOutput))
				CaptureSession.AddOutput (metadataOutput);
		}

		void teardownCaptureSession ()
		{
			if (CaptureSession != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (this, null, CaptureSession);
				NSNotificationCenter.DefaultCenter.RemoveObserver (applicationWillEnterForegroundNotificationObserver);
				applicationWillEnterForegroundNotificationObserver = null;

				CaptureSession = null;
			}
		}

		[Export("captureSessionNotification:")]
		void captureSessionNotification  (NSNotification notification)
		{
			sessionQueue.DispatchAsync (delegate {
				if (notification.Name == AVCaptureSession.WasInterruptedNotification.ToString ()) {
					Console.WriteLine ("Session interrupted");

					captureSessionStoppedRunning ();
				} else if (notification.Name == AVCaptureSession.InterruptionEndedNotification.ToString ())
					Console.WriteLine ("Session interruption ended");
				else if (notification.Name == AVCaptureSession.RuntimeErrorNotification.ToString ()) {
					captureSessionStoppedRunning ();

					NSError error = (NSError)notification.UserInfo [AVCaptureSession.ErrorKey];
					if (error.Code == (int)AVError.DeviceIsNotAvailableInBackground) {
						Console.WriteLine ("Device not available in background");

						if (running)
							startCaptureSessionOnEnteringForeground = true;
					} else if (error.Code == (int)AVError.MediaServicesWereReset)
						Console.WriteLine ("Media services were reset");
					else
						handleNonRecoverableCaptureSessionRuntimeError (error);
				} else if (notification.Name == AVCaptureSession.DidStartRunningNotification)
					Console.WriteLine ("Session started running");
				else if (notification.Name == AVCaptureSession.DidStopRunningNotification)
					Console.WriteLine ("Session stopped running");
			});
		}

		void handleNonRecoverableCaptureSessionRuntimeError (NSError error)
		{
			Console.WriteLine (String.Format ("Fatal runtime error {0}, code {1}", error.Description, error.Code));

			running = false;
			teardownCaptureSession ();

			if (StoppedRunning != null)
				delegateCallbackQueue.DispatchAsync (delegate {
					StoppedRunning (this, error);
				});
		}

		void captureSessionStoppedRunning ()
		{
			teardownVideoPipeline ();
		}

		void applicationWillEnterForeground ()
		{
			sessionQueue.DispatchSync (delegate {
				startCaptureSessionOnEnteringForeground = false;
				if (running)
					CaptureSession.StartRunning ();
			});
		}

		void teardownVideoPipeline ()
		{
			videoPipelineFinishedRunning ();
		}

		void videoPipelineWillStartRunning ()
		{
			pipelineRunningTask = UIApplication.SharedApplication.BeginBackgroundTask (delegate {
				Console.WriteLine ("Video capture pipeline background task expired");
			});
		}

		void videoPipelineFinishedRunning ()
		{
			UIApplication.SharedApplication.EndBackgroundTask (pipelineRunningTask);
			pipelineRunningTask = 0;
		}

		public void DidOutputMetadataObjects (AVCaptureOutput captureOutput, 
		                               AVMetadataObject[] metadataObjects,
		                               AVCaptureConnection connection)
		{
			Barcodes = metadataObjects.ToList ();
		}

		bool supportsFocus {
			get {
				AVCaptureDevice device = videoInput.Device;

				return (device.IsFocusModeSupported (AVCaptureFocusMode.ModeLocked) ||
					device.IsFocusModeSupported (AVCaptureFocusMode.ModeAutoFocus) ||
					device.IsFocusModeSupported (AVCaptureFocusMode.ModeContinuousAutoFocus));
			}
		}

		public AVCaptureFocusMode FocusMode {
			get {
				return videoInput.Device.FocusMode;
			}
			set {
				AVCaptureDevice device = videoInput.Device;
				if (device.IsFocusModeSupported (value)) {
					NSError error;
					if (device.LockForConfiguration (out error)) {
						device.FocusMode = value;
						device.UnlockForConfiguration ();
					}
				}
			}
		}

		bool supportsExpose {
			get {
				AVCaptureDevice device = videoInput.Device;

				return (device.IsExposureModeSupported (AVCaptureExposureMode.Locked) ||
					device.IsExposureModeSupported (AVCaptureExposureMode.AutoExpose) ||
					device.IsExposureModeSupported (AVCaptureExposureMode.ContinuousAutoExposure));
			}
		}

		public AVCaptureExposureMode ExposureMode {
			get {
				return videoInput.Device.ExposureMode;
			}
			set {
				if (value != ExposureMode) {
					if (value == AVCaptureExposureMode.AutoExpose)
						value = AVCaptureExposureMode.ContinuousAutoExposure;
				}

				AVCaptureDevice device = videoInput.Device;
				if (device.IsExposureModeSupported (value)) {
					NSError error;
					if (device.LockForConfiguration (out error)) {
						device.ExposureMode = value;
						device.UnlockForConfiguration ();
					}
				}
			}
		}

		public void AutoFocus (PointF point)
		{
			AVCaptureDevice device = videoInput.Device;

			if (device.FocusPointOfInterestSupported && device.IsFocusModeSupported (AVCaptureFocusMode.ModeAutoFocus)) {
				NSError error;
				if (device.LockForConfiguration (out error)) {
					device.FocusPointOfInterest = point;
					device.FocusMode = AVCaptureFocusMode.ModeAutoFocus;
					device.UnlockForConfiguration ();
				}
			}
		}

		void ContinuousFocus (PointF point)
		{
			AVCaptureDevice device = videoInput.Device;

			if (device.FocusPointOfInterestSupported && device.IsFocusModeSupported (AVCaptureFocusMode.ModeContinuousAutoFocus)) {
				NSError error;
				if (device.LockForConfiguration (out error)) {
					device.FocusPointOfInterest = point;
					device.FocusMode = AVCaptureFocusMode.ModeContinuousAutoFocus;
					device.UnlockForConfiguration ();
				}
			}
		}

		public void Expose (PointF point)
		{
			AVCaptureDevice device = videoInput.Device;

			if (device.ExposurePointOfInterestSupported && device.IsExposureModeSupported (AVCaptureExposureMode.ContinuousAutoExposure)) {
				NSError error;
				if (device.LockForConfiguration (out error)) {
					device.ExposurePointOfInterest = point;
					device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
					device.UnlockForConfiguration ();
				}
			}
		}

		class MetadataObjectsDelegate : AVCaptureMetadataOutputObjectsDelegate
		{
			public Action<AVCaptureMetadataOutput, AVMetadataObject[], AVCaptureConnection> DidOutputMetadataObjectsAction;
		
			public override void DidOutputMetadataObjects (AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
			{
				if (DidOutputMetadataObjectsAction != null)
					DidOutputMetadataObjectsAction (captureOutput, metadataObjects, connection);
			}
		}
	}
}