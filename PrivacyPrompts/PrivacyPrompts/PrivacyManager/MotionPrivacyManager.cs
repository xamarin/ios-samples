using System;
using System.Threading.Tasks;

using CoreMotion;
using Foundation;

namespace PrivacyPrompts
{
	public class MotionPrivacyManager : IPrivacyManager, IDisposable
	{
		CMStepCounter stepCounter;
		string motionStatus = "Indeterminate";
		nint steps = 0;

		CMMotionManager motionManger = new CMMotionManager ();

		bool IsMotionAvailable {
			get {
				return motionManger.DeviceMotionAvailable;
			}
		}

		public MotionPrivacyManager ()
		{
			stepCounter = new CMStepCounter ();

			if (!IsMotionAvailable)
				motionStatus = "Not available";
		}

		public Task RequestAccess ()
		{
			if (!IsMotionAvailable)
				return Task.FromResult<object> (null);

			var yesterday = NSDate.FromTimeIntervalSinceNow (-60 * 60 * 24);

			return stepCounter.QueryStepCountAsync (yesterday, NSDate.Now, NSOperationQueue.MainQueue)
				.ContinueWith (StepQueryContinuation);
		}

		void StepQueryContinuation(Task<nint> t)
		{
			if (t.IsFaulted) {
				var code = ((NSErrorException)t.Exception.InnerException).Code;
				motionStatus = GetStatusForErrorCode (code);
				return;
			}

			motionStatus = "Available";
			steps = t.Result;
		}

		string GetStatusForErrorCode(nint code)
		{
			if (code == (int)CMError.MotionActivityNotAuthorized)
				return "Not Authorized";
			else
				return "Available";
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