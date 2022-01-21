
namespace ARMultiuser;

public partial class ViewController : UIViewController, IARSCNViewDelegate, IARSessionDelegate
{
        private MultipeerSession? multipeerSession;

        protected ViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad ()
        {
                base.ViewDidLoad ();
                sceneView.Delegate = this;
                multipeerSession = new MultipeerSession (ReceivedData);
        }

        public override void ViewDidAppear (bool animated)
        {
                base.ViewDidAppear (animated);

                if (!ARConfiguration.IsSupported)
                {
                        throw new Exception ("ARKit is not available on this device. For apps that require ARKit" +
                                            "for core functionality, use the `arkit` key in the key in the" +
                                            "`UIRequiredDeviceCapabilities` section of the Info.plist to prevent" +
                                            "the app from installing. (If the app can't be installed, this error" +
                                            "can't be triggered in a production scenario.)" +
                                            "In apps where AR is an additive feature, use `isSupported` to" +
                                            "determine whether to show UI for launching AR experiences.");
                        // For details, see https://developer.apple.com/documentation/arkit
                }

                // Start the view's AR session.
                var configuration = new ARWorldTrackingConfiguration { PlaneDetection = ARPlaneDetection.Horizontal };
                sceneView.Session.Run (configuration);

                // Set a delegate to track the number of plane anchors for providing UI feedback.
                sceneView.Session.Delegate = this;
                sceneView.DebugOptions = ARSCNDebugOptions.ShowFeaturePoints;

                // Prevent the screen from being dimmed after a while as users will likely
                // have long periods of interaction without touching the screen or buttons.
                UIApplication.SharedApplication.IdleTimerDisabled = true;
        }

        public override void ViewDidDisappear (bool animated)
        {
                base.ViewDidDisappear (animated);

                // Pause the view's AR session.
                sceneView.Session.Pause ();
        }

        bool HasPeersConnected()
	{
                return multipeerSession?.ConnectedPeers.Any () ?? false;
	}

        #region IARSCNViewDelegate

