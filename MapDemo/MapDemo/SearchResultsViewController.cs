using Foundation;
using MapKit;
using System;
using System.Collections.Generic;
using UIKit;

namespace MapDemo
{
    public class SearchResultsUpdator : UISearchResultsUpdating
    {
        public event Action<string> UpdateSearchResults;

        public override void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            UpdateSearchResults?.Invoke(searchController.SearchBar.Text);
        }
    }

    public class SearchResultsViewController : UITableViewController
    {
        private const string CellIdentifier = "mapItemCellId";

        private List<MKMapItem> items;

        private MKMapView map;

        public SearchResultsViewController(MKMapView map)
        {
            this.map = map;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return items?.Count ?? 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier) ?? new UITableViewCell();
            cell.TextLabel.Text = items[indexPath.Row].Name;
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            // add item to map
            var coordinate = items[indexPath.Row].Placemark.Location.Coordinate;
            map.AddAnnotations(new MKPointAnnotation
            {
                Coordinate = coordinate,
                Title = items[indexPath.Row].Name,
            });

            map.SetCenterCoordinate(coordinate, true);
            DismissViewController(false, null);
        }

        public void UpdateSearchResults(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                // create search request
                var searchRequest = new MKLocalSearchRequest
                {
                    NaturalLanguageQuery = query,
                    Region = new MKCoordinateRegion(map.UserLocation.Coordinate, new MKCoordinateSpan(0.25, 0.25))
                };

                // perform search
                var localSearch = new MKLocalSearch(searchRequest);
                localSearch.Start((response, error) =>
                {
                    if (response != null && error == null)
                    {
                        items = new List<MKMapItem>(response.MapItems);
                        this.TableView.ReloadData();
                    }
                    else
                    {
                        Console.WriteLine($"local search error: {error?.LocalizedDescription ?? ""}");
                    }
                });
            }
        }
    }
}