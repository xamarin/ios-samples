using System;
using Foundation;
using System.Collections.Generic;
using CoreMotion;
using CoreFoundation;
using UIKit;
using System.Threading.Tasks;
using System.Linq;

namespace MotionActivityDemo
{
	public enum ActivityType
	{
		Walking,
		Running,
		Driving,
		Moving,
		Stationary,
		None
	}

	public class ActivityDataManager : NSObject
	{
		public double WalkingDuration;
		public double RunningDuration;
		public double VehicularDuration;
		public double MovingDuration;
		public int StepCounts;

		public List<SignificantActivity> significantActivities;

		public List<SignificantActivity> SignificantActivities {
			get {
				SignificantActivity[] copy = new SignificantActivity[significantActivities.Count];
				significantActivities.CopyTo (copy);
				return copy.ToList ();
			}
			set {
				significantActivities = value;
			}
		}

		CMPedometer pedometer;
		CMMotionActivityManager motionActivityMgr;

		public ActivityDataManager ()
		{
			SignificantActivities = new List<SignificantActivity> ();
			StepCounts = 0;
			WalkingDuration = 0;
			RunningDuration = 0;
			VehicularDuration = 0;

			initMotionActivity ();
		}

		void reset ()
		{
			StepCounts = 0;
			WalkingDuration = 0;
			RunningDuration = 0;
			VehicularDuration = 0;
			SignificantActivities.Clear ();
		}

		public async Task QueryAsync (MotionActivityQuery query)
		{
			reset ();
			await queryHistoricalDataAsync (query.StartDate, query.EndDate);
		}

		async Task additionalProcessingOnAsync (CMMotionActivity[] activities)
		{
			computeTotalDurations (activities);
			SignificantActivities = (await aggregateSignificantActivitiesAsync (activities)).ToList ();
		}

		void computeTotalDurations (CMMotionActivity[] activities)
		{
			WalkingDuration = 0;
			RunningDuration = 0;
			VehicularDuration = 0;
			MovingDuration = 0;

			for (int i = 0; i < activities.Length; ++i) {
				if (i == activities.Length - 1)
					return;

				CMMotionActivity activity = activities [i];
				CMMotionActivity nextActivity = activities [i + 1];

				var duration = nextActivity.StartDate.SecondsSinceReferenceDate - activity.StartDate.SecondsSinceReferenceDate;
				if (!activity.Unknown && !activity.Stationary)
					MovingDuration += duration;
				if (activity.Walking)
					WalkingDuration += duration;
				else if (activity.Running)
					RunningDuration += duration;
				else if (activity.Automotive)
					VehicularDuration += duration;
			}
		}

		async Task<List<SignificantActivity>> aggregateSignificantActivitiesAsync (CMMotionActivity[] activities)
		{
			List<CMMotionActivity> filteredActivities = new List<CMMotionActivity> ();

			// Skip all contiguous unclassified actiivty so that only one remains.
			for (int i = 0; i < activities.Length; ++i) {
				CMMotionActivity activity = activities [i];
				filteredActivities.Add (activity);

				if (!activity.Walking && !activity.Running && !activity.Automotive) {
					while (++i < activities.Length) {
						CMMotionActivity skipThisActivity = activities [i];
						if (skipThisActivity.Walking || skipThisActivity.Running || skipThisActivity.Automotive) {
							i = i - 1;
							break;
						}
					}
				}
			}

			// Ignore all low confidence activities.
			for (int i = 0; i < filteredActivities.Count;) {
				CMMotionActivity activity = filteredActivities [i];
				if (activity.Confidence == CMMotionActivityConfidence.Low) {
					filteredActivities.RemoveAt (i);
				} else {
					++i;
				}
			}

			// Skip all unclassified activities if their duration is smaller than
			// some threshold.  This has the effect of coalescing the remaining med + high
			// confidence activies together.
			for (int i = 0; i < filteredActivities.Count - 1;) {
				CMMotionActivity activity = filteredActivities [i];
				CMMotionActivity nextActivity = filteredActivities [i + 1];

				var duration = nextActivity.StartDate.SecondsSinceReferenceDate - activity.StartDate.SecondsSinceReferenceDate;

				if (duration < 60 * 3 && !activity.Walking && !activity.Running && !activity.Automotive) {
					filteredActivities.RemoveAt (i);
				} else {
					++i;
				}
			}

			// Coalesce activities where they differ only in confidence.
			for (int i = 1; i < filteredActivities.Count;) {
				CMMotionActivity prevActivity = filteredActivities [i - 1];
				CMMotionActivity activity = filteredActivities [i];

				if ((prevActivity.Walking && activity.Walking) ||
				    (prevActivity.Running && activity.Running) ||
				    (prevActivity.Automotive && activity.Automotive)) {
					filteredActivities.RemoveAt (i);
				} else {
					++i;
				}
			}

			// Finally transform into SignificantActivity;
			List<SignificantActivity> significantActivities = new List<SignificantActivity> ();

			for (int i = 0; i < filteredActivities.Count - 1; i++) {
				CMMotionActivity activity = filteredActivities [i];
				CMMotionActivity nextActivity = filteredActivities [i + 1];

				if (!activity.Walking && !activity.Running && !activity.Automotive)
					continue;

				var significantActivity = new SignificantActivity (ActivityDataManager.ActivityToType (activity), (DateTime)activity.StartDate, (DateTime)nextActivity.StartDate);

				try {
					var pedometerData = await pedometer.QueryPedometerDataAsync (significantActivity.StartDate, significantActivity.EndDate);

					significantActivity.StepCounts = pedometerData.NumberOfSteps.Int32Value;
				} catch {
					Console.WriteLine ("Error, unable to retrieve step counts for range {0}, {1}", significantActivity.StartDate.SecondsSinceReferenceDate, significantActivity.EndDate.SecondsSinceReferenceDate);
				}

				significantActivities.Add (significantActivity);
			}

			return significantActivities;
		}

