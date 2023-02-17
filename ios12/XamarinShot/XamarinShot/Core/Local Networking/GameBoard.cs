/*
Abstract:
Manages placement of the game board in real space before starting a game.
*/

namespace XamarinShot.Models {
	using ARKit;
	using CoreAnimation;
	using CoreGraphics;
	using Foundation;
	using SceneKit;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;

	/// GameBoard represents the physical surface which the game is played upon.
	/// In this node's child coordinate space, coordinates are normalized to the
	/// board's width. So if the user wants to see the game appear in worldspace 1.5 meters
	/// wide, the scale portion of this node's transform will be 1.5 in all dimensions.
	public class GameBoard : SCNNode {
		/// The minimum size of the board in meters
		private const float MinimumScale = 0.3f;

		/// The maximum size of the board in meters
		private const float MaximumScale = 11.0f; // 15x27m @ 10, 1.5m x 2.7m @ 1

		/// Duration of the open/close animation
		private const double AnimationDuration = 0.7d;

		/// The level's preferred size.
		/// This is used both to set the aspect ratio and to determine
		/// the default size.
		private CGSize preferredSize = new CGSize (1.5f, 2.7f);

		/// Indicates whether the segments of the border are disconnected.
		private bool isBorderOpen;

		/// Indicates if the game board is currently being animated.
		private bool isAnimating;

		/// The game board's most recent positions.
		private List<SCNVector3> recentPositions = new List<SCNVector3> ();

		/// The game board's most recent rotation angles.
		private List<float> recentRotationAngles = new List<float> ();

		/// Previously visited plane anchors.
		private readonly List<ARAnchor> anchorsOfVisitedPlanes = new List<ARAnchor> ();

		/// The node used to visualize the game border.
		private readonly SCNNode borderNode = new SCNNode ();

		/// List of the segments in the border.
		private readonly List<BorderSegment> borderSegments = new List<BorderSegment> ();

		public GameBoard () : base ()
		{
			// Set initial game board scale
			this.Scale = new SCNVector3 (GameBoard.MinimumScale, GameBoard.MinimumScale, GameBoard.MinimumScale);

			// Create all border segments
			foreach (Corner corner in Enum.GetValues (typeof (Corner))) {
				foreach (Alignment alignment in Enum.GetValues (typeof (Alignment))) {
					var borderSize = new CGSize (1f, this.AspectRatio);
					var borderSegment = new BorderSegment (corner, alignment, borderSize);

					this.borderSegments.Add (borderSegment);
					this.borderNode.AddChildNode (borderSegment);
				}
			}

			// Create fill plane
			this.borderNode.AddChildNode (this.FillPlane);

			// Orient border to XZ plane and set aspect ratio
			this.borderNode.EulerAngles = new SCNVector3 ((float) Math.PI / 2f, this.borderNode.EulerAngles.Y, this.borderNode.EulerAngles.Z);
			this.borderNode.Hidden = true;

			this.AddChildNode (this.borderNode);
		}

		public GameBoard (NSCoder coder) => throw new NotImplementedException ("It has not been implemented");

		/// <summary>
		/// The color of the border
		/// </summary>
		public static UIColor BorderColor { get; } = UIColor.White;

		/// <summary>
		/// The BoardAnchor in the scene
		/// </summary>
		public BoardAnchor Anchor { get; set; }

		/// <summary>
		/// Indicates whether the border is currently hidden
		/// </summary>
		public bool IsBorderHidden => this.borderNode.Hidden || this.borderNode.GetAction ("hide") != null;

		/// <summary>
		/// The aspect ratio of the level.
		/// </summary>
		public float AspectRatio => (float) (this.preferredSize.Height / this.preferredSize.Width);

		/// <summary>
		/// The level's preferred size.
		/// This is used both to set the aspect ratio and to determine
		/// the default size.
		/// </summary>
		public CGSize PreferredSize {
			get {
				return this.preferredSize;
			}

			set {
				this.preferredSize = value;
				this.UpdateBorderAspectRatio ();
			}
		}

		#region Appearance

