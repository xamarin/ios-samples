using System;
using System.Linq;

namespace ListerKit
{
	public static class ListPresenterAlgorithms
	{
		public static ListItem[] FindRemovedListItemsFromInitialListItemsToChangedListItems(ListItem[] initialListItems, ListItem[] changedListItems)
		{
			return initialListItems.Where (item => changedListItems.Contains (item)).ToArray ();
		}
	}
}

