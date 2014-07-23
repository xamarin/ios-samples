using System;
using UIKit;
using Foundation;
using CoreGraphics;

using QuartzSample;

public class RectDrawingView : QuartzView {
	public override void DrawInContext (CGContext context)
	{
		// Drawing with a white stroke color
		context.SetStrokeColor (1, 1, 1, 1);
		// And drawing with a blue fill color
		context.SetFillColor (0, 0, 1, 1);
		// Draw them with a 2 stroke width so they are a bit more visible.
		context.SetLineWidth (2);
		
		// Add Rect to the current path, then stroke it
		context.AddRect (new CGRect (30, 30, 60, 60));
		context.StrokePath ();
		
		// Stroke Rect convenience that is equivalent to above
		context.StrokeRect (new CGRect (30, 120, 60, 60));
		
		// Stroke rect convenience equivalent to the above, plus a call to context.SetLineWidth ().
		context.StrokeRectWithWidth (new CGRect (30, 210, 60, 60), 10);
		// Demonstate the stroke is on both sides of the path.
		context.SaveState ();
		context.SetStrokeColor (1, 0, 0, 1);
		context.StrokeRectWithWidth (new CGRect (30, 210, 60, 60), 2);
		context.RestoreState ();
		
		var rects = new CGRect []
		{
			new CGRect (120, 30, 60, 60),
			new CGRect (120, 120, 60, 60),
			new CGRect (120, 210, 60, 60),
		};
		// Bulk call to add rects to the current path.
		context.AddRects (rects);
		context.StrokePath ();
		
		// Create filled rectangles via two different paths.
		// Add/Fill path
		context.AddRect (new CGRect (210, 30, 60, 60));
		context.FillPath ();
		// Fill convienience.
		context.FillRect (new CGRect (210, 120, 60, 60));
	}	
}

public class PolyDrawingView : QuartzView {
	public override void DrawInContext (CGContext context)
	{
		// Drawing with a white stroke color
		context.SetStrokeColor (1, 1, 1, 1);
		// Drawing with a blue fill color
		context.SetFillColor (0, 0, 1, 1);
		// Draw them with a 2 stroke width so they are a bit more visible.
		context.SetLineWidth (2);
	
		CGPoint center;
	
		// Draw a Star stroked
		center = new CGPoint (90, 90);
		context.MoveTo (center.X, center.Y + 60);
		for (int i = 1; i < 5; ++i)
		{
			float x = (float)(60 * Math.Sin (i * 4 * Math.PI / 5));
			float y = (float)(60 * Math.Cos (i * 4 * Math.PI / 5));
			
			context.AddLineToPoint (center.X + x, center.Y + y);
		}
		// Closing the path connects the current point to the start of the current path.
		context.ClosePath ();
		// And stroke the path
		context.StrokePath ();

		// Draw a Star filled
		center = new CGPoint (90, 210);
		context.MoveTo (center.X, center.Y + 60);
		for (int i = 1; i < 5; ++i)
		{
			float x = (float) (60 * Math.Sin (i * 4 * Math.PI / 5));
			float y = (float) (60 * Math.Cos (i * 4 * Math.PI / 5));
			context.AddLineToPoint (center.X + x, center.Y + y);
		}
		// Closing the path connects the current point to the start of the current path.
		context.ClosePath ();
		// Use the winding-rule fill mode.
		context.FillPath ();
	
		// Draw a Star filled
		center = new CGPoint (90, 330);
		context.MoveTo (center.X, center.Y + 60);
		for (int i = 1; i < 5; ++i)
		{
			float x = (float)(60 * Math.Sin (i * 4 * Math.PI / 5));
			float y = (float)(60 * Math.Cos (i * 4 * Math.PI / 5));
			context.AddLineToPoint (center.X + x, center.Y + y);
		}
		// Closing the path connects the current point to the start of the current path.
		context.ClosePath ();
		// Use the even-odd fill mode.
		context.EOFillPath ();
	
		// Draw a Hexagon stroked
		center = new CGPoint (210, 90);
		context.MoveTo (center.X, center.Y + 60);
		for (int i = 1; i < 6; ++i)
		{
			float x = (float)(60 * Math.Sin (i * 2 * Math.PI / 6));
			float y = (float)(60 * Math.Cos (i * 2 * Math.PI / 6));
			context.AddLineToPoint (center.X + x, center.Y + y);
		}
		// Closing the path connects the current point to the start of the current path.
		context.ClosePath ();
		// And stroke the path
		context.StrokePath ();
	
		// Draw a Hexagon stroked & filled
		center = new CGPoint (210, 240);
		context.MoveTo (center.X, center.Y + 60);
		for (int i = 1; i < 6; ++i)
		{
			float x = (float)(60 * Math.Sin (i * 2 * Math.PI / 6));
			float y = (float)(60 * Math.Cos (i * 2 * Math.PI / 6));
			context.AddLineToPoint (center.X + x, center.Y + y);
		}
		// Closing the path connects the current point to the start of the current path.
		context.ClosePath ();
		// Use the winding-rule fill mode, and stroke the path after.
		context.DrawPath (CGPathDrawingMode.FillStroke);
	}	
}

