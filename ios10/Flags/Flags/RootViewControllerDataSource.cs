using System;

using UIKit;
using Foundation;

namespace Flags {
	public class RootViewControllerDataSource : UIPageViewControllerDataSource {
		readonly string [] regionCodes;
		readonly UIStoryboard storyboard;

		NSCache cachedDataViewControllers = new NSCache ();

		public int NumberOfFlags {
			get {
				return regionCodes.Length;
			}
		}

		public RootViewControllerDataSource (UIStoryboard storyboard)
		{
			this.storyboard = storyboard;

			regionCodes = NSLocale.ISOCountryCodes;
			regionCodes.Shuffle (new Random ());
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
			if (index >= regionCodes.Length - 1)
				return null;

			index += 1;
			return GetViewControllerAt (index);
		}

		public DataViewController GetViewControllerAt (int index)
		{
			var len = regionCodes.Length;
			if (len == 0 || index >= len)
				return null;

			var regionCode = regionCodes [index];

			// Return the data view controller for the given index.
			var dataViewController = (DataViewController) cachedDataViewControllers.ObjectForKey ((NSString) regionCode);
			if (dataViewController != null)
				return dataViewController;

			// Create a new view controller and pass suitable data.
			dataViewController = (DataViewController) storyboard.InstantiateViewController ("DataViewController");
			dataViewController.RegionCode = regionCodes [index];

			// Cache the view controller before returning it.
			cachedDataViewControllers.SetObjectforKey (dataViewController, (NSString) regionCode);
			return dataViewController;
		}

		int IndexOfViewController (DataViewController viewController)
		{
			var regionCode = viewController.RegionCode;
			if (string.IsNullOrWhiteSpace (regionCode))
				return -1;

			return Array.IndexOf (regionCodes, regionCode);
		}
	}

	public static class Utils {
		// For more info https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
		public static void Shuffle<T> (this T [] array, Random rng)
		{
			int n = array.Length;
			for (int i = 0; i < array.Length; i++) {
				var k = rng.Next (i + 1); // 0 <= k <= i
				T tmp = array [k];
				array [k] = array [i];
				array [i] = tmp;
			}
		}
	}
}
