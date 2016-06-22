using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class ErrorViewController : ResultOrErrorViewController
	{
		[Outlet]
		public UILabel ErrorCode { get; set; }

		[Outlet]
		public UITextView ErrorText { get; set; }

		public NSError Error { get; set; }

		public ErrorViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var error = Error;
			if (error == null) {
				ErrorCode.Text = "An unexpected error occurred.";
				return;
			}
			ErrorCode.Text = $"Error Code: {error.Code}";
			ErrorText.Text = error.LocalizedDescription;

			ErrorText.TextContainer.LineFragmentPadding = 0;
			ErrorText.TextContainerInset = UIEdgeInsets.Zero;
		}
	}
}