		#region Utility functions

		public static ActivityType ActivityToType (CMMotionActivity activity)
		{
			if (activity.Walking)
				return ActivityType.Walking;
			if (activity.Running)
				return ActivityType.Running;
			if (activity.Automotive)
				return ActivityType.Driving;
			if (activity.Stationary)
				return ActivityType.Stationary;
			if (!activity.Unknown)
				return ActivityType.Moving;
			else
				return ActivityType.None;
		}

		public static string ActivityTypeToString (ActivityType type)
		{
			switch (type) {
			case ActivityType.Walking:
				return "Walking";
			case ActivityType.Running:
				return "Running";
			case ActivityType.Driving:
				return "Automotive";
			case ActivityType.Stationary:
				return "Stationary";
			case ActivityType.Moving:
				return "Moving";
			default:
				return "Unclassified";
			}
		}

		#endregion

		#region SampleCodeWorkspaceFinished

		static bool? available = null;

		public static bool CheckAvailability ()
		{
			if (available != null)
				return (bool)available;

			available = true;
			if (CMMotionActivityManager.IsActivityAvailable == false) {
				Console.WriteLine ("Motion Activity is not available!");
				available = false;
			}

			if (CMPedometer.IsStepCountingAvailable == false) {
				Console.WriteLine ("Step counting is not available");
				available = false;
			}

			return (bool)available;
		}

		void initMotionActivity ()
		{
			motionActivityMgr = new CMMotionActivityManager ();
			pedometer = new CMPedometer ();
		}

		public async Task<bool> CheckAuthorizationAsync ()
		{
			NSDate now = NSDate.Now;

			try {
				await pedometer.QueryPedometerDataAsync (now, now);
				return true;
			} catch (Exception) {
				return false;
			}
		}

		async Task queryHistoricalDataAsync (NSDate startDate, NSDate endDate)
		{
			try {
				var activities = await motionActivityMgr.QueryActivityAsync (startDate, endDate, NSOperationQueue.MainQueue);

				await additionalProcessingOnAsync (activities);

				var pedometerData = await pedometer.QueryPedometerDataAsync (startDate, endDate);

				StepCounts = pedometerData.NumberOfSteps.Int32Value;
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
		}

		public void StartStepUpdates (Action<int> handler)
		{
			pedometer.StartPedometerUpdates (NSDate.Now, ((pedometerData, error) => {
				DispatchQueue.MainQueue.DispatchAsync (() => {
					handler (pedometerData.NumberOfSteps.Int32Value);
				});
			}));
		}

		public void StopStepUpdates ()
		{
			pedometer.StopPedometerUpdates ();
		}

		public void StartMotionUpdates (Action<ActivityType> handler)
		{
			motionActivityMgr.StartActivityUpdates (NSOperationQueue.MainQueue, ((activity) => {
				handler (ActivityDataManager.ActivityToType (activity));
			}));
		}

		public void StopMotionUpdates ()
		{
			motionActivityMgr.StopActivityUpdates ();
		}

		#endregion
	}

	public class SignificantActivity : NSObject
	{
		public NSDate StartDate;
		public NSDate EndDate;
		public int StepCounts;
		public ActivityType ActivityType;

		public SignificantActivity (ActivityType type, DateTime startDate, DateTime endDate)
		{
			ActivityType = type;
			StartDate = (NSDate)startDate;
			EndDate = (NSDate)endDate;
			StepCounts = 0;
		}
	}
}
