using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace NavigationBar
{
    public class CitiesDataSource : UITableViewDataSource
    {
        private readonly List<string> cities = new List<string>();

        public CitiesDataSource()
        {
            var citiesJSONURL = NSBundle.MainBundle.PathForResource("Cities", "json");
            var citiesJSONData = NSData.FromFile(citiesJSONURL);
            var jsonObject = NSJsonSerialization.Deserialize(citiesJSONData, default(NSJsonReadingOptions), out NSError error);
            if (jsonObject is NSArray jsonCities)
            {
                for (nuint i = 0; i < jsonCities.Count; i++)
                {
                    cities.Add(jsonCities.GetItem<NSString>(i));
                }
            }
        }

        public string this[int index] => this.cities[index];

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var flavor = this.cities[indexPath.Row];
            var cell = tableView.DequeueReusableCell("Cell", indexPath);
            cell.TextLabel.Text = flavor;
            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.cities.Count;
        }
    }
}
