namespace ARKitVision;

/// <summary>
/// Main view controller for the ARKitVision sample.
/// </summary>
public partial class ViewController : UIViewController, IUIGestureRecognizerDelegate, IARSKViewDelegate, IARSessionDelegate
{
        // The view controller that displays the status and "restart experience" UI.
        private StatusViewController? statusViewController;

        protected ViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();

                statusViewController = ChildViewControllers.FirstOrDefault (controller => controller is StatusViewController) as StatusViewController;

                // Configiure and present the SpriteKit scene that draws overlay content.
                var overlayScene = new SKScene { ScaleMode = SKSceneScaleMode.AspectFill };

                sceneView.Delegate = this;
                sceneView.PresentScene (overlayScene);
                sceneView.Session.Delegate = this;

                if (statusViewController is not null)
                {
                        // Hook up status view controller callback.
                        statusViewController.RestartExperienceHandler = RestartSession;
                }
        }

        public override void ViewWillAppear (bool animated)
        {
                base.ViewWillAppear (animated);

                // Create a session configuration
                var configuration = new ARWorldTrackingConfiguration ();

                // Run the view's session
                sceneView.Session.Run (configuration);
        }

        public override void ViewWillDisappear (bool animated)
        {
                base.ViewWillDisappear (animated);

                // Pause the view's session
                sceneView.Session.Pause ();
        }

        #region IARSessionDelegate

        /// <summary>
        /// Pass camera frames received from ARKit to Vision (when not already processing one)
        /// </summary>
        [Export ("session:didUpdateFrame:")]
        public void DidUpdateFrame (ARSession session, ARFrame frame)
        {
                // Do not enqueue other buffers for processing while another Vision task is still running.
                // The camera stream has only a finite amount of buffers available; holding too many buffers for analysis would starve the camera.
                if (currentBuffer is null && frame.Camera.TrackingState == ARTrackingState.Normal)
                {
                        // Retain the image buffer for Vision processing.
                        currentBuffer = frame.CapturedImage;
                        ClassifyCurrentImage ();
                }

                frame.Dispose ();
        }

        #endregion

        #region Vision classification

        // Queue for dispatching vision classification requests
        private readonly DispatchQueue visionQueue = new DispatchQueue ("com.example.apple-samplecode.ARKitVision.serialVisionQueue");

        // The pixel buffer being held for analysis; used to serialize Vision requests.
        private CVPixelBuffer? currentBuffer;

        /// <summary>
        /// Vision classification request and model
        /// </summary>
        private VNCoreMLRequest ClassificationRequest ()
        {
                var model = VNCoreMLModel.FromMLModel (new Inceptionv3 ().Model, out NSError error);
                if (error is null)
                {
                        var request = new VNCoreMLRequest (model!, (internalRequest, internalError) =>
                         {
                                 ProcessClassifications (internalRequest, internalError);
                         });

                        // Crop input images to square area at center, matching the way the ML model was trained.
                        request.ImageCropAndScaleOption = VNImageCropAndScaleOption.CenterCrop;

                        // Use CPU for Vision processing to ensure that there are adequate GPU resources for rendering.
                        request.UsesCpuOnly = true;

                        return request;
                }
                else
                {
                        throw new Exception ($"Failed to load Vision ML model: {error}");
                }
        }

        private void ClassifyCurrentImage ()
        {
                // Most computer vision tasks are not rotation agnostic so it is important to pass in the orientation of the image with respect to device.
                var orientation = CGImagePropertyOrientationExtensions.ConvertFrom (UIDevice.CurrentDevice.Orientation);

                var requestHandler = new VNImageRequestHandler (currentBuffer!, orientation, new VNImageOptions ());
                visionQueue.DispatchAsync (() =>
                 {
                         requestHandler.Perform (new VNRequest [] { ClassificationRequest () }, out NSError error);
                         if (error != null)
                         {
                                 Console.WriteLine ($"Error: Vision request failed with error \"{error}\"");
                         }

                         // Release the pixel buffer when done, allowing the next buffer to be processed.
                         currentBuffer?.Dispose ();
                         currentBuffer = null;
                 });
        }

        // Classification results
        private string identifierString = string.Empty;
        private float confidence = 0f;

        /// <summary>
        /// Handle completion of the Vision request and choose results to display.
        /// </summary>
        private void ProcessClassifications (VNRequest request, NSError error)
        {
                var classifications = request.GetResults<VNClassificationObservation> ();
                if (classifications is null)
                {
                        Console.WriteLine ($"Unable to classify image.\n{error.LocalizedDescription}");
                }

                // Show a label for the highest-confidence result (but only above a minimum confidence threshold).
                var bestResult = classifications.FirstOrDefault (result => result.Confidence > 0.5f);
                if (bestResult is not null)
                {
                        identifierString = bestResult.Identifier.Split (',') [0];
                        confidence = bestResult.Confidence;
                }
                else
                {
                        identifierString = string.Empty;
                        confidence = 0f;
                }

                DispatchQueue.MainQueue.DispatchAsync (() => DisplayClassifierResults ());
        }

        /// <summary>
        /// Show the classification results in the UI.
        /// </summary>
        private void DisplayClassifierResults ()
        {
                if (!string.IsNullOrEmpty (identifierString))
                {
                        var message = $"Detected {identifierString} with {confidence * 100f}% confidence";
                        if (statusViewController is not null)
                                statusViewController.ShowMessage (message);
                }
        }

        #endregion

        #region Tap gesture handler & ARSKViewDelegate

        // Labels for classified objects by ARAnchor UUID
        private Dictionary<NSUuid, string> anchorLabels = new Dictionary<NSUuid, string> ();

        /// <summary>
        /// When the user taps, add an anchor associated with the current classification result.
        /// </summary>
        partial void placeLabelInLocationWithSender (UITapGestureRecognizer sender)
        {
                var hitLocationInView = sender.LocationInView (sceneView);
                var result = sceneView.HitTest (hitLocationInView, ARHitTestResultType.FeaturePoint | ARHitTestResultType.EstimatedHorizontalPlane).FirstOrDefault ();
                if (result is not null)
                {
                        // Add a new anchor at the tap location.
                        var anchor = new ARAnchor (result.WorldTransform);
                        sceneView.Session.AddAnchor (anchor);

                        // Track anchor ID to associate text with the anchor after ARKit creates a corresponding SKNode.
                        anchorLabels [anchor.Identifier] = identifierString;
                }
        }

        /// <summary>
        /// When an anchor is added, provide a SpriteKit node for it and set its text to the classification label.
        /// </summary>
        [Export ("view:didAddNode:forAnchor:")]
        public void DidAddNode (ARSKView view, SKNode node, ARAnchor anchor)
        {
                if (!anchorLabels.TryGetValue (anchor.Identifier, out string? labelText))
                {
                        throw new Exception ("missing expected associated label for anchor");
                }

                node.AddChild (new TemplateLabelNode (labelText));
        }

        #endregion

        #region AR Session Handling

        [Export ("session:cameraDidChangeTrackingState:")]
        public void CameraDidChangeTrackingState (ARSession session, ARCamera camera)
        {
                if (statusViewController is not null)
                {
                        statusViewController.ShowTrackingQualityInfo (camera, true);

                        switch (camera.TrackingState)
                        {
                                case ARTrackingState.NotAvailable:
                                case ARTrackingState.Limited:
                                        statusViewController.EscalateFeedback (camera, 3.0);
                                        break;

                                case ARTrackingState.Normal:
                                        statusViewController.CancelScheduledMessage (MessageType.TrackingStateEscalation);
                                        // Unhide content after successful relocalization.
                                        SetOverlaysHidden (false);
                                        break;
                        }
                }
        }

        [Export ("session:didFailWithError:")]
        public void DidFail (ARSession session, NSError error)
        {
                string [] messages =
                {
                        error.LocalizedDescription,
                        error.LocalizedFailureReason,
                        error.LocalizedRecoverySuggestion
                };

                // Filter out optional error messages.
                var errorMessage = string.Join ("\n", messages);
                DispatchQueue.MainQueue.DispatchAsync (() =>
                 {
                         DisplayErrorMessage ("The AR session failed.", errorMessage);
                 });
        }

        [Export ("sessionWasInterrupted:")]
        public void WasInterrupted (ARSession session)
        {
                SetOverlaysHidden (true);
        }

        [Export ("sessionShouldAttemptRelocalization:")]
        public bool ShouldAttemptRelocalization (ARSession session)
        {
                /*
                 * Allow the session to attempt to resume after an interruption.
                 * This process may not succeed, so the app must be prepared
                 * to reset the session if the relocalizing status continues
                 * for a long time -- see `escalateFeedback` in `StatusViewController`.
                 */
                return true;
        }

        private void SetOverlaysHidden (bool shouldHide)
        {
                if (sceneView is null)
                        return;
                foreach (var node in sceneView.Scene)
                {
                        if (shouldHide)
                        {
                                // Hide overlay content immediately during relocalization.
                                node.Alpha = 0f;
                        }
                        else
                        {
                                // Fade overlay content in after relocalization succeeds.
                                node.RunAction (SKAction.FadeInWithDuration (0.5d));
                        }
                }
        }

        private void RestartSession ()
        {
                if (statusViewController is not null)
                {
                        statusViewController.CancelAllScheduledMessages ();
                        statusViewController.ShowMessage ("RESTARTING SESSION");
                }

                anchorLabels = new Dictionary<NSUuid, string> ();

                var configuration = new ARWorldTrackingConfiguration ();
                sceneView.Session.Run (configuration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
        }

        #endregion

        #region Error handling

        private void DisplayErrorMessage (string title, string message)
        {
                // Present an alert informing about the error that has occurred.
                var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);
                var restartAction = UIAlertAction.Create ("Restart Session", UIAlertActionStyle.Default, (_) =>
                 {
                         alertController.DismissViewController (true, null);
                         RestartSession ();
                 });

                alertController.AddAction (restartAction);
                PresentViewController (alertController, true, null);
        }

        #endregion
}
