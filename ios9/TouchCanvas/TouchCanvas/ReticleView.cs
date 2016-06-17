using System;

using UIKit;
using CoreAnimation;
using CoreGraphics;

namespace TouchCanvas {
	public class ReticleView : UIView {
		const float dotRadius = 8f;
		const float lineWidth = 2f;
		const float radius = 80f;

		static readonly UIColor reticleColor = new UIColor (.516f, .38f, .85f, .4f);
		static readonly UIColor predictedIndicatorColor = new UIColor (.53f, .86f, .91f, 1f);

		CALayer reticleLayer = new CALayer ();
		UIImage reticleImage;

		CALayer dotLayer = new CALayer ();
		CALayer lineLayer = new CALayer ();
		UIColor indicatorColor = UIColor.Red;

		public override CGSize IntrinsicContentSize {
			get {
				return reticleImage.Size;
			}
		}

		readonly CALayer predictedDotLayer = new CALayer ();
		public CALayer PredictedDotLayer {
			get {
				return predictedDotLayer;
			}
		}

		readonly CALayer predictedLineLayer = new CALayer ();
		public CALayer PredictedLineLayer {
			get {
				return predictedLineLayer;
			}
		}

		nfloat actualAzimuthAngle;
		public nfloat ActualAzimuthAngle {
			get {
				return actualAzimuthAngle;
			}
			set {
				actualAzimuthAngle = value;
				SetNeedsLayout ();
			}
		}

		CGVector actualAzimuthUnitVector;
		public CGVector ActualAzimuthUnitVector {
			get {
				return actualAzimuthUnitVector;
			}
			set {
				actualAzimuthUnitVector = value;
				SetNeedsLayout ();
			}
		}

		nfloat actualAltitudeAngle;
		public nfloat ActualAltitudeAngle {
			get {
				return actualAltitudeAngle;
			}
			set {
				actualAltitudeAngle = value;
				SetNeedsLayout ();
			}
		}

		nfloat predictedAzimuthAngle;
		public nfloat PredictedAzimuthAngle {
			get {
				return predictedAzimuthAngle;
			}
			set {
				predictedAzimuthAngle = value;
				SetNeedsLayout ();
			}
		}

		CGVector predictedAzimuthUnitVector;
		public CGVector PredictedAzimuthUnitVector {
			get {
				return predictedAzimuthUnitVector;
			}
			set {
				predictedAzimuthUnitVector = value;
				SetNeedsLayout ();
			}
		}

		nfloat predictedAltitudeAngle;
		public nfloat PredictedAltitudeAngle {
			get {
				return predictedAltitudeAngle;
			}
			set {
				predictedAltitudeAngle = value;
				SetNeedsLayout ();
			}
		}

		public ReticleView (CGRect frame) : base (frame)
		{
			ContentScaleFactor = UIScreen.MainScreen.Scale;

			reticleLayer.ContentsGravity = CALayer.GravityCenter;
			reticleLayer.Position = Layer.Position;
			Layer.AddSublayer (reticleLayer);

			ConfigureDotLayer (predictedDotLayer, predictedIndicatorColor);
			predictedDotLayer.Hidden = true;

			ConfigureLineLayer (predictedLineLayer, predictedIndicatorColor);
			predictedLineLayer.Hidden = true;

			ConfigureDotLayer (dotLayer, indicatorColor);
			ConfigureLineLayer (lineLayer, indicatorColor);

			reticleLayer.AddSublayer (predictedDotLayer);
			reticleLayer.AddSublayer (predictedLineLayer);
			reticleLayer.AddSublayer (dotLayer);
			reticleLayer.AddSublayer (lineLayer);

			RenderReticleImage ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			CATransaction.DisableActions = true;
			reticleLayer.Position = new CGPoint (Bounds.Width / 2, Bounds.Height / 2);
			LayoutIndicator ();

			CATransaction.DisableActions = false;
		}

