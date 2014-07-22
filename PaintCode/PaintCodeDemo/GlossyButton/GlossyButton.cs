using System;
using UIKit;

using CoreGraphics;
using Foundation;

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
	public class GlossyButton : UIButton
	{
		bool isPressed;
		public UIColor NormalColor;

		/// <summary>
		/// Invoked when the user touches 
		/// </summary>
		public event Action<GlossyButton> Tapped;

		/// <summary>
		/// Creates a new instance of the GlassButton using the specified dimensions
		/// </summary>
		public GlossyButton (CGRect frame) : base (frame)
		{
			NormalColor = UIColor.FromRGBA (0.82f, 0.11f, 0.14f, 1.00f);
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
			if (isPressed && Enabled) {
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
				
				
					//// Abstracted Graphic Attributes
					var textContent = this.Title (UIControlState.Normal); //"STOP";			
					var font = UIFont.SystemFontOfSize (18);
					// ------------- START PAINTCODE -------------



//// Color Declarations
					UIColor frameColorTop = UIColor.FromRGBA (0.20f, 0.20f, 0.20f, 1.00f);
					UIColor frameShadowColor = UIColor.FromRGBA (1.00f, 1.00f, 1.00f, 0.40f);

					UIColor glossyColorBottom = UIColor.FromRGBA (
						                            (buttonColorRGBA [0] * 0.6f + 0.4f),
						                            (buttonColorRGBA [1] * 0.6f + 0.4f),
						                            (buttonColorRGBA [2] * 0.6f + 0.4f),
						                            (buttonColorRGBA [3] * 0.6f + 0.4f)
					                            );
					UIColor glossyColorUp = UIColor.FromRGBA (
						                        (buttonColorRGBA [0] * 0.2f + 0.8f),
						                        (buttonColorRGBA [1] * 0.2f + 0.8f),
						                        (buttonColorRGBA [2] * 0.2f + 0.8f),
						                        (buttonColorRGBA [3] * 0.2f + 0.8f)
					                        );

//// Gradient Declarations
					var glossyGradientColors = new CGColor [] {
						glossyColorUp.CGColor,
						glossyColorBottom.CGColor
					};
					var glossyGradientLocations = new nfloat [] { 0, 1 };
					var glossyGradient = new CGGradient (colorSpace, glossyGradientColors, glossyGradientLocations);

//// Shadow Declarations
					var frameInnerShadow = frameShadowColor.CGColor;
					var frameInnerShadowOffset = new CGSize (0, -0);
					var frameInnerShadowBlurRadius = 3;
					var buttonInnerShadow = UIColor.Black.CGColor;
					var buttonInnerShadowOffset = new CGSize (0, -0);
					var buttonInnerShadowBlurRadius = 12;
					var textShadow = UIColor.Black.CGColor;
					var textShadowOffset = new CGSize (0, -0);
					var textShadowBlurRadius = 1;
					var buttonShadow = UIColor.Black.CGColor;

					var buttonShadowOffset = new CGSize (0, isPressed ? 0 : 2);		// ADDED this code after PaintCode
					var buttonShadowBlurRadius = isPressed ? 2 : 3;					// ADDED this code after PaintCode




//// outerFrame Drawing
					var outerFramePath = UIBezierPath.FromRoundedRect (new CGRect (2.5f, 1.5f, 120, 32), 8);
					context.SaveState ();
					context.SetShadow (buttonShadowOffset, buttonShadowBlurRadius, buttonShadow);
					frameColorTop.SetFill ();
					outerFramePath.Fill ();
					context.RestoreState ();

					UIColor.Black.SetStroke ();
					outerFramePath.LineWidth = 1;
					outerFramePath.Stroke ();


//// innerFrame Drawing
					var innerFramePath = UIBezierPath.FromRoundedRect (new CGRect (5.5f, 4.5f, 114, 26), 5);
					context.SaveState ();
					context.SetShadow (frameInnerShadowOffset, frameInnerShadowBlurRadius, frameInnerShadow);
					buttonColor.SetFill ();
					innerFramePath.Fill ();

////// innerFrame Inner Shadow
					var innerFrameBorderRect = innerFramePath.Bounds;
					innerFrameBorderRect.Inflate (buttonInnerShadowBlurRadius, buttonInnerShadowBlurRadius);
					innerFrameBorderRect.Offset (-buttonInnerShadowOffset.Width, -buttonInnerShadowOffset.Height);
					innerFrameBorderRect = CGRect.Union (innerFrameBorderRect, innerFramePath.Bounds);
					innerFrameBorderRect.Inflate (1, 1);

					var innerFrameNegativePath = UIBezierPath.FromRect (innerFrameBorderRect);
					innerFrameNegativePath.AppendPath (innerFramePath);
					innerFrameNegativePath.UsesEvenOddFillRule = true;

					context.SaveState ();
					{
						var xOffset = buttonInnerShadowOffset.Width + (float)Math.Round (innerFrameBorderRect.Width);
						var yOffset = buttonInnerShadowOffset.Height;
						context.SetShadow (
							new CGSize (xOffset + (xOffset >= 0 ? 0.1f : -0.1f), yOffset + (yOffset >= 0 ? 0.1f : -0.1f)),
							buttonInnerShadowBlurRadius,
							buttonInnerShadow);

						innerFramePath.AddClip ();
						var transform = CGAffineTransform.MakeTranslation (-(float)Math.Round (innerFrameBorderRect.Width), 0);
						innerFrameNegativePath.ApplyTransform (transform);
						UIColor.Gray.SetFill ();
						innerFrameNegativePath.Fill ();
					}
					context.RestoreState ();

					context.RestoreState ();

					UIColor.Black.SetStroke ();
					innerFramePath.LineWidth = 1;
					innerFramePath.Stroke ();


//// Rounded Rectangle Drawing
					var roundedRectanglePath = UIBezierPath.FromRoundedRect (new CGRect (8, 6, 109, 9), 4);
					context.SaveState ();
					roundedRectanglePath.AddClip ();
					context.DrawLinearGradient (glossyGradient, new CGPoint (62.5f, 6), new CGPoint (62.5f, 15), 0);
					context.RestoreState ();



//// Text Drawing
					var textRect = new CGRect (18, 6, 90, 28);
					context.SaveState ();
					context.SetShadow (textShadowOffset, textShadowBlurRadius, textShadow);
					glossyColorUp.SetFill ();

					// Use default button-drawn text
					//new NSString(textContent).DrawString(textRect, font, UILineBreakMode.WordWrap, UITextAlignment.Center);
					context.RestoreState ();

					// ------------- END PAINTCODE -------------
				}
			}
		}
	}
}

