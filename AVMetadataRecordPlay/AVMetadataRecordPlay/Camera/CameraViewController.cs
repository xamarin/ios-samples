using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using AVFoundation;
using CoreFoundation;
using Foundation;
using CoreLocation;
using CoreGraphics;
using Photos;
using CoreVideo;

namespace AVMetadataRecordPlay.Camera
{
    public partial class CameraViewController : UIViewController, IAVCaptureFileOutputRecordingDelegate, ICLLocationManagerDelegate
    {



        public CameraViewController(IntPtr p) : base(p)
        {

        }

        public CameraViewController() : base("CameraViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            cameraButton.Enabled = false;
            recordButton.Enabled = false;

            previewView.session = session;

            switch (AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video))
            {
                case AVAuthorizationStatus.Authorized:
                    break;
                case AVAuthorizationStatus.NotDetermined:
                    sessionQueue.Suspend();
                    AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Video, granted =>
                    {
                        if (!granted)
                        {

                            this.setupResult = SessionSetupResult.notAuthorized;
                        }

                        this.sessionQueue.Resume();

                    });

                    break;
                default:
                    setupResult = SessionSetupResult.notAuthorized;
                    break;
            }

            sessionQueue.DispatchAsync(() =>
            {
                this.ConfigureSession();
            });