		/// <summary>
		/// Hides the border.
		/// </summary>
		public void HideBorder (double duration = 0.5d)
		{
			if (this.borderNode.GetAction ("hide") == null) {
				this.borderNode.RemoveAction ("unhide");
				this.borderNode.RunAction (SCNAction.FadeOut (duration), "hide", () => {
					this.borderNode.Hidden = true;
				});
			}
		}

		/// <summary>
		/// Unhides the border.
		/// </summary>
		public void UnhideBorder ()
		{
			if (this.borderNode.GetAction ("unhide") == null) {
				this.borderNode.RemoveAction ("hide");
				this.borderNode.RunAction (SCNAction.FadeIn (0.5), "unhide");
				this.borderNode.Hidden = false;
			}
		}

		/// <summary>
		/// Updates the game board with the latest hit test result and camera.
		/// </summary>
		public void Update (ARHitTestResult hitTestResult, ARCamera camera)
		{
			if (this.IsBorderHidden) {
				this.UnhideBorder ();
			}

			if (hitTestResult.Anchor is ARPlaneAnchor planeAnchor) {
				this.PerformCloseAnimation (!this.anchorsOfVisitedPlanes.Contains (planeAnchor));
				this.anchorsOfVisitedPlanes.Add (planeAnchor);
			} else {
				this.PerformOpenAnimation ();
			}

			this.UpdateTransform (hitTestResult, camera);
		}

		public void Reset ()
		{
			this.borderNode.RemoveAllActions ();
			this.borderNode.Hidden = true;
			this.recentPositions.Clear ();
			this.recentRotationAngles.Clear ();
			this.Hidden = false;
		}

		/// <summary>
		/// Incrementally scales the board by the given amount
		/// </summary>
		public void UpdateScale (float factor)
		{
			// assumes we always scale the same in all 3 dimensions
			var currentScale = this.Scale.X;
			var newScale = DigitExtensions.Clamp (currentScale * factor, GameBoard.MinimumScale, GameBoard.MaximumScale);
			this.Scale = new SCNVector3 (newScale, newScale, newScale);
		}

