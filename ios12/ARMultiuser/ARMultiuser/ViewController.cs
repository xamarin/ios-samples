
namespace ARMultiuser {
	using ARKit;
	using Foundation;
	using MultipeerConnectivity;
	using SceneKit;
	using System;
	using System.Linq;
	using UIKit;

	public partial class ViewController : UIViewController, IARSCNViewDelegate, IARSessionDelegate {
		private MultipeerSession multipeerSession;

		protected ViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.sceneView.Delegate = this;
			this.multipeerSession = new MultipeerSession (this.ReceivedData);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!ARConfiguration.IsSupported) {
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
			this.sceneView.Session.Run (configuration);

			// Set a delegate to track the number of plane anchors for providing UI feedback.
			this.sceneView.Session.Delegate = this;
			this.sceneView.DebugOptions = ARSCNDebugOptions.ShowFeaturePoints;

			// Prevent the screen from being dimmed after a while as users will likely
			// have long periods of interaction without touching the screen or buttons.
			UIApplication.SharedApplication.IdleTimerDisabled = true;
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			// Pause the view's AR session.
			this.sceneView.Session.Pause ();
		}

		#region IARSCNViewDelegate

		[Export ("renderer:didAddNode:forAnchor:")]
		public void DidAddNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
		{
			if (!string.IsNullOrEmpty (anchor.Name) &&
				anchor.Name.StartsWith ("panda", StringComparison.InvariantCulture)) {
				node.AddChildNode (this.LoadRedPandaModel ());
			}
		}

		#endregion

		#region IARSessionDelegate

		[Export ("session:cameraDidChangeTrackingState:")]
		public void CameraDidChangeTrackingState (ARSession session, ARCamera camera)
		{
			this.UpdateSessionInfoLabel (session.CurrentFrame, camera.TrackingState, camera.TrackingStateReason);
		}

		[Export ("session:didUpdateFrame:")]
		public void DidUpdateFrame (ARSession session, ARFrame frame)
		{
			switch (frame.WorldMappingStatus) {
			case ARWorldMappingStatus.NotAvailable:
			case ARWorldMappingStatus.Limited:
				this.sendMapButton.Enabled = false;
				break;

			case ARWorldMappingStatus.Extending:
				this.sendMapButton.Enabled = this.multipeerSession.ConnectedPeers.Any ();
				break;

			case ARWorldMappingStatus.Mapped:
				this.sendMapButton.Enabled = this.multipeerSession.ConnectedPeers.Any ();
				break;
			}

			this.mappingStatusLabel.Text = frame.WorldMappingStatus.GetDescription ();
			this.UpdateSessionInfoLabel (frame, frame.Camera.TrackingState, frame.Camera.TrackingStateReason);

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
			this.sessionInfoLabel.Text = "Session interruption ended";
		}

		[Export ("session:didFailWithError:")]
		public void DidFail (ARSession session, NSError error)
		{
			// Present an error message to the user.
			this.sessionInfoLabel.Text = $"Session failed: {error.LocalizedDescription}";
			this.resetTracking (null);
		}

		[Export ("sessionShouldAttemptRelocalization:")]
		public bool ShouldAttemptRelocalization (ARSession session)
		{
			return true;
		}

		#endregion

		#region Multiuser shared session

		partial void handleSceneTap (UITapGestureRecognizer sender)
		{
			// Hit test to find a place for a virtual object.
			var types = ARHitTestResultType.ExistingPlaneUsingGeometry | ARHitTestResultType.EstimatedHorizontalPlane;
			var hitTestResult = this.sceneView.HitTest (sender.LocationInView (this.sceneView), types).FirstOrDefault ();
			if (hitTestResult != null) {
				// Place an anchor for a virtual character. The model appears in renderer(_:didAdd:for:).
				var anchor = new ARAnchor ("panda", hitTestResult.WorldTransform);
				this.sceneView.Session.AddAnchor (anchor);

				// Send the anchor info to peers, so they can place the same content.
				var data = NSKeyedArchiver.ArchivedDataWithRootObject (anchor, true, out NSError error);
				if (error != null) {
					throw new Exception ("can't encode anchor");
				}

				this.multipeerSession.SendToAllPeers (data);
			}
		}

