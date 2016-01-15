using System;
using UIKit;
using Foundation;
using MapKit;
using System.Collections.Generic;
using CoreLocation;
using System.Linq;


namespace MapDemo
{
	public class SearchResultsViewController : UITableViewController
	{
		static readonly string mapItemCellId = "mapItemCellId";
		MKMapView map;

		public List<MKMapItem> MapItems { get; set; }

		public SearchResultsViewController (MKMapView map)
		{
			this.map = map;

			MapItems = new List<MKMapItem> ();
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return MapItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (mapItemCellId);

			if (cell == null)
				cell = new UITableViewCell ();

			cell.TextLabel.Text = MapItems [indexPath.Row].Name;
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{

			// add item to map
			CLLocationCoordinate2D coord = MapItems [indexPath.Row].Placemark.Location.Coordinate;
			map.AddAnnotations (new MKPointAnnotation () {
				Title = MapItems [indexPath.Row].Name,
				Coordinate = coord
			});

			map.SetCenterCoordinate (coord, true);

			DismissViewController (false, null);
		}

		public void Search (string forSearchString)
		{
			// create search request
			var searchRequest = new MKLocalSearchRequest ();
			searchRequest.NaturalLanguageQuery = forSearchString;
			searchRequest.Region = new MKCoordinateRegion (map.UserLocation.Coordinate, new MKCoordinateSpan (0.25, 0.25));

			// perform search
			var localSearch = new MKLocalSearch (searchRequest);

			localSearch.Start (delegate (MKLocalSearchResponse response, NSError error) {
				if (response != null && error == null) {
					this.MapItems = response.MapItems.ToList ();
					this.TableView.ReloadData ();
				} else {
					Console.WriteLine ("local search error: {0}", error);
				}
			});


		}
	}
}

