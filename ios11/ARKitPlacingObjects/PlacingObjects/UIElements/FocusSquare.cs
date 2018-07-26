using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using SceneKit;
using ARKit;
using System.Linq;

namespace PlacingObjects
{
	public class FocusSquare : SCNNode
	{
		/////////////////////////////////////////////////
		// Variables to configure the focus square

		// Original size of the focus square in m.
		private float FocusSquareSize = 0.17f;

		// Thickness of the focus square lines in m.
		private float FocusSquareThickness = 0.018f;

		// Scale factor for the focus square when it is closed, w.r.t. the original size.
		private float ScaleForClosedSquare = 0.97f;

		// Duration of the open/close animation
		private float AnimationDuration = 0.7f;

		// Color of the focus square
		public static readonly UIColor PrimaryColor = UIColor.FromRGB(255, 211, 0);
		private UIColor FocusSquareColorLight = UIColor.FromRGB(255, 273, 124);

		// For scale adapdation based on the camera distance, see the `ScaleBasedOnDistance(camera:)` method.

		private bool IsOpen = false;
		private bool IsAnimating = false;
		private SCNVector3[] RecentFocusSquarePositions = new SCNVector3[8];
		private int recentFocusSquarePositionIndex = 0;

		private ISet<ARAnchor> AnchorsOfVisitedPlanes = new HashSet<ARAnchor>();

		private SCNNode fillPlane = null;
		private SCNNode FillPlane
		{
			get {
				if (fillPlane == null)
				{
					var c = FocusSquareThickness / 2; //Correction to align lines perfectly
					var plane = new SCNPlane
					{
						Width = (nfloat)(1.0 - FocusSquareThickness * 2 + c),
						Height = (nfloat)(1.0 - FocusSquareThickness * 2 + c)
					};
					fillPlane = new SCNNode { Geometry = plane };
					fillPlane.Name = "fillPlane";
					fillPlane.Opacity = 0.0f;

					var material = plane.FirstMaterial;
					material.Diffuse.ContentColor = FocusSquareColorLight;
					material.DoubleSided = true;
					material.Ambient.ContentColor = UIColor.Black;
					material.LightingModelName = SCNLightingModel.Constant;
					material.Emission.ContentColor = FocusSquareColorLight;
				}
				return fillPlane;
			}
		}

		private FocusSquareSegment[] segments = new FocusSquareSegment[8];
		private FocusSquareSegment[] Segments
		{
			get
			{
				return segments;
			}
		}


		public SCNVector3 LastPositionOnPlane { get; set; }
		public SCNVector3 LastPosition { get; set; }

		public FocusSquare() : base()
		{
			Name = "Focus Square";
			// Initialize
			Opacity = 0.0f;
			AddChildNode(FocusSquareNode());
			Open();
		}

		public FocusSquare(NSCoder coder) : base(coder)
		{
			// Initialize
			Name = "Focus Square";
		}

		private void UpdateTransform(SCNVector3 position, ARCamera camera)
		{
			// Add to list of recently visited positions
			RecentFocusSquarePositions[recentFocusSquarePositionIndex] = position;
			// Increment current position, rolling over to beginning
			recentFocusSquarePositionIndex = ++recentFocusSquarePositionIndex % RecentFocusSquarePositions.Length;
			// Note that we don't really care about "current position" as we just average all the positions 

			// Move to average of recent positions to avoid jitter
			var average = Utilities.AverageVector3List(RecentFocusSquarePositions);
			Position = average;
			this.SetUniformScale(ScaleBasedOnDistance(camera));

			// Correct y rotation of camera square
			if (camera !=null) {
				var tilt = (float)Math.Abs(camera.EulerAngles.X);
				var threshold1 = (float)(Math.PI / 2f * 0.65f);
				var threshold2 = (float)(Math.PI / 2f * 0.75f);
				var yaw = (float)Math.Atan2(camera.Transform.M11, camera.Transform.M12);
				var angle = 0f;

				if (tilt >=0 && tilt<threshold1) {
					angle = camera.EulerAngles.Y;
				} else if (threshold1 < threshold2) {
					var relativeInRange = Math.Abs((tilt - threshold1) / (threshold2 / threshold1));
					var normalizedY = Normalize(camera.EulerAngles.Y, yaw);
					angle = normalizedY * (1 - relativeInRange) + yaw * relativeInRange;
				} else {
					angle = yaw;
				}
				Rotation = new SCNVector4(0, 1, 0, angle);
			}
		}

		private float Normalize(float angle, float forMinimalRotationTo)
		{
			// Normalize angle in steps of 90 degrees such that the rotation to the other angle is minimal
			var normalized = angle;

			while (Math.Abs(normalized - forMinimalRotationTo) > (float)Math.PI / 4)
			{
				if (angle > forMinimalRotationTo)
				{
					normalized -= (float)Math.PI / 2;
				}
				else
				{
					normalized += (float)Math.PI / 2;
				}
			}

			return normalized;
		}

