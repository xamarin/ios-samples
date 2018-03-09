
namespace AVCamBarcode
{
    using AVCamBarcode.Extensions;
    using AVFoundation;
    using CoreAnimation;
    using CoreFoundation;
    using CoreGraphics;
    using CoreText;
    using Foundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using UIKit;

    public partial class CameraViewController : UIViewController, IAVCaptureMetadataOutputObjectsDelegate, IItemSelectionViewControllerDelegate
    {
        private readonly AVCaptureMetadataOutput metadataOutput = new AVCaptureMetadataOutput();

        /// <summary>
        /// Communicate with the session and other session objects on this queue.
        /// </summary>
        private readonly DispatchQueue sessionQueue = new DispatchQueue("session queue");

        private readonly AVCaptureSession session = new AVCaptureSession();

        private SessionSetupResult setupResult = SessionSetupResult.Success;

        private AVCaptureDeviceInput videoDeviceInput;

        private bool isSessionRunning;

        // KVO and Notifications

        private readonly List<MetadataObjectLayer> metadataObjectOverlayLayers = new List<MetadataObjectLayer>();

        private UITapGestureRecognizer openBarcodeURLGestureRecognizer;

        public CameraViewController(IntPtr handle) : base(handle) { }

        protected UITapGestureRecognizer OpenBarcodeURLGestureRecognizer
        {
            get
            {
                if (this.openBarcodeURLGestureRecognizer == null)
                {
                    this.openBarcodeURLGestureRecognizer = new UITapGestureRecognizer(this.OpenBarcodeUrl);
                }

                return this.openBarcodeURLGestureRecognizer;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Disable UI. The UI is enabled if and only if the session starts running.
            this.MetadataObjectTypesButton.Enabled = false;
            this.SessionPresetsButton.Enabled = false;
            this.CameraButton.Enabled = false;
            this.ZoomSlider.Enabled = false;

            // Add the open barcode gesture recognizer to the region of interest view.
            this.PreviewView.AddGestureRecognizer(this.OpenBarcodeURLGestureRecognizer);

            // Set up the video preview view.
            this.PreviewView.Session = session;

            // Check video authorization status. Video access is required and audio
            // access is optional. If audio access is denied, audio is not recorded
            // during movie recording.
            switch (AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video))
            {
                case AVAuthorizationStatus.Authorized:
                    // The user has previously granted access to the camera.
                    break;

                case AVAuthorizationStatus.NotDetermined:
                    // The user has not yet been presented with the option to grant
                    // video access. We suspend the session queue to delay session
                    // setup until the access request has completed.
                    this.sessionQueue.Suspend();
                    AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Video, (granted) =>
                    {
                        if (!granted)
                        {
                            this.setupResult = SessionSetupResult.NotAuthorized;
                        }

                        this.sessionQueue.Resume();
                    });
                    break;

                default:
                    // The user has previously denied access.
                    this.setupResult = SessionSetupResult.NotAuthorized;
                    break;
            }

            // Setup the capture session.
            // In general it is not safe to mutate an AVCaptureSession or any of its
            // inputs, outputs, or connections from multiple threads at the same time.
            //
            // Why not do all of this on the main queue?
            // Because AVCaptureSession.StartRunning() is a blocking call which can
            // take a long time. We dispatch session setup to the sessionQueue so
            // that the main queue isn't blocked, which keeps the UI responsive.
            this.sessionQueue.DispatchAsync(this.ConfigureSession);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.sessionQueue.DispatchAsync(() =>
            {
                switch (this.setupResult)
                {
                    case SessionSetupResult.Success:
                        // Only setup observers and start the session running if setup succeeded.
                        this.AddObservers();
                        this.session.StartRunning();
                        this.isSessionRunning = session.Running;

                        break;

                    case SessionSetupResult.NotAuthorized:
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            var message = "AVCamBarcode doesn't have permission to use the camera, please change privacy settings";
                            var alertController = UIAlertController.Create("AVCamBarcode", message, UIAlertControllerStyle.Alert);
                            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
                            alertController.AddAction(UIAlertAction.Create("Settings", UIAlertActionStyle.Default, action =>
                            {
                                UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
                            }));

                            this.PresentViewController(alertController, true, null);
                        });
                        break;

                    case SessionSetupResult.ConfigurationFailed:
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            var message = "Unable to capture media";
                            var alertController = UIAlertController.Create("AVCamBarcode", message, UIAlertControllerStyle.Alert);
                            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));

