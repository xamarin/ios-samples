using static System.Math;

namespace ScanningAndDetecting3DObjects;

internal static class SCNVector3_Extensions
{
	internal static float Distance (this SCNVector3 self, SCNVector3 other)
	{
		var distanceSquared = Pow (self.X - other.X, 2) + Pow (self.Y - other.Y, 2) + Pow (self.Z - other.Z, 2);
		return (float)Sqrt (distanceSquared);
	}

	internal static NVector3 Times (this SCNVector3 self, float scale)
	{
		return new NVector3 (self.X * scale, self.Y * scale, self.Z * scale);
	}

	internal static SCNVector3 Normalized (this SCNVector3 self)
	{
		var copy = new SCNVector3 (self);
		copy.Normalize ();
		return copy;
	}

	// Elementwise minimum 
	internal static SCNVector3 Min (this SCNVector3 self, SCNVector3 other) => new SCNVector3 (Math.Min (self.X, other.X), Math.Min (self.Y, other.Y), Math.Min (self.Z, other.Z));

	// Elementwise maximum
	internal static SCNVector3 Max (this SCNVector3 self, SCNVector3 other) => new SCNVector3 (Math.Max (self.X, other.X), Math.Max (self.Y, other.Y), Math.Max (self.Z, other.Z));

	internal static NVector3 ToNVector3 (this SCNVector3 self) => new NVector3 (self.X, self.Y, self.Z);

	internal static NVector3 Minus (this SCNVector3 self, NVector3 other) => new NVector3 (self.X - other.X, self.Y - other.Y, self.Z - other.Z);

	internal static SCNVector3 Plus (this SCNVector3 self, NVector3 other) => new SCNVector3 (self.X + other.X, self.Y + other.Y, self.Z + other.Z);

	internal static SCNVector3 Plus (this SCNVector3 self, Vector3d other) => new SCNVector3 (self.X + (float)other.X, self.Y + (float)other.Y, self.Z + (float)other.Z);
}
