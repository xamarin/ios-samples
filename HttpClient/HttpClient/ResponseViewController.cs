using HttpClient.Core;
using System;
using System.IO;
using UIKit;

namespace HttpClient {
	public partial class ResponseViewController : UIViewController {
		public ResponseViewController (IntPtr handle) : base (handle) { }

		public NetworkProvider Provider { get; set; }

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (Provider.GetType () == typeof (NetHttpProvider)) {
				TitleLabel.Text = $"HttpClient is using {NetHttpProvider.GetHandlerType ().Name}";
				TitleLabel.Hidden = false;
			}

			// execute request
			using (var stream = await Provider.ExecuteAsync ()) {
				using (var streamReader = new StreamReader (stream)) {
					ResponseTextView.Text = await streamReader.ReadToEndAsync ();
				}
			}
		}
	}
}
