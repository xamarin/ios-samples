using System;
using Foundation;
using UIKit;

namespace NavigationBar
{
    /// <summary>
    /// Demonstrates using a custom back button image with no chevron and no text.
    /// </summary>
    public partial class CustomBackButtonViewController : UITableViewController
    {
        // Our data source is an array of city names, populated from Cities.json.
        private CitiesDataSource dataSource = new CitiesDataSource();

        public CustomBackButtonViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            base.TableView.DataSource = this.dataSource;

            // Note that images configured as the back bar button's background do
            // not have the current tintColor applied to them, they are displayed as it.
            var backButtonBackgroundImage = UIImage.FromBundle("Menu");
            // The background should be pinned to the left and not stretch.
            backButtonBackgroundImage = backButtonBackgroundImage.CreateResizableImage(new UIEdgeInsets(0f, backButtonBackgroundImage.Size.Width - 1f, 0f, 0f));

            var barAppearance = UINavigationBar.AppearanceWhenContainedIn(typeof(CustomBackButtonNavController));
            barAppearance.BackIndicatorImage = backButtonBackgroundImage;
            barAppearance.BackIndicatorTransitionMaskImage = backButtonBackgroundImage;

            // Provide an empty backBarButton to hide the 'Back' text present by
            // default in the back button.
            //
            // NOTE: You do not need to provide a target or action.  These are set
            //       by the navigation bar.
            // NOTE: Setting the title of this bar button item to ' ' (space) works
            //       around a bug in iOS 7.0.x where the background image would be
            //       horizontally compressed if the back button title is empty.
            var backBarButton = new UIBarButtonItem(" ", UIBarButtonItemStyle.Plain, null);
            base.NavigationItem.BackBarButtonItem = backBarButton;
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "DetailSegue")
            {
                var city = this.dataSource[base.TableView.IndexPathForSelectedRow.Row];
                if (segue.DestinationViewController is CustomBackButtonDetailViewController customBackButtonDetailViewController)
                {
                    customBackButtonDetailViewController.City = city;
                }
            }
        }
    }
}