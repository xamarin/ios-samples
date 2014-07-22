using System;
using UIKit;

using CoreGraphics;

namespace PaintCode
{
	/// <summary>
	/// Blue button example
	/// http://paintcodeapp.com/examples.html
	/// </summary>
	/// <remarks>
	/// This implementation only deals with Normal and Pressed states. 
	/// There is no handling for the Disabled state.
	/// </remarks>
	public class BlueButton : UIButton {
		
		bool isPressed;
		
		public UIColor NormalColor;
		
		/// <summary>
		/// Invoked when the user touches 
		/// </summary>
		public event Action<BlueButton> Tapped;
				
		/// <summary>
		/// Creates a new instance of the GlassButton using the specified dimensions
		/// </summary>
		public BlueButton (CGRect frame) : base (frame)
		{
			NormalColor = UIColor.FromRGBA (0.00f, 0.37f, 0.89f, 1.00f);
		}

		/// <summary>
		/// Whether the button is rendered enabled or not.
		/// </summary>
		public override bool Enabled { 
			get {
				return base.Enabled;
			}
			set {
				base.Enabled = value;
				SetNeedsDisplay ();
			}
		}
		
		public override bool BeginTracking (UITouch uitouch, UIEvent uievent)
		{
			SetNeedsDisplay ();
			isPressed = true;
			return base.BeginTracking (uitouch, uievent);
		}
		
		public override void EndTracking (UITouch uitouch, UIEvent uievent)
		{
			if (isPressed && Enabled){
				if (Tapped != null)
					Tapped (this);
			}
			isPressed = false;
			SetNeedsDisplay ();
			base.EndTracking (uitouch, uievent);
		}
		
		public override bool ContinueTracking (UITouch uitouch, UIEvent uievent)
		{
			var touch = uievent.AllTouches.AnyObject as UITouch;
			if (Bounds.Contains (touch.LocationInView (this)))
				isPressed = true;
			else
				isPressed = false;
			return base.ContinueTracking (uitouch, uievent);
		}
		
