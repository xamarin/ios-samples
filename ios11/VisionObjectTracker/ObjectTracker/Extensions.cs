using CoreGraphics;

namespace ObjectTracker
{
	/// <summary>
	/// Provides extensions methods for normalized points and rectangles (most Vision measurements are in 
	/// units normalized to the range [0, 1].
	/// </summary>
	public static class Extensions
	{
		public static CGPoint Scaled(this CGPoint self, CGSize size)
		{
			return new CGPoint(self.X * size.Width, self.Y * size.Height);
		}

		public static CGRect Scaled(this CGRect self, CGSize size)
		{
			return new CGRect(self.X * size.Width,
							  self.Y * size.Height,
							  self.Size.Width * size.Width,
							  self.Size.Height * size.Height);
		}
	}
}
