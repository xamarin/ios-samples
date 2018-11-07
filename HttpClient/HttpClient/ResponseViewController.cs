using Foundation;
using HttpClient.Core;
using System;
using System.IO;
using UIKit;

namespace HttpClient
{
    public partial class ResponseViewController : UIViewController
    {
        public ResponseViewController (IntPtr handle) : base (handle) { }

        public NetworkProvider Provider { get; set; }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            // execute request
            using (var stream = await Provider.ExecuteAsync())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    ResponseTextView.Text = await streamReader.ReadToEndAsync();
                }
            }
        }
    }
}