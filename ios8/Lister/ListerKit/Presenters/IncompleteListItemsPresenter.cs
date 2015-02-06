using System;

namespace ListerKit
{
	/// <summary>
	/// The <c>IncompleteListItemsPresenter</c> list presenter is responsible for managing the how a list's
	/// incomplete list items are displayed in the iOS and OS X Today widgets as well as the Lister WatchKit app.
	/// The <c>IncompleteListItemsPresenter</c> class implements to <c>IListPresenting</c> so consumers of this class
	/// can work with the presenter using a common interface.

	/// When a list is initially presented with an <c>IncompleteListItemsPresenter</c>, only the incomplete list
	/// items are presented. That can change, however, if a user toggles list items (changing the list item's
	/// completion state). An <c>IncompleteListItemsPresenter</c> always shows the list items that are initially
	/// presented (unless they are removed from the list from another device). If an 
	/// <c>IncompleteListItemsPresenter</c> stops presenting a list that has some presented list items that are complete
	/// (after toggling them) and another <c>IncompleteListItemsPresenter</c> presents the same list, the presenter
	/// displays *only* the incomplete list items.

	/// The <c>IncompleteListItemsPresenter</c> can be interacted with in a two ways. <c>ListItem</c> instances can
	/// be toggled individually, or using a batch update, and the color of the list presenter can be changed. All
	/// of these methods trigger calls to the delegate to be notified about inserted list items, removed list
	/// items, updated list items, etc.
	/// </summary>
	public class IncompleteListItemsPresenter
	{
		public IncompleteListItemsPresenter ()
		{
		}
	}
}

