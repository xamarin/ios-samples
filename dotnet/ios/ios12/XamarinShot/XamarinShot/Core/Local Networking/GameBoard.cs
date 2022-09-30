/*
Abstract:
Manages placement of the game board in real space before starting a game.
*/

namespace XamarinShot.Models;

/// GameBoard represents the physical surface which the game is played upon.
/// In this node's child coordinate space, coordinates are normalized to the
/// board's width. So if the user wants to see the game appear in worldspace 1.5 meters
/// wide, the scale portion of this node's transform will be 1.5 in all dimensions.
public class GameBoard : SCNNode
{
        /// The minimum size of the board in meters
        const float MinimumScale = 0.3f;

        /// The maximum size of the board in meters
        const float MaximumScale = 11.0f; // 15x27m @ 10, 1.5m x 2.7m @ 1

        /// Duration of the open/close animation
        const double AnimationDuration = 0.7d;

        /// The level's preferred size.
        /// This is used both to set the aspect ratio and to determine
        /// the default size.
        CGSize preferredSize = new CGSize (1.5f, 2.7f);

        /// Indicates whether the segments of the border are disconnected.
        bool isBorderOpen;

        /// Indicates if the game board is currently being animated.
        bool isAnimating;

        /// The game board's most recent positions.
        List<SCNVector3> recentPositions = new List<SCNVector3> ();

        /// The game board's most recent rotation angles.
        List<float> recentRotationAngles = new List<float> ();

        /// Previously visited plane anchors.
        readonly List<ARAnchor> anchorsOfVisitedPlanes = new List<ARAnchor> ();

        /// The node used to visualize the game border.
        readonly SCNNode borderNode = new SCNNode ();

        /// List of the segments in the border.
        readonly List<BorderSegment> borderSegments = new List<BorderSegment> ();

        public GameBoard () : base ()
        {
                // Set initial game board scale
                Scale = new SCNVector3 (GameBoard.MinimumScale, GameBoard.MinimumScale, GameBoard.MinimumScale);

                // Create all border segments
                foreach (Corner corner in Enum.GetValues (typeof (Corner)))
                {
                        foreach (Alignment alignment in Enum.GetValues (typeof (Alignment)))
                        {
                                var borderSize = new CGSize (1f, AspectRatio);
                                var borderSegment = new BorderSegment (corner, alignment, borderSize);

                                borderSegments.Add (borderSegment);
                                borderNode.AddChildNode (borderSegment);
                        }
                }

                // Create fill plane
                borderNode.AddChildNode (FillPlane);

                // Orient border to XZ plane and set aspect ratio
                borderNode.EulerAngles = new SCNVector3 ((float)Math.PI / 2f, borderNode.EulerAngles.Y, borderNode.EulerAngles.Z);
                borderNode.Hidden = true;

                AddChildNode (borderNode);
        }

        public GameBoard (NSCoder coder) => throw new NotImplementedException ("It has not been implemented");

        /// <summary>
        /// The color of the border
        /// </summary>
        public static UIColor BorderColor { get; } = UIColor.White;

        /// <summary>
        /// The BoardAnchor in the scene
        /// </summary>
        public BoardAnchor? Anchor { get; set; }

        /// <summary>
        /// Indicates whether the border is currently hidden
        /// </summary>
        public bool IsBorderHidden => borderNode.Hidden || borderNode.GetAction ("hide") is not null;

        /// <summary>
        /// The aspect ratio of the level.
        /// </summary>
        public float AspectRatio => (float)(preferredSize.Height / preferredSize.Width);

        /// <summary>
        /// The level's preferred size.
        /// This is used both to set the aspect ratio and to determine
        /// the default size.
        /// </summary>
        public CGSize PreferredSize
        {
                get
                {
                        return preferredSize;
                }

                set
                {
                        preferredSize = value;
                        UpdateBorderAspectRatio ();
                }
        }

        #region Appearance

        /// <summary>
        /// Hides the border.
        /// </summary>
        public void HideBorder (double duration = 0.5d)
        {
                if (borderNode.GetAction ("hide") is null)
                {
                        borderNode.RemoveAction ("unhide");
                        borderNode.RunAction (SCNAction.FadeOut (duration), "hide", () =>
                        {
                                borderNode.Hidden = true;
                        });
                }
        }

