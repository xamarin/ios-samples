using System;
using Foundation;
using UIKit;
using HealthKit;
using ObjCRuntime;

namespace Fit
{
	public partial class JournalViewController : UITableViewController, IHealthStore
	{
		readonly NSString CellReuseIdentifier = new NSString ("cell");
		NSEnergyFormatter energyFormatter;

		public HKHealthStore HealthStore { get; set; }

		public NSMutableArray FoodItems { get; private set; }

		public NSEnergyFormatter EnergyFormatter {
			get {
				if (energyFormatter == null) {
					energyFormatter = new NSEnergyFormatter {
						UnitStyle = NSFormattingUnitStyle.Long,
						ForFoodEnergyUse = true,
					};

					energyFormatter.NumberFormatter.MaximumFractionDigits = 2;
				}

				return energyFormatter;
			}
		}

		public JournalViewController (IntPtr handle) : base (handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			FoodItems = new NSMutableArray ();
			if(HealthStore != null)
				UpdateJournal (null, null);
			UIApplication.Notifications.ObserveDidBecomeActive (UpdateJournal);
		}

		void UpdateJournal (object sender, EventArgs args)
		{
			var calendar = NSCalendar.CurrentCalendar;

			var startDate = DateTime.Now.Date;
			var endDate = startDate.AddDays(1);

			var sampleType = HKSampleType.GetQuantityType (HKQuantityTypeIdentifierKey.DietaryEnergyConsumed);
			var predicate = HKQuery.GetPredicateForSamples ((NSDate)startDate, (NSDate)endDate, HKQueryOptions.None);

			var query = new HKSampleQuery (sampleType, predicate, 0, new NSSortDescriptor[0], (resultQuery, results, error) => {
				if (error != null) {
					Console.WriteLine ("An error occured fetching the user's tracked food. " +
					"In your app, try to handle this gracefully. The error was: {0}.", error.LocalizedDescription);
					return;
				}

				InvokeOnMainThread (() => {
					FoodItems.RemoveAllObjects ();
					foreach (HKQuantitySample sample in results) {

						var foodName = (NSString)sample.Metadata.Dictionary [HKMetadataKey.FoodType];
						double joules = sample.Quantity.GetDoubleValue (HKUnit.Joule);
						var foodItem = FoodItem.Create (foodName, joules);

						FoodItems.Add (foodItem);
					}

					TableView.ReloadData ();
				});
			});

			HealthStore.ExecuteQuery (query);
		}

		public void AddFoodItem (FoodItem item)
		{
			var quantityType = HKQuantityType.GetQuantityType (HKQuantityTypeIdentifierKey.DietaryEnergyConsumed);
			var quantity = HKQuantity.FromQuantity (HKUnit.Joule, item.Joules);

			var now = NSDate.Now;

			var metadata = new NSDictionary (HKMetadataKey.FoodType, item.Name);
			var caloriesSample = HKQuantitySample.FromType (quantityType, quantity, now, now, metadata);

			HealthStore.SaveObject (caloriesSample, (success, error) => {
				if (success) {
					FoodItems.Insert (item, 0);
					var indexPathForInsertedFoodItem = NSIndexPath.FromRowSection (0, 0);
					InvokeOnMainThread (() => {
						TableView.InsertRows (new NSIndexPath[] { indexPathForInsertedFoodItem }, UITableViewRowAnimation.Automatic);
					});
				} else {
					Console.WriteLine ("An error occured saving the food {0}. In your app, try to handle this gracefully. " +
					"The error was: {1}.", item.Name, error);
				}
			});
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return (nint)FoodItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = TableView.DequeueReusableCell (CellReuseIdentifier, indexPath);
			var foodItem = FoodItems.GetItem<FoodItem> ((nuint)indexPath.Row);
			cell.TextLabel.Text = foodItem.Name;
			cell.DetailTextLabel.Text = EnergyFormatter.StringFromValue (foodItem.Joules, NSEnergyFormatterUnit.Joule);
			return cell;
		}
	}
}
