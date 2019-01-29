using System;
using UIKit;

namespace NavigationBar
{
    /// <summary>
    /// UINavigationController subclass used for targeting appearance proxy changes in the Custom Back Button example.
    /// </summary>
    public partial class CustomBackButtonNavController : UINavigationController
    {
        public CustomBackButtonNavController(IntPtr handle) : base(handle) { }
    }
}