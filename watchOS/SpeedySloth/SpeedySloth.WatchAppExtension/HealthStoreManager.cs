
namespace SpeedySloth.WatchAppExtension {
	using CoreFoundation;
	using CoreLocation;
	using Foundation;
	using HealthKit;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using WatchKit;

	public class HealthStoreManager : CLLocationManagerDelegate {
		private readonly List<HKWorkoutEvent> workoutEvents = new List<HKWorkoutEvent> ();

		private readonly List<HKQuery> activeDataQueries = new List<HKQuery> ();

		private readonly HKHealthStore healthStore = new HKHealthStore ();

		private HKWorkoutRouteBuilder workoutRouteBuilder;

		private CLLocationManager locationManager;

		public List<HKWorkoutEvent> WorkoutEvents => this.workoutEvents;

		public double TotalEnergyBurned { get; private set; } = 0d;

		public double TotalDistance { get; private set; } = 0d;

		#region Health store wrappers

		public void Start (HKWorkoutSession workoutSession)
		{
			this.healthStore.StartWorkoutSession (workoutSession);
		}

		public void End (HKWorkoutSession workoutSession)
		{
			this.healthStore.EndWorkoutSession (workoutSession);
		}

		public void Pause (HKWorkoutSession workoutSession)
		{
			this.healthStore.PauseWorkoutSession (workoutSession);
		}

		public void Resume (HKWorkoutSession workoutSession)
		{
			this.healthStore.ResumeWorkoutSession (workoutSession);
		}

		#endregion

		#region Data Accumulation

		public void StartWalkingRunningQuery (NSDate startDate, Action<List<HKQuantitySample>> handler)
		{
			this.StartQuery (HKQuantityTypeIdentifier.DistanceWalkingRunning,
							startDate,
							(query, addedObjects, deletedObjects, newAnchor, error) => {
								if (addedObjects != null) {
									var quantitySamples = addedObjects.OfType<HKQuantitySample> ().ToList ();
									handler (quantitySamples);
								} else {
									Console.WriteLine ($"Distance walking running query failed with error: ({error?.Description ?? "unknown"})");
								}
							});
		}

		public void StartActiveEnergyBurnedQuery (NSDate startDate, Action<List<HKQuantitySample>> handler)
		{
			this.StartQuery (HKQuantityTypeIdentifier.ActiveEnergyBurned,
							startDate,
							(query, addedObjects, deletedObjects, newAnchor, error) => {
								if (addedObjects != null) {
									var quantitySamples = addedObjects.OfType<HKQuantitySample> ().ToList ();
									handler (quantitySamples);
								} else {
									Console.WriteLine ($"Active energy burned query failed with error: ({error?.Description ?? "unknown"})");
								}
							});
		}

		public void StartAccumulatingLocationData ()
		{
			if (CLLocationManager.LocationServicesEnabled) {
				this.locationManager = new CLLocationManager {
					Delegate = this,
					AllowsBackgroundLocationUpdates = true,
					DesiredAccuracy = CLLocation.AccuracyBest,
				};

				this.locationManager.StartUpdatingLocation ();
				this.workoutRouteBuilder = new HKWorkoutRouteBuilder (this.healthStore, null);
			} else {
				Console.WriteLine ("User does not have location service enabled");
			}
		}

		public void StopAccumulatingData ()
		{
			foreach (var query in this.activeDataQueries) {
				this.healthStore.StopQuery (query);
			}

			this.activeDataQueries.Clear ();
			this.locationManager?.StopUpdatingLocation ();
		}

		private void StartQuery (HKQuantityTypeIdentifier quantityTypeIdentifier, NSDate startDate, HKAnchoredObjectUpdateHandler handler)
		{
			var datePredicate = HKQuery.GetPredicateForSamples (startDate, null, HKQueryOptions.StrictStartDate);
			var devicePredicate = HKQuery.GetPredicateForObjectsFromDevices (new NSSet<HKDevice> (HKDevice.LocalDevice));
			var queryPredicate = NSCompoundPredicate.CreateAndPredicate (new NSPredicate [] { datePredicate, devicePredicate });

			var quantityType = HKQuantityType.Create (quantityTypeIdentifier);
			var query = new HKAnchoredObjectQuery (quantityType, queryPredicate, null, HKSampleQuery.NoLimit, handler);
			query.UpdateHandler = handler;
			this.healthStore.ExecuteQuery (query);

			this.activeDataQueries.Add (query);
		}

		#endregion

		#region Saving Data

