using ARKit;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using SceneKit;
using System;
using UIKit;

namespace EnvironmentTexturing {
	/// <summary>
	/// Main view controller for the AR experience.
	/// </summary>
	public partial class ViewController : UIViewController, IARSCNViewDelegate, IARSessionDelegate {
		// Model of shiny sphere that is added to the scene
		private SCNNode virtualObjectModel;

		// Environment Texturing Configuration

		// The virtual object that the user interacts with in the scene.
		private SCNNode virtualObject;

		// An environment probe for shading that virtual object.
		private AREnvironmentProbeAnchor environmentProbeAnchor;

		// A fallback environment probe encompassing the whole scene.
		private AREnvironmentProbeAnchor sceneEnvironmentProbeAnchor;

		// Place environment probes manually or allow ARKit to place them automatically.
		private AREnvironmentTexturing currentTexturingMode = AREnvironmentTexturing.Automatic;

		// Indicates whether manually placed probes need updating.
		private bool requiresProbeRefresh = true;

		// Tracks timing of manual probe updates to prevent updating too frequently.
		private double lastProbeAnchorUpdateTime;

		// Indicates whether ARKit has provided an environment texture.
		private bool isEnvironmentTextureAvailable;

		// The latest screen touch position when a pan gesture is active
		private CGPoint? lastPanTouchPosition;

		protected ViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.InitializeVirtualObjectModel ();

			this.sceneView.Delegate = this;
			this.sceneView.Session.Delegate = this;

			switch (this.currentTexturingMode) {
			case AREnvironmentTexturing.Manual:
				this.textureModeSelectionControl.SelectedSegment = 1;
				break;
			case AREnvironmentTexturing.Automatic:
				this.textureModeSelectionControl.SelectedSegment = 0;
				break;
			default:
				throw new Exception ("This app supports only manual and automatic environment texturing.");
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Prevent the screen from dimming to avoid interrupting the AR experience.
			UIApplication.SharedApplication.IdleTimerDisabled = true;

			// Start the AR session with automatic environment texturing.
			this.sceneView.Session.Run (new ARWorldTrackingConfiguration {
				PlaneDetection = ARPlaneDetection.Horizontal,
				EnvironmentTexturing = AREnvironmentTexturing.Automatic
			});
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			this.sceneView.Session.Pause ();
		}

		#region Session management

		partial void ChangeTextureMode (UISegmentedControl sender)
		{
			if (sender.SelectedSegment == 0) {
				this.currentTexturingMode = AREnvironmentTexturing.Automatic;
				this.environmentProbeAnchor?.Dispose ();
				this.environmentProbeAnchor = null;
			} else {
				this.currentTexturingMode = AREnvironmentTexturing.Manual;
				this.requiresProbeRefresh = true;
			}

			// Remove anchors and change texturing mode
			this.ResetTracking (true);
		}

		partial void RestartExperience (NSObject sender)
		{
			if (this.virtualObject != null) {
				this.virtualObject.RemoveFromParentNode ();
				this.virtualObject = null;
			}

			this.ResetTracking ();
		}

