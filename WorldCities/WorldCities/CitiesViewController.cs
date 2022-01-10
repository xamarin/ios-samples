using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace WorldCities
{
    public partial class CitiesViewController : UITableViewController
    {
        private const string CellIdentifier = "cellIdentifier";

        private readonly List<WorldCity> cities = new List<WorldCity>();

        public event EventHandler<CityEventArgs> CityChanged;

        public CitiesViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var path = NSBundle.MainBundle.PathForResource("CityList", "plist");
            var citiesArray = NSArray.FromFile(path);
            foreach (var city in NSArray.FromArray<NSDictionary>(citiesArray))
            {
                cities.Add(new WorldCity(city[(NSString)"cityNameKey"].ToString(),
                                         double.Parse(city[(NSString)"latitudeKey"].ToString()),
                                         double.Parse(city[(NSString)"longitudeKey"].ToString())));
            }
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return cities.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier);
            var city = cities[indexPath.Row];

            cell.TextLabel.Text = city.Name;
            cell.DetailTextLabel.Text = $"{city.Latitude} {city.Longitude}";

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            CityChanged?.Invoke(this, new CityEventArgs(cities[indexPath.Row]));
            DismissViewController(true, null);
        }
    }
}