using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace SearchDemo
{
    public partial class SearchItemViewController : UIViewController
    {
        public SearchItem Item { get; set; }

        public SearchItemViewController () : base ("SearchItemViewController", null)
        {
        }
        
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            webView.LoadStarted += (sender, e) => {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
            };

            webView.LoadFinished += (sender, e) => {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            };

            webView.LoadRequest (new NSUrlRequest (new NSUrl (Item.Url)));
        }
    }
}

