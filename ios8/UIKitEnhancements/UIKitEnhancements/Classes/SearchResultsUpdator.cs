using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using CoreGraphics;

namespace UIKitEnhancements
{
	public class SearchResultsUpdator : UISearchResultsUpdating
	{
		#region Constructors
		public SearchResultsUpdator ()
		{

		}
		#endregion

		#region Override Methods
		public override void UpdateSearchResultsForSearchController (UISearchController searchController)
		{
			// Inform caller of the update event
			RaiseUpdateSearchResults (searchController.SearchBar.Text);
		}
		#endregion

		#region Events
		/// <summary>
		/// Update search results delegate.
		/// </summary>
		public delegate void UpdateSearchResultsDelegate(string searchText);
		public event UpdateSearchResultsDelegate UpdateSearchResults;

		/// <summary>
		/// Raises the update search results event.
		/// </summary>
		/// <param name="searchText">Search text.</param>
		private void RaiseUpdateSearchResults(string searchText) {
			// Inform caller
			if (this.UpdateSearchResults != null)
				this.UpdateSearchResults (searchText);
		}
		#endregion
	}
}

