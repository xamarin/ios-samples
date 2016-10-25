using System;
using System.Collections.Generic;
using WatchKit;
using Foundation;
using CoreFoundation;
using HealthKit;
using ObjCRuntime;


namespace ActivityRings.ActivityRingsWatchAppExtension
{
	public partial class InterfaceController : WKInterfaceController, IHKWorkoutSessionDelegate
	{

		public HKHealthStore HealthStore { get; set; } = new HKHealthStore();

		public HKWorkoutSession CurrentWorkoutSession { get; set; }
		public DateTime WorkoutBeginDate { get; set; }
		public DateTime WorkoutEndDate { get; set; }
		public bool IsWorkoutRunning { get; set; }  = false;
		public HKQuery CurrentQuery { get; set; }
		public List<HKSample> ActiveEnergySamples { get; set; } = new List<HKSample>();

		// Start with a zero quantity.
		public HKQuantity CurrentActiveEnergyQuantity { get; set; } = HKQuantity.FromQuantity(HKUnit.Kilocalorie, 0.0);



		protected InterfaceController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Awake(NSObject context)
		{
			base.Awake(context);

			// Configure interface objects here.
			Console.WriteLine("{0} awake with context", this);
		}

		public override void WillActivate()
		{
			// This method is called when the watch view controller is about to be visible to the user.
			Console.WriteLine("{0} will activate", this);


			// Only proceed if health data is available.
			if (!HKHealthStore.IsHealthDataAvailable) { return; };

			// We need to be able to write workouts, so they display as a standalone workout in the Activity app on iPhone.
			// We also need to be able to write Active Energy Burned to write samples to HealthKit to later associating with our app.

			var typesToShare = new NSSet(HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned), HKObjectType.GetWorkoutType());

			var typesToRead = new NSSet(HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned));

