using System;
using Foundation;
using UIKit;
using CoreGraphics;
using SceneKit;
using ARKit;

namespace PlacingObjects
{
	public static class CGPointExtensions
	{
		public static CGPoint FromSize(CGSize size) {
			return new CGPoint(size.Width, size.Height);
		}

		public static CGPoint FromVector(SCNVector3 vector)
		{
			return new CGPoint(vector.X, vector.Y);
		}

		public static float DistanceTo(this CGPoint self, CGPoint point)
		{
			return self.Subtract(point).Length();
		}

		public static float Length(this CGPoint point) {
			return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
		}

		public static CGPoint MidPoint(this CGPoint self, CGPoint point) {
			return self.Add(point).Divide(2f);
		}

		public static CGPoint Add(this CGPoint self, CGPoint point) {
			return new CGPoint(self.X + point.X, self.Y + point.Y);
		}

		public static CGPoint Add(this CGPoint self, float value)
		{
			return new CGPoint(self.X + value, self.Y + value);
		}
         
       public static CGPoint Subtract(this CGPoint self, CGPoint point)
		{
			return new CGPoint(self.X - point.X, self.Y - point.Y);
		}

		public static CGPoint Subtract(this CGPoint self, float value)
		{
			return new CGPoint(self.X - value, self.Y - value);
		}

		public static CGPoint Multiply(this CGPoint self, CGPoint point)
		{
			return new CGPoint(self.X * point.X, self.Y * point.Y);
		}

		public static CGPoint Multiply(this CGPoint self, float value)
		{
			return new CGPoint(self.X * value, self.Y * value);
		}

		public static CGPoint Divide(this CGPoint self, CGPoint point)
		{
			return new CGPoint(self.X / point.X, self.Y / point.Y);
		}

		public static CGPoint Divide(this CGPoint self, float value)
		{
			return new CGPoint(self.X / value, self.Y / value);
		}
	}
}
