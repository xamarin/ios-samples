using System;
using CoreMotion;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using CoreGraphics;

namespace PrivacyPrompts
{
	/*
	Note: Accessing motion activity requires your project to have an entitlements.plist file

	There is no API that allows you to directly check for access. Instead, you have to use
	the technique illustrated here: perform a query and check for an error of type 
	CMError.MotionActivityNotAuthorized

	 */
	public class MotionPrivacyController : PrivacyDetailViewController
	{
		CMStepCounter stepCounter;
		string motionStatus;
		UILabel stepsMessage;

		public MotionPrivacyController () : base(null, null)
		{
			this.CheckAccess = CheckMotionAccess;
			this.RequestAccess = RequestMotionAccess;
			motionStatus = "Indeterminate";
		}

		void AddStepsMessage()
		{
			stepsMessage = new UILabel (CGRect.Empty);
			stepsMessage.TranslatesAutoresizingMaskIntoConstraints = false;
			stepsMessage.Lines = 0;
			stepsMessage.Font = UIFont.SystemFontOfSize (UIFont.SmallSystemFontSize);

			this.View.AddSubview (stepsMessage);

			stepsMessage.AddConstraints (new [] {
				NSLayoutConstraint.Create (stepsMessage, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 56),
				NSLayoutConstraint.Create (stepsMessage, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.Width, 1, 300),
			});

			this.View.AddConstraints (new[] {
				NSLayoutConstraint.Create (stepsMessage, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, titleLabel, NSLayoutAttribute.CenterX, 1, 0),
				NSLayoutConstraint.Create (stepsMessage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, requestAccessButton, NSLayoutAttribute.Bottom, 1, 40),
			});
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			AddStepsMessage ();

			var cm = new CMMotionManager ();
			if (cm.DeviceMotionAvailable == false) {
				motionStatus = "Not available";
				requestAccessButton.Enabled = false;
			}
			accessStatus.Text = motionStatus;
		}


		string CheckMotionAccess()
		{
			return motionStatus;
		}

		void RequestMotionAccess()
		{
			stepCounter = new CMStepCounter ();
			stepCounter.QueryStepCount(
				NSDate.FromTimeIntervalSinceNow(-60 * 60 * 24),
				NSDate.Now,
				NSOperationQueue.MainQueue, ((steps, error) => {
					if(error != null && error.Code == (int) CMError.MotionActivityNotAuthorized)
					{
						motionStatus = "Not Authorized";
						InvokeOnMainThread(() => accessStatus.Text = motionStatus);
					}
					else
					{
						motionStatus = "Available";
						var stepMsg = String.Format("You have taken {0} steps in the past 24 hours", steps);
						InvokeOnMainThread(() => {
							stepsMessage.Text = stepMsg;
							accessStatus.Text = motionStatus;
						});
					}
				}));
		}


	}
}

