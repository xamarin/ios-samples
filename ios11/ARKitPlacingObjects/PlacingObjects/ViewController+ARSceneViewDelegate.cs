using System;
using ARKit;
using CoreFoundation;
using Foundation;
using SceneKit;

namespace PlacingObjects
{
	public partial class ViewController : IARSCNViewDelegate
	{
		[Export("renderer:updateAtTime:")]
		public void RendererUpdateAtTime(SCNSceneRenderer renderer, double updateAtTime)
		{
			if (Session?.CurrentFrame == null)
			{
				return;
			}
			// Vital for memory: Single location to set current frame! (Note: Assignment disposes existing frame -- see `set`
			ViewController.CurrentFrame = Session.CurrentFrame;
			UpdateFocusSquare();

			// If light estimation is enabled, update the intensity of the model's lights and the environment map
			var lightEstimate = ViewController.CurrentFrame.LightEstimate;
			if (lightEstimate != null)
			{
				SceneView.Scene.EnableEnvironmentMapWithIntensity((float)(lightEstimate.AmbientIntensity / 40f), serialQueue);
			}
			else
			{
				SceneView.Scene.EnableEnvironmentMapWithIntensity(40f, serialQueue);
			}
		}


		[Export("renderer:didAddNode:forAnchor:")]
		public void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
		{
			var planeAnchor = anchor as ARPlaneAnchor;
			if (planeAnchor != null)
			{
				AddPlane(node, planeAnchor);
				virtualObjectManager.CheckIfObjectShouldMoveOntoPlane(planeAnchor, node);
			}
		}

		[Export("renderer:didUpdateNode:forAnchor:")]
		public void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
		{
			var planeAnchor = anchor as ARPlaneAnchor;
			if (planeAnchor != null)
			{
				UpdatePlane(planeAnchor);
				virtualObjectManager.CheckIfObjectShouldMoveOntoPlane(planeAnchor, node);
			}
		}

		[Export("renderer:didRemoveNode:forAnchor:")]
		public void DidRemoveNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
		{
			var planeAnchor = anchor as ARPlaneAnchor;
			if (planeAnchor != null)
			{
				serialQueue.DispatchAsync(() => RemovePlane(planeAnchor));
			}
		}

		public void SessionDelegate_CameraDidChangeTrackingState(ARSession session, ARCamera camera)
		{
			UserFeedback.ShowTrackingQualityInfo(camera.TrackingState, camera.TrackingStateReason, true);

			switch (camera.TrackingState)
			{
				case ARTrackingState.NotAvailable:
				case ARTrackingState.Limited:
					UserFeedback.EscalateFeedback(camera.TrackingState, camera.TrackingStateReason, 3);
					break;
				case ARTrackingState.Normal:
					UserFeedback.CancelScheduledMessage(MessageType.TrackingStateEscalation);
					break;
			}
		}

		public void SessionDelegate_DidFail(ARSession session, NSError error)
		{
			var sessionErrorMsg = $"{error.LocalizedDescription} {error.LocalizedFailureReason} ";
			var recoveryOptions = error.LocalizedRecoveryOptions;
			if (recoveryOptions != null)
			{
				foreach (string option in recoveryOptions)
				{
					sessionErrorMsg += $"{option}.";
				}
			}

			DisplayErrorMessage("We're sorry!", sessionErrorMsg, false);
		}

		internal void SessionDelegate_WasInterrupted(ARSession session)
		{
			UserFeedback.BlurBackground();
			UserFeedback.ShowAlert("Session Interrupted", "The session will be reset after the interruption ends.");
		}

		internal void SessionDelegate_InterruptionEnded(ARSession session)
		{
			UserFeedback.UnblurBackground();
			Session.Run(StandardConfiguration(), ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
			RestartExperience(this);
			UserFeedback.ShowMessage("RESETTING SESSION");
		}
	}
}