        /// <summary>
        /// Unhides the border.
        /// </summary>
        public void UnhideBorder ()
        {
                if (borderNode.GetAction ("unhide") is null)
                {
                        borderNode.RemoveAction ("hide");
                        borderNode.RunAction (SCNAction.FadeIn (0.5), "unhide");
                        borderNode.Hidden = false;
                }
        }

        /// <summary>
        /// Updates the game board with the latest hit test result and camera.
        /// </summary>
        public void Update (ARHitTestResult hitTestResult, ARCamera camera)
        {
                if (IsBorderHidden)
                {
                        UnhideBorder ();
                }

                if (hitTestResult?.Anchor is not null && hitTestResult?.Anchor is ARPlaneAnchor planeAnchor)
                {
                        PerformCloseAnimation (!anchorsOfVisitedPlanes.Contains (planeAnchor));
                        anchorsOfVisitedPlanes.Add (planeAnchor);
                }
                else
                {
                        PerformOpenAnimation ();
                }

                UpdateTransform (hitTestResult!, camera);
        }

        public void Reset ()
        {
                borderNode.RemoveAllActions ();
                borderNode.Hidden = true;
                recentPositions.Clear ();
                recentRotationAngles.Clear ();
                Hidden = false;
        }

        /// <summary>
        /// Incrementally scales the board by the given amount
        /// </summary>
        public void UpdateScale (float factor)
        {
                // assumes we always scale the same in all 3 dimensions
                var currentScale = Scale.X;
                var newScale = DigitExtensions.Clamp (currentScale * factor, GameBoard.MinimumScale, GameBoard.MaximumScale);
                Scale = new SCNVector3 (newScale, newScale, newScale);
        }

