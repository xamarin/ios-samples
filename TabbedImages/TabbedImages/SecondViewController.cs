using System;
using UIKit;

namespace TabbedImages
{
    public partial class SecondViewController : UIViewController
    {
        public SecondViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.TabBarItem.Image = UIImage.FromFile("Images/second.png");
        }
    }
}