using System;
using AVFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using RosyWriter.Helpers;
using UIKit;

namespace RosyWriter
{
    public partial class ViewController : UIViewController
    {
        private RosyWriterVideoProcessor videoProcessor;

        private NSTimer timer;
        private bool shouldShowStats;
        private int backgroundRecordingID;

        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            statisticView.Layer.CornerRadius = 4f;

            // Initialize the class responsible for managing AV capture session and asset writer
            videoProcessor = new RosyWriterVideoProcessor();

            // Keep track of changes to the device orientation so we can update the video processor
            var notificationCenter = NSNotificationCenter.DefaultCenter;
            //notificationCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, DeviceOrientationDidChange);

            //UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications ();

            // Setup and start the capture session
            videoProcessor.SetupAndStartCaptureSession();

            // Our interface is always in portrait
            previewView.Transform = videoProcessor.TransformFromCurrentVideoOrientationToOrientation(AVCaptureVideoOrientation.Portrait);
            previewView.Bounds = View.ConvertRectToView(View.Bounds, previewView);
            previewView.Center = new CGPoint(View.Bounds.Size.Width / 2f, View.Bounds.Size.Height / 2f);

            notificationCenter.AddObserver(UIApplication.DidBecomeActiveNotification, OnApplicationDidBecomeActive);

            // Set up labels
            shouldShowStats = true;

            // Video Processor Event Handlers
            videoProcessor.RecordingDidStart += OnRecordingDidStart;
            videoProcessor.RecordingDidStop += OnRecordingDidStop;
            videoProcessor.RecordingWillStart += OnRecordingWillStart;
            videoProcessor.RecordingWillStop += OnRecordingWillStop;
            videoProcessor.PixelBufferReadyForDisplay += OnPixelBufferReadyForDisplay;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            timer = NSTimer.CreateRepeatingScheduledTimer(.25, UpdateLabels);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            timer.Invalidate();
            timer.Dispose();
        }

        // HACK: Updated method to match delegate in NSTimer.CreateRepeatingScheduledTimer()
        private void UpdateLabels(NSTimer time)
        {
            statisticView.Hidden = !shouldShowStats;
            if (shouldShowStats)
            {
                statisticView.UpdateLabel(videoProcessor);
            }
        }

        #region Event Handler

        public void OnPixelBufferReadyForDisplay(CVImageBuffer imageBuffer)
        {
            // Don't make OpenGLES calls while in the backgroud.
            if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background)
                previewView.DisplayPixelBuffer(imageBuffer);
        }

        partial void OnRecordButtonClicked(UIBarButtonItem sender)
        {
            // Wait for the recording to start/stop before re-enabling the record button.
            InvokeOnMainThread(() => recordButton.Enabled = false);

            // The recordingWill/DidStop delegate methods will fire asynchronously in the response to this call.
            if (videoProcessor.IsRecording)
                videoProcessor.StopRecording();
            else
                videoProcessor.StartRecording();
        }

        public void OnApplicationDidBecomeActive(NSNotification notification)
        {
            // For performance reasons, we manually pause/resume the session when saving a recoding.
            // If we try to resume the session in the background it will fail. Resume the session here as well to ensure we will succeed.
            videoProcessor.ResumeCaptureSession();
        }

        #endregion

        #region Video Processer Event handlers

        public void OnRecordingWillStart()
        {
            InvokeOnMainThread(() =>
            {
                recordButton.Enabled = false;
                recordButton.Title = "Stop";

                // Disable the idle timer while we are recording
                UIApplication.SharedApplication.IdleTimerDisabled = true;

                // Make sure we have time to finish saving the movie if the app is backgrounded during recording
                if (UIDevice.CurrentDevice.IsMultitaskingSupported)
                    // HACK: Cast nint to int
                    backgroundRecordingID = (int)UIApplication.SharedApplication.BeginBackgroundTask(() => {
                        UIApplication.SharedApplication.EndBackgroundTask(backgroundRecordingID);
                    });
            });
        }

        public void OnRecordingDidStart()
        {
            InvokeOnMainThread(() => recordButton.Enabled = true);
        }

        public void OnRecordingWillStop()
        {
            InvokeOnMainThread(() => {
                // Disable until saving to the camera roll is complete
                recordButton.Title = "Record";
                recordButton.Enabled = false;

                // Pause the capture session so the saving will be as fast as possible.
                // We resme the session in recordingDidStop
                videoProcessor.PauseCaptureSession();
            });
        }

        public void OnRecordingDidStop()
        {
            InvokeOnMainThread(() => {
                recordButton.Enabled = true;

                UIApplication.SharedApplication.IdleTimerDisabled = false;

                videoProcessor.ResumeCaptureSession();

                if (UIDevice.CurrentDevice.IsMultitaskingSupported)
                {
                    UIApplication.SharedApplication.EndBackgroundTask(backgroundRecordingID);
                    backgroundRecordingID = 0;
                }
            });
        }

        #endregion 
    }
}