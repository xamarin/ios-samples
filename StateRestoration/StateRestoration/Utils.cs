using System;
using UIKit;

namespace StateRestoration
{
	public static class Utils
	{
		// TODO: https://trello.com/c/VURKbJ2M
		public static bool IsLandscape (this UIInterfaceOrientation orientation)
		{
			return orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight;
		}
	}
}

