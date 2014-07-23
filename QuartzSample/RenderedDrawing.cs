using System;
using UIKit;
using Foundation;
using CoreGraphics;

using QuartzSample;

public class GradientDrawingView : QuartzView {
	CGGradient gradient;
	
	public GradientDrawingView () : base ()
	{
		using (var rgb = CGColorSpace.CreateDeviceRGB()){
			nfloat [] colors =
				{
					204f / 255f, 224f / 255f, 244f / 255f, 10f,
					29f / 255f, 156f / 255f, 215f / 255f, 10f,
					0f / 255f,  50f / 255f, 126f / 255f, 10f,
				};
			gradient = new CGGradient (rgb, colors, null);
		}
	}

	// Returns an appropriate starting point for the demonstration of a linear gradient
	static CGPoint demoLGStart (CGRect bounds)
	{
		return new CGPoint  (bounds.X, bounds.Y + bounds.Height * 0.25f);
	}
	
	// Returns an appropriate ending point for the demonstration of a linear gradient
	CGPoint demoLGEnd(CGRect bounds)
	{
		return new CGPoint  (bounds.X, bounds.Y + bounds.Height * 0.75f);
	}
	
	// Returns the center point for for the demonstration of the radial gradient
	CGPoint demoRGCenter(CGRect bounds)
	{
		return new CGPoint  (bounds.X + bounds.Width/2, bounds.Y + bounds.Height/2);
	}
	
	// Returns an appropriate inner radius for the demonstration of the radial gradient
	nfloat demoRGInnerRadius(CGRect bounds)
	{
		nfloat r = bounds.Width < bounds.Height ? bounds.Width : bounds.Height;
		return r * 0.125f;
	}
	
	// Returns an appropriate outer radius for the demonstration of the radial gradient
	nfloat demoRGOuterRadius(CGRect bounds)
	{
		nfloat r = bounds.Width < bounds.Height ? bounds.Width : bounds.Height;
		return r * 0.5f;
	}
	
