using System;

using CoreGraphics;

namespace SpeedSketch
{
	public static class CGMathExtensions
	{
		public static CGPoint GetCenter (this CGRect rect)
		{
			return Add (rect.Location, new CGVector (rect.Width, rect.Height).Divide (2));
		}

		//public static void SetCenter (this CGRect rect)
		//{
		//	//origin = center - CGVector (dx: width, dy: height) / 2
		//}

		static CGPoint Add (CGPoint left, CGVector right)
		{
			return new CGPoint (left.X + right.dx, left.Y + right.dy);
		}

		public static CGVector Sub (this CGPoint left, CGPoint right)
		{
			return new CGVector (left.X - right.X, left.Y - right.Y);
		}

		static CGVector Divide (this CGVector left, float right)
		{
			return new CGVector (left.dx / right, left.dy / right);
		}


		public static nfloat Quadrance (this CGVector vector)
		{
			return vector.dx * vector.dx + vector.dy * vector.dy;
		}

}
}
