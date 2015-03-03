using System;

namespace SpriteKitPhysicsCollisions
{
	// an enum [Flag] could be used but would require more casts
	public static class Category
	{
		public const uint Missile  = 0x1 << 0;
		public const uint Ship     = 0x1 << 1;
		public const uint Asteroid = 0x1 << 2;
		public const uint Planet   = 0x1 << 3;
		public const uint Edge     = 0x1 << 4;
	}
}

