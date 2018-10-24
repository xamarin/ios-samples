using Foundation;
using System;
using UIKit;

namespace ZoomingPdfViewer
{
    /// <summary>
    /// This view controller manages the display of a set of view controllers by way of implementing the UIPageViewControllerDelegate protocol.
    /// </summary>
    public partial class RootViewController : UIViewController, IUIPageViewControllerDelegate
    {
        private UIPageViewController pageViewController;
        private ModelController modelController;

        public RootViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // insantiate our ModelController
            modelController = new ModelController();

            // Do any additional setup after loading the view, typically from a nib.
            // Configure the page view controller and add it as a child view controller.

            pageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.PageCurl,
                                                          UIPageViewControllerNavigationOrientation.Horizontal,
                                                          new NSDictionary());
            pageViewController.Delegate = this;

            var startingViewController = modelController.GetViewController(0, Storyboard);
            pageViewController.SetViewControllers(new[] { startingViewController },
                                                  UIPageViewControllerNavigationDirection.Forward,
                                                  false, 
                                                  null);

            pageViewController.DataSource = modelController;

            AddChildViewController(pageViewController);
            View.AddSubview(pageViewController.View);

            // Set the page view controller's bounds using an inset rect so that self's view is visible around the edges of the pages.
            var pageViewRect = View.Bounds;
            pageViewController.View.Frame = pageViewRect;
            pageViewController.DidMoveToParentViewController(this);

            // Add the page view controller's gesture recognizers to the book view controller's view so that the gestures are started more easily.
            View.GestureRecognizers = pageViewController.GestureRecognizers;
        }

        [Export("pageViewController:spineLocationForInterfaceOrientation:")]
        public UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
        {
            var result = UIPageViewControllerSpineLocation.None;
            if (orientation == UIInterfaceOrientation.Portrait || UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                // In portrait orientation or on iPhone: Set the spine position to "min" and the page view controller's view controllers array to contain just one view controller.
                // Setting the spine position to 'UIPageViewControllerSpineLocationMid' in landscape orientation sets the doubleSided property to YES, so set it to NO here.
                var currentViewController = pageViewController.ViewControllers[0];
                pageViewController.SetViewControllers(new[] { currentViewController },
                                                      UIPageViewControllerNavigationDirection.Forward,
                                                      true,
                                                      null);

                pageViewController.DoubleSided = false;
                result = UIPageViewControllerSpineLocation.Min;
            }
            else
            {
                // In landscape orientation: Set set the spine location to "mid" and the page view controller's view controllers array to contain two view controllers. 
                // If the current page is even, set it to contain the current and next view controllers; if it is odd, set the array to contain the previous and current view controllers.
                var dataViewController = pageViewController.ViewControllers[0] as DataViewController;
                var indexOfCurrentViewController = modelController.IndexOfViewController(dataViewController);
                UIViewController[] viewControllers = null;

                if (indexOfCurrentViewController == 0 || indexOfCurrentViewController % 2 == 0)
                {
                    var nextViewController = modelController.GetNextViewController(pageViewController, dataViewController);
                    viewControllers = new[] { dataViewController, nextViewController };
                }
                else
                {
                    var previousViewController = modelController.GetPreviousViewController(pageViewController, dataViewController);
                    viewControllers = new[] { dataViewController, previousViewController };
                }

                pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
                result = UIPageViewControllerSpineLocation.Mid;
            }

            return result;
        }
    }
}