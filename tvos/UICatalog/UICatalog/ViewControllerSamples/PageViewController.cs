using System;
using System.Linq;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class PageViewController : UIPageViewController, IUIPageViewControllerDataSource {
		readonly DataItem[] dataItems = DataItem.SampleItems.Where (c => c.Group == Group.Lola).ToArray ();
		readonly NSCache dataItemViewControllerCache = new NSCache ();

		[Export ("initWithCoder:")]
		public PageViewController (NSCoder coder): base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			DataSource = this;
			var initialViewController = DataItemViewControllerForPage (0);
			SetViewControllers (
				new [] { initialViewController },
				UIPageViewControllerNavigationDirection.Forward, false, null
			);
		}

		[Export ("presentationCountForPageViewController:")]
		public new nint GetPresentationCount (UIPageViewController pageViewController)
		{
			return (nint)dataItems.Length;
		}

		[Export ("presentationIndexForPageViewController:")]
		public new nint GetPresentationIndex (UIPageViewController pageViewController)
		{
			var firstViewController = pageViewController.ViewControllers.FirstOrDefault () as DataItemViewController;

			if (firstViewController == null)
				throw new Exception ("Unexpected view controller type in page view controller.");

			for (int i = 0; i < dataItems.Length; i++) {
				if (firstViewController.DataItem == dataItems[i])
					return i;
			}

			throw new Exception ("View controller's data item not found.");
		}

		public new UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var index = IndexOfDataItemForViewController (referenceViewController);
			return index > 0 ? DataItemViewControllerForPage (index - 1) : null;
		}

		public new UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var index = IndexOfDataItemForViewController (referenceViewController);
			return index < dataItems.Length - 1 ? DataItemViewControllerForPage (index + 1) : null;
		}

		int IndexOfDataItemForViewController (UIViewController viewController)
		{
			var dataItemViewController = viewController as DataItemViewController;
			if (dataItemViewController == null)
				throw new Exception ("Unexpected view controller type in page view controller");

			for (int i = 0; i < dataItems.Length; i++) {
				if (dataItemViewController.DataItem == dataItems[i])
					return i;
			}

			throw new Exception ("View controller's data item not found.");
		}

		DataItemViewController DataItemViewControllerForPage (int index)
		{
			var dataItem = dataItems [index];
			var cachedController = dataItemViewControllerCache.ObjectForKey ((NSString)dataItem.Identifier) as DataItemViewController;

			if (cachedController == null) {
				var viewController = (DataItemViewController)Storyboard.InstantiateViewController (DataItemViewController.StoryboardIdentifier);
				if (viewController == null)
					throw new Exception ("Unable to instantiate a DataItemViewController.");

				viewController.ConfigureWithDataItem (dataItem);
				dataItemViewControllerCache.SetObjectforKey (viewController, (NSString)dataItem.Identifier);
				return viewController;
			} else {
				return cachedController;
			}
		}
	}
}
