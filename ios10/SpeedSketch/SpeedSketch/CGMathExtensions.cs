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

		public static CGPoint Add (this CGPoint left, CGVector right)
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

		public static CGVector Apply (this CGVector vector, CGAffineTransform transform)
		{
			return vector.CreatePoint ().Apply (transform).CreateVector ();
		}
	}

	public static class CGVectorExtensions
	{
		public static CGPoint CreatePoint (this CGVector self)
		{
			return new CGPoint (self.dx, self.dy);
		}

		public static CGVector RoundTo (this CGVector self, nfloat scale)
		{
			return new CGVector (NMath.Round (self.dx * scale) / scale,
								 NMath.Round (self.dy * scale) / scale);
		}
	}

	public static class CGPointExtensions
	{
		public static CGVector CreateVector (this CGPoint self)
		{
			return new CGVector (self.X, self.Y);
		}

		public static CGPoint Apply (this CGPoint point, CGAffineTransform transform)
		{
			return transform.TransformPoint (point);
		}
	}
}