		/// Reduce visual size change with distance by scaling up when close and down when far away.
		///
		/// These adjustments result in a scale of 1.0x for a distance of 0.7 m or less
		/// (estimated distance when looking at a table), and a scale of 1.2x
		/// for a distance 1.5 m distance (estimated distance when looking at the floor).
		private float ScaleBasedOnDistance(ARCamera camera)
		{

			if (camera == null)
			{
				return 1.0f;
			}

			var distanceFromCamera = Position.Subtract(SCNVector3Extensions.PositionFromTransform(camera.Transform)).LengthFast;

			// This function reduces size changes of the focus square based on the distance by scaling it up if it far away,
			// and down if it is very close.
			// The values are adjusted such that scale will be 1 in 0.7 m distance (estimated distance when looking at a table),
			// and 1.2 in 1.5 m distance (estimated distance when looking at the floor).
			var newScale = (distanceFromCamera < 0.7f) ? (distanceFromCamera / 0.7f) : (0.25f * distanceFromCamera + 0.825f);
			return newScale;
		}

		private SCNAction PulseAction()
		{

			var pulseOutAction = SCNAction.FadeOpacityTo(0.4f, 0.5f);
			var pulseInAction = SCNAction.FadeOpacityTo(1.0f, 0.5f);
			pulseOutAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;
			pulseInAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;

			return SCNAction.RepeatActionForever(SCNAction.Sequence(new SCNAction[] { pulseOutAction, pulseInAction }));
		}

		private void StopPulsing(SCNNode node)
		{
			if (node == null)
			{
				return;
			}

			node.RemoveAction("pulse");
			node.Opacity = 1.0f;
		}

		private SCNAction FlashAnimation(double duration) {

			var action = SCNAction.CustomAction(duration, (node, elapsedTime) => {
				// animate color from HSB 48/100/100 to 48/30/100 and back
				var elapsedTimePercentage = elapsedTime / (float)duration;
				var saturation = 2.8f * (elapsedTimePercentage - 0.5f) * (elapsedTimePercentage - 0.5f) + 0.3f;
				var material = node.Geometry.FirstMaterial;
				if (material !=null) {
					material.Diffuse.Contents = UIColor.FromHSBA(0.1333f, saturation, 1.0f, 1.0f);
				}
			});

			return action;
		}

		private CAKeyFrameAnimation ScaleAnimation(string keyPath){

			var scaleAnimation = CAKeyFrameAnimation.FromKeyPath(keyPath);

			var easeOut = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
			var easeInOut = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			var liner = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);

			var fs = FocusSquareSize;
			var ts = FocusSquareSize * ScaleForClosedSquare;
			var values = new NSNumber[] { fs, fs * 1.15f, fs * 1.15f, ts * 0.97f, ts };
			var keyTimes = new NSNumber[] { 0.00f, 0.25f, 0.50f, 0.75f, 1.00f };
			var timingFunctions = new CAMediaTimingFunction[] { easeOut, liner, easeOut, easeInOut };

			scaleAnimation.Values = values;
			scaleAnimation.KeyTimes = keyTimes;
			scaleAnimation.TimingFunctions = timingFunctions;
			scaleAnimation.Duration = AnimationDuration;

			return scaleAnimation;
		}

		private void Open() {

			if (IsOpen || IsAnimating)
			{
				return;
			}

			// Open animation
			SCNTransaction.Begin();
			SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
			SCNTransaction.AnimationDuration = AnimationDuration / 4f;
			FocusSquareNode().Opacity = 1.0f;

			foreach(var segment in Segments)
			{
				segment.Open();
			}

			SCNTransaction.SetCompletionBlock( () => {
				FocusSquareNode().RunAction(PulseAction(), "pulse");
			});
			SCNTransaction.Commit();

			// Scale/bounce animation
			SCNTransaction.Begin();
			SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
			SCNTransaction.AnimationDuration = AnimationDuration / 4f;
			FocusSquareNode().SetUniformScale(FocusSquareSize);
			SCNTransaction.Commit();

			IsOpen = true;
		}

