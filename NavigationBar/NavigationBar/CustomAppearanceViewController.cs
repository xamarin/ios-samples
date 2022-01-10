using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace NavigationBar
{
    /// <summary>
    /// Demonstrates applying a custom background to a navigation bar.
    /// </summary>
    public partial class CustomAppearanceViewController : UITableViewController
    {
        // Our data source is an array of city names, populated from Cities.json.
        private CitiesDataSource dataSource = new CitiesDataSource();

        public CustomAppearanceViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            base.TableView.DataSource = dataSource;

            // Place the background switcher in the toolbar.
            var backgroundSwitcherItem = new UIBarButtonItem(backgroundSwitcher);
            base.ToolbarItems = new UIBarButtonItem[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null, null),
                backgroundSwitcherItem,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null, null),
            };

            this.ApplyImageBackgroundToTheNavigationBar();
        }

        /// <summary>
        /// Configures the navigation bar to use an image as its background.
        /// </summary>
        private void ApplyImageBackgroundToTheNavigationBar()
        {
            // These background images contain a small pattern which is displayed
            // in the lower right corner of the navigation bar.
            var backgroundImageForDefaultBarMetrics = UIImage.FromBundle("NavigationBarDefault");
            var backgroundImageForLandscapePhoneBarMetrics = UIImage.FromBundle("NavigationBarLandscapePhone");

            // Both of the above images are smaller than the navigation bar's
            // size.  To enable the images to resize gracefully while keeping their
            // content pinned to the bottom right corner of the bar, the images are
            // converted into resizable images width edge insets extending from the
            // bottom up to the second row of pixels from the top, and from the
            // right over to the second column of pixels from the left.  This results
            // in the topmost and leftmost pixels being stretched when the images
            // are resized.  Not coincidentally, the pixels in these rows/columns
            // are empty.

            backgroundImageForDefaultBarMetrics =
                backgroundImageForDefaultBarMetrics.CreateResizableImage(new UIEdgeInsets(0f,
                                                                                          0f,
                                                                                          backgroundImageForDefaultBarMetrics.Size.Height - 1f,
                                                                                          backgroundImageForDefaultBarMetrics.Size.Width - 1f));

            backgroundImageForLandscapePhoneBarMetrics =
                backgroundImageForLandscapePhoneBarMetrics.CreateResizableImage(new UIEdgeInsets(0f,
                                                                                                 0f,
                                                                                                 backgroundImageForLandscapePhoneBarMetrics.Size.Height - 1f,
                                                                                                 backgroundImageForLandscapePhoneBarMetrics.Size.Width - 1f));

            // You should use the appearance proxy to customize the appearance of
            // UIKit elements.  However changes made to an element's appearance
            // proxy do not effect any existing instances of that element currently
            // in the view hierarchy.  Normally this is not an issue because you
            // will likely be performing your appearance customizations in
            // -application:didFinishLaunchingWithOptions:.  However, this example
            // allows you to toggle between appearances at runtime which necessitates
            // applying appearance customizations directly to the navigation bar.
            var navigationBarAppearance = base.NavigationController.NavigationBar;

            // The bar metrics associated with a background image determine when it
            // is used.  The background image associated with the Default bar metrics
            // is used when a more suitable background image can not be found.
            navigationBarAppearance.SetBackgroundImage(backgroundImageForDefaultBarMetrics, UIBarMetrics.Default);
            // The background image associated with the LandscapePhone bar metrics
            // is used by the shorter variant of the navigation bar that is used on
            // iPhone when in landscape.
            navigationBarAppearance.SetBackgroundImage(backgroundImageForLandscapePhoneBarMetrics, UIBarMetrics.Compact);
        }

        /// <summary>
        /// Configures the navigation bar to use a transparent background (see-through but without any blur).
        /// </summary>
        private void ApplyTransparentBackgroundToTheNavigationBar(float opacity)
        {
            UIImage transparentBackground = null;

            // The background of a navigation bar switches from being translucent
            // to transparent when a background image is applied.  The intensity of
            // the background image's alpha channel is inversely related to the
            // transparency of the bar.  That is, a smaller alpha channel intensity
            // results in a more transparent bar and vis-versa.
            //
            // Below, a background image is dynamically generated with the desired
            // opacity.
            UIGraphics.BeginImageContextWithOptions(new CGSize(1f, 1f),
                                                    false,
                                                    base.NavigationController.NavigationBar.Layer.ContentsScale);

            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(1f, 1f, 1f, opacity);
            UIGraphics.RectFill(new CGRect(0f, 0f, 1f, 1f));
            transparentBackground = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // You should use the appearance proxy to customize the appearance of
            // UIKit elements.  However changes made to an element's appearance
            // proxy do not effect any existing instances of that element currently
            // in the view hierarchy.  Normally this is not an issue because you
            // will likely be performing your appearance customizations in
            // -application:didFinishLaunchingWithOptions:.  However, this example
            // allows you to toggle between appearances at runtime which necessitates
            // applying appearance customizations directly to the navigation bar.
            /* let navigationBarAppearance = UINavigationBar.appearance(whenContainedInInstancesOf: [UINavigationController.self]) */
            var navigationBarAppearance = base.NavigationController.NavigationBar;

            navigationBarAppearance.SetBackgroundImage(transparentBackground, UIBarMetrics.Default);
        }

        /// <summary>
        /// Configures the navigation bar to use a custom color as its background. The navigation bar remains translucent.
        /// </summary>
        private void ApplyBarTintColorToTheNavigationBar()
        {
            // Be aware when selecting a barTintColor for a translucent bar that
            // the tint color will be blended with the content passing under
            // the translucent bar.  See QA1808 for more information.
            // <https://developer.apple.com/library/ios/qa/qa1808/_index.html>
            var barTintColor = UIColor.FromRGBA(176f / 255f, 226f / 255f, 172f / 255f, 1f);
            var darkendBarTintColor = UIColor.FromRGBA(176f / 255f - 0.05f, 226f / 255f - 0.02f, 172f / 255f - 0.05f, 1f);

            // You should use the appearance proxy to customize the appearance of
            // UIKit elements.  However changes made to an element's appearance
            // proxy do not effect any existing instances of that element currently
            // in the view hierarchy.  Normally this is not an issue because you
            // will likely be performing your appearance customizations in
            // -application:didFinishLaunchingWithOptions:.  However, this example
            // allows you to toggle between appearances at runtime which necessitates
            // applying appearance customizations directly to the navigation bar.
            /* let navigationBarAppearance = UINavigationBar.appearance(whenContainedInInstancesOf: [UINavigationController.self]) */
            var navigationBarAppearance = base.NavigationController.NavigationBar;

            navigationBarAppearance.BarTintColor = darkendBarTintColor;

            // For comparison, apply the same barTintColor to the toolbar, which
            // has been configured to be opaque.
            base.NavigationController.Toolbar.BarTintColor = barTintColor;
            base.NavigationController.Toolbar.Translucent = false;
        }

        #region UIContentContainer

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            // This works around a bug in iOS 8.0 - 8.2 in which the navigation bar
            // will not display the correct background image after rotating the device.
            // This bug affects bars in navigation controllers that are presented
            // modally. A bar in the window's rootViewController would not be affected.
            coordinator.AnimateAlongsideTransition((_) =>
            {
                // The workaround is to toggle some appearance related setting on the
                // navigation bar when we detect that the view controller has changed
                // interface orientations.  In our example, we call the
                // -configureNewNavBarBackground: which reapplies our appearance
                // based on the current selection.  In a real app, changing just the
                // barTintColor or barStyle would have the same effect.
                this.ConfigureNewNavigationBarBackground(this.backgroundSwitcher);
            }, null);
        }

        #endregion

        #region Background Switcher

        partial void ConfigureNewNavigationBarBackground(UISegmentedControl sender)
        {
            // Reset everything.
            base.NavigationController.NavigationBar.SetBackgroundImage(null, UIBarMetrics.Default);
            base.NavigationController.NavigationBar.SetBackgroundImage(null, UIBarMetrics.Compact);
            base.NavigationController.NavigationBar.BarTintColor = null;
            base.NavigationController.Toolbar.BarTintColor = null;
            base.NavigationController.Toolbar.Translucent = true;

            switch (sender.SelectedSegment)
            {
                case 0: // Transparent Background
                    this.ApplyImageBackgroundToTheNavigationBar();
                    break;

                case 1: // Transparent
                    this.ApplyTransparentBackgroundToTheNavigationBar(0.87f);
                    break;

                case 2: // Colored
                    this.ApplyBarTintColorToTheNavigationBar();
                    break;
            }
        }

        #endregion

        #region UITableViewDelegate

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var selected = this.dataSource[indexPath.Row];
            if (base.NavigationItem.Prompt == selected)
            {
                base.NavigationItem.Prompt = null;
                tableView.DeselectRow(indexPath, true);
            }
            else
            {
                base.NavigationItem.Prompt = this.dataSource[indexPath.Row];
            }
        }

        #endregion 

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.dataSource.Dispose();
            this.dataSource = null;
        }
    }
}