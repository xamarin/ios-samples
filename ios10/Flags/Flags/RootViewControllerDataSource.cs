using System.Collections.Generic;

using UIKit;
using Foundation;

namespace Flags
{
	public class RootViewControllerDataSource : UIPageViewControllerDataSource
	{
		readonly List<string> regionCodes = new List<string> ();

		// TODO: replace with .net analog
		NSCache cachedDataViewControllers = new NSCache ();

		UIStoryboard storyboard;

		public RootViewControllerDataSource (UIStoryboard storyboard)
		{
			this.storyboard = storyboard;

			regionCodes.AddRange (NSLocale.ISOCountryCodes);
		}

		public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var dataViewController = referenceViewController as DataViewController;
			if (dataViewController == null)
				return null;


			var index = IndexOfViewController (dataViewController);
			if (index <= 0)
				return null;

			index -= 1;
			return GetViewControllerAt (index);
		}

		public override UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var dataViewController = referenceViewController as DataViewController;
			if (dataViewController == null)
				return null;

			var index = IndexOfViewController (dataViewController);
			if (index >= regionCodes.Count - 1)
				return null;

			index += 1;
			return GetViewControllerAt (index);
		}

		DataViewController GetViewControllerAt (int index)
		{
			if (regionCodes.Count == 0 || index >= regionCodes.Count)
				return null;

			var regionCode = regionCodes [index];

			// Return the data view controller for the given index.
			var dataViewController = (DataViewController)cachedDataViewControllers.ObjectForKey ((NSString)regionCode);
			if (dataViewController != null)
				return dataViewController;

			// Create a new view controller and pass suitable data.
			dataViewController = (DataViewController)storyboard.InstantiateViewController ("DataViewController");
			dataViewController.RegionCode = regionCodes [index];

			// Cache the view controller before returning it.
			cachedDataViewControllers.SetObjectforKey (dataViewController, (NSString)regionCode);
			return dataViewController;
		}

		int IndexOfViewController (DataViewController viewController)
		{
			var regionCode = viewController.RegionCode;
			if (string.IsNullOrWhiteSpace (regionCode))
				return -1;

			return regionCodes.IndexOf (regionCode);
		}
	}
}
