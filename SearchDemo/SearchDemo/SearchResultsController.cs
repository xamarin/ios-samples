using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.Drawing;

namespace SearchDemo
{
    public partial class SearchResultsController : UITableViewController
    {
        static NSString cellId = new NSString ("SearchResultCell");
        List<SearchItem> searchResults;
        UISearchBar searchBar;

        public SearchResultsController ()
        {
            searchResults = new List<SearchItem> ();
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            Title = "Bing Search Demo ";
            TableView.Source = new TableSource (this);
            
            searchBar = new UISearchBar ();
            searchBar.Placeholder = "Enter Search Text";
            searchBar.SizeToFit ();
            searchBar.AutocorrectionType = UITextAutocorrectionType.No;
            searchBar.AutocapitalizationType = UITextAutocapitalizationType.None;
            searchBar.SearchButtonClicked += (sender, e) => {
                Search (); 
            };
            
            TableView.TableHeaderView = searchBar;
        }

        void Search ()
        {
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
            var bing = new Bing (SyncToMain);
            bing.Search (searchBar.Text);
            searchBar.ResignFirstResponder ();
        }

        void SyncToMain (List<SearchItem> results)
        {
            this.InvokeOnMainThread (delegate {
                if (results != null) {
                    searchResults = results;    
                    TableView.ReloadData ();
                } else {
                    new UIAlertView ("", "Could not retrieve results", null, "OK").Show ();
                }

                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            });
        }

        class TableSource : UITableViewSource
        {
            SearchResultsController controller;

            public TableSource (SearchResultsController controller)
            {
                this.controller = controller;
            }

            public override nint RowsInSection (UITableView tableView, nint section)
            {
                return controller.searchResults.Count;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell = tableView.DequeueReusableCell (cellId);
                
                if (cell == null)
                    cell = new UITableViewCell (
                        UITableViewCellStyle.Default,
                        cellId
                    );
                
				cell.TextLabel.Text = controller.searchResults [(int)indexPath.Row].Title;
                
                return cell;
            }

            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
				var vc = new SearchItemViewController (){Item = controller.searchResults [(int)indexPath.Row]};
                controller.NavigationController.PushViewController (vc, true);
            }
        }
    }
}


