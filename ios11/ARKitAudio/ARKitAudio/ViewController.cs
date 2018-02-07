
namespace ARKitAudio
{
    using ARKit;
    using AVFoundation;
    using CoreGraphics;
    using Foundation;
    using OpenTK;
    using SceneKit;
    using System;
    using System.Linq;
    using UIKit;

    /// <summary>
    /// Main view controller for the AR experience.
    /// </summary>
    public partial class ViewController : UIViewController, IARSCNViewDelegate
    {
        private CGPoint screenCenter = CGPoint.Empty;

        // Shows a preview of the object to be placed and hovers over estimated planes.
        private PreviewNode previewNode;

        // Contains the cup model that is shared by the preview and final nodes.
        private SCNNode cupNode = new SCNNode();

        // Audio source for positional audio feedback.
        private SCNAudioSource source;

        public ViewController(IntPtr handle) : base(handle) { }

        #region View Life Cycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.sceneView.Delegate = this;

            // Show statistics such as FPS and timing information.
            this.sceneView.ShowsStatistics = true;

            // Setup environment mapping.
            var environmentMap = UIImage.FromBundle("art.scnassets/sharedImages/environment_blur.exr");
            this.sceneView.Scene.LightingEnvironment.Contents = environmentMap;

            // Complete rendering setup of ARSCNView.
            this.sceneView.AntialiasingMode = SCNAntialiasingMode.Multisampling4X;
            this.sceneView.AutomaticallyUpdatesLighting = false;
            this.sceneView.ContentScaleFactor = 1.3f;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // Preload the audio file.
            this.source = SCNAudioSource.FromFile("art.scnassets/ping.aif");
            this.source.Loops = true;
            this.source.Load();

            if (ARConfiguration.IsSupported)
            {
                // Start the ARSession.
                var configuration = new ARWorldTrackingConfiguration { PlaneDetection = ARPlaneDetection.Horizontal };
                this.sceneView.Session.Run(configuration, default(ARSessionRunOptions));

                this.screenCenter = new CGPoint(this.sceneView.Bounds.GetMidX(), this.sceneView.Bounds.GetMidY());

                // Prevent the screen from being dimmed after a while as users will likely have
                // long periods of interaction without touching the screen or buttons.
                UIApplication.SharedApplication.IdleTimerDisabled = true;
            }
            else
            {
                this.ShowUnsupportedDeviceError();
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            this.cupNode.RemoveAllAudioPlayers();

            // Pause the view's session.
            this.sceneView.Session.Pause();
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            // The screen's center point changes on orientation switch, so recalculate `screenCenter`.
            this.screenCenter = new CGPoint(toSize.Width / 2f, toSize.Height / 2f);
        }

        #endregion

        #region Internal methods

        private void ShowUnsupportedDeviceError()
        {
            // This device does not support 6DOF world tracking.
            var alertController = UIAlertController.Create("ARKit is not available on this device.",
                                                           "This app requires world tracking, which is available only on iOS devices with the A9 processor or later.",
                                                           UIAlertControllerStyle.Alert);

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            this.PresentModalViewController(alertController, true);
        }

        /// <summary>
        ///  Check the light estimate from the current ARFrame and update the scene.
        /// </summary>
        private void UpdateLightEstimate()
        {
            using (var frame = this.sceneView.Session.CurrentFrame)
            {
                var lightEstimate = frame?.LightEstimate;
                if (lightEstimate != null)
                {
                    this.sceneView.Scene.LightingEnvironment.Intensity = lightEstimate.AmbientIntensity / 40f;
                }
                else
                {
                    this.sceneView.Scene.LightingEnvironment.Intensity = 40f;
                }
            }
        }

        private void ResetTracking()
        {
            var configuration = new ARWorldTrackingConfiguration { PlaneDetection = ARPlaneDetection.Horizontal };
            this.sceneView.Session.Run(configuration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);

            // Reset preview state.
            this.cupNode.RemoveFromParentNode();
            this.cupNode.Dispose();
            this.cupNode = new SCNNode();

            this.previewNode?.RemoveFromParentNode();
            this.previewNode?.Dispose();
            this.previewNode = null;

            this.PlaySound();
        }

        private void SetNewVirtualObjectToAnchor(SCNNode node, ARAnchor anchor, NMatrix4 cameraTransform)
        {
            var cameraWorldPosition = cameraTransform.GetTranslation();
            var cameraToPosition = anchor.Transform.GetTranslation() - cameraWorldPosition;

            // Limit the distance of the object from the camera to a maximum of 10 meters.
            if (cameraToPosition.Length > 10f)
            {
                cameraToPosition = Vector3.Normalize(cameraToPosition);
                cameraToPosition *= 10f;
            }

            node.Position = cameraWorldPosition + cameraToPosition;
        }

        #endregion

        #region ARSCNViewDelegate

        /// <summary>
        /// UpdateAudioPlayback
        /// </summary>
        [Export("renderer:updateAtTime:")]
        public void Update(ISCNSceneRenderer renderer, double timeInSeconds)
        {
            if (this.cupNode.ParentNode == null && this.previewNode == null)
            {
                // If our model hasn't been placed and we lack a preview for placement then setup a preview.
                this.SetupPreviewNode();
                this.UpdatePreviewNode();
            }
            else
            {
                this.UpdatePreviewNode();
            }

            this.UpdateLightEstimate();
            this.CutVolumeIfPlacedObjectIsInView();
        }

