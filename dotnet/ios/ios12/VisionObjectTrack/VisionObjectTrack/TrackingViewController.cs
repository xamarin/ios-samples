
namespace VisionObjectTrack;

public partial class TrackingViewController : UIViewController, IVisionTrackerProcessorDelegate, IUIGestureRecognizerDelegate
{
        AVAsset videoAsset;

        VisionTrackerProcessor visionProcessor;

        DispatchQueue workQueue = new DispatchQueue ("com.apple.VisionTracker");

        TrackedObjectType trackedObjectType = TrackedObjectType.Object;

        List<TrackedPolyRect> objectsToTrack = new List<TrackedPolyRect> ();

        State state = State.Stopped;

        protected TrackingViewController (IntPtr handle) : base (handle) { }

        public AVAsset VideoAsset
        {
                get
                {
                        return videoAsset;
                }

                set
                {
                        videoAsset = value;
                        visionProcessor = new VisionTrackerProcessor (videoAsset) { Delegate = this };
                }
        }

        public State State
        {
                get
                {
                        return state;
                }

                set
                {
                        state = value;
                        HandleStateChange ();
                }
        }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();

                var backplateLayer = frameCounterLabelBackplate.Layer;
                backplateLayer.CornerRadius = backplateLayer.Bounds.Height / 2f;