		// GetWorldMap
		partial void shareSession (RoundedButton sender)
		{
			this.sceneView.Session.GetCurrentWorldMap ((worldMap, error) => {
				if (worldMap != null) {
					var data = NSKeyedArchiver.ArchivedDataWithRootObject (worldMap, true, out NSError archivError);
					if (archivError != null) {
						throw new Exception ("can't encode map");
					}

					this.multipeerSession.SendToAllPeers (data);
				} else if (error != null) {
					Console.WriteLine ($"Error: {error.LocalizedDescription}");
				}
			});
		}

		private MCPeerID mapProvider;

		private void ReceivedData (NSData data, MCPeerID peer)
		{
			if (NSKeyedUnarchiver.GetUnarchivedObject (typeof (ARWorldMap), data, out NSError error) is ARWorldMap worldMap) {
				// Run the session with the received world map.
				var configuration = new ARWorldTrackingConfiguration ();
				configuration.PlaneDetection = ARPlaneDetection.Horizontal;
				configuration.InitialWorldMap = worldMap;
				this.sceneView.Session.Run (configuration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);

				// Remember who provided the map for showing UI feedback.
				this.mapProvider = peer;
			} else if (NSKeyedUnarchiver.GetUnarchivedObject (typeof (ARAnchor), data, out NSError anchorError) is ARAnchor anchor) {
				// Add anchor to the session, ARSCNView delegate adds visible content.
				this.sceneView.Session.AddAnchor (anchor);
			} else {
				Console.WriteLine ($"Unknown data was recieved from {peer}");
			}
		}

		#endregion

		#region AR session management

		private void UpdateSessionInfoLabel (ARFrame frame, ARTrackingState trackingState, ARTrackingStateReason trackingStateReason)
		{
			// Update the UI to provide feedback on the state of the AR experience.
			string message = null;

			switch (trackingState) {
			case ARTrackingState.Normal:
				if (!frame.Anchors.Any () && !this.multipeerSession.ConnectedPeers.Any ()) {
					// No planes detected; provide instructions for this app's AR interactions.
					message = "Move around to map the environment, or wait to join a shared session.";
				} else if (this.multipeerSession.ConnectedPeers.Any () && this.mapProvider == null) {
					var peerNames = this.multipeerSession.ConnectedPeers.Select (peer => peer.DisplayName);
					message = $"Connected with {string.Join (", ", peerNames)}.";
				}
				break;

			case ARTrackingState.NotAvailable:
				message = "Tracking unavailable.";
				break;

			case ARTrackingState.Limited:
				switch (trackingStateReason) {
				case ARTrackingStateReason.ExcessiveMotion:
					message = "Tracking limited - Move the device more slowly.";
					break;
				case ARTrackingStateReason.InsufficientFeatures:
					message = "Tracking limited - Point the device at an area with visible surface detail, or improve lighting conditions.";
					break;

				case ARTrackingStateReason.Initializing:
					if (this.mapProvider != null) {
						message = $"Received map from {this.mapProvider.DisplayName}.";
					} else {
						message = "Initializing AR session.";
					}
					break;

				case ARTrackingStateReason.Relocalizing:

					if (this.mapProvider != null) {
						message = $"Received map from {this.mapProvider.DisplayName}.";
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

			this.sessionInfoLabel.Text = message;
			this.sessionInfoView.Hidden = string.IsNullOrEmpty (message);
		}

		partial void resetTracking (UIButton sender)
		{
			var configuration = new ARWorldTrackingConfiguration { PlaneDetection = ARPlaneDetection.Horizontal };
			this.sceneView.Session.Run (configuration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
		}

		#endregion

		#region AR session management

		private SCNNode LoadRedPandaModel ()
		{
			var sceneURL = NSBundle.MainBundle.GetUrlForResource ("max", "scn", "art.scnassets");
			var referenceNode = SCNReferenceNode.CreateFromUrl (sceneURL);
			referenceNode.Load ();

			return referenceNode;
		}

		#endregion
	}
}