                            this.PresentViewController(alertController, true, null);
                        });
                        break;
                }
            });
        }

        public override void ViewWillDisappear(bool animated)
        {
            this.sessionQueue.DispatchAsync(() =>
            {
                if (this.setupResult == SessionSetupResult.Success)
                {
                    this.session.StopRunning();
                    this.isSessionRunning = this.session.Running;
                    this.RemoveObservers();
                }
            });

            base.ViewWillDisappear(animated);
        }

        public override bool ShouldAutorotate()
        {
            // Do now allow rotation if the region of interest is being resized
            return !this.PreviewView.IsResizingRegionOfInterest;
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            var videoPreviewLayerConnection = this.PreviewView.VideoPreviewLayer.Connection;
            if (videoPreviewLayerConnection != null)
            {
                var deviceOrientation = UIDevice.CurrentDevice.Orientation;
                if (deviceOrientation.IsPortrait() || deviceOrientation.IsLandscape())
                {
                    var newVideoOrientation = this.ConvertOrientation(deviceOrientation);
                    videoPreviewLayerConnection.VideoOrientation = newVideoOrientation;

                    // When we transition to the new size, we need to adjust the region
                    // of interest's origin and size so that it stays anchored relative
                    // to the camera.
                    coordinator.AnimateAlongsideTransition((context) => // animate
                    {
                        var newRegionOfInterest = this.PreviewView.VideoPreviewLayer.MapToLayerCoordinates(this.metadataOutput.RectOfInterest);
                        this.PreviewView.SetRegionOfInterestWithProposedRegionOfInterest(newRegionOfInterest);
                    }, (context) => // completion
                    {
                        // Remove the old metadata object overlays
                        this.RemoveMetadataObjectOverlayLayers();
                    });
                }
            }
        }

        private AVCaptureVideoOrientation ConvertOrientation(UIDeviceOrientation deviceOrientation)
        {
            var result = default(AVCaptureVideoOrientation);
            switch (deviceOrientation)
            {
                case UIDeviceOrientation.Portrait:
                    result = AVCaptureVideoOrientation.Portrait;
                    break;
                case UIDeviceOrientation.PortraitUpsideDown:
                    result = AVCaptureVideoOrientation.PortraitUpsideDown;
                    break;
                // TODO: change to logical naming after it will be fixed (map `LandscapeLeft` to `LandscapeLeft`)
                case UIDeviceOrientation.LandscapeLeft:
                    result = AVCaptureVideoOrientation.LandscapeRight;
                    break;
                case UIDeviceOrientation.LandscapeRight:
                    result = AVCaptureVideoOrientation.LandscapeLeft;
                    break;
                default:
                    throw new InvalidProgramException();
            }

            return result;
        }

        private void PresentItemSelectionViewController<T>(ItemSelectionViewController<T> itemSelectionViewController)
        {
            var navigationController = new UINavigationController(itemSelectionViewController);
            navigationController.NavigationBar.BarTintColor = UIColor.Black;
            navigationController.NavigationBar.TintColor = this.View.TintColor;

            this.PresentViewController(navigationController, true, null);
        }

        #region Session Management

        private readonly DispatchQueue metadataObjectsQueue = new DispatchQueue("metadata objects queue");

        private void ConfigureSession()
        {
            if (setupResult == SessionSetupResult.Success)
            {
                this.session.BeginConfiguration();

                // Add video input
                // Choose the back wide angle camera if available, otherwise default to the front wide angle camera
                AVCaptureDevice defaultVideoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Back) ??
                                                     AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Front) ??
                                                     null;

                if (defaultVideoDevice == null)
                {
                    Console.WriteLine("Could not get video device");
                    this.setupResult = SessionSetupResult.ConfigurationFailed;
                    this.session.CommitConfiguration();
                    return;
                }

                NSError error;
                var videoDeviceInput = AVCaptureDeviceInput.FromDevice(defaultVideoDevice, out error);
                if (this.session.CanAddInput(videoDeviceInput))
                {
                    this.session.AddInput(videoDeviceInput);
                    this.videoDeviceInput = videoDeviceInput;

                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        // Why are we dispatching this to the main queue?
                        // Because AVCaptureVideoPreviewLayer is the backing layer for PreviewView and UIView
                        // can only be manipulated on the main thread
                        // Note: As an exception to the above rule, it's not necessary to serialize video orientation changed
                        // on the AVCaptureVideoPreviewLayer's connection with other session manipulation
                        //
                        // Use the status bar orientation as the internal video orientation. Subsequent orientation changes are
                        // handled by CameraViewController.ViewWillTransition(to:with:).

                        var initialVideoOrientation = AVCaptureVideoOrientation.Portrait;
                        var statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                        if (statusBarOrientation != UIInterfaceOrientation.Unknown)
                        {
                            AVCaptureVideoOrientation videoOrintation;
                            if (Enum.TryParse(statusBarOrientation.ToString(), out videoOrintation))
                            {
                                initialVideoOrientation = videoOrintation;
                            }
                        }

                        this.PreviewView.VideoPreviewLayer.Connection.VideoOrientation = initialVideoOrientation;
                    });
                }
                else if (error != null)
                {
                    Console.WriteLine($"Could not create video device input: {error}");
                    this.setupResult = SessionSetupResult.ConfigurationFailed;
                    this.session.CommitConfiguration();
                    return;
                }
                else
                {
                    Console.WriteLine("Could not add video device input to the session");
                    this.setupResult = SessionSetupResult.ConfigurationFailed;
                    this.session.CommitConfiguration();

                    return;
                }


                // Add metadata output
                if (this.session.CanAddOutput(metadataOutput))
                {
                    this.session.AddOutput(metadataOutput);

                    // Set this view controller as the delegate for metadata objects
                    this.metadataOutput.SetDelegate(this, this.metadataObjectsQueue);
                    this.metadataOutput.MetadataObjectTypes = this.metadataOutput.AvailableMetadataObjectTypes; // Use all metadata object types by default

                    // Set an initial rect of interest that is 80% of the views's shortest side
                    // and 25% of the longest side. This means that the region on interest will
                    // appear in the same spot regardless of whether the app starts in portrait
                    // or landscape

                    var width = 0.25;
                    var height = 0.8;
                    var x = (1 - width) / 2;
                    var y = (1 - height) / 2;
                    var initialRectOfInterest = new CGRect(x, y, width, height);
                    this.metadataOutput.RectOfInterest = initialRectOfInterest;

                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        var initialRegionOfInterest = this.PreviewView.VideoPreviewLayer.MapToLayerCoordinates(initialRectOfInterest);
                        this.PreviewView.SetRegionOfInterestWithProposedRegionOfInterest(initialRegionOfInterest);
                    });
                }
                else
                {
                    Console.WriteLine("Could not add metadata output to the session");
                    this.setupResult = SessionSetupResult.ConfigurationFailed;
                    this.session.CommitConfiguration();

                    return;
                }

                this.session.CommitConfiguration();
            }
        }

        #endregion

        #region Presets

        partial void SelectSessionPreset(UIButton sender)
        {
            var controller = new ItemSelectionViewController<NSString>(this,
                                                                       SessionPresetItemSelectionIdentifier,
                                                                       new List<NSString>(AvailableSessionPresets()),
                                                                       new List<NSString> { this.session.SessionPreset },
                                                                       false);

            this.PresentItemSelectionViewController(controller);
        }

        private NSString[] AvailableSessionPresets()
        {
            return GetAllSessionPresets().Where(preset => session.CanSetSessionPreset(preset)).ToArray();
        }

        private static IEnumerable<NSString> GetAllSessionPresets()
        {
            yield return AVCaptureSession.PresetPhoto;
            yield return AVCaptureSession.PresetLow;
            yield return AVCaptureSession.PresetMedium;
            yield return AVCaptureSession.PresetHigh;
            yield return AVCaptureSession.Preset352x288;
            yield return AVCaptureSession.Preset640x480;
            yield return AVCaptureSession.Preset1280x720;
            yield return AVCaptureSession.PresetiFrame960x540;
            yield return AVCaptureSession.PresetiFrame1280x720;
            yield return AVCaptureSession.Preset1920x1080;
            yield return AVCaptureSession.Preset3840x2160;
        }

        #endregion

        #region Device Configuration

        partial void ChangeCamera(UIButton sender)
        {
            this.MetadataObjectTypesButton.Enabled = false;
            this.SessionPresetsButton.Enabled = false;
            this.CameraButton.Enabled = false;
            this.ZoomSlider.Enabled = false;

            // Remove the metadata overlay layers, if any.
            this.RemoveMetadataObjectOverlayLayers();

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var preferredPosition = AVCaptureDevicePosition.Unspecified;
                switch (this.videoDeviceInput.Device.Position)
                {
                    case AVCaptureDevicePosition.Unspecified:
                    case AVCaptureDevicePosition.Front:
                        preferredPosition = AVCaptureDevicePosition.Back;
                        break;

                    case AVCaptureDevicePosition.Back:
                        preferredPosition = AVCaptureDevicePosition.Front;
                        break;
                }

                var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
                var videoDevice = devices.FirstOrDefault(device => device.Position == preferredPosition);
                if (videoDevice != null)
                {
                    NSError error;
                    var captureDeviceInput = AVCaptureDeviceInput.FromDevice(videoDevice, out error);
                    if (error != null)
                    {
                        Console.WriteLine($"Error occurred while creating video device input: {error}");
                        return;
                    }

                    this.session.BeginConfiguration();

                    // Remove the existing device input first, since using the front and back camera simultaneously is not supported.
                    this.session.RemoveInput(this.videoDeviceInput);

                    // When changing devices, a session preset that may be supported
                    // on one device may not be supported by another. To allow the
                    // user to successfully switch devices, we must save the previous
                    // session preset, set the default session preset (High), and
                    // attempt to restore it after the new video device has been
                    // added. For example, the 4K session preset is only supported
                    // by the back device on the iPhone 6s and iPhone 6s Plus. As a
                    // result, the session will not let us add a video device that
                    // does not support the current session preset.
                    var previousSessionPreset = this.session.SessionPreset;
                    this.session.SessionPreset = AVCaptureSession.PresetHigh;

                    if (this.session.CanAddInput(captureDeviceInput))
                    {
                        this.session.AddInput(captureDeviceInput);
                        this.videoDeviceInput = captureDeviceInput;
                    }
                    else
                    {
                        this.session.AddInput(this.videoDeviceInput);
                    }

                    // Restore the previous session preset if we can.
                    if (this.session.CanSetSessionPreset(previousSessionPreset))
                    {
                        this.session.SessionPreset = previousSessionPreset;
                    }

                    this.session.CommitConfiguration();
                }

                this.MetadataObjectTypesButton.Enabled = true;
                this.SessionPresetsButton.Enabled = true;
                this.CameraButton.Enabled = true;
                this.ZoomSlider.Enabled = true;

                this.ZoomSlider.MaxValue = (float)NMath.Min(this.videoDeviceInput.Device.ActiveFormat.VideoMaxZoomFactor, 8);
                this.ZoomSlider.Value = (float)this.videoDeviceInput.Device.VideoZoomFactor;
            });
        }

        partial void ZoomCamera(UISlider sender)
        {
            var device = this.videoDeviceInput.Device;

            NSError error;
            this.videoDeviceInput.Device.LockForConfiguration(out error);
            if (error == null)
            {
                device.VideoZoomFactor = this.ZoomSlider.Value;
                device.UnlockForConfiguration();
            }
            else
            {
                Console.WriteLine($"Could not lock for configuration: {error}");
            }
        }

        #endregion

        #region KVO and Notifications

        private NSObject interruptionEndedNotificationToken;

        private NSObject wasInterruptedNotificationToken;

        private NSObject runtimeErrorNotificationToken;

        private IDisposable runningChangeToken;

        private void AddObservers()
        {
            this.runningChangeToken = this.session.AddObserver("running", NSKeyValueObservingOptions.New, this.OnRunningChanged);

            // Observe the previewView's regionOfInterest to update the AVCaptureMetadataOutput's
            // RectOfInterest when the user finishes resizing the region of interest.
            this.PreviewView.RegionOfInterestChanged += this.OnRegionOfInterestChanged;

            var notificationCenter = NSNotificationCenter.DefaultCenter;

            this.runtimeErrorNotificationToken = notificationCenter.AddObserver(AVCaptureSession.RuntimeErrorNotification, this.OnRuntimeErrorNotification, this.session);

            // A session can only run when the app is full screen. It will be interrupted
            // in a multi-app layout, introduced in iOS 9, see also the documentation of
            // AVCaptureSessionInterruptionReason.Add observers to handle these session
            // interruptions and show a preview is paused message.See the documentation
            // of AVCaptureSessionWasInterruptedNotification for other interruption reasons.
            this.wasInterruptedNotificationToken = notificationCenter.AddObserver(AVCaptureSession.WasInterruptedNotification, this.OnSessionWasInterrupted, this.session);
            this.interruptionEndedNotificationToken = notificationCenter.AddObserver(AVCaptureSession.InterruptionEndedNotification, this.OnSessionInterruptionEnded, this.session);
        }

        private void RemoveObservers()
        {
            this.runningChangeToken?.Dispose();
            this.runtimeErrorNotificationToken?.Dispose();
            this.wasInterruptedNotificationToken?.Dispose();
            this.interruptionEndedNotificationToken?.Dispose();
            this.PreviewView.RegionOfInterestChanged -= this.OnRegionOfInterestChanged;
        }

        private void OnRegionOfInterestChanged(object sender, EventArgs e)
        {
            var newRegion = (sender as PreviewView).RegionOfInterest;
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                // Ensure we are not drawing old metadata object overlays.
                this.RemoveMetadataObjectOverlayLayers();

                // Translate the preview view's region of interest to the metadata output's coordinate system.
                var metadataOutputRectOfInterest = this.PreviewView.VideoPreviewLayer.MapToMetadataOutputCoordinates(newRegion);

                this.sessionQueue.DispatchAsync(() =>
                {
                    // Update the AVCaptureMetadataOutput with the new region of interest
                    metadataOutput.RectOfInterest = metadataOutputRectOfInterest;
                });
            });
        }

        private void OnRunningChanged(NSObservedChange change)
        {
            var isSessionRunning = ((NSNumber)change.NewValue).BoolValue;

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.CameraButton.Enabled = isSessionRunning && AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video).Length > 1;
                this.MetadataObjectTypesButton.Enabled = isSessionRunning;
                this.SessionPresetsButton.Enabled = isSessionRunning;

                this.ZoomSlider.Enabled = isSessionRunning;
                this.ZoomSlider.MaxValue = (float)NMath.Min(this.videoDeviceInput.Device.ActiveFormat.VideoMaxZoomFactor, 8);
                this.ZoomSlider.Value = (float)(this.videoDeviceInput.Device.VideoZoomFactor);

                // After the session stop running, remove the metadata object overlays,
                // if any, so that if the view appears again, the previously displayed
                // metadata object overlays are removed.
                if (!isSessionRunning)
                {
                    this.RemoveMetadataObjectOverlayLayers();
                }

                // When the session starts running, the aspect ration of the video preview may also change if a new session present was applied .
                // To keep the preview view's region of interest within the visible portion of the video preview, the preview view's region of 
                // interest will need to be updates.
                if (isSessionRunning)
                {
                    this.PreviewView.SetRegionOfInterestWithProposedRegionOfInterest(this.PreviewView.RegionOfInterest);
                }
            });
        }

        private void OnRuntimeErrorNotification(NSNotification notification)
        {
            var args = new AVCaptureSessionRuntimeErrorEventArgs(notification);
            if (args.Error != null)
            {
                var error = (AVError)(long)args.Error.Code;
                Console.WriteLine($"Capture session runtime error: {error}");

                // Automatically try to restart the session running if media services were
                // reset and the last start running succeeded. Otherwise, enable the user
                // to try to resume the session running.

                if (error == AVError.MediaServicesWereReset)
                {
                    this.sessionQueue.DispatchAsync(() =>
                    {
                        if (this.isSessionRunning)
                        {
                            this.session.StartRunning();
                            this.isSessionRunning = session.Running;
                        }
                    });
                }
            }
        }

        private void OnSessionWasInterrupted(NSNotification notification)
        {
            // In some scenarios we want to enable the user to resume the session running.
            // For example, if music playback is initiated via control center while
            // using AVMetadataRecordPlay, then the user can let AVMetadataRecordPlay resume
            // the session running, which will stop music playback. Note that stopping
            // music playback in control center will not automatically resume the session
            // running. Also note that it is not always possible to resume

            var reasonIntegerValue = ((NSNumber)notification.UserInfo[AVCaptureSession.InterruptionReasonKey]).Int32Value;
            var reason = (AVCaptureSessionInterruptionReason)reasonIntegerValue;
            Console.WriteLine($"Capture session was interrupted with reason {reason}");

            if (reason == AVCaptureSessionInterruptionReason.VideoDeviceNotAvailableWithMultipleForegroundApps)
            {
                // Simply fade-in a label to inform the user that the camera is unavailable.
                this.CameraUnavailableLabel.Hidden = false;
                this.CameraUnavailableLabel.Alpha = 0;
                UIView.Animate(0.25d, () => this.CameraUnavailableLabel.Alpha = 1);
            }
        }

        private void OnSessionInterruptionEnded(NSNotification notification)
        {
            Console.WriteLine("Capture session interruption ended");

            if (this.CameraUnavailableLabel.Hidden)
            {
                UIView.Animate(0.25, () =>
                {
                    this.CameraUnavailableLabel.Alpha = 0;
                }, () =>
                {
                    this.CameraUnavailableLabel.Hidden = true;
                });
            }
        }

        #endregion
        
        #region Drawing Metadata Object Overlay Layers

        private NSTimer removeMetadataObjectOverlayLayersTimer;

        partial void SelectMetadataObjectTypes(UIButton sender)
        {
            var controller = new ItemSelectionViewController<AVMetadataObjectType>(this,
                                                                                   MetadataObjectTypeItemSelectionIdentifier,
                                                                                   this.metadataOutput.AvailableMetadataObjectTypes.GetFlags().ToList(),
                                                                                   this.metadataOutput.MetadataObjectTypes.GetFlags().ToList(),
                                                                                   true);

            this.PresentItemSelectionViewController(controller);
        }

        private MetadataObjectLayer CreateMetadataOverlay(AVMetadataObject metadataObject)
        {
            // Transform the metadata object so the bounds are updated to reflect those of the video preview layer.
            var transformedMetadataObject = this.PreviewView.VideoPreviewLayer.GetTransformedMetadataObject(metadataObject);

            // Create the initial metadata object overlay layer that can be used for either machine readable codes or faces.
            var metadataObjectOverlayLayer = new MetadataObjectLayer
            {
                LineWidth = 7,
                LineJoin = CAShapeLayer.JoinRound,
                MetadataObject = transformedMetadataObject,
                FillColor = this.View.TintColor.ColorWithAlpha(0.3f).CGColor,
                StrokeColor = this.View.TintColor.ColorWithAlpha(0.7f).CGColor,
            };

            var barcodeMetadataObject = transformedMetadataObject as AVMetadataMachineReadableCodeObject;
            if (barcodeMetadataObject != null)
            {
                var barcodeOverlayPath = this.BarcodeOverlayPathWithCorners(barcodeMetadataObject.Corners);
                metadataObjectOverlayLayer.Path = barcodeOverlayPath;

                // If the metadata object has a string value, display it.
                string textLayerString = null;
                if (!string.IsNullOrEmpty(barcodeMetadataObject.StringValue))
                {
                    textLayerString = barcodeMetadataObject.StringValue;
                }
                else
                {
                    // TODO: add Descriptor (line 618 in original iOS sample)
                }

                if (!string.IsNullOrEmpty(textLayerString))
                {
                    var barcodeOverlayBoundingBox = barcodeOverlayPath.BoundingBox;

                    var font = UIFont.BoldSystemFontOfSize(19).ToCTFont();
                    var textLayer = new CATextLayer
                    {
                        AlignmentMode = CATextLayer.AlignmentCenter,
                        Bounds = new CGRect(0, 0, barcodeOverlayBoundingBox.Size.Width, barcodeOverlayBoundingBox.Size.Height),
                        ContentsScale = UIScreen.MainScreen.Scale,
                        Position = new CGPoint(barcodeOverlayBoundingBox.GetMidX(), barcodeOverlayBoundingBox.GetMidY()),
                        Wrapped = true,

                        // Invert the effect of transform of the video preview so the text is oriented with the interface orientation
                        Transform = CATransform3D.MakeFromAffine(this.PreviewView.Transform).Invert(),
                        String = textLayerString,
                        AttributedString = new NSAttributedString(textLayerString, new CTStringAttributes
                        {
                            Font = font,
                            StrokeWidth = -5,
                            StrokeColor = UIColor.Black.CGColor,
                            ForegroundColor = UIColor.White.CGColor,
                        }),
                    };

                    textLayer.SetFont(font);
                    metadataObjectOverlayLayer.AddSublayer(textLayer);
                }
            }
            else if (transformedMetadataObject is AVMetadataFaceObject)
            {
                metadataObjectOverlayLayer.Path = CGPath.FromRect(transformedMetadataObject.Bounds);
            }

            return metadataObjectOverlayLayer;
        }

        private CGPath BarcodeOverlayPathWithCorners(CGPoint[] corners)
        {
            var path = new CGPath();

            if (corners.Any())
            {
                path.MoveToPoint(CGAffineTransform.MakeIdentity(), corners[0]);

                for (int i = 1; i < corners.Length; i++)
                {
                    path.AddLineToPoint(corners[i]);
                }

                path.CloseSubpath();
            }

            return path;
        }

        private void RemoveMetadataObjectOverlayLayers()
        {
            this.metadataObjectOverlayLayers.ForEach(layer => layer.RemoveFromSuperLayer());
            this.metadataObjectOverlayLayers.Clear();

            this.removeMetadataObjectOverlayLayersTimer?.Invalidate();
            this.removeMetadataObjectOverlayLayersTimer = null;
        }

        private void AddMetadataOverlayLayers(IEnumerable<MetadataObjectLayer> layers)
        {
            // Add the metadata object overlays as sublayers of the video preview layer. We disable actions to allow for fast drawing.
            CATransaction.Begin();
            CATransaction.DisableActions = true;

            foreach (var layer in layers)
            {
                this.PreviewView.VideoPreviewLayer.AddSublayer(layer);
                this.metadataObjectOverlayLayers.Add(layer); // Save the new metadata object overlays.
            }

            CATransaction.Commit();

            // Create a timer to destroy the metadata object overlays.
            this.removeMetadataObjectOverlayLayersTimer = NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(1), (param) => this.RemoveMetadataObjectOverlayLayers());
        }

        private void OpenBarcodeUrl(UITapGestureRecognizer openBarcodeURLGestureRecognizer)
        {
            foreach (var metadataObjectOverlayLayer in this.metadataObjectOverlayLayers)
            {
                var location = openBarcodeURLGestureRecognizer.LocationInView(this.PreviewView);
                if (metadataObjectOverlayLayer.Path.ContainsPoint(location, false))
                {
                    var barcodeMetadataObject = metadataObjectOverlayLayer.MetadataObject as AVMetadataMachineReadableCodeObject;
                    if (barcodeMetadataObject != null)
                    {
                        if (!string.IsNullOrEmpty(barcodeMetadataObject.StringValue))
                        {
                            var url = NSUrl.FromString(barcodeMetadataObject.StringValue);
                            if (UIApplication.SharedApplication.CanOpenUrl(url))
                            {
                                UIApplication.SharedApplication.OpenUrl(url);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region AVCaptureMetadataOutputObjectsDelegate

        private readonly AutoResetEvent resetEvent = new AutoResetEvent(true);

        [Export("captureOutput:didOutputMetadataObjects:fromConnection:")]
        public void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
        {
            // resetEvent is used to drop new notifications if old ones are still processing, to avoid queuing up a bunch of stale data.
            if (this.resetEvent.WaitOne(0))
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    this.RemoveMetadataObjectOverlayLayers();
                    this.AddMetadataOverlayLayers(metadataObjects.Select(this.CreateMetadataOverlay));

                    this.resetEvent.Set();
                });
            }
        }

        #endregion

        #region ItemSelectionViewControllerDelegate

        private const string MetadataObjectTypeItemSelectionIdentifier = "MetadataObjectTypes";

        private const string SessionPresetItemSelectionIdentifier = "SessionPreset";

        public void ItemSelectionViewController<T>(ItemSelectionViewController<T> itemSelectionViewController, IList<T> selectedItems)
        {
            var identifier = itemSelectionViewController.Identifier;
            if (identifier == MetadataObjectTypeItemSelectionIdentifier)
            {
                this.sessionQueue.DispatchAsync(() =>
                {
                    this.metadataOutput.MetadataObjectTypes = selectedItems.OfType<AVMetadataObjectType>().Aggregate((t1, t2) => t1 | t2);
                });
            }
            else if (identifier == SessionPresetItemSelectionIdentifier)
            {
                this.sessionQueue.DispatchAsync(() =>
                {
                    this.session.SessionPreset = selectedItems.OfType<NSString>().FirstOrDefault();
                });
            }
        }

        #endregion
    }
}