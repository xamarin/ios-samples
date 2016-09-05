using UIKit;
using Foundation;

namespace SpeedSketch
{
	public static class Helpers
	{
		public static NSNumber [] TouchTypes (UITouchType type)
		{
			return new NSNumber [] { new NSNumber ((long)type) };
		}
	}
}
