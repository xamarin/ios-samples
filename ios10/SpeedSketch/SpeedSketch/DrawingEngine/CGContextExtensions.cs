using CoreGraphics;

namespace SpeedSketch
{
	public static class CGContextExtensions
	{
		public static void Move (this CGContext context, CGPoint point)
		{
			context.MoveTo (point.X, point.Y);
		}

		public static void AddLine (this CGContext context, CGPoint point)
		{
			context.AddLineToPoint (point.X, point.Y);
		}
	}
}
