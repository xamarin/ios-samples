using System;

using UIKit;

namespace HomeKitCatalog
{
	public static class UIStoryboardSegueExtensions
	{
		public static UIViewController IntendedDestinationViewController(this UIStoryboardSegue segue)
		{
			var navigationController = segue.DestinationViewController as UINavigationController;
			return navigationController != null ? navigationController.TopViewController : segue.DestinationViewController;
		}
	}
}