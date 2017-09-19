using System;
using OpenTK;
using SceneKit;

namespace PlacingObjects
{
	public static class NMatrix4Extensions
	{
		public static SCNVector3 Translation(this NMatrix4 self)
		{
			return new SCNVector3(self.M14, self.M24, self.M34);
		}

	}
}
