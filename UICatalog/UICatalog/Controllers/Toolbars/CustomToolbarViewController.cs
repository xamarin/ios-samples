using System;
using UIKit;

namespace UICatalog
{
    public partial class CustomToolbarViewController : UIViewController
    {
        public CustomToolbarViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ConfigureToolbar();
        }

        private void ConfigureToolbar()
        {
            toolbar.SetBackgroundImage(UIImage.FromBundle("toolbar_background"), UIToolbarPosition.Bottom, UIBarMetrics.Default);

            // add buttons 

            var leftButton = new UIBarButtonItem(UIImage.FromBundle("tools_icon"), UIBarButtonItemStyle.Plain, OnBarButtonItemClicked);
            leftButton.TintColor = UIColor.Purple;

            var rightButton = new UIBarButtonItem("Button", UIBarButtonItemStyle.Plain, OnBarButtonItemClicked);
            rightButton.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.Purple }, UIControlState.Normal);

            var toolbarButtonItems = new[]
            {
                leftButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, (EventHandler) null),
                rightButton
            };
            toolbar.SetItems(toolbarButtonItems, true);
        }

        private void OnBarButtonItemClicked(object sender, EventArgs e)
        {
            Console.WriteLine("A bar button item was clicked");
        }
    }
}