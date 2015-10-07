using System;

using UIKit;
using Foundation;

namespace Emporium
{
	[Register ("ConfirmationViewController")]
	public class ConfirmationViewController : UIViewController
	{
		public string TransactionIdentifier { get; set; }

		[Outlet ("confirmationLabel")]
		public UILabel confirmationLabel { get; set; }

		public ConfirmationViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			confirmationLabel.Text = TransactionIdentifier;
		}
	}
}