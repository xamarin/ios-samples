using System;
using UIKit;
using Foundation;
using CoreGraphics;

using QuartzSample;

[Register]
public class EllipseArcDrawingView : QuartzView {
	public override void DrawInContext (CGContext context)
	{
		// Drawing with a white stroke color
		context.SetStrokeColor(1, 1, 1, 1);
		// And draw with a blue fill color
		context.SetFillColor(0, 0, 1, 1);
		// Draw them with a 2 stroke width so they are a bit more visible.
		context.SetLineWidth(2);
		
		// Add an ellipse circumscribed in the given rect to the current path, then stroke it
		context.AddEllipseInRect(new CGRect(30, 30, 60, 60));
		context.StrokePath();
		
		// Stroke ellipse convenience that is equivalent to AddEllipseInRect(); StrokePath();
		context.StrokeEllipseInRect(new CGRect(30, 120, 60, 60));
		
		// Fill rect convenience equivalent to AddEllipseInRect(); FillPath();
		context.FillEllipseInRect(new CGRect(30, 210, 60, 60));
		
		// Stroke 2 seperate arcs
		context.AddArc(150, 60, 30, 0, (float) Math.PI/2, false);
		context.StrokePath();
		context.AddArc(150, 60, 30, (float)(3*Math.PI/2), (float)Math.PI, true);
		context.StrokePath();
	
		// Stroke 2 arcs together going opposite directions.
		context.AddArc(150, 150, 30, 0, (float)Math.PI/2, false);
		context.AddArc(150, 150, 30, (float)(3*Math.PI/2), (float)Math.PI, true);
		context.StrokePath();
	
		// Stroke 2 arcs together going the same direction..
		context.AddArc(150, 240, 30, 0, (float)(Math.PI/2), false);
		context.AddArc(150, 240, 30, (float)Math.PI, (float)(3*Math.PI/2), false);
		context.StrokePath();
		
		// Stroke an arc using AddArcToPoint
		CGPoint [] p = {
			new CGPoint (210, 30),
			new CGPoint (210, 60),
			new CGPoint (240, 60),
		};
		context.MoveTo(p[0].X, p[0].Y);
		context.AddArcToPoint(p[1].X, p[1].Y, p[2].X, p[2].Y, 30);
		context.StrokePath();
		
		// Show the two segments that are used to determine the tangent lines to draw the arc.
		context.SetStrokeColor(1, 0, 0, 1);
		context.AddLines(p);
		context.StrokePath();
		
		// As a bonus, we'll combine arcs to create a round rectangle!
		
		// Drawing with a white stroke color
		context.SetStrokeColor(1, 1, 1, 1);
	
		// If you were making this as a routine, you would probably accept a rectangle
		// that defines its bounds, and a radius reflecting the "rounded-ness" of the rectangle.
		var rrect = new CGRect(210, 90, 60, 60);
		var radius = 10;
		// NOTE: At this point you may want to verify that your radius is no more than half
		// the width and height of your rectangle, as this technique degenerates for those cases.
		
		// In order to draw a rounded rectangle, we will take advantage of the fact that
		// context.AddArcToPoint will draw straight lines past the start and end of the arc
		// in order to create the path from the current position and the destination position.
		
		// In order to create the 4 arcs correctly, we need to know the min, mid and max positions
		// on the x and y lengths of the given rectangle.
		nfloat minx = rrect.X, midx = rrect.X+rrect.Width/2, maxx = rrect.X+rrect.Width;
		nfloat miny = rrect.Y, midy = rrect.Y+rrect.Height/2, maxy = rrect.Y+rrect.Height;
		
		// Next, we will go around the rectangle in the order given by the figure below.
		//       minx    midx    maxx
		// miny    2       3       4
		// midy   1 9              5
		// maxy    8       7       6
		// Which gives us a coincident start and end point, which is incidental to this technique, but still doesn't
		// form a closed path, so we still need to close the path to connect the ends correctly.
		// Thus we start by moving to point 1, then adding arcs through each pair of points that follows.
		// You could use a similar tecgnique to create any shape with rounded corners.
		
		// Start at 1
		context.MoveTo(minx, midy);
		// Add an arc through 2 to 3
		context.AddArcToPoint(minx, miny, midx, miny, radius);
		// Add an arc through 4 to 5
		context.AddArcToPoint(maxx, miny, maxx, midy, radius);
		// Add an arc through 6 to 7
		context.AddArcToPoint(maxx, maxy, midx, maxy, radius);
		// Add an arc through 8 to 9
		context.AddArcToPoint(minx, maxy, minx, midy, radius);
		// Close the path
		context.ClosePath();
		// Fill & stroke the path
		context.DrawPath(CGPathDrawingMode.FillStroke);
	}	
}

[Register]
public class BezierDrawingView : QuartzView {
	public override void DrawInContext (CGContext context)
	{
		// Drawing with a white stroke color
		context.SetStrokeColor(1, 1, 1, 1);
		// Draw them with a 2 stroke width so they are a bit more visible.
		context.SetLineWidth(2);
		
		// Draw a bezier curve with end points s,e and control points cp1,cp2
		var s = new CGPoint (30, 120);
		var e = new CGPoint (300, 120);
		var cp1 = new CGPoint (120, 30);
		var cp2 = new CGPoint (210, 210);
		context.MoveTo(s.X, s.Y);
		context.AddCurveToPoint(cp1.X, cp1.Y, cp2.X, cp2.Y, e.X, e.Y);
		context.StrokePath();
		
		// Show the control points.
		context.SetStrokeColor(1, 0, 0, 1);
		context.MoveTo(s.X, s.Y);
		context.AddLineToPoint(cp1.X, cp1.Y);
		context.MoveTo(e.X, e.Y);
		context.AddLineToPoint(cp2.X, cp2.Y);
		context.StrokePath();
		
		// Draw a quad curve with end points s,e and control point cp1
		context.SetStrokeColor(1, 1, 1, 1);
		s = new CGPoint (30, 300);
		e = new CGPoint (270, 300);
		cp1 = new CGPoint (150, 180);
		context.MoveTo(s.X, s.Y);
		context.AddQuadCurveToPoint(cp1.X, cp1.Y, e.X, e.Y);
		context.StrokePath();
	
		// Show the control point.
		context.SetStrokeColor(1, 0, 0, 1);
		context.MoveTo(s.X, s.Y);
		context.AddLineToPoint(cp1.X, cp1.Y);
		context.StrokePath();
	}	
}

