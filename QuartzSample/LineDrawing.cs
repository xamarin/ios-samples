using System;
using UIKit;
using Foundation;
using CoreGraphics;

using QuartzSample;

public class LineDrawingView : QuartzView {
	public override void DrawInContext (CGContext context)
	{
		// Draw lines with a white stroke color
		context.SetStrokeColor (1f, 1f, 1f, 1f);

		// Draw them with a 2.0 stroke width so they are more visible
		context.SetLineWidth (2);

		context.MoveTo (10, 30);
		context.AddLineToPoint (310, 30);
		context.StrokePath ();

		// Draw connected sequence of lines
		var points = new CGPoint [] {
			new CGPoint (10, 90),
			new CGPoint (70, 60),
			new CGPoint (130, 90),
			new CGPoint (190, 60),
			new CGPoint (250, 90),
			new CGPoint (310, 60)
		};
		
		context.AddLines (points);
		context.StrokePath ();

		var segments = new CGPoint [] {
			new CGPoint (10, 150),
			new CGPoint (70, 120),
			new CGPoint (130, 150),
			new CGPoint (190, 120),
			new CGPoint (250, 150),
			new CGPoint (310, 120),
		};

		// Bulk call to stroke a sequence of line segments

		context.StrokeLineSegments (segments);
	}
}

public class LineWidthDrawingView : QuartzView {
	public override void DrawInContext (CGContext context)
	{
		context.SetStrokeColor (1, 1, 1, 1f);
		
		// Draw lines with a stroke width from 1-10
		for (int i = 1; i <= 10; ++i) {
			context.SetLineWidth (i);
			context.MoveTo (10, (float) i * 20.5f);
			context.AddLineToPoint (310, (float)i * 20.5f);
			context.StrokePath ();
		}
		
		// Demonstration that stroke is even on both sides of the line
		context.SetLineWidth(15);
		context.MoveTo (10, 245.5f);
		context.AddLineToPoint (310, 245.5f);
		context.StrokePath ();
	
		context.SetStrokeColor (1, 0, 0, 1);
		context.SetLineWidth (3);
		context.MoveTo (10, 245.5f);
		context.AddLineToPoint (310, 245.5f);
		context.StrokePath ();
	}	
}

public class LineCapJoinDrawingView : QuartzView {
	public override void DrawInContext (CGContext context)
	{
		// Drawing lines with a white stroke color
		context.SetStrokeColor(1, 1, 1, 1);
		
		// Preserve the current drawing state
		context.SaveState();
		
		// Set the line width so that the cap is visible
		context.SetLineWidth(20);
		
		// Line caps demonstration
		
		// Line cap butt, default.
		context.SetLineCap(CGLineCap.Butt);
		context.MoveTo(40, 30);
		context.AddLineToPoint(280, 30);
		context.StrokePath();
		
		// Line cap round
		context.SetLineCap(CGLineCap.Round);
		context.MoveTo(40, 65);
		context.AddLineToPoint(280, 65);
		context.StrokePath();
		
		// Line cap square
		context.SetLineCap(CGLineCap.Square);
		context.MoveTo(40, 100);
		context.AddLineToPoint(280, 100);
		context.StrokePath();
		
		// Restore the previous drawing state, and save it again.
		context.RestoreState();
		context.SaveState();
		
		// Set the line width so that the join is visible
		context.SetLineWidth(20);
		
		// Line join miter, default
		context.SetLineJoin(CGLineJoin.Miter);
		context.MoveTo(40, 260);
		context.AddLineToPoint(160, 140);
		context.AddLineToPoint(280, 260);
		context.StrokePath();
		
		// Line join round
		context.SetLineJoin(CGLineJoin.Round);
		context.MoveTo(40, 320);
		context.AddLineToPoint(160, 200);
		context.AddLineToPoint(280, 320);
		context.StrokePath();
		
		// Line join bevel
		context.SetLineJoin(CGLineJoin.Bevel);
		context.MoveTo(40, 380);
		context.AddLineToPoint(160, 260);
		context.AddLineToPoint(280, 380);
		context.StrokePath();
	
		// Restore the previous drawing state.
		context.RestoreState();
	
		// Demonstrate where the path that generated each line is
		context.SetStrokeColor(1, 0, 0, 1);
		context.SetLineWidth(3);
		context.MoveTo(40, 30);
		context.AddLineToPoint(280, 30);
		context.MoveTo(40, 65);
		context.AddLineToPoint(280, 65);
		context.MoveTo(40, 100);
		context.AddLineToPoint(280, 100);
		context.MoveTo(40, 260);
		context.AddLineToPoint(160, 140);
		context.AddLineToPoint(280, 260);
		context.MoveTo(40, 320);
		context.AddLineToPoint(160, 200);
		context.AddLineToPoint(280, 320);
		context.MoveTo(40, 380);
		context.AddLineToPoint(160, 260);
		context.AddLineToPoint(280, 380);
		context.StrokePath();
	}	
}

public class LineDashDrawingView : QuartzView {
	public override void DrawInContext (CGContext context)
	{
		// Drawing lines with a white stroke color
		context.SetStrokeColor(1, 1, 1, 1);
		// Draw them with a 2 stroke width so they are a bit more visible.
		context.SetLineWidth(2);
		
		// Each dash entry is a run-length in the current coordinate system.
		// For dash1 we demonstrate the effect of the number of entries in the dash array
		// when count==2, we get length 10 drawn, length 10 skipped, etc
		// when count==3, we get 10 drawn, 10 skipped, 20 draw, 10 skipped, 10 drawn, 20 skipped, etc
		// and so on
		nfloat [] dash1 = new nfloat [] {10, 10, 20, 30, 50};
		
		// Different dash lengths
		for(int i = 2; i <= 5; ++i)
		{
			context.SetLineDash(0, dash1, i);
			context.MoveTo(10, (i - 1) * 20);
			context.AddLineToPoint(310, (i - 1) * 20);
			context.StrokePath();
		}
		
		// For dash2 we always use count 4, but use it to demonstrate the phase
		// phase=0 starts us 0 points into the dash, so we draw 10, skip 10, draw 20, skip 20, etc.
		// phase=6 starts 6 points in, so we draw 4, skip 10, draw 20, skip 20, draw 10, skip 10, etc.
		// phase=12 stats us 12 points in, so we skip 8, draw 20, skip 20, draw 10, skip 10, etc.
		// and so on.
		nfloat [] dash2 = {10, 10, 20, 20};
	
		// Different dash phases
		for(int i = 0; i < 10; ++i)
		{
			context.SetLineDash((nfloat) i * 6, dash2, 4);
			context.MoveTo(10, (nfloat) (i + 6) * 20);
			context.AddLineToPoint(310, (float)(i + 6) * 20);
			context.StrokePath();
		}
	}	
}

