using System;
using UIKit;

namespace MapCallouts
{
    /// <summary>
    /// The detail view controller used for displaying the Golden Gate Bridge either in a popover for iPad, or in a modal view controller for iPhone.
    /// </summary>
    public partial class DetailViewController : UIViewController
    {
        public DetailViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (this.imageView.Image != null)
            {
                this.PreferredContentSize = this.imageView.Image.Size;
            }
        }

        partial void DoneAction(UIBarButtonItem sender)
        {
            base.DismissViewController(true, null);
        }
    }
}