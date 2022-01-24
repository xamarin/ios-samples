using static System.Math;

namespace ScanningAndDetecting3DObjects;

internal static class NVector3_Extensions
{
	internal static SCNVector3 ToSCNVector3 (this NVector3 self) => new SCNVector3 (self.X, self.Y, self.Z);

	internal static float Distance (this NVector3 self, NVector3 other)
	{
		var distanceSquared = Pow (self.X - other.X, 2) + Pow (self.Y - other.Y, 2) + Pow (self.Z - other.Z, 2);
		return (float)Sqrt (distanceSquared);
	}

	internal static NVector3 Minus (this NVector3 self, NVector3 other)
	{
		return new NVector3 (self.X - other.X, self.Y - other.Y, self.Z - other.Z);
	}

	internal static NVector3 Plus (this NVector3 self, NVector3 other) => new NVector3 (self.X + other.X, self.Y + other.Y, self.Z + other.Z);


	// Euclidean norm
	internal static float Length (this NVector3 self)
	{
		return (float)Sqrt (self.X * self.X + self.Y * self.Y + self.Z * self.Z);
	}

	internal static NVector3 Times (this NVector3 self, float scale)
	{
		return new NVector3 (self.X * scale, self.Y * scale, self.Z * scale);
	}

	// Element-wise minimum
	internal static NVector3 Min (this NVector3 self, NVector3 other)
	{
		return new NVector3 (Math.Min (self.X, other.X), Math.Min (self.Y, other.Y), Math.Min (self.Y, other.Y));
	}

	// Element-wise maximum
	internal static NVector3 Max (this NVector3 self, NVector3 other)
	{
		return new NVector3 (Math.Max (self.X, other.X), Math.Max (self.Y, other.Y), Math.Max (self.Y, other.Y));
	}
}