        [Export ("renderer:didAddNode:forAnchor:")]
        public void DidAddNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
                if (!string.IsNullOrEmpty (anchor.Name) &&
                    anchor.Name.StartsWith ("panda", StringComparison.InvariantCulture))
                {
                        var panda = LoadRedPandaModel ();
                        if (panda is not null)
                        {
                                node.AddChildNode (panda);
                        } else {
                                throw new Exception ("model not loaded");
			}
                }
        }

        #endregion

        #region IARSessionDelegate

        [Export ("session:cameraDidChangeTrackingState:")]
        public void CameraDidChangeTrackingState (ARSession session, ARCamera camera)
        {
                if (session.CurrentFrame is not null)
                {
                        UpdateSessionInfoLabel (session.CurrentFrame, camera.TrackingState, camera.TrackingStateReason);
                }
        }

        [Export ("session:didUpdateFrame:")]
        public void DidUpdateFrame (ARSession session, ARFrame frame)
        {
                switch (frame.WorldMappingStatus)
                {
                        case ARWorldMappingStatus.NotAvailable:
                        case ARWorldMappingStatus.Limited:
                                sendMapButton.Enabled = false;
                                break;

                        case ARWorldMappingStatus.Extending:
                                sendMapButton.Enabled = HasPeersConnected ();
                                break;

                        case ARWorldMappingStatus.Mapped:
                                sendMapButton.Enabled = HasPeersConnected ();
                                break;
                }

                mappingStatusLabel.Text = frame.WorldMappingStatus.GetDescription ();
                UpdateSessionInfoLabel (frame, frame.Camera.TrackingState, frame.Camera.TrackingStateReason);

                frame?.Dispose ();
        }

        #endregion

        #region ARSessionObserver

        [Export ("sessionWasInterrupted:")]
        public void WasInterrupted (ARSession session)
        {
                // Inform the user that the session has been interrupted, for example, by presenting an overlay.
                this.sessionInfoLabel.Text = "Session was interrupted";
        }

        [Export ("sessionInterruptionEnded:")]
        public void InterruptionEnded (ARSession session)
        {
                // Reset tracking and/or remove existing anchors if consistent tracking is required.
                sessionInfoLabel.Text = "Session interruption ended";
        }

        [Export ("session:didFailWithError:")]
        public void DidFail (ARSession session, NSError error)
        {
                // Present an error message to the user.
                sessionInfoLabel.Text = $"Session failed: {error.LocalizedDescription}";
                resetTracking (null);
        }

        [Export ("sessionShouldAttemptRelocalization:")]
        public bool ShouldAttemptRelocalization (ARSession session) => true;

        #endregion

        #region Multiuser shared session

        partial void handleSceneTap (UITapGestureRecognizer sender)
        {
                // Hit test to find a place for a virtual object.
                var types = ARHitTestResultType.ExistingPlaneUsingGeometry | ARHitTestResultType.EstimatedHorizontalPlane;
                var hitTestResult = sceneView.HitTest (sender.LocationInView (sceneView), types).FirstOrDefault ();
                if (hitTestResult is not null)
                {
                        // Place an anchor for a virtual character. The model appears in renderer(_:didAdd:for:).
                        var anchor = new ARAnchor ("panda", hitTestResult.WorldTransform);
                        sceneView.Session.AddAnchor (anchor);

                        // Send the anchor info to peers, so they can place the same content.
                        var data = NSKeyedArchiver.ArchivedDataWithRootObject (anchor, true, out NSError? error);
                        if (error is not null)
                        {
                                throw new Exception ("can't encode anchor");
                        }

                        multipeerSession?.SendToAllPeers (data);
                }
        }

        // GetWorldMap
        partial void shareSession (RoundedButton sender)
        {
                this.sceneView.Session.GetCurrentWorldMap ((worldMap, error) =>
                 {
                         if (worldMap is not null)
                         {
                                 var data = NSKeyedArchiver.ArchivedDataWithRootObject (worldMap, true, out NSError? archivError);
                                 if (archivError is not null)
                                 {
                                         throw new Exception ("can't encode map");
                                 }

                                 multipeerSession?.SendToAllPeers (data);
                         }
                         else if (error is not null)
                         {
                                 Console.WriteLine ($"Error: {error.LocalizedDescription}");
                         }
                 });
        }

        MCPeerID? mapProvider;

        void ReceivedData (NSData data, MCPeerID peer)
        {
                if (NSKeyedUnarchiver.GetUnarchivedObject (typeof (ARWorldMap), data, out NSError? error) is ARWorldMap worldMap)
                {
                        // Run the session with the received world map.
                        var configuration = new ARWorldTrackingConfiguration ();
                        configuration.PlaneDetection = ARPlaneDetection.Horizontal;
                        configuration.InitialWorldMap = worldMap;
                        sceneView.Session.Run (configuration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);

                        // Remember who provided the map for showing UI feedback.
                        mapProvider = peer;
                } else if (NSKeyedUnarchiver.GetUnarchivedObject (typeof (ARAnchor), data, out NSError? anchorError) is ARAnchor anchor) {
                        // Add anchor to the session, ARSCNView delegate adds visible content.
                        sceneView.Session.AddAnchor (anchor);
                } else {
                        Console.WriteLine ($"Unknown data was recieved from {peer}");
                }
        }

        #endregion

        #region AR session management

        void UpdateSessionInfoLabel (ARFrame frame, ARTrackingState trackingState, ARTrackingStateReason trackingStateReason)
        {
                // Update the UI to provide feedback on the state of the AR experience.
                string? message = null;

                switch (trackingState)
                {
                        case ARTrackingState.Normal:
                                if (!frame.Anchors.Any () && !HasPeersConnected ())
                                {
                                        // No planes detected; provide instructions for this app's AR interactions.
                                        message = "Move around to map the environment, or wait to join a shared session.";
                                } else if (HasPeersConnected () && mapProvider is null) {
                                        var peerNames = multipeerSession!.ConnectedPeers.Select (peer => peer.DisplayName);
                                        message = $"Connected with {string.Join (", ", peerNames)}.";
                                }
                                break;

                        case ARTrackingState.NotAvailable:
                                message = "Tracking unavailable.";
                                break;

                        case ARTrackingState.Limited:
                                switch (trackingStateReason)
                                {
                                        case ARTrackingStateReason.ExcessiveMotion:
                                                message = "Tracking limited - Move the device more slowly.";
                                                break;
                                        case ARTrackingStateReason.InsufficientFeatures:
                                                message = "Tracking limited - Point the device at an area with visible surface detail, or improve lighting conditions.";
                                                break;

                                        case ARTrackingStateReason.Initializing:
                                                if (mapProvider is not null)
                                                {
                                                        message = $"Received map from {mapProvider.DisplayName}.";
                                                } else {
                                                        message = "Initializing AR session.";
                                                }
                                                break;

                                        case ARTrackingStateReason.Relocalizing:

                                                if (mapProvider is not null)
                                                {
                                                        message = $"Received map from {mapProvider.DisplayName}.";
                                                } else {
                                                        message = "Resuming session — move to where you were when the session was interrupted.";
                                                }
                                                break;

                                        default:
                                                break;
                                }

                                break;

                        default:
                                // No feedback needed when tracking is normal and planes are visible.
                                // (Nor when in unreachable limited-tracking states.)
                                message = "";
                                break;
                }

                sessionInfoLabel.Text = message;
                sessionInfoView.Hidden = string.IsNullOrEmpty (message);
        }

        partial void resetTracking (UIButton sender)
        {
                var configuration = new ARWorldTrackingConfiguration { PlaneDetection = ARPlaneDetection.Horizontal };
                sceneView.Session.Run (configuration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
        }

        #endregion

        #region AR session management

        private SCNNode? LoadRedPandaModel ()
        {
                var sceneURL = NSBundle.MainBundle.GetUrlForResource ("max", "scn", "art.scnassets");
                var referenceNode = SCNReferenceNode.CreateFromUrl (sceneURL);
                referenceNode?.Load ();

                return referenceNode;
        }

        #endregion
}
