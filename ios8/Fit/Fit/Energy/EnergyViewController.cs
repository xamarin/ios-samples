using System;
using Foundation;
using HealthKit;
using ObjCRuntime;
using UIKit;

namespace Fit
{
	public partial class EnergyViewController : UITableViewController, IHealthStore
	{
		double simulatedBurntEnergy;
		double consumedEnergy;
		double netEnergy;
		NSEnergyFormatter energyFormatter;

		NSEnergyFormatter EnergyFormatter {
			get {
				if (energyFormatter == null) {
					energyFormatter = new NSEnergyFormatter {
						UnitStyle = NSFormattingUnitStyle.Long,
						ForFoodEnergyUse = true
					};

					energyFormatter.NumberFormatter.MaximumFractionDigits = 2;
				}

				return energyFormatter;
			}
		}

		public HKHealthStore HealthStore  { get; set; }

		public double SimulatedBurntEnergy {
			get {
				return simulatedBurntEnergy;
			}

			set {
				simulatedBurntEnergy = value;
				simulatedBurntEnergyValueLabel.Text = EnergyFormatter.StringFromJoules (simulatedBurntEnergy);
			}
		}

		public double ConsumedEnergy {
			get {
				return consumedEnergy;
			}

			set {
				consumedEnergy = value;
				consumedEnergyValueLabel.Text = EnergyFormatter.StringFromJoules (consumedEnergy);
			}
		}

		public double NetEnergy {
			get {
				return netEnergy;
			}

			set {
				netEnergy = value;
				netEnergyValueLabel.Text = EnergyFormatter.StringFromJoules (netEnergy);
			}
		}

		public EnergyViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			RefreshControl.ValueChanged += RefreshStatistics;
			RefreshStatistics (null, null);
			UIApplication.Notifications.ObserveDidBecomeActive (RefreshStatistics);
		}

		void RefreshStatistics (object sender, EventArgs args)
		{
			RefreshControl.BeginRefreshing ();
			FetchMostRecentData ((totalJoulesConsumed, error) => {
				InvokeOnMainThread (delegate {
					SimulatedBurntEnergy = new Random ().Next (0, 300000);
					ConsumedEnergy = totalJoulesConsumed;
					NetEnergy = consumedEnergy - simulatedBurntEnergy;
					RefreshControl.EndRefreshing ();
				});
			});
		}

		void FetchMostRecentData (Action <double, NSError> completionHandler)
		{
			var calendar = NSCalendar.CurrentCalendar;
			var startDate = DateTime.Now.Date;
			var endDate = startDate.AddDays (1);

			var sampleType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.DietaryEnergyConsumed);
			var predicate = HKQuery.GetPredicateForSamples ((NSDate)startDate, (NSDate)endDate, HKQueryOptions.StrictStartDate);

			var query = new HKStatisticsQuery (sampleType, predicate, HKStatisticsOptions.CumulativeSum,
				            (HKStatisticsQuery resultQuery, HKStatistics results, NSError error) => {

					if (error != null && completionHandler != null)
						completionHandler (0.0f, error);

					var totalCalories = results.SumQuantity ();
					if (totalCalories == null)
						totalCalories = HKQuantity.FromQuantity (HKUnit.Joule, 0.0);

					if (completionHandler != null)
						completionHandler (totalCalories.GetDoubleValue (HKUnit.Joule), error);
			});

			HealthStore.ExecuteQuery (query);
		}
	}
}
