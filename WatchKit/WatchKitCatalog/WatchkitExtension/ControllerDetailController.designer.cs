using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("ControllerDetailController")]
	partial class ControllerDetailController
	{
		[Action ("presentPages:")]
		partial void PresentPages (NSObject obj);

		[Action ("menuItemTapped:")]
		partial void MenuItemTapped (NSObject obj);
	}
}

