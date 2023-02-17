
namespace SpeedySloth.WatchAppExtension {
	using CoreLocation;
	using Foundation;
	using HealthKit;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using WatchKit;

	public partial class ConfigurationInterfaceController : WKInterfaceController {
		private readonly List<WorkoutType> activityTypes = new List<WorkoutType> { WorkoutType.Walking, WorkoutType.Running, WorkoutType.Hiking };

		private readonly List<LocationType> locationTypes = new List<LocationType> { LocationType.Indoor, LocationType.Outdoor };

		private readonly HKHealthStore healthStore = new HKHealthStore ();

		private LocationType selectedLocationType;

		private WorkoutType selectedActivityType;

		public ConfigurationInterfaceController () : base ()
		{
			this.selectedLocationType = this.locationTypes [0];
			this.selectedActivityType = this.activityTypes [0];
		}

		protected ConfigurationInterfaceController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);
			this.SetTitle ("Speedy Sloth");

			// populate the activity type picker
			var activityTypePickerItems = this.activityTypes.Select (type => new WKPickerItem { Title = type.DisplayString () });
			this.ActivityTypePicker.SetItems (activityTypePickerItems.ToArray ());

			// populate the location type picker
			var locationTypePickerItems = this.locationTypes.Select (type => new WKPickerItem { Title = type.DisplayString () });
			this.LocationTypePicker.SetItems (locationTypePickerItems.ToArray ());
		}

		#region Post processing of workouts (Slothify)

		private void MakeWorkoutsSlothy ()
		{
			this.LoadWorkouts ((workouts) => {
				foreach (var workout in workouts) {
					this.MakeWorkoutsSlothy (workout);
				}
			});
		}

		private void LoadWorkouts (Action<List<HKWorkout>> completion)
		{
			var workoutType = HKObjectType.GetWorkoutType ();
			var predicate = HKQuery.GetPredicateForObjectsFromSource (HKSource.GetDefaultSource);
			var query = new HKSampleQuery (workoutType, predicate, HKSampleQuery.NoLimit, null, (sender, results, error) => {
				var isSuccess = results != null;
				if (isSuccess) {
					var workouts = results.OfType<HKWorkout> ().ToList ();
					isSuccess = workouts.Any ();
					if (isSuccess) {
						completion (workouts);
					}
				}

				if (!isSuccess) {
					Console.WriteLine ($"An error occurred: ({error?.LocalizedDescription ?? "Unknown"})");
				}
			});

			this.healthStore.ExecuteQuery (query);
		}

		private void MakeWorkoutsSlothy (HKWorkout workout)
		{
			// Query for workout's rotes
			var routeType = HKSeriesType.WorkoutRouteType;
			var workoutPredicate = HKQuery.GetPredicateForObjectsFromWorkout (workout);
			var routeQuery = new HKSampleQuery (routeType, workoutPredicate, HKSampleQuery.NoLimit, null, (sender, results, error) => {
				var route = results?.FirstOrDefault () as HKWorkoutRoute;
				if (route != null) {
					var version = route.Metadata?.SyncVersion;
					if (!version.HasValue) {
						Console.WriteLine ($"Route does not have a sync version ({route})");
					} else if (version.Value == 1) {
						this.MakeRouteWorkoutsSlothy (workout, route);
					}
				} else {
					Console.WriteLine ($"An error occurred fetching the route ({error?.LocalizedDescription ?? "Workout has no routes"})");
				}
			});

			this.healthStore.ExecuteQuery (routeQuery);
		}

		private void MakeRouteWorkoutsSlothy (HKWorkout workout, HKWorkoutRoute route)
		{
			// Get all of the locations
			this.LoadRouteLocations (route, (locations) => {
				// Slothify route
				var newLocations = this.SlothifyRouteLocations (locations);
				this.UpdateWorkoutLocationsRoute (workout, route, newLocations);
			});
		}

		private void LoadRouteLocations (HKWorkoutRoute route, Action<List<CLLocation>> completion)
		{
			var locations = new List<CLLocation> ();

			var locationQuery = new HKWorkoutRouteQuery (route, (query, routeData, done, error) => {
				if (routeData != null) {
					locations.AddRange (routeData);
					if (done) {
						completion (locations);
					}
				} else {
					Console.WriteLine ($"Error occurred wile querying for locations: ({error?.LocalizedDescription ?? ""})");
				}
			});

			this.healthStore.ExecuteQuery (locationQuery);
		}


		/// <summary>
		/// Slothifying a workout route's locations will shift the locations left and right  to form a moving spiral
		/// around the original route
		/// </summary>
		/// <param name="locations"></param>
		/// <returns></returns>
		private List<CLLocation> SlothifyRouteLocations (List<CLLocation> locations)
		{
			var newLocations = new List<CLLocation> ();

			var start = locations.FirstOrDefault () ?? new CLLocation (0d, 0d);
			newLocations.Add (start);

			var radius = 0.0001;

			var theta = 0d;
			for (int i = 1; i < locations.Count - 1; i++) {
				var location = locations [i];

				theta += Math.PI / 8d;
				var latitude = Math.Sin (theta) * radius;
				var longitude = Math.Cos (theta) * radius;

				var coordinate = locations [i].Coordinate;
				coordinate.Latitude += latitude;
				coordinate.Longitude += longitude;
				newLocations.Add (new CLLocation (coordinate,
												location.Altitude,
												location.HorizontalAccuracy,
												location.VerticalAccuracy,
												location.Course,
												location.Speed,
												location.Timestamp));
			}

			// Then  jump to the last location
			var lastLocation = locations.LastOrDefault ();
			if (lastLocation != null) {
				newLocations.Add (lastLocation);
			}

			return newLocations;
		}

		private void UpdateWorkoutLocationsRoute (HKWorkout workout, HKWorkoutRoute route, List<CLLocation> newLocations)
		{
			// create a workout route builder
			var workoutRouteBuilder = new HKWorkoutRouteBuilder (this.healthStore, null);

			// insert updated route locations
			workoutRouteBuilder.InsertRouteData (newLocations.ToArray (), (success, error) => {
				if (success) {
					var syncIdentifier = route.Metadata?.SyncIdentifier;
					if (!string.IsNullOrEmpty (syncIdentifier)) {
						// new metadata with the same sync identifier and a higher version
						var objects = new NSObject [] { new NSString (syncIdentifier), NSNumber.FromInt32 (2) };
						var keys = new NSString [] { HKMetadataKey.SyncIdentifier, HKMetadataKey.SyncVersion };

						var dictionary = NSDictionary.FromObjectsAndKeys (objects, keys);
						var metadata = new HKMetadata (dictionary);

						// finish the route with updated metadata
						workoutRouteBuilder.FinishRoute (workout, metadata, (workoutRoute, routeRrror) => {
							if (workoutRoute != null) {
								Console.WriteLine ($"Workout route updated: ({route})");
							} else {
								Console.WriteLine ($"An error occurred while finishing the new route: ({error?.LocalizedDescription ?? "Unknown"})");
							}
						});
					} else {
						throw new ArgumentNullException (nameof (syncIdentifier), "Missing expected sync identifier on route");
					}
				} else {
					Console.WriteLine ($"An error occurred while inserting route data ({error?.LocalizedDescription ?? "Unknown"})");
				}
			});
		}

		#endregion

		partial void SlothifyWorkouts ()
		{
			this.MakeWorkoutsSlothy ();
		}

		partial void ActivityTypePickerSelectedItemChanged (nint sender)
		{
			this.selectedActivityType = this.activityTypes [(int) sender];
		}

		partial void LocationTypePickerSelectedItemChanged (nint sender)
		{
			this.selectedLocationType = this.locationTypes [(int) sender];
		}

		partial void DidTapStartButton ()
		{
			// Create workout configuration
			var workoutConfiguration = new HKWorkoutConfiguration {
				ActivityType = this.selectedActivityType.Map (),
				LocationType = this.selectedLocationType.Map ()
			};

			// Pass configuration to next interface controller
			WKInterfaceController.ReloadRootPageControllers (new [] { nameof (WorkoutInterfaceController) },
															new [] { workoutConfiguration },
															WKPageOrientation.Vertical,
															0);
		}
	}
}