            locationManager.Delegate = (CLLocationManagerDelegate)Self;
            locationManager.RequestWhenInUseAuthorization();
            locationManager.DistanceFilter = CLLocationDistance.FilterNone;
            locationManager.HeadingFilter = 5.0;
            locationManager.DesiredAccuracy = CLLocation.AccuracyBest;


        }

        public override void ViewWillAppear(Boolean animated)
        {
            base.ViewWillAppear(animated);

            sessionQueue.DispatchAsync(() =>
            {
                switch (setupResult)
                {
                    case SessionSetupResult.success:
                        this.AddObservers;
                        this.session.StartRunning();
                        this.isSessionRunning = this.session.Running;
                        break;
                    case SessionSetupResult.notAuthorized:
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            var alertController = UIAlertController.Create("AVMetadataRecordPlay", "AVMetadataRecordPlay doesn’t have permission to use the camera, please change privacy settings", UIAlertControllerStyle.Alert);
                            alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            alertController.AddAction(UIAlertAction.Create("Settings", UIAlertActionStyle.Default, (actionSettings) =>
                            {
                                UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString), new NSDictionary(), null);
                            }));
                            this.PresentViewController(alertController, true, null);
                        });
                        break;
                    case SessionSetupResult.configurationFailed:
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            var alertController = UIAlertController.Create("AVMetadataRecordPlay", "Unable to capture media", UIAlertControllerStyle.Alert);
                            alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                            this.PresentViewController(alertController, true, null);
                        });
                        break;

                }
            });

        }

        public override void ViewDidDisappear(Boolean animated)
        {
            sessionQueue.DispatchAsync(() =>
            {
                if (this.setupResult == SessionSetupResult.success)
                {
                    this.session.StopRunning();
                    this.RemoveObservers();
                }
            });
            base.ViewDidDisappear(animated);
        }

        public override void ViewWillTransitionToSize(CoreGraphics.CGSize toSize, IUIViewControllerTransitionCoordinator coordinator){
            base.ViewWillTransitionToSize(toSize, coordinator);
            var videoPreviewLayerConnection = previewView.videoPreviewLayer.Connection;
            if (videoPreviewLayerConnection != null){
                coordinator.AnimateAlongsideTransition((context)=>{
                    var interfaceOrientation = UIApplication.SharedApplication.StatusBarOrientation;

					AVCaptureVideoOrientation newVideoOrientation;
                    if (TryConvertToVideoOrientation(interfaceOrientation, out newVideoOrientation)){
                        videoPreviewLayerConnection.VideoOrientation = newVideoOrientation;
                    }
						


                }, null);
            }
        }



		#region Session Management

		enum SessionSetupResult{
            success,
            notAuthorized,
            configurationFailed
        }

        AVCaptureSession session = new AVCaptureSession();

        bool isSessionRunning = false;

        SessionSetupResult setupResult = SessionSetupResult.success;

        AVCaptureDeviceInput videoDeviceInput;

        DispatchQueue sessionQueue = new DispatchQueue("sessionqueue");
 
        public void ConfigureSession(){
            if (setupResult != SessionSetupResult.success){
                return;
            }

            session.BeginConfiguration();

            //Add Video Input

            try
            {
                AVCaptureDevice defaultVideoDevice = null;
                var dualCameraDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualCamera, AVMediaType.Video, AVCaptureDevicePosition.Back);
                if (dualCameraDevice != null)
                {
                    defaultVideoDevice = dualCameraDevice;
                }
                else
                {
                    var backCameraDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Back);
                    if (backCameraDevice != null)
                    {
                        defaultVideoDevice = backCameraDevice;
                    }
                    else
                    {
                        var frontCameraDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Front);
                        if (frontCameraDevice != null)
                        {
                            defaultVideoDevice = frontCameraDevice;
                        }
                    }
                }
                var videoDeviceInput = AVCaptureDeviceInput.FromDevice(defaultVideoDevice);

                if (session.CanAddInput(videoDeviceInput)){
                    session.AddInput(videoDeviceInput);
                    this.videoDeviceInput = videoDeviceInput;
					
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        var statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                        AVCaptureVideoOrientation initialVideoOrientation = AVCaptureVideoOrientation.Portrait;
                        if (statusBarOrientation != UIInterfaceOrientation.Unknown){
							AVCaptureVideoOrientation videoOrientation;
                            if (TryConvertToVideoOrientation(statusBarOrientation, out videoOrientation)){
                                initialVideoOrientation = videoOrientation;
                            }
 
                        }
                        this.previewView.VideoPreviewLayer.Connection.VideoOrientation = initialVideoOrientation;
                    });
                }else{
                    Console.WriteLine("Could not add video device input to the session");
                    setupResult = SessionSetupResult.configurationFailed;
                    session.CommitConfiguration();
                    return;
                }

            }
            catch (NSErrorException e){
                Console.WriteLine($"Could not create video device input: {e.Error.LocalizedDescription}");
				setupResult = SessionSetupResult.configurationFailed;
				session.CommitConfiguration();
				return;
            }


			// Add Audio Input

			try
			{
                var audioDevice = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Audio);
                var audioDeviceInput = AVCaptureDeviceInput.FromDevice(audioDevice);

                if (session.CanAddInput(audioDeviceInput))
				{
                    session.AddInput(audioDeviceInput);

				}
				else
				{
					Console.WriteLine("Could not add audio device input to the session");

				}

			}
			catch (NSErrorException e)
			{
				Console.WriteLine($"Could not create audio device input: {e.Error.LocalizedDescription}");

			}

			// Add movie file output.

			if (session.CanAddOutput(movieFileOutput)){
                session.AddOutput(movieFileOutput);

                var movieFileOutputVideoConnection = movieFileOutput.ConnectionFromMediaType(AVMediaType.Video);
                if (movieFileOutputVideoConnection.SupportsVideoStabilization){
                    movieFileOutputVideoConnection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;
                }

                movieFileOutput.SetRecordsVideoOrientationAndMirroringChanges(true, movieFileOutputVideoConnection);

            }else{
				Console.WriteLine("Could not add video file output to the session");
				setupResult = SessionSetupResult.configurationFailed;
				session.CommitConfiguration();
				return;
            }

            // Make connections between all metadataInputPorts and the session.

            this.ConnectMetadataPorts();

            this.session.CommitConfiguration();

		
        }

        partial void ResumeInterruptedSession(UIButton sender)
        {
			sessionQueue.DispatchAsync(() =>
            {
                session.StartRunning();
                this.isSessionRunning = this.session.Running;
                if (!this.session.Running){
					DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
	                    var alertController = UIAlertController.Create("AVMetadataRecordPlay", "Unable to resume", UIAlertControllerStyle.Alert);
	                    alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
	                    this.PresentViewController(alertController, true, null);
                    });
                }else{
					DispatchQueue.MainQueue.DispatchAsync(() =>
					{
                        this.resumeButton.Hidden = true;
					});
                }
            });
        }


		#endregion


		#region Device Configuration

		AVCaptureDeviceDiscoverySession videoDeviceDiscoverySession = AVCaptureDeviceDiscoverySession.Create(
			new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInWideAngleCamera, AVCaptureDeviceType.BuiltInDuoCamera },
			AVMediaType.Video, AVCaptureDevicePosition.Unspecified);

		partial void ChangeCamera(UIButton sender)
		{
            this.cameraButton.Enabled = false;
            this.recordButton.Enabled = false;

			sessionQueue.DispatchAsync(() =>
			{

				AVCaptureDevice currentVideoDevice = videoDeviceInput.Device;
				AVCaptureDevicePosition currentPosition = currentVideoDevice.Position;

				AVCaptureDevicePosition preferredPosition = 0;
				AVCaptureDeviceType preferredDeviceType = 0;

				switch (currentPosition)
				{
					case AVCaptureDevicePosition.Unspecified:
					case AVCaptureDevicePosition.Front:
						preferredPosition = AVCaptureDevicePosition.Back;
						preferredDeviceType = AVCaptureDeviceType.BuiltInDuoCamera;
						break;

					case AVCaptureDevicePosition.Back:
						preferredPosition = AVCaptureDevicePosition.Front;
						preferredDeviceType = AVCaptureDeviceType.BuiltInWideAngleCamera;
						break;
				}

				var devices = videoDeviceDiscoverySession.Devices;
				AVCaptureDevice newVideoDevice = null;

				// First, look for a device with both the preferred position and device type. Otherwise, look for a device with only the preferred position.
				newVideoDevice = devices.FirstOrDefault(d => d.Position == preferredPosition && d.DeviceType == preferredDeviceType)
							  ?? devices.FirstOrDefault(d => d.Position == preferredPosition);

				if (newVideoDevice != null)
				{
					NSError error;
					var input = AVCaptureDeviceInput.FromDevice(newVideoDevice, out error);
					if (error == null)
					{
						session.BeginConfiguration();

						// Remove the existing device input first, since using the front and back camera simultaneously is not supported.
						session.RemoveInput(videoDeviceInput);

						if (session.CanAddInput(input))
						{
                            NSNotificationCenter.DefaultCenter.RemoveObserver(this, AVCaptureDevice.SubjectAreaDidChangeNotification, currentVideoDevice);
							
							NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, input.Device);
							session.AddInput(input);
							this.videoDeviceInput = input;
						}
						else
						{
							session.AddInput(videoDeviceInput);
						}

                        this.ConnectMetadataPorts();

                        var movieFileOutputVideoConnection = this.movieFileOutput.ConnectionFromMediaType(AVMediaType.Video);
						if (movieFileOutputVideoConnection != null)
						{
							// Enable video stabilization.
							if (movieFileOutputVideoConnection.SupportsVideoStabilization)
								movieFileOutputVideoConnection.PreferredVideoStabilizationMode = AVCaptureVideoStabilizationMode.Auto;

							// Enable video orientation timed metadata.
							this.movieFileOutput.SetRecordsVideoOrientationAndMirroringChanges(true, movieFileOutputVideoConnection);
						}

	
						session.CommitConfiguration();
                    }else{
                        Console.WriteLine($"Error ocurred while creating video device input: {error.LocalizedDescription}");
                    }
				}

				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
                    this.recordButton.Enabled = true;
                    this.cameraButton.Enabled = true;
				});
			});

		}

		partial void focusAndExposeTap(UITapGestureRecognizer sender)
		{
            var devicePoint = this.previewView.VideoPreviewLayer.CaptureDevicePointOfInterestForPoint(sender.LocationInView(sender.View));
            this.Focus(AVCaptureFocusMode.AutoFocus, AVCaptureExposureMode.AutoExpose, devicePoint, true);
		}

		void Focus(AVCaptureFocusMode focusMode, AVCaptureExposureMode exposureMode, CGPoint point, bool monitorSubjectAreaChange)
		{
			sessionQueue.DispatchAsync(() =>
			{
				var device = videoDeviceInput?.Device;
				if (device == null)
					return;

				NSError error;
				if (device.LockForConfiguration(out error))
				{
					// Setting (Focus/Exposure)PointOfInterest alone does not initiate a (focus/exposure) operation.
					// Set (Focus/Exposure)Mode to apply the new point of interest.
					if (device.FocusPointOfInterestSupported && device.IsFocusModeSupported(focusMode))
					{
						device.FocusPointOfInterest = point;
						device.FocusMode = focusMode;
					}
					if (device.ExposurePointOfInterestSupported && device.IsExposureModeSupported(exposureMode))
					{
						device.ExposurePointOfInterest = point;
						device.ExposureMode = exposureMode;
					}
					device.SubjectAreaChangeMonitoringEnabled = monitorSubjectAreaChange;
					device.UnlockForConfiguration();
				}
				else
				{
					Console.WriteLine($"Could not lock device for configuration: {error.LocalizedDescription}");
				}
			});
		}




		/*




 */

		#endregion


		#region Recording Movies

		AVCaptureMovieFileOutput movieFileOutput = new AVCaptureMovieFileOutput();

        nint backgroundRecordingID;

		partial void ToggleMovieRecording(UIButton sender)
		{
			var output = movieFileOutput;
			if (output == null)
				return;

			// Disable the Camera button until recording finishes, and disable
			// the Record button until recording starts or finishes.
			// See the AVCaptureFileOutputRecordingDelegate methods.

            cameraButton.Enabled = false;
			recordButton.Enabled = false;
            playerButton.Enabled = false;

			// Retrieve the video preview layer's video orientation on the main queue
			// before entering the session queue.We do this to ensure UI elements are
			// accessed on the main thread and session configuration is done on the session queue.
            var videoPreviewLayerOrientation = previewView.VideoPreviewLayer.Connection.VideoOrientation;

			sessionQueue.DispatchAsync(() =>
			{
				if (!output.Recording)
				{
					if (UIDevice.CurrentDevice.IsMultitaskingSupported)
					{
						// Setup background task. This is needed because the IAVCaptureFileOutputRecordingDelegate.FinishedRecording
						// callback is not received until AVCam returns to the foreground unless you request background execution time.
						// This also ensures that there will be time to write the file to the photo library when AVCam is backgrounded.
						// To conclude this background execution, UIApplication.SharedApplication.EndBackgroundTask is called in
						// IAVCaptureFileOutputRecordingDelegate.FinishedRecording after the recorded file has been saved.
						backgroundRecordingID = UIApplication.SharedApplication.BeginBackgroundTask(null);
					}

					// Update the orientation on the movie file output video connection before starting recording.
					AVCaptureConnection connection = movieFileOutput?.ConnectionFromMediaType(AVMediaType.Video);
					if (connection != null)
						connection.VideoOrientation = videoPreviewLayerOrientation;

					// Start recording to a temporary file.
					var outputFileName = new NSUuid().AsString();
					var outputFilePath = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(outputFileName, "mov"));
					output.StartRecordingToOutputFile(NSUrl.FromFilename(outputFilePath), this);
				}
				else
				{
					output.StopRecording();
                    this.locationManager.StopUpdatingLocation();
				}
			});
		}

		public void DidStartRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections)
		{
			// Enable the Record button to let the user stop the recording.
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
                recordButton.Enabled = true;
				recordButton.SetTitle("Stop", UIControlState.Normal);
			});
		}

		public void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			// Note that currentBackgroundRecordingID is used to end the background task associated with this recording.
			// This allows a new recording to be started, associated with a new UIBackgroundTaskIdentifier, once the movie file output's isRecording property
			// is back to false — which happens sometime after this method returns.
			// Note: Since we use a unique file path for each recording, a new recording will not overwrite a recording currently being saved.

			Action cleanup = () =>
			{
				var path = outputFileUrl.Path;
				if (NSFileManager.DefaultManager.FileExists(path))
				{
					NSError err;
					if (!NSFileManager.DefaultManager.Remove(path, out err))
						Console.WriteLine($"Could not remove file at url: {outputFileUrl}");

				}
				var currentBackgroundRecordingID = backgroundRecordingID;
				if (currentBackgroundRecordingID != -1)
				{
					backgroundRecordingID = -1;
					UIApplication.SharedApplication.EndBackgroundTask(currentBackgroundRecordingID);
				}
			};

			bool success = true;
			if (error != null)
			{
				Console.WriteLine($"Movie file finishing error: {error.LocalizedDescription}");
				success = ((NSNumber)error.UserInfo[AVErrorKeys.RecordingSuccessfullyFinished]).BoolValue;
			}

			if (success)
			{
				// Check authorization status.
				PHPhotoLibrary.RequestAuthorization(status =>
				{
					if (status == PHAuthorizationStatus.Authorized)
					{
						// Save the movie file to the photo library and cleanup.
						PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
						{
							var options = new PHAssetResourceCreationOptions
							{
								ShouldMoveFile = true
							};
							var creationRequest = PHAssetCreationRequest.CreationRequestForAsset();
							creationRequest.AddResource(PHAssetResourceType.Video, outputFileUrl, options);
						}, (success2, error2) =>
						{
							if (!success2)
								Console.WriteLine($"Could not save movie to photo library: {error2}");
							cleanup();
						});
					}
					else
					{
						cleanup();
					}
				});
			}
			else
			{
				cleanup();
			}

			// Enable the Camera and Record buttons to let the user switch camera and start another recording.
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				// Only enable the ability to change camera if the device has more than one camera.
				cameraButton.Enabled = UniqueDevicePositionsCount(videoDeviceDiscoverySession) > 1;
				recordButton.Enabled = true;
				recordButton.SetTitle("Record", UIControlState.Normal);
			});
		}



        #endregion


        #region Metadata Support

        private void ConnectMetadataPorts()
        {
            var specs = new string[] {CoreMedia.CMMetadataFormatDescription.FromMetadataSpecifications(null,new string[]{"asd"});};

        }

        private void ConnectSpecificMetadataPort (NSString metadataIdentifier){
            foreach (var inputPort in videoDeviceInput.Ports){
                if (inputPort.FormatDescription != null && inputPort.FormatDescription.MediaType == CoreMedia.CMMediaType.Metadata){
                    var metadataIdentifiers = ((CoreMedia.CMMetadataFormatDescription)inputPort.FormatDescription).GetIdentifiers();
                    if (metadataIdentifiers.Contains(metadataIdentifier)){
                        var connection = AVCaptureConnection.FromInputPorts(new AVCaptureInputPort[]{inputPort}, movieFileOutput);
                    }
                }
            }
        }

        private bool IsConnectionActiveWithInputPort (NSString portType){

            foreach (var connection in movieFileOutput.Connections)
			{
                foreach (var inputPort in connection.InputPorts)
				{
                    var formatDescription = (CoreMedia.CMMetadataFormatDescription)inputPort.FormatDescription;
                    if (formatDescription.MediaType == CoreMedia.CMMediaType.Metadata){
                        var metadataIdentifiers = formatDescription.GetIdentifiers();
                        if (metadataIdentifiers.Contains(portType)){
                            return connection.Active;
                        }

                    }
				}
			}

            return false;
        }

 

		/*
		  

		 
		 * 
		 * 
		 * private func connectMetadataPorts() {
        // Location metadata
        if !isConnectionActiveWithInputPort(AVMetadataIdentifierQuickTimeMetadataLocationISO6709) {
            // Create a format description for the location metadata.
            let specs = [kCMMetadataFormatDescriptionMetadataSpecificationKey_Identifier as String: AVMetadataIdentifierQuickTimeMetadataLocationISO6709,
                         kCMMetadataFormatDescriptionMetadataSpecificationKey_DataType as String: kCMMetadataDataType_QuickTimeMetadataLocation_ISO6709 as String]
            
            var locationMetadataDesc: CMFormatDescription?
            CMMetadataFormatDescriptionCreateWithMetadataSpecifications(kCFAllocatorDefault, kCMMetadataFormatType_Boxed, [specs] as CFArray, &locationMetadataDesc)
            
            // Create the metadata input and add it to the session.
            guard let newLocationMetadataInput = AVCaptureMetadataInput(formatDescription: locationMetadataDesc, clock: CMClockGetHostTimeClock())
                else {
                    print("Unable to obtain metadata input.")
                    return
            }
            session.addInputWithNoConnections(newLocationMetadataInput)
            
            // Connect the location metadata input to the movie file output.
            let inputPort = newLocationMetadataInput.ports[0]
            session.add(AVCaptureConnection(inputPorts: [inputPort], output: movieFileOutput))
            
            locationMetadataInput = newLocationMetadataInput
        }
        
        // Face metadata
        if !isConnectionActiveWithInputPort(AVMetadataIdentifierQuickTimeMetadataDetectedFace) {
            connectSpecificMetadataPort(AVMetadataIdentifierQuickTimeMetadataDetectedFace)
        }
    }
    

		private func connectSpecificMetadataPort(_ metadataIdentifier: String)
		{

			// Iterate over the videoDeviceInput's ports (individual streams of media data) and find the port that matches metadataIdentifier.
			for inputPort in videoDeviceInput.ports as! [AVCaptureInputPort] {

				guard(inputPort.formatDescription != nil) && (CMFormatDescriptionGetMediaType(inputPort.formatDescription) == kCMMediaType_Metadata),
                let metadataIdentifiers = CMMetadataFormatDescriptionGetIdentifiers(inputPort.formatDescription) as NSArray? else {
					continue

			}

				if metadataIdentifiers.contains(metadataIdentifier) {
					// Add an AVCaptureConnection to connect the input port to the AVCaptureOutput (movieFileOutput).
					if let connection = AVCaptureConnection(inputPorts: [inputPort], output: movieFileOutput) {
						session.add(connection)
	
				}
				}
			}
		}


		private func isConnectionActiveWithInputPort(_ portType: String) -> Bool {
        
        for connection in movieFileOutput.connections as! [AVCaptureConnection] {
            for inputPort in connection.inputPorts as! [AVCaptureInputPort] {
                if let formatDescription = inputPort.formatDescription, CMFormatDescriptionGetMediaType(formatDescription) == kCMMediaType_Metadata {
                    if let metadataIdentifiers = CMMetadataFormatDescriptionGetIdentifiers(inputPort.formatDescription) as NSArray? {
                        if metadataIdentifiers.contains(portType) {
                            return connection.isActive
                        }
                    }
                }
            }
        }
        
        return false
    }

         */

		#endregion

		#region Location


		private CLLocationManager locationManager = new CLLocationManager();


        #endregion

        #region KVO and Notifications

        private IntPtr sessionRunningObserveContext;

		private void AddObservers()
		{
            this.session.AddObserver(this, "running", NSKeyValueObservingOptions.New, sessionRunningObserveContext);

            NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureDevice.SubjectAreaDidChangeNotification, SubjectAreaDidChange, videoDeviceInput);
            NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.RuntimeErrorNotification, SessionRuntimeError, session);    


            NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.WasInterruptedNotification, SessionWasInterrupted, session);
            NSNotificationCenter.DefaultCenter.AddObserver(AVCaptureSession.InterruptionEndedNotification, SessionInterruptionEnded, session);
            NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, DeviceOrientationDidChange, null);

		}

        private void RemoveObservers()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
            this.session.RemoveObserver(this, "running", sessionRunningObserveContext);			

		}
        private void SessionWasInterrupted(NSNotification notification)
        {
            
        }
		private void SessionInterruptionEnded(NSNotification notification)
		{

		}
		private void DeviceOrientationDidChange(NSNotification notification)
		{

		}
		private void SubjectAreaDidChange(NSNotification notification)
		{

		}
		private void SessionRuntimeError(NSNotification notification)
		{

		}

		#endregion
		/*



*/

		#region IAVCaptureFileOutputRecordingDelegate

		[Export("captureOutput:didStartRecordingToOutputFileAtURL:fromConnections:")]
		public void DidStartRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections)
		{
			
		}

		public void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			
		}

		#endregion

		static bool TryConvertToVideoOrientation(UIDeviceOrientation orientation, out AVCaptureVideoOrientation result)
		{
			switch (orientation)
			{
				case UIDeviceOrientation.Portrait:
					result = AVCaptureVideoOrientation.Portrait;
					return true;

				case UIDeviceOrientation.PortraitUpsideDown:
					result = AVCaptureVideoOrientation.PortraitUpsideDown;
					return true;

				case UIDeviceOrientation.LandscapeLeft:
					result = AVCaptureVideoOrientation.LandscapeRight;
					return true;

				case UIDeviceOrientation.LandscapeRight:
					result = AVCaptureVideoOrientation.LandscapeLeft;
					return true;

				default:
					result = 0;
					return false;
			}
		}

		static bool TryConvertToVideoOrientation(UIInterfaceOrientation orientation, out AVCaptureVideoOrientation result)
		{
			switch (orientation)
			{
				case UIInterfaceOrientation.Portrait:
					result = AVCaptureVideoOrientation.Portrait;
					return true;

				case UIInterfaceOrientation.PortraitUpsideDown:
					result = AVCaptureVideoOrientation.PortraitUpsideDown;
					return true;

				case UIInterfaceOrientation.LandscapeLeft:
					result = AVCaptureVideoOrientation.LandscapeRight;
					return true;

				case UIInterfaceOrientation.LandscapeRight:
					result = AVCaptureVideoOrientation.LandscapeLeft;
					return true;

				default:
					result = 0;
					return false;
			}
		}


		static int UniqueDevicePositionsCount(AVCaptureDeviceDiscoverySession session)
		{
			var uniqueDevicePositions = new List<AVCaptureDevicePosition>();

			foreach (var device in session.Devices)
			{
				if (!uniqueDevicePositions.Contains(device.Position))
					uniqueDevicePositions.Add(device.Position);
			}

			return uniqueDevicePositions.Count;
		}



        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

