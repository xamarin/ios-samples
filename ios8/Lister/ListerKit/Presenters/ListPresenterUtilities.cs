using System;
using System.Collections.Generic;

namespace ListerKit
{
	public static class ListPresenterUtilities
	{
		// TODO: Rename
		// TODO: maybe listItemsToRemove should be arrays ?
		public static void RemoveListItemsFromListItemsWithListPresenter(IListPresenter listPresenter, List<ListItem> initialListItems, List<ListItem> listItemsToRemove)
		{
			// TODO: why we need to sort items before remove ???
			// TODO: should I make a copy of listItemsToRemove before sort ???
			listItemsToRemove.Sort ((lhs, rhs) => {
				return initialListItems.IndexOf(lhs) - initialListItems.IndexOf(rhs);
			});

			foreach (var item in listItemsToRemove) {
				// Use the index of the list item to remove in the current list's list items.
				// IndexOf will call ListItem.Equals and ListItem.GetHashCode
				int oldIndex = initialListItems.IndexOf (item);
				initialListItems.RemoveAt (oldIndex);
				listPresenter.Delegate.DidRemoveListItem (listPresenter, item, oldIndex);
			}
		}

		// TODO: Rename
		public static void InsertListItemsIntoListItemsWithListPresenter(IListPresenter listPresenter, List<ListItem> initialListItems, List<ListItem> listItemsToInsert)
		{
			for (int index = 0; index < listItemsToInsert.Count; index++) {
				var insertItem = listItemsToInsert [index];
				initialListItems.Insert (index, insertItem);
				listPresenter.Delegate.DidInsertListItem(listPresenter, insertItem, index);
			}
		}

		// TODO: Rename
		public static void UpdateListItemsWithListItemsForListPresenter(IListPresenter listPresenter, IList<ListItem> presentedListItems, IEnumerable<ListItem> newUpdatedListItems)
		{
			foreach (var newlyUpdated in newUpdatedListItems) {
				int index = presentedListItems.IndexOf (newlyUpdated);
				presentedListItems [index] = newlyUpdated;
				listPresenter.Delegate.DidUpdateListItem(listPresenter, newlyUpdated, index);
			}
		}

		public static bool UpdateListColorForListPresenterIfDifferent(IListPresenter listPresenter, List presentedList, ListColor newColor, ListColorUpdateAction listColorUpdateAction)
		{
			// Don't trigger any updates if the new color is the same as the current color.
			if (presentedList.Color == newColor)
				return false;

			bool? isInitialLayout = IsInitialLayout (listColorUpdateAction);
			if (isInitialLayout.HasValue)
				listPresenter.Delegate.WillChangeListLayout (listPresenter, isInitialLayout.Value);

			presentedList.Color = newColor;
			listPresenter.Delegate.DidUpdateListColorWithColor (listPresenter, newColor);

			if (isInitialLayout.HasValue)
				listPresenter.Delegate.DidChangeListLayout (listPresenter, isInitialLayout.Value);

			return true;
		}

		static bool? IsInitialLayout(ListColorUpdateAction actionType)
		{
			switch (actionType) {
				case ListColorUpdateAction.SendDelegateChangeLayoutCallsForInitialLayout:
					return true;
				case ListColorUpdateAction.SendDelegateChangeLayoutCallsForNonInitialLayout:
					return false;
				default:
					return null;
			}
		}
	}
}

