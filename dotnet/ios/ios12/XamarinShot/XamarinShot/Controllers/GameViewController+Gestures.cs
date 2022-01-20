
namespace XamarinShot;


/// <summary>
/// Gesture interaction methods for the Game Scene View Controller.
/// </summary>
partial class GameViewController : IUIGestureRecognizerDelegate
{
        partial void handleTap (UITapGestureRecognizer sender)
        {
                if (sender.State == UIGestureRecognizerState.Ended)
                {
                        var location = sender.LocationInView (sceneView);

                        switch (sessionState)
                        {
                                case SessionState.PlacingBoard:
                                case SessionState.AdjustingBoard:
                                        if (!gameBoard.IsBorderHidden)
                                        {
                                                SessionState = SessionState.SetupLevel;
                                        }
                                        break;

                                case SessionState.GameInProgress:
                                        gameManager?.HandleTouch (TouchType.Tapped);
                                        break;
                        }
                }
        }

        partial void handlePinch (ThresholdPinchGestureRecognizer sender)
        {
                if (CanAdjustBoard)
                {
                        SessionState = SessionState.AdjustingBoard;

                        switch (sender.State)
                        {
                                case UIGestureRecognizerState.Changed:
                                        if (sender.IsThresholdExceeded)
                                        {
                                                gameBoard.UpdateScale ((float)sender.Scale);
                                                sender.Scale = 1f;
                                        }
                                        break;
                        }
                }
        }

        partial void handleRotation (ThresholdRotationGestureRecognizer sender)
        {
                if (CanAdjustBoard)
                {
                        SessionState = SessionState.AdjustingBoard;

                        switch (sender.State)
                        {
                                case UIGestureRecognizerState.Changed:
                                        if (sender.IsThresholdExceeded)
                                        {
                                                var newY = gameBoard.EulerAngles.Y;
                                                if (gameBoard.EulerAngles.X > Math.PI / 2d)
                                                {
                                                        newY += (float)sender.Rotation;
                                                }
                                                else
                                                {
                                                        newY -= (float)sender.Rotation;
                                                }

                                                gameBoard.EulerAngles = new SCNVector3 (gameBoard.EulerAngles.X, newY, this.gameBoard.EulerAngles.Z);
                                                sender.Rotation = 0f;
                                        }
                                        break;
                        }
                }
        }


        partial void handlePan (ThresholdPanGestureRecognizer sender)
        {
                if (CanAdjustBoard)
                {
                        SessionState = SessionState.AdjustingBoard;

                        var location = sender.LocationInView (sceneView);
                        var results = sceneView.HitTest (location, ARHitTestResultType.ExistingPlane);
                        var nearestPlane = results.FirstOrDefault ();
                        if (nearestPlane is not null)
                        {
                                switch (sender.State)
                                {
                                        case UIGestureRecognizerState.Began:
                                                panOffset = nearestPlane.WorldTransform.Column3.Xyz - gameBoard.WorldPosition;
                                                break;
                                        case UIGestureRecognizerState.Changed:
                                                gameBoard.WorldPosition = nearestPlane.WorldTransform.Column3.Xyz - panOffset;
                                                break;
                                }
                        }
                }
        }

        private void HandleLongPress (UILongPressGestureRecognizer sender)
        {
                if (CanAdjustBoard)
                {
                        SessionState = SessionState.AdjustingBoard;
                        gameBoard.UseDefaultScale ();
                }
        }

        public override void TouchesBegan (NSSet touches, UIEvent? evt)
        {
                Touch (TouchType.Began);
        }

        public override void TouchesEnded (NSSet touches, UIEvent? evt)
        {
                Touch (TouchType.Ended);
        }

        public override void TouchesCancelled (NSSet touches, UIEvent? evt)
        {
                Touch (TouchType.Ended);
        }

        void Touch (TouchType type)
        {
                gameManager?.HandleTouch (type);
        }

        bool GestureRecognizer (UIGestureRecognizer first, UIGestureRecognizer second)
        {
                if (first is UIRotationGestureRecognizer && second is UIPinchGestureRecognizer)
                {
                        return true;
                } else if (first is UIRotationGestureRecognizer && second is UIPanGestureRecognizer)
                {
                        return true;
                } else if (first is UIPinchGestureRecognizer && second is UIRotationGestureRecognizer)
                {
                        return true;
                } else if (first is UIPinchGestureRecognizer && second is UIPanGestureRecognizer)
                {
                        return true;
                } else if (first is UIPanGestureRecognizer && second is UIPinchGestureRecognizer)
                {
                        return true;
                }
                else if (first is UIPanGestureRecognizer && second is UIRotationGestureRecognizer)
                {
                        return true;
                }

                return false;
        }
}
