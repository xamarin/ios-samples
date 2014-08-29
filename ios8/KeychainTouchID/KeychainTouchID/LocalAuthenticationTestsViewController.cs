using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.LocalAuthentication;
using MonoTouch.UIKit;

namespace KeychainTouchID
{
	public partial class LocalAuthenticationTestsViewController : BasicTestViewController
	{
		public LocalAuthenticationTestsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Tests = new List<Test> {
				new Test { Name = Text.TOUCH_ID_PREFLIGHT, Details = Text.USING_CAN_EVALUATE_POLICY, Method = CanEvaluatePolicy },
				new Test { Name = Text.TOUCH_ID, Details = Text.USING_EVALUATE_POLICY, Method = EvaluatePolicy }
			};

			tableView.WeakDataSource = this;
			tableView.WeakDelegate = this;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			textView.ScrollRangeToVisible (new NSRange (0, textView.Text.Length));
		}

		public override void ViewDidLayoutSubviews ()
		{
			float height = Math.Min (View.Bounds.Size.Height, tableView.ContentSize.Height);
			dynamicViewHeight.Constant = height;
			View.LayoutIfNeeded ();
		}

		private void CanEvaluatePolicy ()
		{
			var context = new LAContext ();
			string message = string.Empty;
			NSError error;
			bool success = context.CanEvaluatePolicy (LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error);
			message = success ? Text.TOUCH_ID_IS_AVAILABLE : Text.TOUCH_ID_IS_NOT_AVAILABLE;

			PrintResult (textView, message);
		}

		private void EvaluatePolicy ()
		{
			var context = new LAContext ();
			context.EvaluatePolicy (LAPolicy.DeviceOwnerAuthenticationWithBiometrics, Text.UNLOCK_ACCESS_TO_LOCKED_FATURE, HandleLAContextReplyHandler);
		}

		private void HandleLAContextReplyHandler (bool success, NSError error)
		{
			string message = success ? Text.EVALUATE_POLICY_SUCCESS : string.Format (Text.EVALUATE_POLICY_WITH_ERROR, error.LocalizedDescription);
			PrintResult (textView, message);
		}
	}
}
