using System;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

using Foundation;
using CoreMotion;
using CoreFoundation;

namespace MusicMotion {
	public static class MotionContext {

		public static readonly string LowIntensity = "Low Intensity";
		public static readonly string MediumIntensity = "Medium Intensity";
		public static readonly string HighIntensity = "High Intensity";
		public static readonly string Driving = "Driving";
	}

	public class MotionManager {
		
		const int MaxActivitySamples = 2;
		const int MaxAltitudeSamples = 10;
		const int MaxPedometerSamples = 1;
		const double MetersForSignificantAltitudeChange = 5.0;
		const double MediumPace = 0.671080; // 18 minutes per mile in meters per second.
		const double HighPace = 0.447387; // 12 minutes per mile in meters per second.

		string currentContext = MotionContext.LowIntensity;

		List<CMMotionActivity> recentMotionActivities;
		List<CMPedometerData> recentPedometerData;
		List<CMAltitudeData> recentAltitudeData;

		CMAltimeter altimeter;
		CMPedometer pedometer;
		CMMotionActivityManager activityManager;

		public event EventHandler LowIntensityContextStarted;
		public event EventHandler MediumIntensityContextStarted;
		public event EventHandler HighIntensityContextStarted;
		public event EventHandler DrivingContextStarted;
		public event EventHandler DidEncounterAuthorizationError;

		public List<Activity> RecentActivities { get; private set; }

		NSOperationQueue motionQueue;
		NSOperationQueue MotionQueue {
			get {
				motionQueue = motionQueue ?? new NSOperationQueue  { Name = "musicMotionQueue" };
				return motionQueue;
			}
		}

		bool IsInHighIntensityContext =>
			(IsUserPaceHigh && IsUserRunning) || HasAltitudeChangedRecently;

		bool IsInMediumIntensityContext =>
			IsUserPaceMedium && IsUserWalking;

		bool IsInLowIntensityContext =>
			IsUserStationary;

		bool IsInDrivingContext => 
			IsUserDriving;

		bool IsUserPaceHigh => 
			(CurrentPace == 0) ? true : CurrentPace < MotionManager.HighPace;

		bool IsUserPaceMedium =>
			(CurrentPace == 0) ? true : (CurrentPace < MotionManager.MediumPace && CurrentPace > MotionManager.HighPace);

		bool IsUserDriving =>
			ActivitesMatch (activity => activity.Automotive);

		bool IsUserRunning =>
			ActivitesMatch (activity => activity.Running);

		bool IsUserStationary =>
			ActivitesMatch (activity => activity.Stationary);

		bool IsUserWalking =>
			ActivitesMatch (activity => activity.Walking);

		bool HasAltitudeChangedRecently {
			get {
				var firstAltitude = recentAltitudeData.FirstOrDefault ();
				var lastAltitude = recentAltitudeData.LastOrDefault ();

				if (firstAltitude == null || lastAltitude == null)
					return false;
				
				return Math.Abs (firstAltitude.RelativeAltitude.DoubleValue - lastAltitude.RelativeAltitude.DoubleValue) > MotionManager.MetersForSignificantAltitudeChange;
			}
		}

		double CurrentPace {
			get {
				var pedometerData = recentPedometerData.FirstOrDefault ();
				return (pedometerData == null) ? 0.0 : pedometerData.CurrentPace.DoubleValue;
			}
		}

		public MotionManager ()
		{
			RecentActivities = new List<Activity> ();
			recentMotionActivities = new List<CMMotionActivity> ();
			recentPedometerData = new List<CMPedometerData> ();
			recentAltitudeData = new List<CMAltitudeData> ();

			activityManager = new CMMotionActivityManager ();
			pedometer = new CMPedometer ();
			altimeter = new CMAltimeter ();
		}

		public void StartMonitoring ()
		{
			if (CMMotionActivityManager.IsActivityAvailable) {
				activityManager.StartActivityUpdates (MotionQueue, activity => {
					if (!activity.HasActivitySignature ())
						return;
					SaveMotionActivity (activity);
					UpdateUserContext ();
				});
			} else {
				WriteLine ("Activity updates are not available.");
			}

			if (CMPedometer.IsStepCountingAvailable) {
				pedometer.StartPedometerUpdates (NSDate.Now, (pedometerData, error) => {
					if (pedometerData != null) {
						SavePedometerData (pedometerData);
						UpdateUserContext ();
					} else if (error != null) {
						HandleError (error);
					}
				});
			} else {
				WriteLine ("Step counting is not available.");
			}
		}

		public void QueryForRecentActivityData (Action completionHandler)
		{
			var now = NSDate.Now;
			var dateComponents = new NSDateComponents ();
			dateComponents.SetValueForComponent (-7, NSCalendarUnit.Day);
			var startDay = NSCalendar.CurrentCalendar.DateByAddingComponents (dateComponents, now, NSCalendarOptions.None);

			activityManager.QueryActivity (startDay, now, MotionQueue, (activities, error) => {
				if (activities != null)
					CreateActivityDataWithActivities (activities, completionHandler);
				else if (error != null)
					HandleError (error);
			});
		}