        public void UseDefaultScale ()
        {
                var scale = (float)preferredSize.Width;
                Scale = new SCNVector3 (scale, scale, scale);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Update the transform of the game board with the latest hit test result and camera
        /// </summary>
        void UpdateTransform (ARHitTestResult hitTestResult, ARCamera camera)
        {
                var position = hitTestResult.WorldTransform.GetTranslation ();

                // Average using several most recent positions.
                recentPositions.Add (position);
                recentPositions = new List<SCNVector3> (recentPositions.TakeLast (10));

                // Move to average of recent positions to avoid jitter.
                var average = recentPositions.Reduce (new SCNVector3 (0f, 0f, 0f)) / (float)recentPositions.Count;
                Position = average;

                // Orient bounds to plane if possible
                if (hitTestResult.Anchor is ARPlaneAnchor planeAnchor)
                {
                        OrientToPlane (planeAnchor, camera);
                        ScaleToPlane (planeAnchor);
                } else {
                        // Fall back to camera orientation
                        OrientToCamera (camera);
                        Scale = new SCNVector3 (GameBoard.MinimumScale, GameBoard.MinimumScale, GameBoard.MinimumScale);
                }

                // Remove any animation duration if present
                SCNTransaction.AnimationDuration = 0;
        }

        void OrientToCamera (ARCamera camera)
        {
                Rotate (camera.EulerAngles.Y);
        }

        void OrientToPlane (ARPlaneAnchor planeAnchor, ARCamera camera)
        {
                // Get board rotation about y
                Orientation = SimdExtensions.CreateQuaternion (planeAnchor.Transform.ToSCNMatrix4 ());
                var boardAngle = EulerAngles.Y;

                // If plane is longer than deep, rotate 90 degrees
                if (planeAnchor.Extent.X > planeAnchor.Extent.Z)
                {
                        boardAngle += (float)Math.PI / 2f;
                }

                // Normalize angle to closest 180 degrees to camera angle
                boardAngle = boardAngle.NormalizedAngle (camera.EulerAngles.Y, (float)Math.PI);

                Rotate (boardAngle);
        }

        void Rotate (float angle)
        {
                // Avoid interpolating between angle flips of 180 degrees
                var previouAngle = recentRotationAngles.Sum () / (float)recentRotationAngles.Count;
                if (Math.Abs (angle - previouAngle) > Math.PI / 2)
                {
                        recentRotationAngles = recentRotationAngles.Select (value => value.NormalizedAngle (angle, (float)Math.PI)).ToList ();
                }

                // Average using several most recent rotation angles.
                recentRotationAngles.Add (angle);
                recentRotationAngles = new List<float> (recentRotationAngles.TakeLast (20));

                // Move to average of recent positions to avoid jitter.
                var averageAngle = recentRotationAngles.Sum () / (float)recentRotationAngles.Count;
                Rotation = new SCNVector4 (0f, 1f, 0f, averageAngle);
        }

        void ScaleToPlane (ARPlaneAnchor planeAnchor)
        {
                // Determine if extent should be flipped (plane is 90 degrees rotated)
                var planeXAxis = planeAnchor.Transform.Column0.Xyz;
                var axisFlipped = Math.Abs (SCNVector3.Dot (planeXAxis, WorldRight)) < 0.5f;

                // Flip dimensions if necessary
                var planeExtent = planeAnchor.Extent;
                if (axisFlipped)
                {
                        planeExtent = new OpenTK.NVector3 (planeExtent.Z, 0f, planeExtent.X);
                }

                // Scale board to the max extent that fits in the plane
                var width = Math.Min (planeExtent.X, GameBoard.MaximumScale);
                var depth = Math.Min (planeExtent.Z, width * AspectRatio);
                width = depth / AspectRatio;
                Scale = new SCNVector3 (width, width, width);

                // Adjust position of board within plane's bounds
                var planeLocalExtent = new SCNVector3 (width, 0f, depth);
                if (axisFlipped)
                {
                        planeLocalExtent = new SCNVector3 (planeLocalExtent.Z, 0f, planeLocalExtent.X);
                }

                AdjustPosition (planeAnchor, planeLocalExtent);
        }

        void AdjustPosition (ARPlaneAnchor planeAnchor, SCNVector3 extent)
        {
                var positionAdjusted = false;
                var planeAnchorTransform = planeAnchor.Transform.ToSCNMatrix4 ();
                var worldToPlane = SCNMatrix4.Invert (planeAnchorTransform);

                // Get current position in the local plane coordinate space
                var planeLocalPosition = worldToPlane.Multiply (Transform.Column3);

                // Compute bounds min and max
                var boardMin = planeLocalPosition.Xyz - extent / 2f;
                var boardMax = planeLocalPosition.Xyz + extent / 2f;

                var planeAnchorCenter = planeAnchor.Center.ToSCNVector3 ();
                var planeAnchorExtent = planeAnchor.Extent.ToSCNVector3 ();
                var planeMin = planeAnchorCenter - planeAnchorExtent / 2f;
                var planeMax = planeAnchorCenter + planeAnchorExtent / 2f;

                // Adjust position for x within plane bounds
                if (boardMin.X < planeMin.X)
                {
                        planeLocalPosition.X += planeMin.X - boardMin.X;
                        positionAdjusted = true;
                } else if (boardMax.X > planeMax.X) {
                        planeLocalPosition.X -= boardMax.X - planeMax.X;
                        positionAdjusted = true;
                }

                // Adjust position for z within plane bounds
                if (boardMin.Z < planeMin.Z)
                {
                        planeLocalPosition.Z += planeMin.Z - boardMin.Z;
                        positionAdjusted = true;
                } else if (boardMax.Z > planeMax.Z) {
                        planeLocalPosition.Z -= boardMax.Z - planeMax.Z;
                        positionAdjusted = true;
                }

                if (positionAdjusted)
                {
                        Position = (planeAnchorTransform.Multiply (planeLocalPosition)).Xyz;
                }
        }

        void UpdateBorderAspectRatio ()
        {
                var borderSize = new CGSize (1f, AspectRatio);
                foreach (var segment in borderSegments)
                {
                        segment.BorderSize = borderSize;
                }

                if (FillPlane.Geometry is SCNPlane plane)
                {
                        var length = 1 - 2 * BorderSegment.Thickness;
                        plane.Height = length * AspectRatio;

                        var textureScale = SimdExtensions.CreateFromScale (new SCNVector3 (40f, 40f * AspectRatio, 1f));
                        if (plane.FirstMaterial is not null)
                        {
                                plane.FirstMaterial.Diffuse.ContentsTransform = textureScale;
                                plane.FirstMaterial.Emission.ContentsTransform = textureScale;
                        }
                }

                isBorderOpen = false;
        }

        #endregion

        #region Animations

        void PerformOpenAnimation ()
        {
                if (!isBorderOpen && !isAnimating)
                {
                        isBorderOpen = true;
                        isAnimating = true;

                        // Open animation
                        SCNTransaction.Begin ();
                        SCNTransaction.AnimationDuration = GameBoard.AnimationDuration / 4f;
                        SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);

                        borderNode.Opacity = 1f;
                        foreach (var segment in borderSegments)
                        {
                                segment.Open ();
                        }

                        Scale = new SCNVector3 (GameBoard.MinimumScale, GameBoard.MinimumScale, GameBoard.MinimumScale);

                        // completion is run on main-thread
                        SCNTransaction.SetCompletionBlock (() =>
                        {
                                SCNTransaction.Begin ();
                                borderNode.RunAction (PulseAction (), "pulse");
                                isAnimating = false;
                                SCNTransaction.Commit ();
                        });


                        SCNTransaction.Commit ();
                }
        }

