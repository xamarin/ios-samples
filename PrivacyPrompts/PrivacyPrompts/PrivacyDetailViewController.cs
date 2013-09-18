using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PrivacyPrompts {

	public partial class PrivacyDetailViewController : UIViewController	{

		public Action CheckAccess;
		public Action RequestAccess;

		public PrivacyDetailViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			titleLabel.Text = Title;
			if (CheckAccess == null)
				checkAccessButton.Hidden = true;
			if (RequestAccess == null)
				requestAccessButton.Hidden = true;
		}

		partial void tappedCheckAccessButton (NSObject sender)
		{
			if (CheckAccess != null)
				CheckAccess ();
		}

		partial void tappedRequestAccessButton (NSObject sender)
		{
			if (RequestAccess != null)
				RequestAccess ();
		}
	}
}