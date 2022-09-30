namespace XamarinShot.Utils;

/// <summary>
/// Helpers for CGPoint
/// </summary>
public static class CGPointExtensions
{
	/// <summary>
	/// /// Returns the length of a point when considered as a vector. (Used with gesture recognizers.)
	/// </summary>
	public static float Length (this CGPoint point)
	{
		return (float)Math.Sqrt (point.X * point.X + point.Y * point.Y);
	}
}
