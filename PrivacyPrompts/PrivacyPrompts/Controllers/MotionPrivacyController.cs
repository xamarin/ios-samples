using System;
using CoreMotion;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using CoreGraphics;

namespace PrivacyPrompts
{
	/// <summary>
	/// Note: Accessing motion activity requires your project to have an entitlements.plist file
	/// There is no API that allows you to directly check for access. Instead, you have to use
	/// the technique illustrated here: perform a query and check for an error of type
	/// CMError.MotionActivityNotAuthorized
	/// </summary>
	[Register("MotionPrivacyController")]
	public partial class MotionPrivacyController : PrivacyDetailViewController
	{
		CMStepCounter stepCounter;
		string motionStatus;

		public MotionPrivacyController (IntPtr handle)
			: base (handle)
		{
			
		}

		public MotionPrivacyController ()
		{
			motionStatus = "Indeterminate";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			StepsLbl.Text = "102 steps";

			/*
			var cm = new CMMotionManager ();
			if (cm.DeviceMotionAvailable == false) {
				motionStatus = "Not available";
				RequestAccessButton.Enabled = false;
			}
			AccessStatus.Text = motionStatus;
			*/
		}

		/*
		protected override string CheckAccess ()
		{
			return motionStatus;
		}

		protected override void RequestAccess ()
		{
			stepCounter = new CMStepCounter ();
			stepCounter.QueryStepCount(
				NSDate.FromTimeIntervalSinceNow(-60 * 60 * 24),
				NSDate.Now,
				NSOperationQueue.MainQueue, ((steps, error) => {
					if(error != null && error.Code == (int) CMError.MotionActivityNotAuthorized)
					{
						motionStatus = "Not Authorized";
						UpdateStatus();
					}
					else
					{
						motionStatus = "Available";
						var stepMsg = String.Format("You have taken {0} steps in the past 24 hours", steps);
						InvokeOnMainThread(() => {
							stepsMessage.Text = stepMsg;
							UpdateStatus();
						});
					}
				}));
		}
		*/

	}
}