                frameCounterLabelBackplate.Hidden = true;
                frameCounterLabel.Font = UIFont.MonospacedDigitSystemFontOfSize (16, UIFontWeight.Light);
                handleModeSelection (modeSelector);
        }

        public override void ViewDidAppear (bool animated)
        {
                base.ViewDidAppear (animated);

                workQueue.DispatchAsync (DisplayFirstVideoFrame);
        }

        public override void ViewWillDisappear (bool animated)
        {
                visionProcessor.CancelTracking ();
                base.ViewWillDisappear (animated);
        }

        public override bool PrefersStatusBarHidden ()
        {
                return true;
        }

        void DisplayFirstVideoFrame ()
        {
                var isTrackingRects = trackedObjectType == TrackedObjectType.Rectangle;
                visionProcessor.ReadAndDisplayFirstFrame (isTrackingRects, out NSError? error);
                if (error is not null)
                {
                        HandleError (error);
                }
        }

        void StartTracking ()
        {
                visionProcessor.PerformTracking (trackedObjectType, out NSError? error);
                if (error is not null)
                {
                        HandleError (error);
                }
        }

        void HandleError (NSError error)
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        string? title = null;
                        string? message = null;

                        if (error is VisionTrackerProcessorError processorError)
                        {
                                title = "Vision Processor Error";
                                switch (processorError.Type)
                                {
                                        case VisionTrackerProcessorErrorType.FirstFrameReadFailed:
                                                message = "Cannot read the first frame from selected video.";
                                                break;

                                        case VisionTrackerProcessorErrorType.ObjectTrackingFailed:
                                                message = "Tracking of one or more objects failed.";
                                                break;

                                        case VisionTrackerProcessorErrorType.ReaderInitializationFailed:
                                                message = "Cannot create a Video Reader for selected video.";
                                                break;

                                        case VisionTrackerProcessorErrorType.RectangleDetectionFailed:
                                                message = "Rectagle Detector failed to detect rectangles on the first frame of selected video.";
                                                break;
                                }
                        } else {
                                title = "Error";
                                message = error.LocalizedDescription;
                        }

                        var alert = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
                        alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null));
                        PresentViewController (alert, true, null);
                });
        }

        void HandleStateChange ()
        {
                UIBarButtonItem? newBarButton = null;
                var navBarHidden = false;
                var frameCounterHidden = false;

                switch (state)
                {
                        case State.Stopped:
                                navBarHidden = false;
                                frameCounterHidden = true;

                                // reveal settings view
                                trackingViewTopConstraint.Constant = 0;
                                entitySelector.Enabled = true;
                                newBarButton = new UIBarButtonItem (UIBarButtonSystemItem.Play, (s, e) => handleStartStopButton (null));
                                break;

                        case State.Tracking:
                                navBarHidden = true;
                                frameCounterHidden = false;

                                // cover settings view
                                trackingViewTopConstraint.Constant = -settingsView.Frame.Height;
                                entitySelector.Enabled = false;
                                newBarButton = new UIBarButtonItem (UIBarButtonSystemItem.Stop, (s, e) => handleStartStopButton (null));
                                break;
                }

                NavigationController?.SetNavigationBarHidden (navBarHidden, true);
                UIView.Animate (0.5d, () =>
                {
                        View!.LayoutIfNeeded ();
                        NavigationItem.RightBarButtonItem = newBarButton;
                        frameCounterLabelBackplate.Hidden = frameCounterHidden;
                });
        }

        partial void handleEntitySelection (UISegmentedControl sender)
        {
                switch (sender.SelectedSegment)
                {
                        case 0:
                                trackedObjectType = TrackedObjectType.Object;
                                NavigationItem.Prompt = "Drag to select objects";
                                clearRectsButton.Enabled = true;
                                break;

                        case 1:
                                trackedObjectType = TrackedObjectType.Rectangle;
                                NavigationItem.Prompt = "Rectangles are detected automatically";
                                clearRectsButton.Enabled = false;
                                break;
                }

                workQueue.DispatchAsync (() => DisplayFirstVideoFrame ());
        }

        partial void handleModeSelection (UISegmentedControl sender)
        {
                switch (sender.SelectedSegment)
                {
                        case 0:
                                visionProcessor.TrackingLevel = VNRequestTrackingLevel.Fast;
                                break;

                        case 1:
                                visionProcessor.TrackingLevel = VNRequestTrackingLevel.Accurate;
                                break;
                }
        }

        partial void handleClearRectsButton (UIButton sender)
        {
                objectsToTrack.Clear ();
                workQueue.DispatchAsync (DisplayFirstVideoFrame);
        }

        partial void handleStartStopButton (UIBarButtonItem sender)
        {
                switch (state)
                {
                        case State.Tracking:
                                // stop tracking
                                visionProcessor.CancelTracking ();
                                State = State.Stopped;
                                workQueue.DispatchAsync (DisplayFirstVideoFrame);
                                break;

                        case State.Stopped:
                                // initialize processor and start tracking
                                State = State.Tracking;
                                visionProcessor.ObjectsToTrack = objectsToTrack;
                                workQueue.DispatchAsync (StartTracking);
                                break;
                }
        }

        [Export ("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
        {
                var locationInView = touch.LocationInView (trackingView);
                return trackingView.IsPointWithinDrawingArea (locationInView) && trackedObjectType == TrackedObjectType.Object;
        }

        partial void handlePan (UIPanGestureRecognizer sender)
        {
                switch (sender.State)
                {
                        case UIGestureRecognizerState.Began:
                                // Initiate object selection
                                var locationInView = sender.LocationInView (trackingView);
                                if (trackingView.IsPointWithinDrawingArea (locationInView))
                                {
                                        trackingView.RubberbandingStart = locationInView; // start new rubberbanding
                                }
                                break;

                        case UIGestureRecognizerState.Changed:
                                // Process resizing of the object's bounding box
                                var translationPoint = sender.TranslationInView (trackingView);
                                var translation = CGAffineTransform.MakeTranslation (translationPoint.X, translationPoint.Y);
                                var endPoint = translation.TransformPoint (trackingView.RubberbandingStart);

                                if (trackingView.IsPointWithinDrawingArea (endPoint))
                                {
                                        trackingView.RubberbandingVector = translationPoint;
                                        trackingView.SetNeedsDisplay ();
                                }
                                break;

                        case UIGestureRecognizerState.Ended:
                                // Finish resizing of the object's boundong box
                                var selectedBBox = trackingView.RubberbandingRectNormalized;
                                if (selectedBBox.Width > 0 && selectedBBox.Height > 0)
                                {
                                        var rectColor = TrackedObjectsPalette.Color (objectsToTrack.Count);
                                        objectsToTrack.Add (new TrackedPolyRect (selectedBBox, rectColor));

                                        DisplayFrame (null, CGAffineTransform.MakeIdentity (), objectsToTrack);
                                }
                                break;
                }
        }

        partial void handleTap (UITapGestureRecognizer sender)
        {
                // toggle navigation bar visibility if tracking is in progress
                if (state == State.Tracking && sender.State == UIGestureRecognizerState.Ended)
                {
                        NavigationController?.SetNavigationBarHidden (!NavigationController.NavigationBarHidden, true);
                }
        }

        #region IVisionTrackerProcessorDelegate

        public void DisplayFrame (CVPixelBuffer? frame, CGAffineTransform transform, IList<TrackedPolyRect>? rects)
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                 {
                         if (frame is not null)
                         {
                                 var ciImage = new CIImage (frame).ImageByApplyingTransform (transform);
                                 var uiImage = new UIImage (ciImage);
                                 trackingView.Image = uiImage;
                         }

                         trackingView.PolyRects = rects?.ToList () ?? (trackedObjectType == TrackedObjectType.Object ? objectsToTrack
                                                                                                                : new List<TrackedPolyRect> ());
                         trackingView.RubberbandingStart = CGPoint.Empty;
                         trackingView.RubberbandingVector = CGPoint.Empty;

                         trackingView.SetNeedsDisplay ();
                 });
        }

        public void DisplayFrameCounter (int frame)
        {
                DispatchQueue.MainQueue.DispatchAsync (() => frameCounterLabel.Text = $"Frame: {frame}");
        }

        public void DidFinishTracking ()
        {
                workQueue.DispatchAsync (DisplayFirstVideoFrame);
                DispatchQueue.MainQueue.DispatchAsync (() => State = State.Stopped);
        }

        #endregion
}

public enum State
{
        Tracking,
        Stopped,
}