		public void SaveWorkout (HKWorkoutSession workoutSession, NSDate startDate, NSDate endDate)
		{
			// Create and save a workout sample
			var configuration = workoutSession.WorkoutConfiguration;
			var metadata = new HKMetadata {
				IndoorWorkout = configuration.LocationType == HKWorkoutSessionLocationType.Indoor,
			};

			var workout = HKWorkout.Create (configuration.ActivityType,
										   startDate,
										   endDate,
										   this.workoutEvents.ToArray (),
										   this.TotalBurningEnergyQuantity (),
										   this.TotalDistanceQuantity (),
										   metadata);

			this.healthStore.SaveObject (workout, (isSuccess, error) => {
				if (isSuccess) {
					this.AddSamples (workout, startDate, endDate);
				}
			});
		}

		private void AddSamples (HKWorkout workout, NSDate startDate, NSDate endDate)
		{
			// Create energy and distance sample
			var totalEnergyBurnedSample = HKQuantitySample.FromType (HKQuantityType.Create (HKQuantityTypeIdentifier.ActiveEnergyBurned),
																						  this.TotalBurningEnergyQuantity (),
																						  startDate,
																						  endDate);

			var totalDistanceSample = HKQuantitySample.FromType (HKQuantityType.Create (HKQuantityTypeIdentifier.DistanceWalkingRunning),
																					  this.TotalDistanceQuantity (),
																					  startDate,
																					  endDate);
			// add samples to workout
			this.healthStore.AddSamples (new HKSample [] { totalEnergyBurnedSample, totalDistanceSample }, workout, (isSuccess, error) => {
				if (isSuccess) {
					DispatchQueue.MainQueue.DispatchAsync (() => {
						WKInterfaceController.ReloadRootPageControllers (new string [] { nameof (SummaryInterfaceController) },
																		new NSObject [] { workout },
																		WKPageOrientation.Vertical,
																		0);
					});
				} else {
					Console.WriteLine ($"Adding workout subsamples failed with error: ({error?.Description ?? "unknown"})");
				}
			});

			// finish the route with a syn identifier so we can easily update the route later
			var objects = new NSObject [] { new NSString (new NSUuid ().AsString ()), NSNumber.FromInt32 (1) };
			var keys = new NSString [] { HKMetadataKey.SyncIdentifier, HKMetadataKey.SyncVersion };

			var dictionary = NSDictionary.FromObjectsAndKeys (objects, keys);
			var metadata = new HKMetadata (dictionary);

			this.workoutRouteBuilder?.FinishRoute (workout, metadata, (workoutRoute, error) => {
				if (workoutRoute == null) {
					Console.WriteLine ($"Finishing route failed with error: ({error?.Description ?? "unknown"})");
				}
			});
		}

		#endregion

		#region CLLocationManagerDelegate

		public override void LocationsUpdated (CLLocationManager manager, CLLocation [] locations)
		{
			var filtredLocations = locations.Where (location => location.HorizontalAccuracy <= CLLocation.AccuracyNearestTenMeters).ToArray ();
			if (filtredLocations.Any ()) {
				this.workoutRouteBuilder?.InsertRouteData (filtredLocations, (isSuccess, error) => {
					if (!isSuccess) {
						Console.WriteLine ($"Inserting route data failed with error: ({error?.Description ?? "unknown"})");
					}
				});
			}
		}

		#endregion

		#region Convenience

		public void ProcessWalkingRunningSamples (List<HKQuantitySample> samples)
		{
			this.TotalDistance += samples.Sum (sample => sample.Quantity.GetDoubleValue (HKUnit.Meter));
		}

		public void ProcessActiveEnergySamples (List<HKQuantitySample> samples)
		{
			this.TotalEnergyBurned += samples.Sum (sample => sample.Quantity.GetDoubleValue (HKUnit.Kilocalorie));
		}

		private HKQuantity TotalBurningEnergyQuantity ()
		{
			return HKQuantity.FromQuantity (HKUnit.Kilocalorie, this.TotalEnergyBurned);
		}

		private HKQuantity TotalDistanceQuantity ()
		{
			return HKQuantity.FromQuantity (HKUnit.Meter, this.TotalDistance);
		}

		#endregion

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			this.healthStore.Dispose ();
			this.workoutEvents.Clear ();
			this.activeDataQueries.Clear ();

			if (this.locationManager != null) {
				this.locationManager.Dispose ();
				this.locationManager = null;
			}

			if (this.workoutRouteBuilder != null) {
				this.workoutRouteBuilder.Dispose ();
				this.workoutRouteBuilder = null;
			}
		}
	}
}