		private void Close(bool flash = false) {

			if (! IsOpen || IsAnimating)
			{
				return;
			}

			IsAnimating = true;
			StopPulsing(FocusSquareNode());

			// Close animation
			SCNTransaction.Begin();
			SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
			SCNTransaction.AnimationDuration = AnimationDuration / 2f;
			FocusSquareNode().Opacity = 0.99f;
			SCNTransaction.SetCompletionBlock(() => {
				SCNTransaction.Begin();
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
				SCNTransaction.AnimationDuration = AnimationDuration / 4f;

				foreach(var segment in segments)
				{
					segment.Close();
				}

				SCNTransaction.SetCompletionBlock(() => {
					IsAnimating = false;
				});
				SCNTransaction.Commit();
			});
			SCNTransaction.Commit();

			// Scale/bounce animation
			FocusSquareNode().AddAnimation(ScaleAnimation("transform.scale.x"), "transform.scale.y");
			FocusSquareNode().AddAnimation(ScaleAnimation("transform.scale.y"), "transform.scale.y");
			FocusSquareNode().AddAnimation(ScaleAnimation("transform.scale.z"), "transform.scale.z");

			// Flash?
			if (flash) {
				var waitAction = SCNAction.Wait(AnimationDuration * 0.75f);
				var fadeInAction = SCNAction.FadeOpacityTo(0.25f, AnimationDuration * 0.125f);
				var fadeOutAction = SCNAction.FadeOpacityTo(0.0f, AnimationDuration * 0.125f);
				FillPlane.RunAction(SCNAction.Sequence(new SCNAction[] {waitAction, fadeInAction, fadeOutAction}));

				var flashSquareAction = FlashAnimation(AnimationDuration * 0.25f);
				foreach(var segment in Segments)
				{
					segment.RunAction(SCNAction.Sequence(new[] { waitAction, flashSquareAction }));
				}
			}

			IsOpen = false;
		}

		private SCNNode focusSquareNode = null;
		private SCNNode FocusSquareNode() {
			if (focusSquareNode == null)
			{
				/*
				The focus square consists of eight Segments as follows, which can be individually animated.

					s1  s2
					_   _
				s3 |     | s4

				s5 |     | s6
					-   -
					s7  s8
				*/

				var sl = 0.5f; // segment length
				var c = FocusSquareThickness / 2f; // correction to align lines perfectly

				var s1 = new FocusSquareSegment("s1", Corner.TopLeft, Alignment.Horizontal);
				var s2 = new FocusSquareSegment("s2", Corner.TopRight, Alignment.Horizontal);
				var s3 = new FocusSquareSegment("s3", Corner.TopLeft, Alignment.Vertical);
				var s4 = new FocusSquareSegment("s4", Corner.TopRight, Alignment.Vertical);
				var s5 = new FocusSquareSegment("s5", Corner.BottomLeft, Alignment.Vertical);
				var s6 = new FocusSquareSegment("s6", Corner.BottomRight, Alignment.Vertical);
				var s7 = new FocusSquareSegment("s7", Corner.BottomLeft, Alignment.Horizontal);
				var s8 = new FocusSquareSegment("s8", Corner.BottomRight, Alignment.Horizontal);

				s1.Position = s1.Position.Add(new SCNVector3(-(sl / 2 - c), -(sl - c), 0));
				s2.Position = s2.Position.Add(new SCNVector3(sl / 2 - c, -(sl - c), 0));
				s3.Position = s3.Position.Add(new SCNVector3(-sl, -sl / 2, 0));
				s4.Position = s4.Position.Add(new SCNVector3(sl, -sl / 2, 0));
				s5.Position = s5.Position.Add(new SCNVector3(-sl, sl / 2, 0));
				s6.Position = s6.Position.Add(new SCNVector3(sl, sl / 2, 0));
				s7.Position = s7.Position.Add(new SCNVector3(-(sl / 2 - c), sl - c, 0));
				s8.Position = s8.Position.Add(new SCNVector3(sl / 2 - c, sl - c, 0));

				var planeNode = new SCNNode();
				planeNode.EulerAngles = new SCNVector3((float) (Math.PI / 2), planeNode.EulerAngles.Y, planeNode.EulerAngles.Z);
				planeNode.SetUniformScale(FocusSquareSize * ScaleForClosedSquare);

				planeNode.AddChildNode(s1);
				planeNode.AddChildNode(s2);
				planeNode.AddChildNode(s3);
				planeNode.AddChildNode(s4);
				planeNode.AddChildNode(s5);
				planeNode.AddChildNode(s6);
				planeNode.AddChildNode(s7);
				planeNode.AddChildNode(s8);
				planeNode.AddChildNode(FillPlane);
				segments = new FocusSquareSegment[] { s1, s2, s3, s4, s5, s6, s7, s8 };

				IsOpen = false;

				// Always render focus square on top
				planeNode.RenderOnTop();
				focusSquareNode = planeNode;
			}
			return focusSquareNode;
		}

		public void Update(SCNVector3 position, ARPlaneAnchor planeAnchor, ARCamera camera)
		{
			LastPosition = position;

			var anchor = planeAnchor;
			if (anchor != null)
			{
				Close(!AnchorsOfVisitedPlanes.Contains(anchor));
				LastPositionOnPlane = position;
				AnchorsOfVisitedPlanes.Add(anchor);
			} else {
				Open();
			}
			UpdateTransform(position, camera);
		}

		public void Hide() {
			if (Math.Abs(Opacity - 1.0) < Double.Epsilon) {
				RunAction(SCNAction.FadeOut(0.5f));
			}
		}

		public void Show() {
			if (Opacity == 0)
			{
				RunAction(SCNAction.FadeIn(0.5f));
			}
		}
	}
}
