using System;
using UIKit;

namespace UICatalog
{
    public partial class ActivityIndicatorViewController : UITableViewController
    {
        public ActivityIndicatorViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.tintedActivityIndicatorView.Color = UIColor.Purple;
        }
    }
}