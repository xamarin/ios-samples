using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace TableSearch
{
    /// <summary>
    /// The application's primary table view controller showing a list of products.
    /// </summary>
    public partial class MainTableViewController : BaseTableViewController, 
                                                   IUISearchBarDelegate, 
                                                   IUISearchControllerDelegate, 
                                                   IUISearchResultsUpdating
    {
        // Data model for the table view.
        private readonly List<Product> products = new List<Product> 
        {
            new Product ("iPhone", 2007, 599),
            new Product ("iPod", 2001, 399),
            new Product ("iPod touch", 2007, 210),
            new Product ("iPad", 2010, 499),
            new Product ("iPad mini", 2012, 659),
            new Product ("iMac", 1997, 1299),
            new Product ("Mac Pro", 2006, 2499),
            new Product ("MacBook Air", 2008, 1799),
            new Product ("MacBook Pro", 2006, 1499),
        };

        /*
         * They are implicitly unwrapped optionals because they are used in many other places throughout this view controller.
         */

        // Search controller to help us with filtering.
        private UISearchController searchController;

        // Secondary search results table view.
        private ResultsTableController resultsTableController;

        // Restoration state for UISearchController
        private readonly SearchControllerRestorableState restoredState = new SearchControllerRestorableState();

        public MainTableViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.resultsTableController = new ResultsTableController();
            this.resultsTableController.TableView.Delegate = this;

            this.searchController = new UISearchController(this.resultsTableController) { SearchResultsUpdater = this };
            this.searchController.SearchBar.AutocapitalizationType = UITextAutocapitalizationType.None;

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                // For iOS 11 and later, place the search bar in the navigation bar.
                base.NavigationItem.SearchController = this.searchController;

                // Make the search bar always visible.
                base.NavigationItem.HidesSearchBarWhenScrolling = false;
            }
            else
            {
                // For iOS 10 and earlier, place the search controller's search bar in the table view's header.
                base.TableView.TableHeaderView = this.searchController.SearchBar;
            }

            this.searchController.Delegate = this;
            this.searchController.DimsBackgroundDuringPresentation = false; // The default is true.
            this.searchController.SearchBar.Delegate = this;// Monitor when the search button is tapped.

            /*
             * Search presents a view controller by applying normal view controller presentation semantics.
             * This means that the presentation moves up the view controller hierarchy until it finds the root
             * view controller or one that defines a presentation context.
             */

            /*
             * Specify that this view controller determines how the search controller is presented.
             * The search controller should be presented modally and match the physical size of this view controller.
             */

            this.DefinesPresentationContext = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // Restore the searchController's active state.
            if (this.restoredState.WasActive)
            {
                this.searchController.Active = this.restoredState.WasActive;
                this.restoredState.WasActive = false;

                if (this.restoredState.WasFirstResponder)
                {
                    this.searchController.SearchBar.BecomeFirstResponder();
                    this.restoredState.WasFirstResponder = false;
                }
            }
        }

        #region UITableViewDelegate

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            // Check to see which table view cell was selected.
            var selectedProduct = tableView == base.TableView ? this.products[indexPath.Row] :
                                                                resultsTableController.FilteredProducts[indexPath.Row];

            // Set up the detail view controller to show.
            var detailViewController = DetailViewController.Create(selectedProduct);
            base.NavigationController?.PushViewController(detailViewController, true);

            tableView.DeselectRow(indexPath, false);
        }

        #endregion

        #region UITableViewDataSource

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.products.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(BaseTableViewController.TableViewCellIdentifier, indexPath);
            base.ConfigureCell(cell, products[indexPath.Row]);

            return cell;
        }

        #endregion

        #region IUISearchBarDelegate

        [Export("searchBarSearchButtonClicked:")]
        public void SearchButtonClicked(UISearchBar searchBar)
        {
            searchBar.ResignFirstResponder();
        }

        #endregion

        #region IUISearchControllerDelegate

        // Use these delegate functions for additional control over the search controller.

        [Export("presentSearchController:")]
        public void PresentSearchController(UISearchController searchController)
        {
            Console.WriteLine("UISearchControllerDelegate invoked method: 'PresentSearchController'.");
        }

        [Export("willPresentSearchController:")]
        public void WillPresentSearchController(UISearchController searchController)
        {
            Console.WriteLine("UISearchControllerDelegate invoked method: 'WillPresentSearchController'.");
        }

        [Export("didPresentSearchController:")]
        public void DidPresentSearchController(UISearchController searchController)
        {
            Console.WriteLine("UISearchControllerDelegate invoked method: 'DidPresentSearchController'.");
        }

        [Export("didDismissSearchController:")]
        public void DidDismissSearchController(UISearchController searchController)
        {
            Console.WriteLine("UISearchControllerDelegate invoked method: 'DidDismissSearchController'.");
        }

        [Export("willDismissSearchController:")]
        public void WillDismissSearchController(UISearchController searchController)
        {
            Console.WriteLine("UISearchControllerDelegate invoked method: 'WillDismissSearchController'.");
        }

        #endregion

        #region IUISearchResultsUpdating

        public void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            // Strip out all the leading and trailing spaces.
            var searchItems = searchController.SearchBar.Text.Trim().Split(' ');

            /* here you can choose the way how to do search */

            var filteredResults = this.PerformSearch_Swift(searchItems);
            //var filteredResults = this.PerformSearch_CSharp(searchItems);

            // Apply the filtered results to the search results table.
            if (searchController.SearchResultsController is ResultsTableController resultsController)
            {
                resultsController.FilteredProducts = filteredResults;
                resultsController.TableView.ReloadData();
            }
        }

        private NSCompoundPredicate FindMatches(string searchString)
        {
            /*
             * Each searchString creates an OR predicate for: name, yearIntroduced, introPrice.
             * Example if searchItems contains "Gladiolus 51.99 2001":
             * - name CONTAINS[c] "gladiolus"
             * - name CONTAINS[c] "gladiolus", yearIntroduced ==[c] 2001, introPrice ==[c] 51.99
             * - name CONTAINS[c] "ginger", yearIntroduced ==[c] 2007, introPrice ==[c] 49.98
             */
            var searchItemsPredicate = new List<NSPredicate>();

            /*
             * Below we use NSExpression represent expressions in our predicates.
             * NSPredicate is made up of smaller, atomic parts:
             * two NSExpressions (a left-hand value and a right-hand value).
             */

            // Name field matching.
            var titleExpression = NSExpression.FromKeyPath(ExpressionKeys.Title);
            var searchStringExpression = NSExpression.FromConstant(new NSString(searchString));

            var titleSearchComparisonPredicate = new NSComparisonPredicate(titleExpression,
                                                                           searchStringExpression,
                                                                           NSComparisonPredicateModifier.Direct,
                                                                           NSPredicateOperatorType.Contains,
                                                                           NSComparisonPredicateOptions.CaseInsensitive | NSComparisonPredicateOptions.DiacriticInsensitive);

            searchItemsPredicate.Add(titleSearchComparisonPredicate);

            var numberFormatter = new NSNumberFormatter
            {
                NumberStyle = NSNumberFormatterStyle.None,
                FormatterBehavior = NSNumberFormatterBehavior.Default
            };

            // The `searchString` may fail to convert to a number.
            var targetNumber = numberFormatter.NumberFromString(searchString);
            if (targetNumber != null)
            {
                // Use `targetNumberExpression` in both the following predicates.
                var targetNumberExpression = NSExpression.FromConstant(targetNumber);

                // The `yearIntroduced` field matching.
                var yearIntroducedExpression = NSExpression.FromKeyPath(ExpressionKeys.YearIntroduced);
                var yearIntroducedPredicate = new NSComparisonPredicate(yearIntroducedExpression, 
                                                                        targetNumberExpression, 
                                                                        NSComparisonPredicateModifier.Direct, 
                                                                        NSPredicateOperatorType.EqualTo, 
                                                                        NSComparisonPredicateOptions.CaseInsensitive | NSComparisonPredicateOptions.DiacriticInsensitive);

                searchItemsPredicate.Add(yearIntroducedPredicate);

                // The `price` field matching.
                var priceExpression = NSExpression.FromKeyPath(ExpressionKeys.IntroPrice);
                var finalPredicate = new NSComparisonPredicate(priceExpression, 
                                                               targetNumberExpression, 
                                                               NSComparisonPredicateModifier.Direct,
                                                               NSPredicateOperatorType.EqualTo,
                                                               NSComparisonPredicateOptions.CaseInsensitive | NSComparisonPredicateOptions.DiacriticInsensitive);

                searchItemsPredicate.Add(finalPredicate);
            }

            return NSCompoundPredicate.CreateOrPredicate(searchItemsPredicate.ToArray());
        }

        private List<Product> PerformSearch_Swift(string[] searchItems)
        {
            // Update the filtered array based on the search text.
            var searchResults = this.products;

            // Build all the "AND" expressions for each value in searchString.
            var andMatchPredicates = searchItems.Select(searchItem => FindMatches(searchItem)).ToArray();

            // Match up the fields of the Product object.
            var finalCompoundPredicate = NSCompoundPredicate.CreateAndPredicate(andMatchPredicates);
            var filteredResults = searchResults.Where(searchResult => finalCompoundPredicate.EvaluateWithObject(searchResult));

            return filteredResults.ToList();
        }

        private List<Product> PerformSearch_CSharp(string[] searchItems)
        {
            var results = new List<Product>();
            // Update the filtered array based on the search text.
            var localProducts = this.products;

            foreach (var searchItem in searchItems)
            {
                int.TryParse(searchItem, out int @int);
                double.TryParse(searchItem, out double @double);
                results.AddRange(localProducts.Where(product => product.Title.Contains(searchItem, StringComparison.OrdinalIgnoreCase) ||
                                                     product.IntroPrice == @double || 
                                                     product.YearIntroduced == @int));
            }
                       
            return results.Distinct().ToList();
        }

        #endregion

        #region UIStateRestoration

        public override void EncodeRestorableState(NSCoder coder)
        {
            base.EncodeRestorableState(coder);

            // Encode the view state so it can be restored later.

            // Encode the title.
            coder.Encode(new NSString(base.NavigationItem.Title), RestorationKeys.ViewControllerTitle);

            // Encode the search controller's active state.
            coder.Encode(this.searchController.Active, RestorationKeys.SearchControllerIsActive);

            // Encode the first responser status.
            coder.Encode(this.searchController.SearchBar.IsFirstResponder, RestorationKeys.SearchBarIsFirstResponder);

            // Encode the search bar text.
            coder.Encode(new NSString(this.searchController.SearchBar.Text), RestorationKeys.SearchBarText);
        }

        public override void DecodeRestorableState(NSCoder coder)
        {
            base.DecodeRestorableState(coder);

            // Restore the title.
            if (coder.DecodeObject(RestorationKeys.ViewControllerTitle) is NSString decodedTitle)
            {
                base.NavigationItem.Title = decodedTitle;
            }
            else
            {
                throw new Exception("A title did not exist. In your app, handle this gracefully.");
            }

            /*
             * Restore the active state:
             * We can't make the searchController active here since it's not part of the view
             * hierarchy yet, instead we do it in viewWillAppear.
             */
            this.restoredState.WasActive = coder.DecodeBool(RestorationKeys.SearchControllerIsActive);

            /*
             * Restore the first responder status:
             * Like above, we can't make the searchController first responder here since it's not part of the view
             * hierarchy yet, instead we do it in viewWillAppear.
             */
            this.restoredState.WasFirstResponder = coder.DecodeBool(RestorationKeys.SearchBarIsFirstResponder);

            // Restore the text in the search field.
            this.searchController.SearchBar.Text = coder.DecodeObject(RestorationKeys.SearchBarText) as NSString;
        }

        #endregion

        private class SearchControllerRestorableState
        {
            public bool WasActive { get; set; }
            public bool WasFirstResponder { get; set; }
        }

        /// <summary>
        /// State restoration values.
        /// </summary>
        private static class RestorationKeys
        {
            public const string ViewControllerTitle = "viewControllerTitle";
            public const string SearchControllerIsActive = "searchControllerIsActive";
            public const string SearchBarText = "searchBarText";
            public const string SearchBarIsFirstResponder = "searchBarIsFirstResponder";
        }

        /// <summary>
        /// NSPredicate expression keys.
        /// </summary>
        private static class ExpressionKeys
        {
            public const string Title = "title";
            public const string YearIntroduced = "yearIntroduced";
            public const string IntroPrice = "introPrice";
        }
    }
}