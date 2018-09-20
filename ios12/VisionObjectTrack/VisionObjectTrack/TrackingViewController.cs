
namespace VisionObjectTrack
{
    using AVFoundation;
    using CoreFoundation;
    using CoreGraphics;
    using CoreImage;
    using CoreVideo;
    using Foundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UIKit;
    using Vision;
    using VisionObjectTrack.Enums;

    public partial class TrackingViewController : UIViewController, IVisionTrackerProcessorDelegate, IUIGestureRecognizerDelegate
    {
        private AVAsset videoAsset;

        private VisionTrackerProcessor visionProcessor;

        private DispatchQueue workQueue = new DispatchQueue("com.apple.VisionTracker");//, qos: .userInitiated)

        private TrackedObjectType trackedObjectType = TrackedObjectType.Object;

        private List<TrackedPolyRect> objectsToTrack = new List<TrackedPolyRect>();

        private State state = State.Stopped;

        public TrackingViewController(IntPtr handle) : base(handle) { }

        public AVAsset VideoAsset
        {
            get 
            {
                return this.videoAsset;
            }

            set
            {
                this.videoAsset = value;
                this.visionProcessor = new VisionTrackerProcessor(this.videoAsset) { Delegate = this };
            }
        }

        public State State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.state = value;
                this.HandleStateChange();
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var backplateLayer = this.frameCounterLabelBackplate.Layer;
            backplateLayer.CornerRadius = backplateLayer.Bounds.Height / 2f;

            this.frameCounterLabelBackplate.Hidden = true;
            this.frameCounterLabel.Font = UIFont.MonospacedDigitSystemFontOfSize(16, UIFontWeight.Light);
            this.handleModeSelection(this.modeSelector);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            this.workQueue.DispatchAsync(() => this.DisplayFirstVideoFrame());
        }

        public override void ViewWillDisappear(bool animated)
        {
            this.visionProcessor.CancelTracking();
            base.ViewWillDisappear(animated);
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        private void DisplayFirstVideoFrame()
        {
            var isTrackingRects = this.trackedObjectType == TrackedObjectType.Rectangle;
            this.visionProcessor.ReadAndDisplayFirstFrame(isTrackingRects, out NSError error);
            if (error != null)
            {
                this.HandleError(error);
            }
        }

        private void StartTracking()
        {
            this.visionProcessor.PerformTracking(this.trackedObjectType, out NSError error);
            if (error != null)
            {
                this.HandleError(error);
            }
        }

        private void HandleError(NSError error)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                string title = null;
                string message = null;

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
                }
                else
                {
                    title = "Error";
                    message = error.LocalizedDescription;
                }