		public void UseDefaultScale ()
		{
			var scale = (float) this.preferredSize.Width;
			this.Scale = new SCNVector3 (scale, scale, scale);
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Update the transform of the game board with the latest hit test result and camera
		/// </summary>
		private void UpdateTransform (ARHitTestResult hitTestResult, ARCamera camera)
		{
			var position = hitTestResult.WorldTransform.GetTranslation ();

			// Average using several most recent positions.
			this.recentPositions.Add (position);
			this.recentPositions = new List<SCNVector3> (this.recentPositions.TakeLast (10));

			// Move to average of recent positions to avoid jitter.
			var average = this.recentPositions.Reduce (new SCNVector3 (0f, 0f, 0f)) / (float) this.recentPositions.Count;
			this.Position = average;

			// Orient bounds to plane if possible
			if (hitTestResult.Anchor is ARPlaneAnchor planeAnchor) {
				this.OrientToPlane (planeAnchor, camera);
				this.ScaleToPlane (planeAnchor);
			} else {
				// Fall back to camera orientation
				this.OrientToCamera (camera);
				this.Scale = new SCNVector3 (GameBoard.MinimumScale, GameBoard.MinimumScale, GameBoard.MinimumScale);
			}

			// Remove any animation duration if present
			SCNTransaction.AnimationDuration = 0;
		}

		private void OrientToCamera (ARCamera camera)
		{
			this.Rotate (camera.EulerAngles.Y);
		}

		private void OrientToPlane (ARPlaneAnchor planeAnchor, ARCamera camera)
		{
			// Get board rotation about y
			this.Orientation = SimdExtensions.CreateQuaternion (planeAnchor.Transform.ToSCNMatrix4 ());
			var boardAngle = this.EulerAngles.Y;

			// If plane is longer than deep, rotate 90 degrees
			if (planeAnchor.Extent.X > planeAnchor.Extent.Z) {
				boardAngle += (float) Math.PI / 2f;
			}

			// Normalize angle to closest 180 degrees to camera angle
			boardAngle = boardAngle.NormalizedAngle (camera.EulerAngles.Y, (float) Math.PI);

			this.Rotate (boardAngle);
		}

		private void Rotate (float angle)
		{
			// Avoid interpolating between angle flips of 180 degrees
			var previouAngle = this.recentRotationAngles.Sum () / (float) this.recentRotationAngles.Count;
			if (Math.Abs (angle - previouAngle) > Math.PI / 2) {
				this.recentRotationAngles = this.recentRotationAngles.Select (value => value.NormalizedAngle (angle, (float) Math.PI)).ToList ();
			}

			// Average using several most recent rotation angles.
			this.recentRotationAngles.Add (angle);
			this.recentRotationAngles = new List<float> (this.recentRotationAngles.TakeLast (20));

			// Move to average of recent positions to avoid jitter.
			var averageAngle = this.recentRotationAngles.Sum () / (float) this.recentRotationAngles.Count;
			this.Rotation = new SCNVector4 (0f, 1f, 0f, averageAngle);
		}

		private void ScaleToPlane (ARPlaneAnchor planeAnchor)
		{
			// Determine if extent should be flipped (plane is 90 degrees rotated)
			var planeXAxis = planeAnchor.Transform.Column0.Xyz;
			var axisFlipped = Math.Abs (SCNVector3.Dot (planeXAxis, this.WorldRight)) < 0.5f;

			// Flip dimensions if necessary
			var planeExtent = planeAnchor.Extent;
			if (axisFlipped) {
				planeExtent = new OpenTK.NVector3 (planeExtent.Z, 0f, planeExtent.X);
			}

			// Scale board to the max extent that fits in the plane
			var width = Math.Min (planeExtent.X, GameBoard.MaximumScale);
			var depth = Math.Min (planeExtent.Z, width * this.AspectRatio);
			width = depth / this.AspectRatio;
			this.Scale = new SCNVector3 (width, width, width);

			// Adjust position of board within plane's bounds
			var planeLocalExtent = new SCNVector3 (width, 0f, depth);
			if (axisFlipped) {
				planeLocalExtent = new SCNVector3 (planeLocalExtent.Z, 0f, planeLocalExtent.X);
			}

			this.AdjustPosition (planeAnchor, planeLocalExtent);
		}

		private void AdjustPosition (ARPlaneAnchor planeAnchor, SCNVector3 extent)
		{
			var positionAdjusted = false;
			var planeAnchorTransform = planeAnchor.Transform.ToSCNMatrix4 ();
			var worldToPlane = SCNMatrix4.Invert (planeAnchorTransform);

			// Get current position in the local plane coordinate space
			var planeLocalPosition = worldToPlane.Multiply (this.Transform.Column3);

			// Compute bounds min and max
			var boardMin = planeLocalPosition.Xyz - extent / 2f;
			var boardMax = planeLocalPosition.Xyz + extent / 2f;

			var planeAnchorCenter = planeAnchor.Center.ToSCNVector3 ();
			var planeAnchorExtent = planeAnchor.Extent.ToSCNVector3 ();
			var planeMin = planeAnchorCenter - planeAnchorExtent / 2f;
			var planeMax = planeAnchorCenter + planeAnchorExtent / 2f;

			// Adjust position for x within plane bounds
			if (boardMin.X < planeMin.X) {
				planeLocalPosition.X += planeMin.X - boardMin.X;
				positionAdjusted = true;
			} else if (boardMax.X > planeMax.X) {
				planeLocalPosition.X -= boardMax.X - planeMax.X;
				positionAdjusted = true;
			}

			// Adjust position for z within plane bounds
			if (boardMin.Z < planeMin.Z) {
				planeLocalPosition.Z += planeMin.Z - boardMin.Z;
				positionAdjusted = true;
			} else if (boardMax.Z > planeMax.Z) {
				planeLocalPosition.Z -= boardMax.Z - planeMax.Z;
				positionAdjusted = true;
			}

			if (positionAdjusted) {
				this.Position = (planeAnchorTransform.Multiply (planeLocalPosition)).Xyz;
			}
		}

		private void UpdateBorderAspectRatio ()
		{
			var borderSize = new CGSize (1f, this.AspectRatio);
			foreach (var segment in this.borderSegments) {
				segment.BorderSize = borderSize;
			}

			if (this.FillPlane.Geometry is SCNPlane plane) {
				var length = 1 - 2 * BorderSegment.Thickness;
				plane.Height = length * this.AspectRatio;

				var textureScale = SimdExtensions.CreateFromScale (new SCNVector3 (40f, 40f * this.AspectRatio, 1f));
				if (plane.FirstMaterial != null) {
					plane.FirstMaterial.Diffuse.ContentsTransform = textureScale;
					plane.FirstMaterial.Emission.ContentsTransform = textureScale;
				}
			}

			this.isBorderOpen = false;
		}

		#endregion

		#region Animations

		private void PerformOpenAnimation ()
		{
			if (!this.isBorderOpen && !this.isAnimating) {
				this.isBorderOpen = true;
				this.isAnimating = true;

				// Open animation
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = GameBoard.AnimationDuration / 4f;
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);

				this.borderNode.Opacity = 1f;
				foreach (var segment in this.borderSegments) {
					segment.Open ();
				}

				this.Scale = new SCNVector3 (GameBoard.MinimumScale, GameBoard.MinimumScale, GameBoard.MinimumScale);

				// completion is run on main-thread
				SCNTransaction.SetCompletionBlock (() => {
					SCNTransaction.Begin ();
					this.borderNode.RunAction (this.PulseAction (), "pulse");
					this.isAnimating = false;
					SCNTransaction.Commit ();
				});


				SCNTransaction.Commit ();
			}
		}

