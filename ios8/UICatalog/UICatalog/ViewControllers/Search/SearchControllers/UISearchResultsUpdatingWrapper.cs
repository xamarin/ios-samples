using System;
using MonoTouch.UIKit;

namespace UICatalog
{
	// TODO: remove this subclass https://trello.com/c/bEtup8us
	internal class UISearchResultsUpdatingWrapper : UISearchResultsUpdating
	{
		private readonly SearchResultsViewController _searchResultsViewController;

		public UISearchResultsUpdatingWrapper(SearchResultsViewController searchResultsViewController)
		{
			_searchResultsViewController = searchResultsViewController;
		}

		public override void UpdateSearchResultsForSearchController (UISearchController searchController)
		{
			_searchResultsViewController.UpdateSearchResultsForSearchController (searchController);
		}
	}
}

