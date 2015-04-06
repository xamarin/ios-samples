using System;

using UIKit;

namespace PrivacyPrompts
{
	public partial class PrivacyDetailViewController : UIViewController
	{
		// Dependency Injection via property
		public IPrivacyManager PrivacyManager { get; set; }

		public PrivacyDetailViewController(IntPtr handle)
			: base(handle)
		{
		}

		public PrivacyDetailViewController()
		{
			throw new InvalidProgramException ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			requestBtn.TouchUpInside += RequestAccessButtonClicked;

			titleLbl.Text = Title;
			accessStatus.Text = "Indeterminate";
			requestBtn.SetTitle ("Request access", UIControlState.Normal);

			accessStatus.Text = PrivacyManager.CheckAccess ();
		}

		async void RequestAccessButtonClicked (object sender, EventArgs e)
		{
			await PrivacyManager.RequestAccess ();
			accessStatus.Text = PrivacyManager.CheckAccess ();
		}
	}
}