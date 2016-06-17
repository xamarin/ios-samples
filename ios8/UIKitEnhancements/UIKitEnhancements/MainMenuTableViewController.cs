	using System;
	using Foundation;
	using UIKit;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using CoreGraphics;

namespace UIKitEnhancements
{
	public partial class MainMenuTableViewController : UITableViewController
	{
		#region Computed Properties
		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}

		/// <summary>
		/// Gets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public MainMenuTableSource DataSource {
			get { return (MainMenuTableSource)TableView.Source; }
		}

		/// <summary>
		/// Gets or sets the menu item.
		/// </summary>
		/// <value>The menu item.</value>
		public MenuItem MenuItem { get; set; }

		/// <summary>
		/// Gets or sets the search controller.
		/// </summary>
		/// <value>The search controller.</value>
		public UISearchController SearchController { get; set;}
		#endregion

		#region Constructors
		public MainMenuTableViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Reloads the data.
		/// </summary>
		public void ReloadData() {

			// Ask the table to redisplay its information
			DataSource.LoadData ();
			TableView.ReloadData ();
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Views the did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register the tableview's datasource
			TableView.Source = new MainMenuTableSource (this);

			// Create a search results table
			var searchResultsController = new UITableViewController (UITableViewStyle.Plain);
			var searchSource = new SearchResultsTableSource (this);
			searchResultsController.TableView.Source = searchSource;

			// Create search updater and wire it up
			var searchUpdater = new SearchResultsUpdator ();
			searchUpdater.UpdateSearchResults += (searchText) => {
				// Preform search and reload search table
				searchSource.Search(searchText);
				searchResultsController.TableView.ReloadData();
			};

			// Create a new search controller
			SearchController = new UISearchController (searchResultsController);
			SearchController.SearchResultsUpdater = searchUpdater;

			// Display the search controller
			SearchController.SearchBar.Frame = new CGRect (SearchController.SearchBar.Frame.X, SearchController.SearchBar.Frame.Y, SearchController.SearchBar.Frame.Width, 44f);
			TableView.TableHeaderView = SearchController.SearchBar;
			DefinesPresentationContext = true;
		}

		/// <summary>	
		/// Prepares for segue.
		/// </summary>
		/// <param name="segue">Segue.</param>
		/// <param name="sender">Sender.</param>
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			// Take action based on the segue type
			switch (segue.Identifier) {
			case "WebSegue":
				var webView = segue.DestinationViewController as WebViewController;
				webView.URL = MenuItem.URL;
				break;
			} 

		}
		#endregion
	}
}