		/// <summary>
		/// Runs the session with a new AR configuration to change modes or reset the experience.
		/// </summary>
		private void ResetTracking (bool changeTextureMode = false)
		{
			var configuration = new ARWorldTrackingConfiguration ();
			configuration.PlaneDetection = ARPlaneDetection.Horizontal;
			configuration.EnvironmentTexturing = this.currentTexturingMode;

			var session = this.sceneView.Session;
			if (changeTextureMode) {
				// Remove existing environment probe anchors.
				if (session.CurrentFrame?.Anchors != null) {
					foreach (var anchor in session.CurrentFrame.Anchors) {
						session.RemoveAnchor (anchor);
					}
				}

				// Don't reset tracking when changing modes in the same session.
				session.Run (configuration);
			} else {
				session.Run (configuration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
			}

			this.isEnvironmentTextureAvailable = false;
			this.sceneEnvironmentProbeAnchor?.Dispose ();
			this.sceneEnvironmentProbeAnchor = null;
			configuration.Dispose ();
			session.Dispose ();
		}

		/// <summary>
		/// Updates the UI to provide feedback on the state of the AR experience.
		/// </summary>
		private void UpdateSessionInfoLabel (ARCamera camera)
		{
			string message = null;

			switch (camera.TrackingState) {
			case ARTrackingState.NotAvailable:
				message = "Tracking Unavailable";
				break;

			case ARTrackingState.Limited:
				switch (camera.TrackingStateReason) {
				case ARTrackingStateReason.ExcessiveMotion:
					message = "Tracking Limited\nExcessive motion - Try slowing down your movement, or reset the session.";
					break;

				case ARTrackingStateReason.InsufficientFeatures:
					message = "Tracking Limited\nLow detail - Try pointing at a flat surface, or reset the session.";
					break;

				case ARTrackingStateReason.Initializing:
					message = "Initializing";
					break;

				case ARTrackingStateReason.Relocalizing:
					message = "Recovering from interruption";
					break;
				}
				break;

			case ARTrackingState.Normal:
				if (this.virtualObject == null) {
					if (this.isEnvironmentTextureAvailable) {
						message = "Tap to place a sphere, then tap or drag to move it or pinch to scale it.";
					} else {
						message = "Generating environment texture.";
					}
				}
				break;
			}


			// Show the message, or hide the label if there's no message.
			DispatchQueue.MainQueue.DispatchAsync (() => {
				if (this.sessionInfoLabel.Text != message) {
					UIView.Animate (0.25d, () => {
						this.sessionInfoLabel.Text = message;
						if (!string.IsNullOrEmpty (message)) {
							this.sessionInfoView.Alpha = 1;
						} else {
							this.sessionInfoView.Alpha = 0;
						}
					});
				}
			});
		}

		#endregion

		#region Environment Texturing

		private void UpdateEnvironmentProbe (double time)
		{
			// Update the probe only if the object has been moved or scaled,
			// only when manually placed, not too often.
			if (this.virtualObject != null &&
				this.currentTexturingMode == AREnvironmentTexturing.Manual &&
				time - this.lastProbeAnchorUpdateTime >= 1d &&
				this.requiresProbeRefresh) {
				// Remove existing probe anchor, if any.
				var probeAnchor = this.environmentProbeAnchor;
				if (probeAnchor != null) {
					this.sceneView.Session.RemoveAnchor (probeAnchor);
					this.environmentProbeAnchor.Dispose ();
					this.environmentProbeAnchor = null;
				}

				// Make sure the probe encompasses the object and provides some surrounding area to appear in reflections.
				var extent = SCNVector3.Multiply (this.virtualObject.GetExtents (), this.virtualObject.Scale);
				extent.X *= 3; // Reflect an area 3x the width of the object.
				extent.Z *= 3; // Reflect an area 3x the depth of the object.

				// Also include some vertical area around the object, but keep the bottom of the probe at the
				// bottom of the object so that it captures the real-world surface underneath.
				var verticalOffset = new SCNVector3 (0, extent.Y, 0);
				var transform = NMatrix4Extensions.CreateTranslation (this.virtualObject.Position + verticalOffset);
				extent.Y *= 2;

				// Create the new environment probe anchor and add it to the session.
				probeAnchor = new AREnvironmentProbeAnchor (transform, new OpenTK.NVector3 (extent.X, extent.Y, extent.Z));
				this.sceneView.Session.AddAnchor (probeAnchor);

				// Remember state to prevent updating the environment probe too often.
				this.environmentProbeAnchor = probeAnchor;
				this.lastProbeAnchorUpdateTime = CoreAnimation.CAAnimation.CurrentMediaTime ();
				this.requiresProbeRefresh = false;
			}
		}

		private void UpdateSceneEnvironmentProbe (ARFrame frame)
		{
			if (this.sceneEnvironmentProbeAnchor == null && this.currentTexturingMode == AREnvironmentTexturing.Manual) {
				// Create an environment probe anchor with room-sized extent to act as fallback when the probe anchor of
				// an object is removed and added during translation and scaling
				this.sceneEnvironmentProbeAnchor = new AREnvironmentProbeAnchor ("sceneProbe", OpenTK.NMatrix4.Identity, new OpenTK.NVector3 (5f, 5f, 5f));
				this.sceneView.Session.AddAnchor (this.sceneEnvironmentProbeAnchor);
			}
		}

		#endregion

		#region IARSCNViewDelegate

		[Export ("renderer:updateAtTime:")]
		public void Update (ISCNSceneRenderer renderer, double timeInSeconds)
		{
			this.UpdateEnvironmentProbe (timeInSeconds);
		}

		[Export ("renderer:didUpdateNode:forAnchor:")]
		public void DidUpdateNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
		{
			// Check for whether any environment textures have been generated.
			if (anchor is AREnvironmentProbeAnchor envProbeAnchor && !this.isEnvironmentTextureAvailable) {
				this.isEnvironmentTextureAvailable = envProbeAnchor.EnvironmentTexture != null;
			}
		}

		#endregion

		#region ARSessionObserver

		[Export ("session:cameraDidChangeTrackingState:")]
		public void CameraDidChangeTrackingState (ARSession session, ARCamera camera)
		{
			var frame = session.CurrentFrame;
			if (frame == null) {
				throw new Exception ("ARSession should have an ARFrame");
			}

			this.UpdateSessionInfoLabel (camera);
			frame.Dispose ();
		}

		[Export ("session:didFailWithError:")]
		public void DidFail (ARSession session, NSError error)
		{
			this.ResetTracking ();
		}

		[Export ("sessionShouldAttemptRelocalization:")]
		public bool ShouldAttemptRelocalization (ARSession session)
		{
			return true;
		}

		#endregion

		#region IARSessionDelegate

		[Export ("session:didUpdateFrame:")]
		public void DidUpdateFrame (ARSession session, ARFrame frame)
		{
			this.UpdateSceneEnvironmentProbe (frame);
			this.UpdateSessionInfoLabel (frame.Camera);
			frame.Dispose ();
		}

		#endregion

		#region Virtual Object gesture interaction

		partial void DidPan (UIPanGestureRecognizer gesture)
		{
			if (this.virtualObject != null) {
				switch (gesture.State) {
				case UIGestureRecognizerState.Changed:
					var translation = gesture.TranslationInView (this.sceneView);

					var previousPosition = this.lastPanTouchPosition ?? CGPointExtensions.Create (this.sceneView.ProjectPoint (this.virtualObject.Position));
					// Calculate the new touch position
					var currentPosition = new CGPoint (previousPosition.X + translation.X, previousPosition.Y + translation.Y);
					using (var hitTestResult = this.sceneView.SmartHitTest (currentPosition)) {
						if (hitTestResult != null) {
							this.virtualObject.Position = hitTestResult.WorldTransform.GetTranslation ();
							// Refresh the probe as the object keeps moving
							this.requiresProbeRefresh = true;
						}
					}

					this.lastPanTouchPosition = currentPosition;
					// reset the gesture's translation
					gesture.SetTranslation (CGPoint.Empty, this.sceneView);
					break;

				default:
					// Clear the current position tracking.
					this.lastPanTouchPosition = null;
					break;

				}
			}
		}

		partial void DidTap (UITapGestureRecognizer gesture)
		{
			// Allow placing objects only when ARKit tracking is in a good state for hit testing,
			// and environment texture is available (to prevent undesirable changes in reflected texture).
			var camera = this.sceneView.Session.CurrentFrame?.Camera;
			if (camera != null && camera.TrackingState == ARTrackingState.Normal && this.isEnvironmentTextureAvailable) {
				var touchLocation = gesture.LocationInView (this.sceneView);

				if (this.virtualObject != null) {
					using (var hitTestResult = this.sceneView.SmartHitTest (touchLocation)) {
						if (hitTestResult != null) {
							// Teleport the object to wherever the user touched the screen.
							this.virtualObject.Position = hitTestResult.WorldTransform.GetTranslation ();
							// Update the environment probe anchor when object is teleported.
							this.requiresProbeRefresh = true;
						}
					}
				} else {
					// Add the object to the scene at the tap location.
					DispatchQueue.DefaultGlobalQueue.DispatchAsync (() => {
						this.Place (this.virtualObjectModel, touchLocation);

						// Newly added object requires an environment probe anchor.
						this.requiresProbeRefresh = true;
					});
				}

				camera.Dispose ();
			}
		}

		partial void DidScale (UIPinchGestureRecognizer gesture)
		{
			if (this.virtualObject != null && gesture.State == UIGestureRecognizerState.Changed) {
				var newScale = SCNVector3.Multiply (this.virtualObject.Scale, (float) gesture.Scale);
				this.virtualObject.Scale = newScale;
				gesture.Scale = 1f;
				// Realistic reflections require an environment probe extent based on the size of the object,
				// so update the environment probe when the object is resized.
				this.requiresProbeRefresh = true;
			}
		}

		private void Place (SCNNode node, CGPoint location)
		{
			using (var hitTestResult = this.sceneView.SmartHitTest (location)) {
				if (hitTestResult != null) {
					this.sceneView.Scene.RootNode.AddChildNode (node);
					this.virtualObject = node;// Remember that the object has been placed.

					node.Position = hitTestResult.WorldTransform.GetTranslation ();

					// Update the status UI to indicate the newly placed object.
					var frame = this.sceneView.Session.CurrentFrame;
					if (frame != null) {
						this.UpdateSessionInfoLabel (frame.Camera);
					}
				}
			}
		}

		#endregion

		private void InitializeVirtualObjectModel ()
		{
			var sceneUrl = NSBundle.MainBundle.GetUrlForResource ("sphere", "scn", "art.scnassets/sphere");
			if (sceneUrl != null) {
				var referenceNode = SCNReferenceNode.CreateFromUrl (sceneUrl);
				if (referenceNode != null) {
					referenceNode.Load ();
					this.virtualObjectModel = referenceNode;
				}
			}

			if (this.virtualObjectModel == null) {
				throw new Exception ("can't load virtual object");
			}
		}
	}
}
