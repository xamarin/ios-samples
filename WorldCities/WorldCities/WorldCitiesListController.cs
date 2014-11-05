using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace WorldCities
{
	public partial class WorldCitiesListController : UIViewController
	{
		const string MoveToMapSegueName = "MoveToMap";
		
		List<WorldCity> cityList = new List<WorldCity> ();
		
		public WorldCity SelectedCity { get; set; }
		
		public WorldCitiesListController (IntPtr handle) : base (handle)
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var path = NSBundle.MainBundle.PathForResource ("CityList", "plist");
			var cities = NSArray.FromFile (path);
			foreach (var city in NSArray.FromArray <NSDictionary> (cities)) {
				cityList.Add (new WorldCity (city[(NSString)"cityNameKey"].ToString (),
					double.Parse (city[(NSString)"latitudeKey"].ToString ()),
					double.Parse (city[(NSString)"longitudeKey"].ToString ())));
			}
			
			TableView.Source = new MyTableViewDelegate (this);
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}
		
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == MoveToMapSegueName) {
				var view = (WorldCitiesViewController)segue.DestinationViewController;
				view.SelectedCity = SelectedCity;
			}
		}
		
		class MyTableViewDelegate : UITableViewSource
		{
			WorldCitiesListController controller;
			
			public MyTableViewDelegate (WorldCitiesListController controller)
			{
				this.controller = controller;
			}
			
			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return controller.cityList.Count; 
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				const string cellName = "Cell";
				var cell = tableView.DequeueReusableCell (cellName);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellName);
					cell.EditingAccessory = UITableViewCellAccessory.DetailDisclosureButton;
				}
				var city = controller.cityList [indexPath.Row];
				cell.TextLabel.Text = city.Name;
				cell.DetailTextLabel.Text = string.Format ("{0} {1}", city.Latitude, city.Longitude);
				return cell;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				controller.SelectedCity = controller.cityList [indexPath.Row];
				controller.PerformSegue (MoveToMapSegueName, controller);
			}
		}
	}
}
