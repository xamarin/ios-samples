using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace tvText
{
	/// <summary>
	/// Controlls the Collection View that will be used to display the results of the user's
	/// search input. This controller both provides the data for the Collection, but responds
	/// to the user's input.
	/// </summary>
	/// <remarks>
	/// See our Working with Collection View documentation for more information:
	/// https://developer.xamarin.com/guides/ios/tvos/user-interface/collection-views/
	/// </remarks>
	public partial class SearchResultsViewController : UICollectionViewController , IUISearchResultsUpdating
    {
		#region Constants
		/// <summary>
		/// The cell identifier as entered in the Storyboard.
		/// </summary>
		public const string CellID = "ImageCell";
		#endregion

		#region Private Variables
		/// <summary>
		/// The backing store for the filter string.
		/// </summary>
		private string _searchFilter = "";
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets all pictures that the user can search for.
		/// </summary>
		/// <value>A collection of <c>PictureInformation</c> objects.</value>
		public List<PictureInformation> AllPictures { get; set;}

		/// <summary>
		/// Gets or sets the pictures that match the user's search term either by
		/// title of keywords.
		/// </summary>
		/// <value>A collection of <c>PictureInformation</c> objects.</value>
		public List<PictureInformation> FoundPictures { get; set; }

		/// <summary>
		/// Gets or sets the search filter enter by the user to limit the
		/// list of returned pictures.
		/// </summary>
		/// <value>A <c>string</c> containing the filter parameters.</value>
		public string SearchFilter {
			get { return _searchFilter; }
			set {
				_searchFilter = value.ToLower();
				FindPictures ();
				CollectionView?.ReloadData ();
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:tvText.SearchResultsViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
        public SearchResultsViewController (IntPtr handle) : base (handle)
        {
			// Initialize
			this.AllPictures = new List<PictureInformation> ();
			this.FoundPictures = new List<PictureInformation> ();
			PopulatePictures ();
			FindPictures ();

        }
		#endregion

		#region Private Methods
		/// <summary>
		/// Populates the collection of available pictures.
		/// </summary>
		private void PopulatePictures ()
		{
			// Clear list
			AllPictures.Clear ();

			// Add images
			AllPictures.Add (new PictureInformation ("Antipasta Platter","Antipasta","cheese,grapes,tomato,coffee,meat,plate"));
			AllPictures.Add (new PictureInformation ("Cheese Plate", "CheesePlate", "cheese,plate,bread"));
			AllPictures.Add (new PictureInformation ("Coffee House", "CoffeeHouse", "coffee,people,menu,restaurant,cafe"));
			AllPictures.Add (new PictureInformation ("Computer and Expresso", "ComputerExpresso", "computer,coffee,expresso,phone,notebook"));
			AllPictures.Add (new PictureInformation ("Hamburger", "Hamburger", "meat,bread,cheese,tomato,pickle,lettus"));
			AllPictures.Add (new PictureInformation ("Lasagna Dinner", "Lasagna", "salad,bread,plate,lasagna,pasta"));
			AllPictures.Add (new PictureInformation ("Expresso Meeting", "PeopleExpresso", "people,bag,phone,expresso,coffee,table,tablet,notebook"));
			AllPictures.Add (new PictureInformation ("Soup and Sandwich", "SoupAndSandwich", "soup,sandwich,bread,meat,plate,tomato,lettus,egg"));
			AllPictures.Add (new PictureInformation ("Morning Coffee", "TabletCoffee", "tablet,person,man,coffee,magazine,table"));
			AllPictures.Add (new PictureInformation ("Evening Coffee", "TabletMagCoffee", "tablet,magazine,coffee,table"));
		}

		/// <summary>
		/// Populates the <c>FoundPictures</c> collection with any picture (from the <c>AllPictures</c> collection) that
		/// matches the <c>SearchFilter</c> by either the title or keywords.
		/// </summary>
		private void FindPictures ()
		{
			// Clear list
			FoundPictures.Clear ();

			// Scan each picture for a match
			foreach (PictureInformation picture in AllPictures) {
				if (SearchFilter == "") {
					// If no search term, everything matches
					FoundPictures.Add (picture);
				} else if (picture.Title.Contains (SearchFilter) || picture.Keywords.Contains (SearchFilter)) {
					// If the search term is in the title or keywords, we've found a match
					FoundPictures.Add (picture);
				}
			}
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Returns the number of section in the Collection View
		/// </summary>
		/// <returns>The of sections.</returns>
		/// <param name="collectionView">Collection view.</param>
		public override nint NumberOfSections (UICollectionView collectionView)
		{
			// Only one section in this collection
			return 1;
		}

		/// <summary>
		/// Gets the number of found pictures matching the <c>SearchFilter</c>.
		/// </summary>
		/// <returns>The items count.</returns>
		/// <param name="collectionView">Collection view.</param>
		/// <param name="section">Section.</param>
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// Return the number of matching pictures
			return FoundPictures.Count;
		}

		/// <summary>
		/// Gets a new <c>SearchResultViewCell</c> for the given row in the Collection View.
		/// </summary>
		/// <returns>A <c>SearchResultViewCell</c>.</returns>
		/// <param name="collectionView">Collection view.</param>
		/// <param name="indexPath">Index path.</param>
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Get a new cell and return it
			var cell = collectionView.DequeueReusableCell (CellID, indexPath);
			return (UICollectionViewCell)cell;
		}

		/// <summary>
		/// Called before a Collection View Cell is displayed to allow it to be initialized.
		/// </summary>
		/// <param name="collectionView">Collection view.</param>
		/// <param name="cell">Cell.</param>
		/// <param name="indexPath">Index path.</param>
		public override void WillDisplayCell (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			// Grab the cell
			var currentCell = cell as SearchResultViewCell;
			if (currentCell == null)
				throw new Exception ("Expected to display a `SearchResultViewCell`.");

			// Display the current picture info in the cell
			var item = FoundPictures [indexPath.Row];
			currentCell.PictureInfo = item;
		}

		/// <summary>
		/// Respond to an item being selected (clicked on) in the Collection View.
		/// </summary>
		/// <param name="collectionView">Collection view.</param>
		/// <param name="indexPath">Index path.</param>
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// If this Search Controller was presented as a modal view, close
			// it before continuing
			// DismissViewController (true, null);

			// Grab the picture being selected and report it
			var picture = FoundPictures [indexPath.Row];
			Console.WriteLine ("Selected: {0}", picture.Title);
		}

		/// <summary>
		/// Updates the search results for search controller.
		/// </summary>
		/// <param name="searchController">Search controller.</param>
		public void UpdateSearchResultsForSearchController (UISearchController searchController)
		{
			// Save the search filter and update the Collection View
			SearchFilter = searchController.SearchBar.Text ?? string.Empty;
		}

		/// <summary>
		/// Called when the focus shifts from one Collection View Cell to another.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="coordinator">Coordinator.</param>
		/// <remarks>We are using this method to highligh the currently In-Focus picture.</remarks>
		public override void DidUpdateFocus (UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator)
		{
			var previousItem = context.PreviouslyFocusedView as SearchResultViewCell;
			if (previousItem != null) {
				UIView.Animate (0.2, () => {
					previousItem.TextColor = UIColor.LightGray;
				});
			}

			var nextItem = context.NextFocusedView as SearchResultViewCell;
			if (nextItem != null) {
				UIView.Animate (0.2, () => {
					nextItem.TextColor = UIColor.Black;
				});
			}
		}
		#endregion
    }
}