	public override void DrawInContext (CGContext context)
	{
		// The clipping rects we plan to use, which also defines the location and span of each gradient
		var clips = new CGRect [] 
		{
			new CGRect(10, 30, 60, 90),
			new CGRect(90, 30, 60, 90),
			new CGRect(170, 30, 60, 90),
			new CGRect(250, 30, 60, 90),
			new CGRect(30, 140, 120, 120),
			new CGRect(170, 140, 120, 120),
			new CGRect(30, 280, 120, 120),
			new CGRect(170, 280, 120, 120),
		};

		// Linear Gradients
		CGPoint start, end;
		
		// Clip to area to draw the gradient, and draw it. Since we are clipping, we save the graphics state
		// so that we can revert to the previous larger area.
		context.SaveState();
		context.ClipToRect(clips[0]);
		
		// A linear gradient requires only a starting & ending point.
		// The colors of the gradient are linearly interpolated along the line segment connecting these two points
		// A gradient location of 0 means that color is expressed fully at the 'start' point
		// a location of 1 means that color is expressed fully at the 'end' point.
		// The gradient fills outwards perpendicular to the line segment connectiong start & end points
		// (which is why we need to clip the context, or the gradient would fill beyond where we want it to).
		// The gradient options (last) parameter determines what how to fill the clip area that is "before" and "after"
		// the line segment connecting start & end.
		start = demoLGStart(clips[0]);
		end = demoLGEnd(clips[0]);
		context.DrawLinearGradient(gradient, start, end, 0);
		context.RestoreState();

		// Same as above for each combination of CGGradientDrawingOptions.DrawsBeforeStartLocation & CGGradientDrawingOptions.DrawsAfterEndLocation
		
		context.SaveState();
		context.ClipToRect(clips[1]);
		start = demoLGStart(clips[1]);
		end = demoLGEnd(clips[1]);
		context.DrawLinearGradient(gradient, start, end, CGGradientDrawingOptions.DrawsBeforeStartLocation);
		context.RestoreState();
		
		context.SaveState();
		context.ClipToRect(clips[2]);
		start = demoLGStart(clips[2]);
		end = demoLGEnd(clips[2]);
		context.DrawLinearGradient(gradient, start, end, CGGradientDrawingOptions.DrawsAfterEndLocation);
		context.RestoreState();
		
		context.SaveState();
		context.ClipToRect(clips[3]);
		start = demoLGStart(clips[3]);
		end = demoLGEnd(clips[3]);
		context.DrawLinearGradient(gradient, start, end, CGGradientDrawingOptions.DrawsBeforeStartLocation | CGGradientDrawingOptions.DrawsAfterEndLocation);
		context.RestoreState();
		
		// Radial Gradients
		
		nfloat startRadius, endRadius;
	
		// Clip to area to draw the gradient, and draw it. Since we are clipping, we save the graphics state
		// so that we can revert to the previous larger area.
		context.SaveState();
		context.ClipToRect(clips[4]);
		
		// A radial gradient requires a start & end point as well as a start & end radius.
		// Logically a radial gradient is created by linearly interpolating the center, radius and color of each
		// circle using the start and end point for the center, start and end radius for the radius, and the color ramp
		// inherant to the gradient to create a set of stroked circles that fill the area completely.
		// The gradient options specify if this interpolation continues past the start or end points as it does with
		// linear gradients.
		start = end = demoRGCenter(clips[4]);
		startRadius = demoRGInnerRadius(clips[4]);
		endRadius = demoRGOuterRadius(clips[4]);
		context.DrawRadialGradient(gradient, start, startRadius, end, endRadius, 0);
		context.RestoreState();
	
		// Same as above for each combination of CGGradientDrawingOptions.DrawsBeforeStartLocation & CGGradientDrawingOptions.DrawsAfterEndLocation
	
		context.SaveState();
		context.ClipToRect(clips[5]);
		start = end = demoRGCenter(clips[5]);
		startRadius = demoRGInnerRadius(clips[5]);
		endRadius = demoRGOuterRadius(clips[5]);
		context.DrawRadialGradient(gradient, start, startRadius, end, endRadius, CGGradientDrawingOptions.DrawsBeforeStartLocation);
		context.RestoreState();
	
		context.SaveState();
		context.ClipToRect(clips[6]);
		start = end = demoRGCenter(clips[6]);
		startRadius = demoRGInnerRadius(clips[6]);
		endRadius = demoRGOuterRadius(clips[6]);
		context.DrawRadialGradient(gradient, start, startRadius, end, endRadius, CGGradientDrawingOptions.DrawsAfterEndLocation);
		context.RestoreState();
	
		context.SaveState();
		context.ClipToRect(clips[7]);
		start = end = demoRGCenter(clips[7]);
		startRadius = demoRGInnerRadius(clips[7]);
		endRadius = demoRGOuterRadius(clips[7]);
		context.DrawRadialGradient(gradient, start, startRadius, end, endRadius, CGGradientDrawingOptions.DrawsBeforeStartLocation | CGGradientDrawingOptions.DrawsAfterEndLocation);
		context.RestoreState();
		
		// Show the clipping areas
		context.SetLineWidth(2);
		context.SetStrokeColor(1, 0, 0, 1);
		context.AddRects(clips);
		context.StrokePath();
	}	
}

[Register]
public class PatternDrawingView : QuartzView {
	CGColor coloredPatternColor;
	CGPattern uncoloredPattern;
	CGColorSpace uncoloredPatternColorSpace;
	
	static void DrawColored (CGContext context)
	{
		// Dark Blue
		context.SetFillColor(29 / 255f, 156 / 255f, 215 / 255f, 10);
		context.FillRect(new CGRect(0, 0, 8, 8));
		context.FillRect(new CGRect(8, 8, 8, 8));
		
		// Light Blue
		context.SetFillColor(204 / 255f, 224 / 255f, 244 / 255f, 10);
		context.FillRect(new CGRect(8, 0, 8, 8));
		context.FillRect(new CGRect(0, 8, 8, 8));
	}
	
	// Uncolored patterns take their color from the given context
	static void DrawUncolored (CGContext context)
	{
		context.FillRect(new CGRect(0, 0, 8, 8));
		context.FillRect(new CGRect(8, 8, 8, 8));
	}
	