        void PerformCloseAnimation (bool flash = false)
        {
                if (isBorderOpen && !isAnimating)
                {
                        isBorderOpen = false;
                        isAnimating = true;

                        borderNode.RemoveAction ("pulse");
                        borderNode.Opacity = 1f;

                        // Close animation
                        SCNTransaction.Begin ();
                        SCNTransaction.AnimationDuration = GameBoard.AnimationDuration / 2f;
                        SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);

                        borderNode.Opacity = 0.99f;
                        SCNTransaction.SetCompletionBlock (() =>
                        {
                                SCNTransaction.Begin ();
                                SCNTransaction.AnimationDuration = GameBoard.AnimationDuration / 2f;
                                SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
                                
                                foreach (var segment in borderSegments)
                                {
                                        segment.Close ();
                                }
                                SCNTransaction.SetCompletionBlock (() =>
                                {
                                        isAnimating = false;
                                });

                                SCNTransaction.Commit ();
                        });

                        SCNTransaction.Commit ();

                        if (flash)
                        {
                                var waitAction = SCNAction.Wait (GameBoard.AnimationDuration * 0.75f);
                                var fadeInAction = SCNAction.FadeOpacityTo (0.6f, GameBoard.AnimationDuration * 0.125f);
                                var fadeOutAction = SCNAction.FadeOpacityTo (0f, GameBoard.AnimationDuration * 0.125f);
                                FillPlane.RunAction (SCNAction.Sequence (new SCNAction [] { waitAction, fadeOutAction, fadeInAction }));
                        }
                }
        }

        #endregion

        #region Convenience Methods

        protected SCNNode FillPlane => GetFillPlane ();

        SCNNode GetFillPlane ()
        {
                var length = 1f - 2f * BorderSegment.Thickness;
                var plane = SCNPlane.Create (length, length * AspectRatio);
                var node = SCNNode.FromGeometry (plane);
                node.Name = "fillPlane";
                node.Opacity = 0.6f;

                var material = plane.FirstMaterial;
                if (material is not null)
                {
                        material.Diffuse.Contents = UIImage.FromBundle ("art.scnassets/textures/grid.png");

                        var textureScale = SimdExtensions.CreateFromScale (new SCNVector3 (40f, 40f * AspectRatio, 1f));
                        material.Diffuse.ContentsTransform = textureScale;
                        material.Emission.Contents = UIImage.FromBundle ("art.scnassets/textures/grid.png");
                        material.Emission.ContentsTransform = textureScale;
                        material.Diffuse.WrapS = SCNWrapMode.Repeat;
                        material.Diffuse.WrapT = SCNWrapMode.Repeat;
                        material.DoubleSided = true;
                        material.Ambient.Contents = UIColor.Black;
                        material.LightingModelName = SCNLightingModel.Constant;
                }

                return node;
        }

        #endregion

        #region Animations and Actions

        SCNAction PulseAction ()
        {
                var pulseOutAction = SCNAction.FadeOpacityTo (0.4f, 0.5f);
                var pulseInAction = SCNAction.FadeOpacityTo (1f, 0.5f);
                pulseOutAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;
                pulseInAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;

                return SCNAction.RepeatActionForever (SCNAction.Sequence (new SCNAction [] { pulseOutAction, pulseInAction }));
        }

        #endregion
}
