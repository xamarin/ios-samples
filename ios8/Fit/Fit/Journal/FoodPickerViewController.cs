using System;
using Foundation;
using UIKit;

namespace Fit
{
	public partial class FoodPickerViewController : UITableViewController
	{
		readonly NSString CellIdentifier = new NSString ("cell");
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

		public NSArray FoodItems { get; private set; }

		public FoodItem SelectedFoodItem { get; private set; }

		public FoodPickerViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			FoodItems = NSArray.FromObjects (new object[] {
				FoodItem.Create ("Wheat Bagel", 240000.0),
				FoodItem.Create ("Bran with Raisins", 190000.0),
				FoodItem.Create ("Regular Instant Coffee", 1000.0),
				FoodItem.Create ("Banana", 439320.0),
				FoodItem.Create ("Cranberry Bagel", 416000.0),
				FoodItem.Create ("Oatmeal", 150000.0),
				FoodItem.Create ("Fruits Salad", 60000.0),
				FoodItem.Create ("Fried Sea Bass", 200000.0),
				FoodItem.Create ("Chips", 190000.0),
				FoodItem.Create ("Chicken Taco", 170000.0)
			});
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return (nint)FoodItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = TableView.DequeueReusableCell (CellIdentifier, indexPath);
			var foodItem = FoodItems.GetItem<FoodItem> ((nuint)indexPath.Row);

			cell.TextLabel.Text = foodItem.Name;
			cell.DetailTextLabel.Text = EnergyFormatter.StringFromJoules (foodItem.Joules);
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			NSIndexPath indexPathForSelectedRow = TableView.IndexPathForSelectedRow;
			SelectedFoodItem = FoodItems.GetItem<FoodItem> ((nuint)indexPathForSelectedRow.Row);
			((JournalViewController)NavigationController.ViewControllers [NavigationController.ViewControllers.Length - 2]).
				AddFoodItem (SelectedFoodItem);
			NavigationController.PopViewController (true);
		}
	}
}