		private void PerformCloseAnimation (bool flash = false)
		{
			if (this.isBorderOpen && !this.isAnimating) {
				this.isBorderOpen = false;
				this.isAnimating = true;

				this.borderNode.RemoveAction ("pulse");
				this.borderNode.Opacity = 1f;

				// Close animation
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = GameBoard.AnimationDuration / 2f;
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);

				this.borderNode.Opacity = 0.99f;
				SCNTransaction.SetCompletionBlock (() => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = GameBoard.AnimationDuration / 2f;
					SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);

					foreach (var segment in this.borderSegments) {
						segment.Close ();
					}

					SCNTransaction.SetCompletionBlock (() => {
						this.isAnimating = false;
					});

					SCNTransaction.Commit ();
				});

				SCNTransaction.Commit ();

				if (flash) {
					var waitAction = SCNAction.Wait (GameBoard.AnimationDuration * 0.75f);
					var fadeInAction = SCNAction.FadeOpacityTo (0.6f, GameBoard.AnimationDuration * 0.125f);
					var fadeOutAction = SCNAction.FadeOpacityTo (0f, GameBoard.AnimationDuration * 0.125f);
					this.FillPlane.RunAction (SCNAction.Sequence (new SCNAction [] { waitAction, fadeOutAction, fadeInAction }));
				}
			}
		}

		#endregion

		#region Convenience Methods

		protected SCNNode FillPlane => this.GetFillPlane ();

		private SCNNode GetFillPlane ()
		{
			var length = 1f - 2f * BorderSegment.Thickness;
			var plane = SCNPlane.Create (length, length * this.AspectRatio);
			var node = SCNNode.FromGeometry (plane);
			node.Name = "fillPlane";
			node.Opacity = 0.6f;

			var material = plane.FirstMaterial;
			material.Diffuse.Contents = UIImage.FromBundle ("art.scnassets/textures/grid.png");

			var textureScale = SimdExtensions.CreateFromScale (new SCNVector3 (40f, 40f * this.AspectRatio, 1f));
			material.Diffuse.ContentsTransform = textureScale;
			material.Emission.Contents = UIImage.FromBundle ("art.scnassets/textures/grid.png");
			material.Emission.ContentsTransform = textureScale;
			material.Diffuse.WrapS = SCNWrapMode.Repeat;
			material.Diffuse.WrapT = SCNWrapMode.Repeat;
			material.DoubleSided = true;
			material.Ambient.Contents = UIColor.Black;
			material.LightingModelName = SCNLightingModel.Constant;

			return node;
		}

		#endregion

		#region Animations and Actions

		private SCNAction PulseAction ()
		{
			var pulseOutAction = SCNAction.FadeOpacityTo (0.4f, 0.5f);
			var pulseInAction = SCNAction.FadeOpacityTo (1f, 0.5f);
			pulseOutAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;
			pulseInAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;

			return SCNAction.RepeatActionForever (SCNAction.Sequence (new SCNAction [] { pulseOutAction, pulseInAction }));
		}

		#endregion
	}
}