			HealthStore.RequestAuthorizationToShare(typesToShare, typesToRead, (bool success, NSError error) =>
			{
				if (error != null && !success)
				{
					Console.WriteLine("You didn't allow HealthKit to access these read/write data types. " +
									  "In your app, try to handle this error gracefully when a user decides not to provide access. " +
									  "The error was: {0}. If you're using a simulator, try it on a device.", error.LocalizedDescription);
				}
			});
		}

		public override void DidDeactivate()
		{
			// This method is called when the watch view controller is no longer visible to the user.
			Console.WriteLine("{0} did deactivate", this);
		}

		partial void ToggleWorkout()
		{
			if (IsWorkoutRunning){
				if (CurrentWorkoutSession != null)
				{
					HealthStore.EndWorkoutSession(CurrentWorkoutSession);
					IsWorkoutRunning = false;
				}
				
			}else {
				// Begin workout.
				IsWorkoutRunning = true;

				// Clear the local Active Energy Burned quantity when beginning a workout session.
				CurrentActiveEnergyQuantity = HKQuantity.FromQuantity(HKUnit.Kilocalorie, 0.0);

				CurrentQuery = null;
				ActiveEnergySamples = new List<HKSample>();

				// An indoor walk workout session. There are other activity and location types available to you.

				// Create a workout configuration
				var configuration = new HKWorkoutConfiguration()
				{
					ActivityType = HKWorkoutActivityType.Walking,
					LocationType = HKWorkoutSessionLocationType.Indoor
				};


				NSError error = null;
				var workoutSession = new HKWorkoutSession(configuration, out error);
				workoutSession.Delegate = this;
				CurrentWorkoutSession = workoutSession;
				HealthStore.StartWorkoutSession(workoutSession);

			}

		}



		public void SaveWorkout()
		{
			// Obtain the `HKObjectType` for active energy burned.
			var activeEnergyType = HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned);
			if (activeEnergyType == null) return;

			var beginDate = WorkoutBeginDate;
			var endDate = WorkoutEndDate;

			TimeSpan timeDifference = endDate.Subtract(beginDate);
			double duration = timeDifference.TotalSeconds;
			NSDictionary metadata = null;

			var workout = HKWorkout.Create(HKWorkoutActivityType.Walking,
			                               (NSDate)beginDate,
			                               (NSDate)endDate,
			                               duration,
			                               CurrentActiveEnergyQuantity,
			                               HKQuantity.FromQuantity(HKUnit.Mile, 0.0),
			                               metadata);

			var finalActiveEnergySamples = ActiveEnergySamples;

			if (HealthStore.GetAuthorizationStatus(activeEnergyType) != HKAuthorizationStatus.SharingAuthorized ||
			    HealthStore.GetAuthorizationStatus(HKObjectType.GetWorkoutType()) != HKAuthorizationStatus.SharingAuthorized
			   )
			{
				return;
			}

			HealthStore.SaveObject(workout, (success, error) =>
			{
				if (success)
				{
					if (finalActiveEnergySamples.Count > 0)
					{
						HealthStore.AddSamples(finalActiveEnergySamples.ToArray(), workout, (addSuccess, addError) =>
						{
							// Handle any errors
							if (addError == null)
							{
								// Was the save successful
								if (addSuccess)
								{

								}
							}
							else {
								// Report error
								Console.WriteLine("An error occured adding the samples. In your app, try to handle this gracefully. " +
					"The error was: {0}.", error);
							}
						});
					}
				}
				else {
					Console.WriteLine("An error occured saving the workout. In your app, try to handle this gracefully. " +
					"The error was: {0}.", error);
				}
			});

		}


		public void BeginWorkout(DateTime beginDate)
		{
			// Obtain the `HKObjectType` for active energy burned and the `HKUnit` for kilocalories.
			var activeEnergyType = HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned);
			if (activeEnergyType == null) return;

			var energyUnit = HKUnit.Kilocalorie;

			// Update properties.
			WorkoutBeginDate = beginDate;
			workoutButton.SetTitle("End Workout");

			// Set up a predicate to obtain only samples from the local device starting from `beginDate`.
		
			var datePredicate = HKQuery.GetPredicateForSamples((NSDate)beginDate, null, HKQueryOptions.None);

			var devices = new NSSet<HKDevice>(new HKDevice[] { HKDevice.LocalDevice });
			var devicePredicate = HKQuery.GetPredicateForObjectsFromDevices(devices);
			var predicate = NSCompoundPredicate.CreateAndPredicate(new NSPredicate[] { datePredicate, devicePredicate });



			//Create a results handler to recreate the samples generated by a query of active energy samples so that they can be associated with this app in the move graph.It should be noted that if your app has different heuristics for active energy burned you can generate your own quantities rather than rely on those from the watch.The sum of your sample's quantity values should equal the energy burned value provided for the workout
		
			Action <List<HKSample>> sampleHandler;
			sampleHandler = (List<HKSample> samples) =>
			{
				DispatchQueue.MainQueue.DispatchAsync(delegate
				{
					List<HKQuantitySample> accumulatedSamples = new List<HKQuantitySample>();

					var initialActivityEnergy = CurrentActiveEnergyQuantity.GetDoubleValue(energyUnit);

					double accumulatedValue = initialActivityEnergy;
					foreach (HKQuantitySample sample in samples)
					{
						accumulatedValue = accumulatedValue + sample.Quantity.GetDoubleValue(energyUnit);
						var ourSample = HKQuantitySample.FromType(activeEnergyType, sample.Quantity, sample.StartDate, sample.EndDate);
						accumulatedSamples.Add(ourSample);

					}

					// Update the UI.
					CurrentActiveEnergyQuantity = HKQuantity.FromQuantity(energyUnit, accumulatedValue);
					activeEnergyBurnedLabel.SetText(String.Format("{0}", accumulatedValue));

					// Update our samples.
					ActiveEnergySamples.AddRange(accumulatedSamples);


				});
			};
			 

			// Create a query to report new Active Energy Burned samples to our app.
			var activeEnergyQuery = new HKAnchoredObjectQuery(activeEnergyType, predicate, null, HKSampleQuery.NoLimit, (query, addedObjects, deletedObjects, newAnchor, error) =>
			{
				// Valid?
				if (error == null)
				{
					// NOTE: `deletedObjects` are not considered in the handler as there is no way to delete samples from the watch during a workout
					ActiveEnergySamples = new List<HKSample>(addedObjects);
					sampleHandler(ActiveEnergySamples);

				}
				else {
					Console.WriteLine("An error occured executing the query. In your app, try to handle this gracefully. " +
					"The error was: {0}.", error);
				}

			});

			// Assign the same handler to process future samples generated while the query is still active.
			activeEnergyQuery.UpdateHandler = (query, addedObjects, deletedObjects, newAnchor, error) =>
			{

				if (error == null)
				{
					
					ActiveEnergySamples = new List<HKSample>(addedObjects);
					sampleHandler(ActiveEnergySamples);

				}
				else {
					Console.WriteLine("An error occured executing the query. In your app, try to handle this gracefully. " +
					"The error was: {0}.", error);
				}

			};

			// Start Query
			CurrentQuery = activeEnergyQuery;
			HealthStore.ExecuteQuery(activeEnergyQuery);

		}



		public void EndWorkout(DateTime endDate)
		{
			WorkoutEndDate = endDate;
			workoutButton.SetTitle("Begin Workout");
			activeEnergyBurnedLabel.SetText("0.0");
			if (CurrentQuery != null)
			{
				var query = CurrentQuery;
				HealthStore.StopQuery(query);
			}
			SaveWorkout();
		}


		public void DidChangeToState(HKWorkoutSession workoutSession, HKWorkoutSessionState toState, HKWorkoutSessionState fromState, NSDate date)
		{
			DispatchQueue.MainQueue.DispatchAsync(delegate
			{
				// Take action based on the change in state
				switch (toState)
				{

					case HKWorkoutSessionState.Running:
						BeginWorkout((DateTime)date);
						break;
					case HKWorkoutSessionState.Ended:
						EndWorkout((DateTime)date);
						break;
					default:
						Console.WriteLine("Unexpected workout session: {0}.", toState);
						break;
				}
			});

		}

		public void DidFail(HKWorkoutSession workoutSession, NSError error) 
		{ 
			Console.WriteLine("An error occured with the workout session. In your app, try to handle this gracefully. " +
					"The error was: {0}.", error);
		}
	}
}
