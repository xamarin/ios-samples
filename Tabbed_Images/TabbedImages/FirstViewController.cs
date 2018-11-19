using System;
using UIKit;

namespace TabbedImages
{
    public partial class FirstViewController : UIViewController
    {
        public FirstViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.TabBarItem.Image = UIImage.FromBundle("first");
        }
    }
}