		public override void Draw (CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext ()) {
				var bounds = Bounds;
			
				//UIColor background = Enabled ? isPressed ? HighlightedColor : NormalColor : DisabledColor;
			
			
				UIColor buttonColor = NormalColor; //UIColor.FromRGBA (0.00f, 0.37f, 0.89f, 1.00f);
				var buttonColorRGBA = new nfloat[4];
				buttonColor.GetRGBA (
					out buttonColorRGBA [0],
					out buttonColorRGBA [1],
					out buttonColorRGBA [2],
					out buttonColorRGBA [3]
				);
				if (isPressed) {
					// Get the Hue Saturation Brightness Alpha copy of the color				
					var buttonColorHSBA = new nfloat[4];
					buttonColor.GetHSBA (
						out buttonColorHSBA [0],
						out buttonColorHSBA [1],
						out buttonColorHSBA [2],
						out buttonColorHSBA [3]
					);
					// Change the brightness to a fixed value (0.5f)
					buttonColor = UIColor.FromHSBA (buttonColorHSBA [0], buttonColorHSBA [1], 0.5f, buttonColorHSBA [3]);
					// Re-set the base buttonColorRGBA because everything else is relative to it
					buttonColorRGBA = new nfloat[4];
					buttonColor.GetRGBA (
						out buttonColorRGBA [0],
						out buttonColorRGBA [1],
						out buttonColorRGBA [2],
						out buttonColorRGBA [3]
					);
				}
			
				
				using (var colorSpace = CGColorSpace.CreateDeviceRGB ()) {
				
				
				
					// ------------- START PAINTCODE -------------

//// Color Declarations
					UIColor upColorOut = UIColor.FromRGBA (0.79f, 0.79f, 0.79f, 1.00f);
					UIColor bottomColorDown = UIColor.FromRGBA (0.21f, 0.21f, 0.21f, 1.00f);
					UIColor upColorInner = UIColor.FromRGBA (0.17f, 0.18f, 0.20f, 1.00f);
					UIColor bottomColorInner = UIColor.FromRGBA (0.98f, 0.98f, 0.99f, 1.00f);


					UIColor buttonFlareUpColor = UIColor.FromRGBA (
						                            (buttonColorRGBA [0] * 0.3f + 0.7f),
						                            (buttonColorRGBA [1] * 0.3f + 0.7f),
						                            (buttonColorRGBA [2] * 0.3f + 0.7f),
						                            (buttonColorRGBA [3] * 0.3f + 0.7f)
					                            );
					UIColor buttonTopColor = UIColor.FromRGBA (
						                        (buttonColorRGBA [0] * 0.8f),
						                        (buttonColorRGBA [1] * 0.8f),
						                        (buttonColorRGBA [2] * 0.8f),
						                        (buttonColorRGBA [3] * 0.8f + 0.2f)
					                        );
					UIColor buttonBottomColor = UIColor.FromRGBA (
						                           (buttonColorRGBA [0] * 0 + 1),
						                           (buttonColorRGBA [1] * 0 + 1),
						                           (buttonColorRGBA [2] * 0 + 1),
						                           (buttonColorRGBA [3] * 0 + 1)
					                           );
					UIColor buttonFlareBottomColor = UIColor.FromRGBA (
						                                (buttonColorRGBA [0] * 0.8f + 0.2f),
						                                (buttonColorRGBA [1] * 0.8f + 0.2f),
						                                (buttonColorRGBA [2] * 0.8f + 0.2f),
						                                (buttonColorRGBA [3] * 0.8f + 0.2f)
					                                );
					UIColor flareWhite = UIColor.FromRGBA (1.00f, 1.00f, 1.00f, 0.83f);

//// Gradient Declarations
					var ringGradientColors = new CGColor [] {
						upColorOut.CGColor,
						bottomColorDown.CGColor
					};
					var ringGradientLocations = new nfloat [] { 0, 1 };
					var ringGradient = new CGGradient (colorSpace, ringGradientColors, ringGradientLocations);
					var ringInnerGradientColors = new CGColor [] {
						upColorInner.CGColor,
						bottomColorInner.CGColor
					};
					var ringInnerGradientLocations = new nfloat [] { 0, 1 };
					var ringInnerGradient = new CGGradient (colorSpace, ringInnerGradientColors, ringInnerGradientLocations);
					var buttonGradientColors = new CGColor [] {
						buttonBottomColor.CGColor,
						buttonTopColor.CGColor
					};
					var buttonGradientLocations = new nfloat [] { 0, 1 };
					var buttonGradient = new CGGradient (colorSpace, buttonGradientColors, buttonGradientLocations);
					var overlayGradientColors = new CGColor [] {
						flareWhite.CGColor,
						UIColor.Clear.CGColor
					};
					var overlayGradientLocations = new nfloat [] { 0, 1 };
					var overlayGradient = new CGGradient (colorSpace, overlayGradientColors, overlayGradientLocations);
					var buttonFlareGradientColors = new CGColor [] {
						buttonFlareUpColor.CGColor,
						buttonFlareBottomColor.CGColor
					};
					var buttonFlareGradientLocations = new nfloat [] { 0, 1 };
					var buttonFlareGradient = new CGGradient (colorSpace, buttonFlareGradientColors, buttonFlareGradientLocations);

//// Shadow Declarations
					var buttonInnerShadow = UIColor.Black.CGColor;
					var buttonInnerShadowOffset = new CGSize (0, -0);
					var buttonInnerShadowBlurRadius = 5;
					var buttonOuterShadow = UIColor.Black.CGColor;
					var buttonOuterShadowOffset = new CGSize (0, 2);
				
				
					var buttonOuterShadowBlurRadius = isPressed ? 2 : 5;	// ADDED this code after PaintCode


//// outerOval Drawing
					var outerOvalPath = UIBezierPath.FromOval (new CGRect (5, 5, 63, 63));
					context.SaveState ();
					context.SetShadow (buttonOuterShadowOffset, buttonOuterShadowBlurRadius, buttonOuterShadow);
					context.BeginTransparencyLayer (null);
					outerOvalPath.AddClip ();
					context.DrawLinearGradient (ringGradient, new CGPoint (36.5f, 5), new CGPoint (36.5f, 68), 0);
					context.EndTransparencyLayer ();
					context.RestoreState ();



//// overlayOval Drawing
					var overlayOvalPath = UIBezierPath.FromOval (new CGRect (5, 5, 63, 63));
					context.SaveState ();
					overlayOvalPath.AddClip ();
					context.DrawRadialGradient (overlayGradient,
						new CGPoint (36.5f, 12.23f), 17.75f,
						new CGPoint (36.5f, 36.5f), 44.61f,
						CGGradientDrawingOptions.DrawsBeforeStartLocation | CGGradientDrawingOptions.DrawsAfterEndLocation);
					context.RestoreState ();



//// innerOval Drawing
					var innerOvalPath = UIBezierPath.FromOval (new CGRect (12, 12, 49, 49));
					context.SaveState ();
					innerOvalPath.AddClip ();
					context.DrawLinearGradient (ringInnerGradient, new CGPoint (36.5f, 12), new CGPoint (36.5f, 61), 0);
					context.RestoreState ();



//// buttonOval Drawing
					var buttonOvalPath = UIBezierPath.FromOval (new CGRect (14, 13, 46, 46));
					context.SaveState ();
					buttonOvalPath.AddClip ();
					context.DrawRadialGradient (buttonGradient,
						new CGPoint (37, 63.23f), 2.44f,
						new CGPoint (37, 44.48f), 23.14f,
						CGGradientDrawingOptions.DrawsBeforeStartLocation | CGGradientDrawingOptions.DrawsAfterEndLocation);
					context.RestoreState ();

////// buttonOval Inner Shadow
					var buttonOvalBorderRect = buttonOvalPath.Bounds;
					buttonOvalBorderRect.Inflate (buttonInnerShadowBlurRadius, buttonInnerShadowBlurRadius);
					buttonOvalBorderRect.Offset (-buttonInnerShadowOffset.Width, -buttonInnerShadowOffset.Height);
					buttonOvalBorderRect = CGRect.Union (buttonOvalBorderRect, buttonOvalPath.Bounds);
					buttonOvalBorderRect.Inflate (1, 1);

					var buttonOvalNegativePath = UIBezierPath.FromRect (buttonOvalBorderRect);
					buttonOvalNegativePath.AppendPath (buttonOvalPath);
					buttonOvalNegativePath.UsesEvenOddFillRule = true;

					context.SaveState ();
					{
						var xOffset = buttonInnerShadowOffset.Width + (float)Math.Round (buttonOvalBorderRect.Width);
						var yOffset = buttonInnerShadowOffset.Height;
						context.SetShadow (
							new CGSize (xOffset + (xOffset >= 0 ? 0.1f : -0.1f), yOffset + (yOffset >= 0 ? 0.1f : -0.1f)),
							buttonInnerShadowBlurRadius,
							buttonInnerShadow);

						buttonOvalPath.AddClip ();
						var transform = CGAffineTransform.MakeTranslation (-(float)Math.Round (buttonOvalBorderRect.Width), 0);
						buttonOvalNegativePath.ApplyTransform (transform);
						UIColor.Gray.SetFill ();
						buttonOvalNegativePath.Fill ();
					}
					context.RestoreState ();




//// flareOval Drawing
					var flareOvalPath = UIBezierPath.FromOval (new CGRect (22, 14, 29, 15));
					context.SaveState ();
					flareOvalPath.AddClip ();
					context.DrawLinearGradient (buttonFlareGradient, new CGPoint (36.5f, 14), new CGPoint (36.5f, 29), 0);
					context.RestoreState ();


					// ------------- END PAINTCODE -------------
				}
			}
		}
	}
}

