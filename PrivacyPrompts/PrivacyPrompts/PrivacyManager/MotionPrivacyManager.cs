using System;
using System.Threading.Tasks;

using CoreMotion;
using Foundation;
using UIKit;

namespace PrivacyPrompts
{
	public class MotionPrivacyManager : IPrivacyManager, IDisposable
	{
		CMStepCounter stepCounter;
		string motionStatus = "Indeterminate";
		nint steps = 0;

		CMMotionManager motionManger; // before iOS 8.0
		CMPedometer pedometer; // since iOS 8.0

		public MotionPrivacyManager ()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				pedometer = new CMPedometer ();
				motionStatus = CMPedometer.IsStepCountingAvailable ? "Available" : "Not available";
			} else {
				stepCounter = new CMStepCounter ();
				motionManger = new CMMotionManager ();
				motionStatus = motionManger.DeviceMotionAvailable ? "Available" : "Not available";
			}
		}

		public Task RequestAccess ()
		{
			var yesterday = NSDate.FromTimeIntervalSinceNow (-60 * 60 * 24);

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				if(!CMPedometer.IsStepCountingAvailable)
					return Task.FromResult<object> (null);

				return pedometer.QueryPedometerDataAsync (yesterday, NSDate.Now)
					.ContinueWith (PedometrQueryContinuation);
			} else {
				if (!motionManger.DeviceMotionAvailable)
					return Task.FromResult<object> (null);

				return stepCounter.QueryStepCountAsync (yesterday, NSDate.Now, NSOperationQueue.MainQueue)
					.ContinueWith (StepQueryContinuation);
			}

		}

		void PedometrQueryContinuation(Task<CMPedometerData> t)
		{
			if (t.IsFaulted) {
				var code = ((NSErrorException)t.Exception.InnerException).Code;
				if (code == (int)CMError.MotionActivityNotAuthorized)
					motionStatus = "Not Authorized";
				return;
			}

			steps = t.Result.NumberOfSteps.NIntValue;
		}

		void StepQueryContinuation(Task<nint> t)
		{
			if (t.IsFaulted) {
				var code = ((NSErrorException)t.Exception.InnerException).Code;
				if (code == (int)CMError.MotionActivityNotAuthorized)
					motionStatus = "Not Authorized";
				return;
			}

			steps = t.Result;
		}

		public string CheckAccess ()
		{
			return motionStatus;
		}

		public string GetCountsInfo()
		{
			return steps > 0 ? string.Format ("You have taken {0} steps in the past 24 hours", steps) : string.Empty;
		}

		public void Dispose ()
		{
			motionManger.Dispose ();
			stepCounter.Dispose ();
		}
	}
}