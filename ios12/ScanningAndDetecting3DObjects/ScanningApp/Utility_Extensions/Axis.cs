using OpenTK;
using System;

namespace ScanningAndDetecting3DObjects
{
	internal enum Axis
	{
		X,
		Y,
		Z
	}

	static class Axis_Extensions
	{
		internal static NVector3 Normal(this Axis self)
		{
			switch(self)
			{
				case Axis.X : return new NVector3(1, 0, 0);
				case Axis.Y : return new NVector3(0, 1, 0);
				case Axis.Z : return new NVector3(0, 0, 1);
			}
			throw new ArgumentException("Should never reach here.");
		}
	}
}