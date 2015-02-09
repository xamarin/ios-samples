using System;
using System.Linq;
using System.Collections.Generic;

namespace ListerKit
{
	/// <summary>
	/// Simple internal helper functions to share across <c>IncompleteListItemsPresenter</c> and
	/// <c>AllListItemsPresenter</c>. These functions help diff two arrays of <c>ListItem</c> objects.
	/// </summary>
	public static class ListPresenterAlgorithms
	{
		/// <summary>
		/// Returns an array of <c>ListItem</c> objects in <c>initialListItems</c> that don't exist in <c>changedListItems</c>.
		/// </summary>
		public static IList<ListItem> FindRemovedListItemsFromInitialListItemsToChangedListItems(IList<ListItem> initialListItems, IList<ListItem> changedListItems)
		{
			return initialListItems.Where (item => changedListItems.Contains (item)).ToArray ();
		}

		/// <summary>
		/// Returns an array of <c>ListItem</c> objects in <c>changedListItems</c> that don't exist in <c>initialListItems</c>.
		/// </summary>
		/// <returns>The inserted list items from initial list items to changed list items.</returns>
		/// <param name="initialListItems">Initial list items.</param>
		/// <param name="changedListItems">Changed list items.</param>
		/// <param name="filterHandlerOrNil">Filter predicate or null.</param>
		public static IList<ListItem> FindInsertedListItemsFromInitialListItemsToChangedListItems(IList<ListItem> initialListItems, IList<ListItem> changedListItems, Func<ListItem, bool> filterOrNull)
		{
			IEnumerable<ListItem> filtered = changedListItems.Except (initialListItems);

			if (filterOrNull != null)
				return filtered.Where (filterOrNull).ToArray ();

			return filtered.ToArray ();
		}

		/// <summary>
		/// Returns an array of <c>ListItem objects in <c>changed</c> whose completion state changed from <c>initial</c>
		/// relative to `changed`.
		/// </summary>
		public static IList<ListItem> FindToggledListItemsFromInitialListItemsToChangedListItems(ListItem[] initial, ListItem[] changed)
		{
			return IntersectThenFilter (initial, changed, (f, s) => f.IsComplete != s.IsComplete).ToArray ();
		}

		/// <summary>
		/// Returns an array of <c>ListItem</c> objects in </c>changed</c> whose text changed from <c>initial</c>
		/// relative to <c>changed</c>.
		/// </summary>
		public static IList<ListItem> FindListItemsWithUpdatedTextFromInitialListItemsToChangedListItems(IEnumerable<ListItem> initial, IEnumerable<ListItem> changed)
		{
			return IntersectThenFilter (initial, changed, (f, s) => f.Text != s.Text).ToArray ();
		}

		static IEnumerable<ListItem> IntersectThenFilter(IEnumerable<ListItem> first, IEnumerable<ListItem> second, Func<ListItem, ListItem, bool> comparer)
		{
			var map = first.ToDictionary (item => item.UID);
			return second.Where (item => {
				ListItem other;
				if (map.TryGetValue (item.UID, out other))
					return comparer (item, other);
				return false;
			});
		}

		/// <summary>
		/// Update <c>replaceable</c> in place with all of the list items that are equal in <c>replacement</c>.
		/// For example, if <c>replaceable</c> has list items of UUID "1", "2", and "3" and <c>replacement</c>
		/// has list items of UUID "2" and "3", the <c>replaceable</c> array will have it's list items with UUID
		/// "2" and "3" replaced with the list items whose UUID is "2" and "3" in <c>replacement</c>. This is
		/// used to ensure that the list items in multiple arrays are referencing the same objects in memory as what the
		/// presented list items are presenting.
		/// </summary>
		public static void ReplaceAnyEqualUnchangedNewListItemsWithPreviousUnchangedListItems(IList<ListItem> replaceable, IList<ListItem> replacement)
		{
			Dictionary<Guid, int> map = new Dictionary<Guid, int> ();
			for (int i = 0; i < replaceable.Count; i++) {
				ListItem item = replaceable [i];
				map [item.UID] = i;
			}

			foreach (var item in replacement) {
				int index;
				if (map.TryGetValue (item.UID, out index))
					replaceable [index] = item;
			}
		}

		/// <summary>
		/// Returns the type of <c>BatchChangeKind</c> based on the different types of changes. The parameters
		/// for this function should be based on the result of the functions above. If there were no changes whatsoever,
		/// <c>BatchChangeKind.None</c> is returned.
		/// </summary>
		public static BatchChangeKind ListItemsBatchChangeKindForChanges(IList<ListItem> removedListItems, IList<ListItem> insertedListItems, IList<ListItem> toggledListItems, IList<ListItem> listItemsWithUpdatedText)
		{
			var listChangeKind = BatchChangeKind.None;

			// A simple helper closure that takes in the new change kind. If there has already been a change kind set
			// to a value other than BatchChangeKind.Multiple, the block updates listItemsBatchChangeKind
			// to be BatchChangeKind.Multiple instead of newChangeKind.
			Action<BatchChangeKind> setListItemsBatchChangeKind = newChangeKind => {
				listChangeKind = listChangeKind == BatchChangeKind.None ? newChangeKind : BatchChangeKind.Multiple;
			};

			if (removedListItems != null && removedListItems.Count > 0)
				setListItemsBatchChangeKind(BatchChangeKind.Removed);
			if (insertedListItems != null && insertedListItems.Count > 0)
				setListItemsBatchChangeKind(BatchChangeKind.Inserted);
			if (toggledListItems != null && toggledListItems.Count > 0)
				setListItemsBatchChangeKind(BatchChangeKind.Toggled);
			if (listItemsWithUpdatedText != null && listItemsWithUpdatedText.Count > 0)
				setListItemsBatchChangeKind(BatchChangeKind.UpdatedText);

			return listChangeKind;
		}
	}
}