                var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
                this.PresentViewController(alert, true, null);
            });
        }

        private void HandleStateChange()
        {
            UIBarButtonItem newBarButton = null;
            bool navBarHidden = false;
            bool frameCounterHidden = false;

            switch (this.state)
            {
                case State.Stopped:
                    navBarHidden = false;
                    frameCounterHidden = true;

                    // reveal settings view
                    this.trackingViewTopConstraint.Constant = 0;
                    this.entitySelector.Enabled = true;
                    newBarButton = new UIBarButtonItem(UIBarButtonSystemItem.Play, (s, e) => this.handleStartStopButton(null));
                    break;

                case State.Tracking:
                    navBarHidden = true;
                    frameCounterHidden = false;

                    // cover settings view
                    this.trackingViewTopConstraint.Constant = -settingsView.Frame.Height;
                    this.entitySelector.Enabled = false;
                    newBarButton = new UIBarButtonItem(UIBarButtonSystemItem.Stop, (s, e) => this.handleStartStopButton(null));
                    break;
            }

            this.NavigationController?.SetNavigationBarHidden(navBarHidden, true);
            UIView.Animate(0.5d, () =>
            {
                this.View.LayoutIfNeeded();
                this.NavigationItem.RightBarButtonItem = newBarButton;
                this.frameCounterLabelBackplate.Hidden = frameCounterHidden;
            });
        }

        partial void handleEntitySelection(UISegmentedControl sender)
        {
            switch (sender.SelectedSegment)
            {
                case 0:
                    this.trackedObjectType = TrackedObjectType.Object;
                    this.NavigationItem.Prompt = "Drag to select objects";
                    this.clearRectsButton.Enabled = true;
                    break;

                case 1:
                    this.trackedObjectType = TrackedObjectType.Rectangle;
                    this.NavigationItem.Prompt = "Rectangles are detected automatically";
                    this.clearRectsButton.Enabled = false;
                    break;
            }

            this.workQueue.DispatchAsync(() => this.DisplayFirstVideoFrame());
        }

        partial void handleModeSelection(UISegmentedControl sender)
        {
            switch (sender.SelectedSegment)
            {
                case 0:
                    this.visionProcessor.TrackingLevel = VNRequestTrackingLevel.Fast;
                    break;

                case 1:
                    this.visionProcessor.TrackingLevel = VNRequestTrackingLevel.Accurate;
                    break;
            }
        }

        partial void handleClearRectsButton(UIButton sender)
        {
            this.objectsToTrack.Clear();
            this.workQueue.DispatchAsync(() => this.DisplayFirstVideoFrame());
        }

        partial void handleStartStopButton(UIBarButtonItem sender)
        {
            switch (this.state)
            {
                case State.Tracking:
                    // stop tracking
                    this.visionProcessor.CancelTracking();
                    this.State = State.Stopped;
                    this.workQueue.DispatchAsync(() => this.DisplayFirstVideoFrame());
                    break;

                case State.Stopped:
                    // initialize processor and start tracking
                    this.State = State.Tracking;
                    this.visionProcessor.ObjectsToTrack = this.objectsToTrack;
                    this.workQueue.DispatchAsync(() => this.StartTracking());
                    break;
            }
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            var locationInView = touch.LocationInView(this.trackingView);
            return this.trackingView.IsPointWithinDrawingArea(locationInView) && this.trackedObjectType == TrackedObjectType.Object;
        }

        partial void handlePan(UIPanGestureRecognizer gestureRecognizer)
        {
            switch (gestureRecognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    // Initiate object selection
                    var locationInView = gestureRecognizer.LocationInView(this.trackingView);
                    if (this.trackingView.IsPointWithinDrawingArea(locationInView))
                    {
                        this.trackingView.RubberbandingStart = locationInView; // start new rubberbanding
                    }
                    break;

                case UIGestureRecognizerState.Changed:
                    // Process resizing of the object's bounding box
                    var translationPoint = gestureRecognizer.TranslationInView(this.trackingView);
                    var translation = CGAffineTransform.MakeTranslation(translationPoint.X, translationPoint.Y);
                    var endPoint = translation.TransformPoint(this.trackingView.RubberbandingStart);

                    if (this.trackingView.IsPointWithinDrawingArea(endPoint))
                    {
                        this.trackingView.RubberbandingVector = translationPoint;
                        this.trackingView.SetNeedsDisplay();
                    }
                    break;

                case UIGestureRecognizerState.Ended:
                    // Finish resizing of the object's boundong box
                    var selectedBBox = this.trackingView.RubberbandingRectNormalized;
                    if (selectedBBox.Width > 0 && selectedBBox.Height > 0)
                    {
                        var rectColor = TrackedObjectsPalette.Color(this.objectsToTrack.Count);
                        this.objectsToTrack.Add(new TrackedPolyRect(selectedBBox, rectColor));

                        this.DisplayFrame(null, CGAffineTransform.MakeIdentity(), this.objectsToTrack);
                    }
                    break;
            }
        }

        partial void handleTap(UITapGestureRecognizer gestureRecognizer)
        {
            // toggle navigation bar visibility if tracking is in progress
            if (this.state == State.Tracking && gestureRecognizer.State == UIGestureRecognizerState.Ended)
            {
                this.NavigationController?.SetNavigationBarHidden(!this.NavigationController.NavigationBarHidden, true);
            }
        }

        #region IVisionTrackerProcessorDelegate

        public void DisplayFrame(CVPixelBuffer frame, CGAffineTransform transform, IList<TrackedPolyRect> rects)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                if (frame != null)
                {
                    var ciImage = new CIImage(frame).ImageByApplyingTransform(transform);
                    var uiImage = new UIImage(ciImage);
                    this.trackingView.Image = uiImage;
                }

                this.trackingView.PolyRects = rects?.ToList() ?? (this.trackedObjectType == TrackedObjectType.Object ? this.objectsToTrack
                                                                                                                    : new List<TrackedPolyRect>());
                this.trackingView.RubberbandingStart = CGPoint.Empty;
                this.trackingView.RubberbandingVector = CGPoint.Empty;

                this.trackingView.SetNeedsDisplay();
            });
        }

        public void DisplayFrameCounter(int frame)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => this.frameCounterLabel.Text = $"Frame: {frame}");
        }

        public void DidFinifshTracking()
        {
            this.workQueue.DispatchAsync(() => this.DisplayFirstVideoFrame());
            DispatchQueue.MainQueue.DispatchAsync(() => this.State = State.Stopped);
        }

        #endregion
    }

    public enum State
    {
        Tracking,
        Stopped,
    }
}