using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace TableSearch
{
    /// <summary>
    /// The application's primary table view controller showing a list of products.
    /// </summary>
    public partial class MainTableViewController : BaseTableViewController, IUISearchBarDelegate, IUISearchControllerDelegate, IUISearchResultsUpdating
    {
        /// Data model for the table view.
        private List<Product> products = new List<Product>();

        /** The following 2 properties are set in viewDidLoad(),
            They are implicitly unwrapped optionals because they are used in many other places
            throughout this view controller.
        */

        /// Search controller to help us with filtering.
        private UISearchController searchController;

        /// Secondary search results table view.
        private ResultsTableController resultsTableController;

        /// Restoration state for UISearchController
        private SearchControllerRestorableState restoredState = new SearchControllerRestorableState();

        public MainTableViewController(IntPtr handle) : base(handle)
        {
            products = new List<Product> {
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
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.resultsTableController = new ResultsTableController();

            this.resultsTableController.TableView.Delegate = this;


            searchController = new UISearchController(resultsTableController);
            searchController.SearchResultsUpdater = this;
            searchController.SearchBar.AutocapitalizationType = UITextAutocapitalizationType.None;

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                // For iOS 11 and later, place the search bar in the navigation bar.
                base.NavigationItem.SearchController = searchController;

                // Make the search bar always visible.
                base.NavigationItem.HidesSearchBarWhenScrolling = false;
            }
            else
            {
                // For iOS 10 and earlier, place the search controller's search bar in the table view's header.
                base.TableView.TableHeaderView = searchController.SearchBar;
            }

            searchController.Delegate = this;
            searchController.DimsBackgroundDuringPresentation = false; // The default is true.
            searchController.SearchBar.Delegate = this;// Monitor when the search button is tapped.

            /** Search presents a view controller by applying normal view controller presentation semantics.
                This means that the presentation moves up the view controller hierarchy until it finds the root
                view controller or one that defines a presentation context.
            */

            /** Specify that this view controller determines how the search controller is presented.
                The search controller should be presented modally and match the physical size of this view controller.
            */
            this.DefinesPresentationContext = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // Restore the searchController's active state.
            if (restoredState.wasActive)
            {
                searchController.Active = restoredState.wasActive;
                restoredState.wasActive = false;

                if (restoredState.wasFirstResponder)
                {
                    searchController.SearchBar.BecomeFirstResponder();
                    restoredState.wasFirstResponder = false;
                }
            }
        }

        #region UITableViewDelegate

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            Product selectedProduct = null;

            // Check to see which table view cell was selected.
            if (tableView == base.TableView)
            {
                selectedProduct = products[indexPath.Row];
            }
            else
            {
                selectedProduct = resultsTableController.filteredProducts[indexPath.Row];
            }

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
            var cell = tableView.DequeueReusableCell(BaseTableViewController.tableViewCellIdentifier, indexPath);

            var product = products[indexPath.Row];
            base.ConfigureCell(cell, product);


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

        private NSCompoundPredicate FindMatches(string searchString)
        {
            /** Each searchString creates an OR predicate for: name, yearIntroduced, introPrice.
                Example if searchItems contains "Gladiolus 51.99 2001":
                name CONTAINS[c] "gladiolus"
                name CONTAINS[c] "gladiolus", yearIntroduced ==[c] 2001, introPrice ==[c] 51.99
                name CONTAINS[c] "ginger", yearIntroduced ==[c] 2007, introPrice ==[c] 49.98
            */
            var searchItemsPredicate = new List<NSPredicate>();

            /** Below we use NSExpression represent expressions in our predicates.
                NSPredicate is made up of smaller, atomic parts:
                two NSExpressions (a left-hand value and a right-hand value).
            */

            // Name field matching.
            var titleExpression = NSExpression.FromKeyPath(ExpressionKeys.title);
            var searchStringExpression = NSExpression.FromConstant(new NSString(searchString));

            var titleSearchComparisonPredicate = new NSComparisonPredicate(titleExpression,
                                  searchStringExpression,
                                   NSComparisonPredicateModifier.Direct,
                                   NSPredicateOperatorType.Contains,
                                   NSComparisonPredicateOptions.CaseInsensitive | NSComparisonPredicateOptions.DiacriticInsensitive);

            searchItemsPredicate.Add(titleSearchComparisonPredicate);

            var numberFormatter = new NSNumberFormatter();
            numberFormatter.NumberStyle = NSNumberFormatterStyle.None;
            numberFormatter.FormatterBehavior = NSNumberFormatterBehavior.Default;

            // The `searchString` may fail to convert to a number.
            var targetNumber = numberFormatter.NumberFromString(searchString);
            if (targetNumber != null)
            {
                // Use `targetNumberExpression` in both the following predicates.
                var targetNumberExpression = NSExpression.FromConstant(targetNumber);

                // The `yearIntroduced` field matching.
                var yearIntroducedExpression = NSExpression.FromKeyPath(ExpressionKeys.yearIntroduced);
                var yearIntroducedPredicate = new NSComparisonPredicate(yearIntroducedExpression,
                                      targetNumberExpression,
                                       NSComparisonPredicateModifier.Direct,
                                       NSPredicateOperatorType.EqualTo,
                                       NSComparisonPredicateOptions.CaseInsensitive | NSComparisonPredicateOptions.DiacriticInsensitive);

                searchItemsPredicate.Add(yearIntroducedPredicate);

                // The `price` field matching.
                var lhs = NSExpression.FromKeyPath(ExpressionKeys.introPrice);

                var finalPredicate = new NSComparisonPredicate(lhs,
                                      targetNumberExpression,
                                       NSComparisonPredicateModifier.Direct,
                                       NSPredicateOperatorType.EqualTo,
                                       NSComparisonPredicateOptions.CaseInsensitive | NSComparisonPredicateOptions.DiacriticInsensitive);


                searchItemsPredicate.Add(finalPredicate);
            }

            var orMatchPredicate = NSCompoundPredicate.CreateOrPredicate(searchItemsPredicate.ToArray());
            return orMatchPredicate;
        }

        public void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            // Update the filtered array based on the search text.
            var searchResults = products;

            // Strip out all the leading and trailing spaces.
            var strippedString = searchController.SearchBar.Text.Trim();
            var searchItems = strippedString.Split(' ');

            // Build all the "AND" expressions for each value in searchString.
            NSPredicate[] andMatchPredicates = searchItems.Select(item => FindMatches(item)).ToArray();

            // Match up the fields of the Product object.
            var finalCompoundPredicate = NSCompoundPredicate.CreateAndPredicate(andMatchPredicates);
            var filteredResults = searchResults.Where(r => finalCompoundPredicate.EvaluateWithObject(r));

            // Apply the filtered results to the search results table.
            if (searchController.SearchResultsController is ResultsTableController resultsController)
            {
                resultsController.filteredProducts = filteredResults.ToList();
                resultsController.TableView.ReloadData();
            }
        }

        #endregion

        #region UIStateRestoration

        public override void EncodeRestorableState(NSCoder coder)
        {
            base.EncodeRestorableState(coder);

            // Encode the view state so it can be restored later.

            // Encode the title.
            coder.Encode(new NSString(NavigationItem.Title), RestorationKeys.viewControllerTitle);

            // Encode the search controller's active state.
            coder.Encode(searchController.Active, RestorationKeys.searchControllerIsActive);

            // Encode the first responser status.
            coder.Encode(searchController.SearchBar.IsFirstResponder, RestorationKeys.searchBarIsFirstResponder);

            // Encode the search bar text.
            coder.Encode(new NSString(searchController.SearchBar.Text), RestorationKeys.searchBarText);
        }

        public override void DecodeRestorableState(NSCoder coder)
        {

            // Restore the title.
            var decodedTitle = coder.DecodeObject(RestorationKeys.viewControllerTitle) as NSString;
            if (decodedTitle == null)
            {
                throw new Exception("A title did not exist. In your app, handle this gracefully.");
            }

            NavigationItem.Title = decodedTitle;

            /** Restore the active state:
            We can't make the searchController active here since it's not part of the view
            hierarchy yet, instead we do it in viewWillAppear.
        */
            restoredState.wasActive = coder.DecodeBool(RestorationKeys.searchControllerIsActive);

            /** Restore the first responder status:
                Like above, we can't make the searchController first responder here since it's not part of the view
                hierarchy yet, instead we do it in viewWillAppear.
            */
            restoredState.wasFirstResponder = coder.DecodeBool(RestorationKeys.searchBarIsFirstResponder);

            // Restore the text in the search field.
            searchController.SearchBar.Text = coder.DecodeObject(RestorationKeys.searchBarText) as NSString;
            base.DecodeRestorableState(coder);
        }

        #endregion

        private class SearchControllerRestorableState
        {
            public bool wasActive { get; set; }
            public bool wasFirstResponder { get; set; }
        }

        /// State restoration values.
        class RestorationKeys
        {
            public const string viewControllerTitle = "viewControllerTitle";
            public const string searchControllerIsActive = "searchControllerIsActive";
            public const string searchBarText = "searchBarText";
            public const string searchBarIsFirstResponder = "searchBarIsFirstResponder";
        }

        /// NSPredicate expression keys.
        class ExpressionKeys
        {
            public const string title = "title";
            public const string yearIntroduced = "yearIntroduced";
            public const string introPrice = "introPrice";
        }
    }
}