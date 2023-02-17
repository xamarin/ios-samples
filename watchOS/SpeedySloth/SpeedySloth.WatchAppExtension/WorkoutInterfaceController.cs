
namespace SpeedySloth.WatchAppExtension {
	using CoreFoundation;
	using Foundation;
	using HealthKit;
	using System;
	using System.Collections.Generic;
	using WatchKit;

	public partial class WorkoutInterfaceController : WKInterfaceController, IHKWorkoutSessionDelegate {
		private readonly HealthStoreManager healthStoreManager = new HealthStoreManager ();

		private readonly ParentConnector parentConnector = new ParentConnector ();

		private HKWorkoutSession workoutSession;

		private NSDate startDate;

		private NSDate endDate;

		private NSTimer timer;

		protected WorkoutInterfaceController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		protected bool IsPaused => this.workoutSession.State == HKWorkoutSessionState.Paused;

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			// Configure interface objects here.
			Console.WriteLine ("{0} awake with context", this);

			var workoutConfiguration = context as HKWorkoutConfiguration;
			if (workoutConfiguration == null) {
				throw new ArgumentException ("The 'context' is not a HKWorkoutConfiguration", nameof (context));
			}

			// create a workout session with the workout configuration
			NSError error;
			this.workoutSession = new HKWorkoutSession (workoutConfiguration, out error);
			if (error == null) {
				// start a workout session
				this.workoutSession.Delegate = this;
				this.healthStoreManager.Start (this.workoutSession);
			} else {
				throw new Exception (error.Description ?? "Unknown exception");
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			this.workoutSession.Dispose ();
			this.parentConnector.Dispose ();
			this.healthStoreManager.Dispose ();
		}

		#region Actions

		partial void DidTapPauseResumeButton ()
		{
			this.RequestPauseOrResume ();
		}

		partial void DidTapMarkerButton ()
		{
			var markerEvent = HKWorkoutEvent.Create (HKWorkoutEventType.Marker, new NSDateInterval (), (HKMetadata) null);
			this.healthStoreManager.WorkoutEvents.Add (markerEvent);
			this.NotifyEvent ();
		}

		partial void DidTapStopButton ()
		{
			this.healthStoreManager.End (this.workoutSession);
		}

		#endregion

		#region UI

		private void UpdateLabels ()
		{
			this.CaloriesLabel.SetText (Utilities.Format (totalEnergyBurned: this.healthStoreManager.TotalEnergyBurned));
			this.DistanceLabel.SetText (Utilities.Format (totalDistance: this.healthStoreManager.TotalDistance));

			var events = new List<HKWorkoutEvent> (this.healthStoreManager.WorkoutEvents);
			var duration = Utilities.ComputeDurationOfWorkout (events, this.startDate, this.endDate);
			this.DurationLabel.SetText (Utilities.FormatDuration (duration));
		}

		private void UpdateStates ()
		{
			switch (this.workoutSession.State) {
			case HKWorkoutSessionState.NotStarted:
				this.SetTitle (NSBundle.MainBundle.GetLocalizedString ("Starting"));
				break;
			case HKWorkoutSessionState.Running:
				this.SetTitle (this.workoutSession.WorkoutConfiguration.ActivityType.DisplayString ());
				this.parentConnector.Send ("running");
				this.PauseResumeButton.SetTitle (NSBundle.MainBundle.GetLocalizedString ("Pause"));
				break;
			case HKWorkoutSessionState.Ended:
				this.SetTitle (NSBundle.MainBundle.GetLocalizedString ("Ended"));
				this.parentConnector.Send ("ended");
				break;
			case HKWorkoutSessionState.Paused:
				this.SetTitle (NSBundle.MainBundle.GetLocalizedString ("Paused"));
				this.parentConnector.Send ("paused");
				this.PauseResumeButton.SetTitle (NSBundle.MainBundle.GetLocalizedString ("Resume"));
				break;
			default:
				break;
			}
		}

		private void NotifyEvent ()
		{
			WKInterfaceDevice.CurrentDevice.PlayHaptic (WKHapticType.Notification);

			this.MarkerLabel.SetAlpha (1);
			DispatchQueue.MainQueue.DispatchAsync (() => {
				this.AnimateWithDuration (1d, () => { this.MarkerLabel.SetAlpha (0); });
			});
		}

		#endregion

		#region Data Accumulation

		private void StartAccumulatingData ()
		{
			this.healthStoreManager.StartWalkingRunningQuery (this.startDate, (quantitySamples) => {
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (!this.IsPaused) {
						this.healthStoreManager.ProcessWalkingRunningSamples (quantitySamples);
						this.UpdateLabels ();
					}
				});
			});

			this.healthStoreManager.StartActiveEnergyBurnedQuery (this.startDate, (quantitySamples) => {
				DispatchQueue.MainQueue.DispatchAsync (() => {
					if (!this.IsPaused) {
						this.healthStoreManager.ProcessActiveEnergySamples (quantitySamples);
						this.UpdateLabels ();
					}
				});
			});

			if (this.workoutSession.WorkoutConfiguration.LocationType == HKWorkoutSessionLocationType.Outdoor) {
				this.healthStoreManager.StartAccumulatingLocationData ();
			}
		}

		private void StopAccumulatingData ()
		{
			this.healthStoreManager.StopAccumulatingData ();
		}

		#endregion

		#region Timer

		private void StartTimer ()
		{
			this.timer = NSTimer.CreateScheduledTimer (1, true, (sender) => {
				this.UpdateLabels ();
			});
		}

		private void StopTimer ()
		{
			if (this.timer != null) {
				this.timer.Invalidate ();
				this.timer.Dispose ();
				this.timer = null;
			}
		}

		#endregion

		#region HKWorkoutSessionDelegate

		[Export ("workoutSession:didFailWithError:")]
		public void DidFail (HKWorkoutSession workoutSession, NSError error)
		{
			this.StopTimer ();
			this.workoutSession.Dispose ();
			Console.WriteLine ($"workout session failed with an error: ({error})");
		}

		[Export ("workoutSession:didChangeToState:fromState:date:")]
		public void DidChangeToState (HKWorkoutSession workoutSession, HKWorkoutSessionState toState, HKWorkoutSessionState fromState, NSDate date)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				this.HandleWorkoutSessionState (toState, fromState);
			});
		}

		[Export ("workoutSession:didGenerateEvent:")]
		public void WorkoutSession (HKWorkoutSession workoutSession, HKWorkoutEvent @event)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				this.healthStoreManager.WorkoutEvents.Add (@event);
			});
		}

		private void HandleWorkoutSessionState (HKWorkoutSessionState toState, HKWorkoutSessionState fromState)
		{
			if (fromState == HKWorkoutSessionState.NotStarted && toState == HKWorkoutSessionState.Running) {
				this.startDate = new NSDate ();
				this.StartTimer ();
				this.StartAccumulatingData ();
			} else if (toState == HKWorkoutSessionState.Ended) {
				this.StopAccumulatingData ();
				this.endDate = new NSDate ();
				this.StopTimer ();
				this.healthStoreManager.SaveWorkout (workoutSession, this.startDate, this.endDate);
			}

			this.UpdateLabels ();
			this.UpdateStates ();
		}

		#endregion

		#region Convenience

		private void RequestPauseOrResume ()
		{
			if (this.IsPaused) {
				this.healthStoreManager.Resume (this.workoutSession);
			} else {
				this.healthStoreManager.Pause (this.workoutSession);
			}
		}

		#endregion
	}
}
