using System;
using UIKit;

namespace NavigationBar
{
    /// <summary>
    /// The detail view controller in the Custom Back Button example.
    /// </summary>
    public partial class CustomBackButtonDetailViewController : UIViewController
    {
        public CustomBackButtonDetailViewController(IntPtr handle) : base(handle) { }

        public string City { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.cityLabel.Text = this.City;
        }
    }
}