	public PatternDrawingView () : base () {
		
		// First we need to create a CGPattern that specifies the qualities of our pattern.
		
		using (var coloredPattern = new CGPattern (
			       new CGRect(0, 0, 16, 16), // the pattern coordinate space, drawing is clipped to this rectangle
			       CGAffineTransform.MakeIdentity (), // a transform on the pattern coordinate space used before it is drawn.
			       16, 16, // the spacing (horizontal, vertical) of the pattern - how far to move after drawing each cell
			       CGPatternTiling.NoDistortion,
			       true, // this is a colored pattern, which means that you only specify an alpha value when drawing it
			       DrawColored)){
			
			// To draw a pattern, you need a pattern colorspace.
			// Since this is an colored pattern, the parent colorspace is NULL, indicating that it only has an alpha value.
			using (var coloredPatternColorSpace = CGColorSpace.CreatePattern (null)){
				float alpha = 1;
				
				// Since this pattern is colored, we'll create a CGColor for it to make drawing it easier and more efficient.
				// From here on, the colored pattern is referenced entirely via the associated CGColor rather than the
				// originally created CGPatternRef.
				coloredPatternColor = new CGColor (coloredPatternColorSpace, coloredPattern, new nfloat [] { alpha });
			}
		}
		
		// Uncolored Pattern setup
		// As above, we create a CGPattern that specifies the qualities of our pattern
		uncoloredPattern = new CGPattern (
			new CGRect(0, 0, 16, 16), // coordinate space
			CGAffineTransform.MakeIdentity (), // transform
			16, 16, // spacing
			CGPatternTiling.NoDistortion,
			false, // this is an uncolored pattern, thus to draw it we need to specify both color and alpha
			DrawUncolored); // callbacks for this pattern
		
			// With an uncolored pattern we still need to create a pattern colorspace, but now we need a parent colorspace
			// We'll use the DeviceRGB colorspace here. We'll need this colorspace along with the CGPatternRef to draw this pattern later.
		using (var deviceRGB = CGColorSpace.CreateDeviceRGB()){
			uncoloredPatternColorSpace = CGColorSpace.CreatePattern(deviceRGB);
		}
	}
	
	public override void DrawInContext (CGContext context)
	{
		// Draw the colored pattern. Since we have a CGColorRef for this pattern, we just set
		// that color current and draw.
		context.SetFillColor(coloredPatternColor);
		context.FillRect(new CGRect(10, 10, 90, 90));
		
		// You can also stroke with a pattern.
		context.SetStrokeColor(coloredPatternColor);
		context.StrokeRectWithWidth(new CGRect(120, 10, 90, 90), 8);
		
		// Since we aren't encapsulating our pattern in a CGColor for the uncolored pattern case, setup requires two steps.
		// First you have to set the context's current colorspace (fill or stroke) to a pattern colorspace,
		// indicating to Quartz that you want to draw a pattern.
		context.SetFillColorSpace(uncoloredPatternColorSpace);

	
		// Next you set the pattern and the color that you want the pattern to draw with.
		var color1 = new nfloat [] {1, 0, 0, 1};
		context.SetFillPattern(uncoloredPattern, color1);
		// And finally you draw!
		context.FillRect(new CGRect(10, 120, 90, 90));

		// As long as the current colorspace is a pattern colorspace, you are free to change the pattern or pattern color
		var color2 = new nfloat [] {0, 1, 0, 1};
		context.SetFillPattern(uncoloredPattern, color2);
		context.FillRect(new CGRect(10, 230, 90, 90));

		// And of course, just like the colored case, you can stroke with a pattern as well.
		context.SetStrokeColorSpace(uncoloredPatternColorSpace);
		context.SetStrokePattern(uncoloredPattern, color1);
		context.StrokeRectWithWidth(new CGRect(120, 120, 90, 90), 8);
		// As long as the current colorspace is a pattern colorspace, you are free to change the pattern or pattern color
		context.SetStrokePattern(uncoloredPattern, color2);
		context.StrokeRectWithWidth(new CGRect(120, 230, 90, 90), 8);

	}	
}
