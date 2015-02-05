using System;
using System.Collections.Generic;

namespace ListerKit
{
	public static class ListPresenterUtilities
	{
		void RemoveListItemsFromListItemsWithListPresenter(IListPresenter listPresenter, List<ListItem> initialListItems, List<ListItem> listItemsToRemove)
		{
//			NSArray *sortedListItemsToRemove = [listItemsToRemove sortedArrayUsingComparator:^NSComparisonResult(AAPLListItem *lhs, AAPLListItem *rhs) {
//				return [initialListItems indexOfObject:lhs] > [initialListItems indexOfObject:rhs];
//			}];
//
//			for (AAPLListItem *listItemToRemove in sortedListItemsToRemove) {
//				// Use the index of the list item to remove in the current list's list items.
//				NSInteger indexOfListItemToRemoveInOldList = [initialListItems indexOfObject:listItemToRemove];
//
//				[initialListItems removeObjectAtIndex:indexOfListItemToRemoveInOldList];
//
//				[listPresenter.delegate listPresenter:listPresenter didRemoveListItem:listItemToRemove atIndex:indexOfListItemToRemoveInOldList];
//			}
		}

	}
}

