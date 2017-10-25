using Foundation;
using System;
using UIKit;

namespace largetitles
{
    public partial class DetailViewController : UIViewController
    {
        public void SetTitle (string title)
        {
            Title = title;
            LargeLabel.Text = $"The title \"{title}\" is {title.Length} characters long.";
        }

        public DetailViewController (IntPtr handle) : base (handle)
        {
            LargeLabel = new UILabel();
            LargeLabel.Frame = new CoreGraphics.CGRect(10, 100, View.Bounds.Width - 20, View.Bounds.Height / 2);
        }

        public override void ViewDidLoad()
        {
            NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Never;
        }
    }
}