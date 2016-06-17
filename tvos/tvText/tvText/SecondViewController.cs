using System;

using UIKit;

namespace tvText
{
	/// <summary>
	/// Controls the View for the Search tab.
	/// </summary>
	public partial class SecondViewController : UIViewController
	{
		#region Constants
		/// <summary>
		/// The ID of the View Controller used to present search results to the user
		/// as defined in the Storyboard file.
		/// </summary>
		public const string SearchResultsID = "SearchResults";
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the results controller.
		/// </summary>
		/// <value>The <c>SearchResultsViewController</c>.</value>
		public SearchResultsViewController ResultsController { get; set;}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvText.SecondViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public SecondViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Shows the search controller.
		/// </summary>
		public void ShowSearchController ()
		{
			// Build an instance of the Search Results View Controller from the Storyboard
			ResultsController = Storyboard.InstantiateViewController (SearchResultsID) as SearchResultsViewController;
			if (ResultsController == null)
				throw new Exception ("Unable to instantiate a SearchResultsViewController.");

			// Create an initialize a new search controller
			var searchController = new UISearchController (ResultsController) {
				SearchResultsUpdater = ResultsController,
				HidesNavigationBarDuringPresentation = false
			};

			// Set any required search parameters
			searchController.SearchBar.Placeholder = "Enter keyword (e.g. coffee)";

			// The Search Results View Controller can be presented as a modal view
			// PresentViewController (searchController, true, null);

			// Or in the case of this sample, the Search View Controller is being
			// presented as the contents of the Search Tab directly. Use either one
			// or the other method to display the Search Controller (not both).
			var container = new UISearchContainerViewController (searchController);
			var navController = new UINavigationController (container);
			AddChildViewController (navController);
			View.Add (navController.View);
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called when the View has been loaded from the Storyboard.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// If the Search Controller is being displayed as the content
			// of the search tab, include it here.
			ShowSearchController ();
		}

		/// <summary>
		/// Called just before the View is displayed to allow you to configure it.
		/// </summary>
		/// <param name="animated">Animated.</param>
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// If the Search Controller is being presented as a modal view,
			// call it here to display it over the contents of the Search
			// tab.
			// ShowSearchController ();
		}

		/// <summary>
		/// Called if the View Controller recieves a low memory warning.
		/// </summary>
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion
	}
}

