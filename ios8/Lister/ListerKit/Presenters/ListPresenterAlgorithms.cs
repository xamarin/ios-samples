using System;
using System.Linq;

namespace ListerKit
{
	public static class ListPresenterAlgorithms
	{
		ListItem[] FindRemovedListItemsFromInitialListItemsToChangedListItems(ListItem[] initialListItems, ListItem[] changedListItems)
		{
			initialListItems.Where (item => changedListItems.Contains (item)).ToArray ();
		}
	}
}

