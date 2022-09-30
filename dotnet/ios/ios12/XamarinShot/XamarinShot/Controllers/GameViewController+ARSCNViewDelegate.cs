namespace XamarinShot;


/// <summary>
/// ARSCNViewDelegate methods for the Game Scene View Controller.
/// </summary>
partial class GameViewController : IARSCNViewDelegate
{
        [Export ("renderer:nodeForAnchor:")]
        public SCNNode? GetNode (ISCNSceneRenderer renderer, ARAnchor anchor)
        {
                SCNNode? result = null;
                if (anchor == gameBoard.Anchor)
                {
                        // If board anchor was added, setup the level.
                        DispatchQueue.MainQueue.DispatchAsync (() =>
                        {
                                if (sessionState == SessionState.LocalizingToBoard)
                                {
                                        SessionState = SessionState.SetupLevel;
                                }
                        });

                        // We already created a node for the board anchor
                        result = gameBoard;
                }

                return result;
        }

        [Export ("renderer:didUpdateNode:forAnchor:")]
        public void DidUpdateNode (ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
                if (anchor is BoardAnchor boardAnchor)
                {
                        // Update the game board's scale from the board anchor
                        // The transform will have already been updated - without the scale
                        var width = (float)boardAnchor.Size.Width;
                        node.Scale = new SCNVector3 (width, width, width);
                }
        }

        [Export ("session:cameraDidChangeTrackingState:")]
        public void CameraDidChangeTrackingState (ARSession session, ARCamera camera)
        {
                //os_log(.info, "camera tracking state changed to %s", "\(camera.trackingState)")
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        trackingStateLabel.Text = camera.TrackingState.ToString ();
                });

                switch (camera.TrackingState)
                {
                        case ARTrackingState.Normal:
                                // Resume game if previously interrupted
                                if (isSessionInterrupted)
                                {
                                        IsSessionInterrupted = false;
                                }

                                // Fade in the board if previously hidden
                                if (gameBoard.Hidden)
                                {
                                        gameBoard.Opacity = 1f;
                                        gameBoard.Hidden = false;
                                }

                                // Fade in the level if previously hidden
                                if (renderRoot.Opacity == 0f)
                                {
                                        renderRoot.Opacity = 1f;
                                }
                                break;

                        case ARTrackingState.Limited:
                                // Hide the game board and level if tracking is limited
                                gameBoard.Hidden = true;
                                renderRoot.Opacity = 0f;
                                break;
                }
        }

        [Export ("session:didFailWithError:")]
        public void DidFail (ARSession session, NSError error)
        {
                // Get localized strings from error
                string [] messages = {
                        error.LocalizedDescription,
                        error.LocalizedFailureReason,
                        error.LocalizedRecoverySuggestion
                };

                // Use `compactMap(_:)` to remove optional error messages.
                var errorMessage = string.Join ("\n", messages.Where (message => !string.IsNullOrEmpty (message)));

                // Present the error message to the user
                ShowAlert ("Session Error", errorMessage);
        }

        [Export ("sessionWasInterrupted:")]
        public void WasInterrupted (ARSession session)
        {
                // Inform the user that the session has been interrupted
                IsSessionInterrupted = true;

                // Hide game board and level
                gameBoard.Hidden = true;
                renderRoot.Opacity = 0f;
        }

        [Export ("sessionShouldAttemptRelocalization:")]
        public bool ShouldAttemptRelocalization (ARSession session)
        {
                return true;
        }
}