		void UpdateUserContext ()
		{
			if (IsUserRunning || IsUserWalking) {
				if (!CMAltimeter.IsRelativeAltitudeAvailable) {
					altimeter.StartRelativeAltitudeUpdates (MotionQueue, (altitudeData, error) => {
						if (altitudeData != null) {
							SaveAltitudeData (altitudeData);
							UpdateUserContext ();
						} else if (error != null) {
							HandleError (error);
						}
					});
				} else {
					WriteLine ("Relative altitude is not available.");
				}
			} else if (CMAltimeter.IsRelativeAltitudeAvailable) {
				altimeter.StopRelativeAltitudeUpdates ();
			}

			UpdateContextAndNotifyDelegate ();
		}

		void UpdateContextAndNotifyDelegate ()
		{
			if (currentContext != MotionContext.Driving && IsInDrivingContext) {
				currentContext = MotionContext.Driving;
				DrivingContextStarted?.Invoke (this, null);
			} else if (currentContext != MotionContext.LowIntensity && IsInLowIntensityContext) {
				currentContext = MotionContext.LowIntensity;
				LowIntensityContextStarted?.Invoke (this, null);
			} else if (currentContext != MotionContext.MediumIntensity && IsInMediumIntensityContext) {
				currentContext = MotionContext.MediumIntensity;
				MediumIntensityContextStarted?.Invoke (this, null);
			} else if (currentContext != MotionContext.HighIntensity && IsInHighIntensityContext) {
				currentContext = MotionContext.HighIntensity;
				HighIntensityContextStarted?.Invoke (this, null);
			}
		}

		List<Activity> CreateActivityDataWithActivities (CMMotionActivity[] activities, Action completionHandler)
		{
			var results = new List<Activity> ();

			var group = DispatchGroup.Create ();
			var queue = new DispatchQueue ("resultQueue");

			var filteredActivities = activities.Where (activity => activity.HasActivitySignature ()
                 && !activity.Stationary
                 && activity.Confidence != CMMotionActivityConfidence.Low).ToArray<CMMotionActivity> ();

			var activitySegments = FindActivitySegments (filteredActivities);

			foreach (var segment in activitySegments) {
				group.Enter ();
				pedometer.QueryPedometerData (segment.Item1.StartDate, (NSDate)segment.Item2, (pedometerData, error) => {
					queue.DispatchAsync (() => {
						var activity = new Activity (segment.Item1,
			               ((DateTime)segment.Item1.StartDate).ToLocalTime (),
			               segment.Item2.ToLocalTime (),
			               pedometerData);
						
						results.Add (activity);
					});

					if (error != null)
						HandleError (error);

					group.Leave ();
				});
			}

			group.Notify (DispatchQueue.MainQueue, () => {
				queue.DispatchSync (() => {
					RecentActivities = results;
					RecentActivities.Reverse ();
					completionHandler?.Invoke ();
				});
			});

			return results;
		}

		List<Tuple<CMMotionActivity, DateTime>> FindActivitySegments (CMMotionActivity[] activities)
		{
			var segments = new List<Tuple<CMMotionActivity, DateTime>> ();

			for (int i = 0; i < activities.Length - 1; i++) {
				var activity = activities [i];
				var startDate = (DateTime)activity.StartDate;

				var nextActivity = activities [++i];
				var endDate = (DateTime)nextActivity.StartDate;

				while (i < activities.Length - 1) {
					
					if (!activity.IsSimilarToActivity (nextActivity))
						break;

					var previousActivityEnd = (DateTime)activities [i - 1].StartDate;
					var secondsBetweenActivites = (endDate - previousActivityEnd).TotalSeconds;

					if (secondsBetweenActivites >= 60 * 60)
						break;

					nextActivity = activities [++i];
					endDate = (DateTime)nextActivity.StartDate;
				}

				nextActivity = (i != activities.Length - 1) ? activities [--i] : activities [i];
				endDate = (DateTime)nextActivity.StartDate;

				if ((endDate - startDate).TotalSeconds > 60)
					segments.Add (new Tuple<CMMotionActivity, DateTime> (activity, endDate));
			}

			return segments;
		}

		void SavePedometerData (CMPedometerData pedometerData)
		{
			recentPedometerData.Insert (0, pedometerData);

			if (recentPedometerData.Count > MotionManager.MaxPedometerSamples)
				recentPedometerData.RemoveAt (recentPedometerData.Count - 1);
		}

		void SaveAltitudeData (CMAltitudeData altitude)
		{
			recentAltitudeData.Insert (0, altitude);

			if (recentAltitudeData.Count > MotionManager.MaxAltitudeSamples)
				recentAltitudeData.RemoveAt (recentPedometerData.Count - 1);
		}

		void SaveMotionActivity (CMMotionActivity activity)
		{
			recentMotionActivities.Insert (0, activity);
			recentMotionActivities = recentMotionActivities.Where (c => c.StartDate.SecondsSinceReferenceDate <= 60).ToList<CMMotionActivity> ();

			if (recentMotionActivities.Count > MotionManager.MaxActivitySamples)
				recentMotionActivities.RemoveAt (recentPedometerData.Count - 1);
		}

		void HandleError (NSError error)
		{
			if (error.Code == (int)CMError.MotionActivityNotAuthorized)
				DidEncounterAuthorizationError?.Invoke (this, null);
			else
				WriteLine (error.LocalizedDescription);
		}

		bool ActivitesMatch (Func<CMMotionActivity, bool> predicate)
		{
			return (recentMotionActivities.Count == 0) ? false : recentMotionActivities.Any (predicate);
		}
	}
}

