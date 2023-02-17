
namespace XamarinShot {
	using ARKit;
	using Foundation;
	using SceneKit;
	using XamarinShot.Models.Enums;
	using XamarinShot.Models.GestureRecognizers;
	using System;
	using System.Linq;
	using UIKit;

	/// <summary>
	/// Gesture interaction methods for the Game Scene View Controller.
	/// </summary>
	partial class GameViewController : IUIGestureRecognizerDelegate {
		partial void handleTap (UITapGestureRecognizer gesture)
		{
			if (gesture.State == UIGestureRecognizerState.Ended) {
				var location = gesture.LocationInView (this.sceneView);

				switch (this.sessionState) {
				case SessionState.PlacingBoard:
				case SessionState.AdjustingBoard:
					if (!this.gameBoard.IsBorderHidden) {
						this.SessionState = SessionState.SetupLevel;
					}
					break;

				case SessionState.GameInProgress:
					this.gameManager?.HandleTouch (TouchType.Tapped);
					break;
				}
			}
		}

		partial void handlePinch (ThresholdPinchGestureRecognizer gesture)
		{
			if (this.CanAdjustBoard) {
				this.SessionState = SessionState.AdjustingBoard;

				switch (gesture.State) {
				case UIGestureRecognizerState.Changed:
					if (gesture.IsThresholdExceeded) {
						this.gameBoard.UpdateScale ((float) gesture.Scale);
						gesture.Scale = 1f;
					}
					break;
				}
			}
		}

		partial void handleRotation (ThresholdRotationGestureRecognizer gesture)
		{
			if (this.CanAdjustBoard) {
				this.SessionState = SessionState.AdjustingBoard;

				switch (gesture.State) {
				case UIGestureRecognizerState.Changed:
					if (gesture.IsThresholdExceeded) {
						var newY = this.gameBoard.EulerAngles.Y;
						if (this.gameBoard.EulerAngles.X > Math.PI / 2d) {
							newY += (float) gesture.Rotation;
						} else {
							newY -= (float) gesture.Rotation;
						}

						this.gameBoard.EulerAngles = new SCNVector3 (this.gameBoard.EulerAngles.X, newY, this.gameBoard.EulerAngles.Z);
						gesture.Rotation = 0f;
					}
					break;
				}
			}
		}


		partial void handlePan (ThresholdPanGestureRecognizer gesture)
		{
			if (this.CanAdjustBoard) {
				this.SessionState = SessionState.AdjustingBoard;

				var location = gesture.LocationInView (this.sceneView);
				var results = this.sceneView.HitTest (location, ARHitTestResultType.ExistingPlane);
				var nearestPlane = results.FirstOrDefault ();
				if (nearestPlane != null) {
					switch (gesture.State) {
					case UIGestureRecognizerState.Began:
						this.panOffset = nearestPlane.WorldTransform.Column3.Xyz - this.gameBoard.WorldPosition;
						break;
					case UIGestureRecognizerState.Changed:
						this.gameBoard.WorldPosition = nearestPlane.WorldTransform.Column3.Xyz - this.panOffset;
						break;
					}
				}
			}
		}

		private void HandleLongPress (UILongPressGestureRecognizer gesture)
		{
			if (this.CanAdjustBoard) {
				this.SessionState = SessionState.AdjustingBoard;
				this.gameBoard.UseDefaultScale ();
			}
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			this.Touch (TouchType.Began);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			this.Touch (TouchType.Ended);
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			this.Touch (TouchType.Ended);
		}

		private void Touch (TouchType type)
		{
			this.gameManager?.HandleTouch (type);
		}

		private bool GestureRecognizer (UIGestureRecognizer first, UIGestureRecognizer second)
		{
			if (first is UIRotationGestureRecognizer && second is UIPinchGestureRecognizer) {
				return true;
			} else if (first is UIRotationGestureRecognizer && second is UIPanGestureRecognizer) {
				return true;
			} else if (first is UIPinchGestureRecognizer && second is UIRotationGestureRecognizer) {
				return true;
			} else if (first is UIPinchGestureRecognizer && second is UIPanGestureRecognizer) {
				return true;
			} else if (first is UIPanGestureRecognizer && second is UIPinchGestureRecognizer) {
				return true;
			} else if (first is UIPanGestureRecognizer && second is UIRotationGestureRecognizer) {
				return true;
			}

			return false;
		}
	}
}
