using System;

using UIKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;

namespace AudioTapProcessor
{
	[Register ("VolumeUnitMeterView")]
	public class VolumeUnitMeterView : UIView
	{
		// This is an arbitrary calibration to define 0 db in the VU meter.
		const float UnitMeterCalibration = 12;

		static readonly float DegreeToRadian = (float)Math.PI / 180;

		CALayer NeedleLayer { get; set; }

		float v;
		public float Value {
			get {
				return v;
			}
			set {
				if (v == value)
					return;

				v = value;

				// Convert RMS amplitude into dB (using some arbitrary calibration values).
				var valueDB = (20.0f * Math.Log10 (value) + UnitMeterCalibration);
				NeedleLayer.AffineTransform = CGAffineTransform.MakeRotation (ConvertValueToNeedleAngle ((float)valueDB));
			}
		}

		public VolumeUnitMeterView (IntPtr handle)
			: base (handle)
		{
			SetupLayerTree ();
		}

		void SetupLayerTree ()
		{
			CGRect viewLayerBounds = Layer.Bounds;

			// Set layer background image.
			Layer.Contents = UIImage.FromBundle ("VUMeterBackground").CGImage;

			CALayer shadowLayer = CreateShadow ();
			Layer.AddSublayer (shadowLayer);

			NeedleLayer = CreateNeedle (viewLayerBounds);
			shadowLayer.AddSublayer (NeedleLayer);

			CALayer foregroundLayer = CreateForeground (viewLayerBounds);
			Layer.AddSublayer (foregroundLayer);
		}

		static CALayer CreateShadow ()
		{
			CALayer shadowLayer = CALayer.Create ();
			shadowLayer.ShadowColor = UIColor.Black.CGColor;
			shadowLayer.ShadowOffset = new CGSize (0, 6);
			shadowLayer.ShadowOpacity = 0.5f;
			shadowLayer.ShadowRadius = 2;

			return shadowLayer;
		}

		static CALayer CreateNeedle (CGRect bounds)
		{
			UIImage needleImage = UIImage.FromBundle ("VUMeterNeedle");
			CGSize needleImageSize = needleImage.Size;

			var needleLayer = CALayer.Create ();
			needleLayer.AffineTransform = CGAffineTransform.MakeRotation (ConvertValueToNeedleAngle (nfloat.NegativeInfinity));
			needleLayer.AnchorPoint = new CGPoint (0.5f, 1);
			needleLayer.Bounds = new CGRect (0, 0, needleImageSize.Width, needleImageSize.Height);
			needleLayer.Contents = needleImage.CGImage;
			needleLayer.Position = new CGPoint (0.5f * bounds.Width, needleImageSize.Height);

			return needleLayer;
		}

		static CALayer CreateForeground (CGRect bounds)
		{
			CALayer foregroundLayer = CALayer.Create ();
			foregroundLayer.AnchorPoint = CGPoint.Empty;
			foregroundLayer.Bounds = bounds;
			foregroundLayer.Contents = UIImage.FromBundle ("VUMeterForeground").CGImage;
			foregroundLayer.Position = CGPoint.Empty;

			return foregroundLayer;
		}

		static nfloat ConvertValueToNeedleAngle (nfloat value)
		{
			nfloat degree = (value * 5 + 20);

			// The mapping from dB amplitude to angle is not linear on our VU meter.
			if (value < -7f && value >= -10)
				degree = (-15 + (10f / 3) * (value + 7));
			else if (value < -10)
				degree = (-25 + (13f / 10) * (value + 10));

			// Limit to visible angle.
			degree = NMath.Max (-38, NMath.Min (degree, 43));

			return DegreeToRadian * degree;
		}
	}
}