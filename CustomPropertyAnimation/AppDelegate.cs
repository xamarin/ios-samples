using System;
using CoreGraphics;

using CoreAnimation;

using Foundation;
using UIKit;

namespace CustomPropertyAnimation
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UIViewController vc;
		CircleLayer testLayer;
		CABasicAnimation radiusAnimation;
		CABasicAnimation thicknessAnimation;
		CABasicAnimation colorAnimation;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
		
			vc = new UIViewControllerRotation ();

			vc.View.BackgroundColor = UIColor.Black;
			testLayer = new CircleLayer();
			testLayer.Color = UIColor.Green.CGColor;
			testLayer.Thickness = 19f;
			testLayer.Radius = 60f;
	
			testLayer.Frame = vc.View.Layer.Bounds;
			vc.View.Layer.AddSublayer(testLayer);
			
			testLayer.SetNeedsDisplay();
			
			radiusAnimation = CABasicAnimation.FromKeyPath ("radius");
			radiusAnimation.Duration = 3;
			radiusAnimation.To = NSNumber.FromDouble (120);
			radiusAnimation.RepeatCount = 1000;
			
			thicknessAnimation = CABasicAnimation.FromKeyPath ("thickness");
			thicknessAnimation.Duration = 2;
			thicknessAnimation.From = NSNumber.FromDouble (5);
			thicknessAnimation.To = NSNumber.FromDouble (38);
			thicknessAnimation.RepeatCount = 1000;
			
			colorAnimation = CABasicAnimation.FromKeyPath ("circleColor");
			colorAnimation.Duration = 4;
			colorAnimation.To = new NSObject (UIColor.Blue.CGColor.Handle);
			colorAnimation.RepeatCount = 1000;
			
			testLayer.AddAnimation (radiusAnimation, "radiusAnimation");
			testLayer.AddAnimation (thicknessAnimation, "thicknessAnimation");
			testLayer.AddAnimation (colorAnimation, "colorAnimation");
			
			window.RootViewController = vc;
			// make the window visible
			window.MakeKeyAndVisible ();
			return true;
		}
		
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}

	public class UIViewControllerRotation : UIViewController
	{
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);

			// call our helper method to position the controls
			CALayer[] layers = this.View.Layer.Sublayers;
			foreach (CALayer layer in layers) 
			{
				layer.Frame = this.View.Layer.Bounds;
			}
		}

	}
	
	public class CircleLayer : CALayer
	{
		public CircleLayer ()
		{
		}

		[Export ("initWithLayer:")]
		public CircleLayer (CALayer other)
			: base (other)
		{
		}

		public override void Clone (CALayer other)
		{
			CircleLayer o = (CircleLayer) other;
			Radius = o.Radius;
			Color = o.Color;
			Thickness = o.Thickness;
			base.Clone (other);
		}
		
		[Export ("radius")]
		public double Radius { get; set; }
		
		[Export ("thickness")]
		public double Thickness { get; set; }
		
		[Export ("circleColor")]
		public CGColor Color { get; set; }

		[Export ("needsDisplayForKey:")]
		static bool NeedsDisplayForKey (NSString key)
		{
			switch (key.ToString ()) {
			case "radius":
			case "thickness":
			case "circleColor":
				return true;
			default:
				return CALayer.NeedsDisplayForKey (key);
			}
		}
		
		public override void DrawInContext (CGContext context)
		{
			base.DrawInContext (context);

			// Console.WriteLine ("DrawInContext Radius: {0} Thickness: {1} Color: {2}", Radius, Thickness, Color);
			//Console.WriteLine (this.Bounds.Width+"   "+ this.Bounds.Height);


			CGPoint centerPoint = new CGPoint (this.Bounds.Width / 2, this.Bounds.Height / 2);
			CGColor glowColor = new UIColor (Color).ColorWithAlpha (0.85f).CGColor;
			double innerRadius = (Radius - Thickness) > 0 ? Radius - Thickness : 0;
	
			// Outer circle
			context.AddEllipseInRect (new CGRect (centerPoint.X - (float) Radius,
			                                        centerPoint.Y - (float) Radius,
			                                        (float) Radius * 2,
			                                        (float) Radius * 2));
			// Inner circle
			context.AddEllipseInRect (new CGRect (centerPoint.X - (float) innerRadius,
			                                        centerPoint.Y - (float) innerRadius,
			                                        (float) innerRadius * 2,
			                                        (float) innerRadius * 2));
			
			// Fill in circle
			context.SetFillColor (Color);
			context.SetShadow (CGSize.Empty, 10.0f, glowColor);
			context.EOFillPath();
		}
	}
}
