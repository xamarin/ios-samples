
namespace XamarinShot {
	using ARKit;
	using CoreFoundation;
	using Foundation;
	using SceneKit;
	using XamarinShot.Models;
	using XamarinShot.Models.Enums;
	using System.Linq;

	/// <summary>
	/// ARSCNViewDelegate methods for the Game Scene View Controller.
	/// </summary>
	partial class GameViewController : IARSCNViewDelegate {
		[Export ("renderer:nodeForAnchor:")]
		public SCNNode GetNode (SceneKit.ISCNSceneRenderer renderer, ARKit.ARAnchor anchor)
		{
			SCNNode result = null;
			if (anchor == this.gameBoard.Anchor) {
				// If board anchor was added, setup the level.
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (this.sessionState == SessionState.LocalizingToBoard) {
						this.SessionState = SessionState.SetupLevel;
					}
				});

				// We already created a node for the board anchor
				result = this.gameBoard;
			}

			return result;
		}

		[Export ("renderer:didUpdateNode:forAnchor:")]
		public void DidUpdateNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
		{
			if (anchor is BoardAnchor boardAnchor) {
				// Update the game board's scale from the board anchor
				// The transform will have already been updated - without the scale
				var width = (float) boardAnchor.Size.Width;
				node.Scale = new SCNVector3 (width, width, width);
			}
		}

		[Export ("session:cameraDidChangeTrackingState:")]
		public void CameraDidChangeTrackingState (ARSession session, ARCamera camera)
		{
			//os_log(.info, "camera tracking state changed to %s", "\(camera.trackingState)")
			DispatchQueue.MainQueue.DispatchAsync (() => {
				this.trackingStateLabel.Text = camera.TrackingState.ToString ();
			});

			switch (camera.TrackingState) {
			case ARTrackingState.Normal:
				// Resume game if previously interrupted
				if (this.isSessionInterrupted) {
					this.IsSessionInterrupted = false;
				}

				// Fade in the board if previously hidden
				if (this.gameBoard.Hidden) {
					this.gameBoard.Opacity = 1f;
					this.gameBoard.Hidden = false;
				}

				// Fade in the level if previously hidden
				if (this.renderRoot.Opacity == 0f) {
					this.renderRoot.Opacity = 1f;
				}
				break;

			case ARTrackingState.Limited:
				// Hide the game board and level if tracking is limited
				this.gameBoard.Hidden = true;
				this.renderRoot.Opacity = 0f;
				break;
			}
		}

		[Export ("session:didFailWithError:")]
		public void DidFail (ARSession session, NSError error)
		{
			// Get localized strings from error
			string [] messages =
			{
				error.LocalizedDescription,
				error.LocalizedFailureReason,
				error.LocalizedRecoverySuggestion
			};

			// Use `compactMap(_:)` to remove optional error messages.
			var errorMessage = string.Join ("\n", messages.Where (message => !string.IsNullOrEmpty (message)));

			// Present the error message to the user
			this.ShowAlert ("Session Error", errorMessage);
		}

		[Export ("sessionWasInterrupted:")]
		public void WasInterrupted (ARSession session)
		{
			// Inform the user that the session has been interrupted
			this.IsSessionInterrupted = true;

			// Hide game board and level
			this.gameBoard.Hidden = true;
			this.renderRoot.Opacity = 0f;
		}

		[Export ("sessionShouldAttemptRelocalization:")]
		public bool ShouldAttemptRelocalization (ARSession session)
		{
			return true;
		}
	}
}
