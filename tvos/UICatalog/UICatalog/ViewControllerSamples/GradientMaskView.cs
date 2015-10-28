using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace UICatalog {
	public class GradientMaskView : UIView {

		CGPoint maskPosition;
		public CGPoint MaskPosition {
			get {
				return maskPosition;
			}
			set {
				maskPosition = value;
				UpdateGradientLayer ();
			}
		}

		CAGradientLayer gradientLayer;
		CAGradientLayer GradientLayer {
			get {
				if (gradientLayer == null)
					gradientLayer = new CAGradientLayer {
						Colors = new [] {
							UIColor.FromWhiteAlpha (0f, 0f).CGColor,
							UIColor.FromWhiteAlpha (0f, 1f).CGColor
						}
					};

				return gradientLayer;
			}
		}

		public GradientMaskView (CGRect frame) : base (frame)
		{
			Initialize ();
		}

		void Initialize ()
		{
			MaskPosition = CGPoint.Empty;
			Layer.AddSublayer (GradientLayer);
			UpdateGradientLayer ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			UpdateGradientLayer ();
		}

		void UpdateGradientLayer ()
		{
			GradientLayer.Frame = new CGRect (CGPoint.Empty, Bounds.Size);
			GradientLayer.Locations = new [] {
				NSNumber.FromNFloat (MaskPosition.Y / Bounds.Height),
				NSNumber.FromNFloat (MaskPosition.X / Bounds.Height)
			};
		}
	}
}

