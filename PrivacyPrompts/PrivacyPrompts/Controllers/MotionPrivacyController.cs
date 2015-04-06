using System;

using UIKit;

namespace PrivacyPrompts
{
	/// <summary>
	/// Note: Accessing motion activity requires your project to have an entitlements.plist file
	/// There is no API that allows you to directly check for access. Instead, you have to use
	/// the technique illustrated here: perform a query and check for an error of type
	/// CMError.MotionActivityNotAuthorized
	/// </summary>
	public partial class MotionPrivacyController : UIViewController
	{
		public MotionPrivacyManager PrivacyManager { get; set; }

		public MotionPrivacyController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			requestBtn.TouchUpInside += RequestAccessButtonClicked;

			titleLbl.Text = Title;
			accessStatus.Text = "Indeterminate";
			requestBtn.SetTitle ("Request access", UIControlState.Normal);

			accessStatus.Text = PrivacyManager.CheckAccess ();
			StepsLbl.Text = string.Empty;
		}

		async void RequestAccessButtonClicked (object sender, EventArgs e)
		{
			await PrivacyManager.RequestAccess ();
			accessStatus.Text = PrivacyManager.CheckAccess ();
			StepsLbl.Text = PrivacyManager.GetCountsInfo ();
		}
	}
}