		void RenderReticleImage ()
		{
			var imageRadius = NMath.Ceiling (radius * 1.2f);
			var imageSize = new CGSize (imageRadius * 2f, imageRadius * 2f);
			UIGraphics.BeginImageContextWithOptions (imageSize, false, ContentScaleFactor);

			var ctx = UIGraphics.GetCurrentContext ();
			ctx.TranslateCTM (imageRadius, imageRadius);
			ctx.SetLineWidth (2f);

			ctx.SetStrokeColor (reticleColor.CGColor);
			ctx.AddEllipseInRect (new CGRect (-radius, -radius, radius * 2, radius * 2));

			// Draw targeting lines.
			var path = new CGPath ();
			var transform = CGAffineTransform.MakeIdentity ();

			for (int i = 0; i < 4; i++) {
				path.MoveToPoint (transform, radius * .5f, 0f);
				path.AddLineToPoint (transform, radius * 1.15f, 0);
				transform.Rotate (NMath.PI / 2f);
			}

			ctx.AddPath (path);
			ctx.StrokePath ();

			reticleImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			reticleLayer.Contents = reticleImage.CGImage;
			reticleLayer.Bounds = new CGRect (0f, 0f, imageRadius * 2f, imageRadius * 2f);
			reticleLayer.ContentsScale = ContentScaleFactor;
		}

		void LayoutIndicator ()
		{
			// Predicted
			LayoutIndicatorForAzimuthAngle (predictedAzimuthAngle, predictedAzimuthUnitVector, predictedAltitudeAngle, predictedLineLayer, predictedDotLayer);

			// Actual
			LayoutIndicatorForAzimuthAngle (actualAzimuthAngle, actualAzimuthUnitVector, actualAltitudeAngle, lineLayer, dotLayer);
		}

		void LayoutIndicatorForAzimuthAngle (nfloat azimuthAngle, CGVector azimuthUnitVector, nfloat altitudeAngle, CALayer targetLineLayer, CALayer targetDotLayer)
		{
			var reticleBounds = reticleLayer.Bounds;
			var centeringTransform = CGAffineTransform.MakeTranslation (reticleBounds.Width / 2f, reticleBounds.Height / 2f);

			var rotationTransform = CGAffineTransform.MakeRotation (azimuthAngle);

			// Draw the indicator opposite the azimuth by rotating pi radians, for easy visualization.
			rotationTransform = CGAffineTransform.Rotate (rotationTransform, NMath.PI);
			var altitudeRadius = (1f - altitudeAngle / NMath.PI / 2f) * radius;
			var lineTransform = CGAffineTransform.MakeScale (altitudeRadius, 1);

			lineTransform = CGAffineTransform.Multiply (lineTransform, rotationTransform);
			lineTransform = CGAffineTransform.Multiply (lineTransform, centeringTransform);
			targetLineLayer.AffineTransform = lineTransform;

			var dotTransform = CGAffineTransform.MakeTranslation (-azimuthUnitVector.dx * altitudeRadius, -azimuthUnitVector.dy * altitudeRadius);
			dotTransform = CGAffineTransform.Multiply (dotTransform, centeringTransform);
			targetDotLayer.AffineTransform = dotTransform;
		}

		static void ConfigureDotLayer (CALayer targetLayer, UIColor color)
		{
			targetLayer.BackgroundColor = color.CGColor;
			targetLayer.Bounds = new CGRect (0f, 0f, dotRadius * 2f, dotRadius * 2f);
			targetLayer.CornerRadius = dotRadius;
			targetLayer.Position = CGPoint.Empty;
		}

		static void ConfigureLineLayer (CALayer targetLayer, UIColor color)
		{
			targetLayer.BackgroundColor = color.CGColor;
			targetLayer.Bounds = new CGRect (0f, 0f, 1f, lineWidth);
			targetLayer.AnchorPoint = new CGPoint (0f, .5f);
			targetLayer.Position = CGPoint.Empty;
		}
	}
}