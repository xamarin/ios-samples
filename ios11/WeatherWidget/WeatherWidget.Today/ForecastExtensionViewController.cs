using CoreGraphics;
using Foundation;
using NotificationCenter;
using System;
using System.Collections.Generic;
using UIKit;

namespace WeatherWidget.Today {
	/// <summary>
	/// The widget view controller to show either today's weather forecast or a whole week of forecasts.
	/// </summary>
	public partial class ForecastExtensionViewController : UITableViewController, INCWidgetProviding {
		private List<WeatherForecast> weatherForecastData = WeatherForecast.LoadSharedData ();

		public ForecastExtensionViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Allow the today widget to be expanded or contracted.
			base.ExtensionContext?.SetWidgetLargestAvailableDisplayMode (NCWidgetDisplayMode.Expanded);

			// Register the table view cell.
			var weatherForecastTableViewCellNib = UINib.FromName ("ForecastTableViewCell", null);
			base.TableView.RegisterNibForCellReuse (weatherForecastTableViewCellNib, ForecastTableViewCell.ReuseIdentifier);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			base.TableView.ReloadData ();
		}

		#region Widget provider protocol

		[Export ("widgetPerformUpdateWithCompletionHandler:")]
		public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
		{
			// Reload the data from disk
			this.weatherForecastData = WeatherForecast.LoadSharedData ();
			completionHandler (NCUpdateResult.NewData);
		}

		[Export ("widgetActiveDisplayModeDidChange:withMaximumSize:")]
		public void WidgetActiveDisplayModeDidChange (NCWidgetDisplayMode activeDisplayMode, CGSize maxSize)
		{
			switch (activeDisplayMode) {
			case NCWidgetDisplayMode.Compact:
				// The compact view is a fixed size.
				base.PreferredContentSize = maxSize;
				break;

			case NCWidgetDisplayMode.Expanded:
				// Dynamically calculate the height of the cells for the extended height.
				var height = 0f;
				for (var index = 0; index < this.weatherForecastData.Count; index++) {
					switch (index) {
					case 0:
						height += ForecastTableViewCell.TodayCellHeight;
						break;

					default:
						height += ForecastTableViewCell.StandardCellHeight;
						break;
					}
				}

				base.PreferredContentSize = new CGSize (maxSize.Width, NMath.Min (height, maxSize.Height));
				break;
			}
		}

		#endregion

		#region Content container protocol

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);

			var updatedVisibleCellCount = this.NumberOfTableRowsToDisplay ();
			var currentVisibleCellCount = base.TableView.VisibleCells.Length;
			var cellCountDifference = updatedVisibleCellCount - currentVisibleCellCount;

			// If the number of visible cells has changed, animate them in/out along with the resize animation.
			if (cellCountDifference != 0) {
				coordinator.AnimateAlongsideTransition ((_) => {
					base.TableView.PerformBatchUpdates (() => {
						var indexPaths = new List<NSIndexPath> ();
						// Build an array of IndexPath objects representing the rows to be inserted or deleted.
						for (int i = 1; i <= Math.Abs (cellCountDifference); i++) {
							indexPaths.Add (NSIndexPath.FromRowSection (i, 0));
						}

						// Animate the insertion or deletion of the rows.
						if (cellCountDifference > 0) {
							base.TableView.InsertRows (indexPaths.ToArray (), UITableViewRowAnimation.Fade);
						} else {
							base.TableView.DeleteRows (indexPaths.ToArray (), UITableViewRowAnimation.Fade);
						}
					}, null);
				}, null);
			}
		}

		#endregion

		#region Table view data source

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return this.NumberOfTableRowsToDisplay ();
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (ForecastTableViewCell.ReuseIdentifier, indexPath) as ForecastTableViewCell;

			var weatherForecast = weatherForecastData [indexPath.Row];
			cell.Label = weatherForecast.DaysFromNowDescription;
			cell.Image = weatherForecast.Forecast.GetImageAsset ();
			cell.ForecastLabel = weatherForecast.Forecast.GetDescription ();
			return cell;
		}

		#endregion

		#region Table view delegate

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Row) {
			case 0:
				return ForecastTableViewCell.TodayCellHeight;
			default:
				return ForecastTableViewCell.StandardCellHeight;
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// Open the main app at the correct page for the day tapped in the widget.
			var weatherForecast = this.weatherForecastData [indexPath.Row];
			var appURL = new NSUrl ($"weatherwidget://?daysFromNow={weatherForecast.daysFromNow}");
			base.ExtensionContext?.OpenUrl (appURL, null);

			// Don't leave the today extension with a selected row.
			tableView.DeselectRow (indexPath, true);
		}

		#endregion

		private int NumberOfTableRowsToDisplay ()
		{
			if (base.ExtensionContext?.GetWidgetActiveDisplayMode () == NCWidgetDisplayMode.Compact) {
				return 1;
			} else {
				return this.weatherForecastData.Count;
			}
		}
	}
}
