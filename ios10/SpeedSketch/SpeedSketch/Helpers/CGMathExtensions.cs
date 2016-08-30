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

		public static CGVector Vector (CGPoint start, CGPoint end)
		{
			return new CGVector (end.X - start.X, end.Y - start.Y);
		}

		public static CGPoint Sub (this CGPoint left, CGVector right)
		{
			return new CGPoint (left.X - right.dx, left.Y - right.dy);
		}

		public static nfloat Quadrance (this CGVector vector)
		{
			return vector.dx * vector.dx + vector.dy * vector.dy;
		}

		public static CGVector Apply (this CGVector vector, CGAffineTransform transform)
		{
			return vector.CreatePoint ().Apply (transform).CreateVector ();
		}

		public static nfloat Distance (CGPoint? point1, CGPoint? point2)
		{
			if (point1.HasValue && point2.HasValue)
				return NMath.Sqrt (Vector (point1.Value, point2.Value).Quadrance ());
			return nfloat.PositiveInfinity;
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

		// CGVector pointing in the same direction as self, with a length of 1.0 - or nil if the length is zero.
		public static CGVector? Normalize (this CGVector self)
		{
			var quadrance = self.Quadrance ();
			return (quadrance > 0) ? self.Divide (NMath.Sqrt(quadrance)) : (CGVector?)null;
		}

		public static CGVector? Normal (this CGVector self)
		{
			bool isZero = (self.dx == 0 && self.dy == 0);
			return isZero ? (CGVector?)null : new CGVector (-self.dy, self.dx);
		}

		public static CGVector Divide (this CGVector left, nfloat right)
		{
			return new CGVector (left.dx / right, left.dy / right);
		}

		public static CGVector Mult (this CGVector left, nfloat right)
		{
			return new CGVector (left.dx * right, left.dy * right);
		}

		public static CGVector? Add (this CGVector? left, CGVector? right)
		{
			if (!left.HasValue || !right.HasValue)
				return null;

			var l = left.Value;
			var r = right.Value;
			return new CGVector (l.dx + r.dx, l.dy + r.dy);
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

		public static CGRect ToRect (this CGPoint location)
		{
			return new CGRect (location, CGSize.Empty);
		}
	}

	public static class CGSizeExtensions
	{
		public static CGSize Add (this CGSize left, nfloat right)
		{
			return new CGSize (left.Width + right, left.Height + right);
		}
	}
}