        /// <summary>
        /// PlaceARContent
        /// </summary>
        [Export("renderer:didAddNode:forAnchor:")]
        public void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            // Place content only for anchors found by plane detection.
            if (anchor is ARPlaneAnchor && this.previewNode != null)
            {
                // Stop showing a preview version of the object to be placed.
                this.cupNode.RemoveFromParentNode();

                this.previewNode?.RemoveFromParentNode();
                this.previewNode?.Dispose();
                this.previewNode = null;

                // Add the cupNode to the scene's root node using the anchor's position.
                var cameraTransform = this.sceneView.Session.CurrentFrame?.Camera?.Transform;
                if (cameraTransform.HasValue)
                {
                    this.SetNewVirtualObjectToAnchor(this.cupNode, anchor, cameraTransform.Value);
                    this.sceneView.Scene.RootNode.AddChildNode(this.cupNode);

                    // Disable plane detection after the model has been added.
                    var configuration = new ARWorldTrackingConfiguration { PlaneDetection = ARPlaneDetection.Horizontal };
                    this.sceneView.Session.Run(configuration, default(ARSessionRunOptions));

                    // Set up positional audio to play in case the object moves offscreen.
                    this.PlaySound();
                }
            }
        }

        /// <summary>
        /// PlaceARContent
        /// </summary>
        [Export("session:cameraDidChangeTrackingState:")]
        private void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
        {
            var message = string.Empty;

            // Inform the user of their camera tracking state.
            switch (camera.TrackingState)
            {
                case ARTrackingState.NotAvailable:
                    message = "Tracking unavailable";
                    break;

                case ARTrackingState.Limited:
                    switch (camera.TrackingStateReason)
                    {
                        case ARTrackingStateReason.ExcessiveMotion:
                            message = "Tracking limited - Too much camera movement";
                            break;
                        case ARTrackingStateReason.InsufficientFeatures:
                            message = "Tracking limited - Not enough surface detail";
                            break;
                        case ARTrackingStateReason.Initializing:
                            message = "Initializing AR Session";
                            break;
                    }
                    break;

                case ARTrackingState.Normal:
                    message = "Tracking normal";
                    break;
            }

            this.sessionInfoLabel.Text = message;
        }

        [Export("session:didFailWithError:")]
        public void DidFail(ARSession session, NSError error)
        {
            // Present an error message to the user.
            this.sessionInfoLabel.Text = $"Session failed: {error.LocalizedDescription}";
            this.ResetTracking();
        }

        [Export("sessionWasInterrupted:")]
        public void WasInterrupted(ARSession session)
        {
            // Inform the user that the session has been interrupted, for example, by presenting an overlay.
            this.sessionInfoLabel.Text = "Session was interrupted";
            this.ResetTracking();
        }

        [Export("sessionInterruptionEnded:")]
        public void InterruptionEnded(ARSession session)
        {
            // Reset tracking and/or remove existing anchors if consistent tracking is required.
            this.sessionInfoLabel.Text = "Session interruption ended";
            this.ResetTracking();
        }

        #endregion

        #region Preview Node

        /// <summary>
        /// Loads the cup model (`cupNode`) that is used for the duration of the app.
        /// Initializes a `PreviewNode` that contains the `cupNode` and adds it to the node hierarchy.
        /// </summary>
        private void SetupPreviewNode()
        {
            if (this.cupNode.FindChildNode("candle", false) == null)
            {
                // Load the cup scene from the bundle only once.
                var modelScene = SCNScene.FromFile("art.scnassets/candle/candle.scn");
                // Get a handle to the cup model.
                var cup = modelScene.RootNode.FindChildNode("candle", true);
                // Set the cup model onto `cupNode`.
                this.cupNode.AddChildNode(cup);
            }

            // Initialize `previewNode` to display the cup model.
            this.previewNode = new PreviewNode(this.cupNode);
            // Add `previewNode` to the node hierarchy.
            this.sceneView.Scene.RootNode.AddChildNode(this.previewNode);
        }

        /// <summary>
        /// `previewNode` exists when ARKit is finding a plane. During this time, get a world position for the areas closest to the scene's point of view that ARKit believes might be a plane, and use it to update the `previewNode` position.
        /// </summary>
        private void UpdatePreviewNode()
        {
            if (this.previewNode != null)
            {
                var (worldPosition, planeAnchor, _) = Utilities.WorldPositionFromScreenPosition(this.screenCenter, this.sceneView, this.previewNode.Position);
                if (worldPosition.HasValue)
                {
                    this.previewNode.Update(worldPosition.Value, planeAnchor, this.sceneView.Session.CurrentFrame?.Camera);
                }
            }
        }

        #endregion

        #region Sound

        /// <summary>
        /// Determines whether the `cupNode` is visible. If the `cupNode` isn't visible, a sound is played using
        /// SceneKit's positional audio functionality to locate the `cupNode`.
        /// </summary>
        private void CutVolumeIfPlacedObjectIsInView()
        {
            if (this.previewNode == null && this.sceneView.PointOfView != null)
            {
                var player = this.cupNode.AudioPlayers?.FirstOrDefault();
                var avNode = player?.AudioNode as AVAudioPlayerNode;
                if (player != null && avNode != null)
                {
                    var placedObjectIsInView = this.sceneView.IsNodeInsideFrustum(this.cupNode, this.sceneView.PointOfView);
                    avNode.Volume = placedObjectIsInView ? 0f : 1f;
                }
            }
        }

        /// <summary>
        /// Plays a sound on the cupNode using SceneKit's positional audio.
        /// </summary>
        private void PlaySound()
        {
            // ensure there is only one audio player
            this.cupNode.RemoveAllAudioPlayers();
                   
            if (this.cupNode.AudioPlayers == null || !this.cupNode.AudioPlayers.Any())
            {
                this.cupNode.AddAudioPlayer(new SCNAudioPlayer(this.source));
            }
        }

        #endregion
    }
}