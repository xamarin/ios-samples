using System;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace ListerKit
{
	// TODO: add comments
	public class AllListItemsPresenter : NSObject, IListPresenter
	{
		/// The internal storage for the list that we're presenting. By default, it's an empty list.
		List list;

		// Flag to see whether or not the first SetList call should trigger a batch reload.
		bool isInitialList;

		public NSUndoManager undoManager { get; set; }

		public AllListItemsPresenter ()
		{
			list = new List ();
			isInitialList = true;
		}

		#region IListPresenter implementation

		public void SetList (List newList)
		{
			// If this is the initial list that's being presented, just tell the delegate
			// to reload all of the data.
			if(isInitialList) {
				isInitialList = true;

				list = newList;
				ReorderedListItemsFromListItems (newList.Items);

				Delegate.DidRefreshCompleteLayout (this);
			}

			// Perform more granular changes (if we can). To do this, we'll group the changes into the different
			// types of possible changes. If we know that a group of similar changes occured, we'll batch them
			// together (e.g. four updates to list items). If multiple changes occur that we can't correctly resolve
			// (an implementation detail), we'll refresh the complete layout. An example of this is if more than one
			// list item is inserted or toggled. Since this algorithm doesn't track the indexes that list items
			// are inserted at, we will just refresh the complete layout to make sure that the list items are presented
			// correctly. This applies for multiple groups of changes (e.g. one insert and one toggle), and also for
			// any unique group of toggles / inserts where there's more than a single update.

			List oldList = list;

			var newRemovedListItems = ListPresenterAlgorithms.FindRemovedListItemsFromInitialListItemsToChangedListItems(oldList.Items, newList.Items);
			var newInsertedListItems = ListPresenterAlgorithms.FindInsertedListItemsFromInitialListItemsToChangedListItems(oldList.Items, newList.Items, null);
			var newToggledListItems = ListPresenterAlgorithms.FindToggledListItemsFromInitialListItemsToChangedListItems(oldList.Items, newList.Items);
			var newListItemsWithUpdatedText = ListPresenterAlgorithms.FindListItemsWithUpdatedTextFromInitialListItemsToChangedListItems(oldList.Items, newList.Items);

			// Determine if there was a unique group of batch changes we can make. Otherwise, we'll
			// refresh all the data in the list.

			BatchChangeKind listItemsBatchChangeKind = ListPresenterAlgorithms.ListItemsBatchChangeKindForChanges(newRemovedListItems, newInsertedListItems, newToggledListItems, newListItemsWithUpdatedText);

			if (listItemsBatchChangeKind == BatchChangeKind.None) {
				if (oldList.Color != newList.Color) {
					undoManager.RemoveAllActions (this);
					ListPresenterUtilities.UpdateListColorForListPresenterIfDifferent (this, list, newList.Color, ListColorUpdateAction.SendDelegateChangeLayoutCallsForInitialLayout);
				}
				return;
			}

			// Check to see if there was more than one kind of unique group of changes, or if there were multiple toggled /
			// inserted list items that we don't handle.
			if (listItemsBatchChangeKind == BatchChangeKind.Multiple || newToggledListItems.Count > 1 || newInsertedListItems.Count > 1) {
				undoManager.RemoveAllActions (this);

				list = newList;
				newList.Items = ReorderedListItemsFromListItems (newList.Items);
				Delegate.DidRefreshCompleteLayout (this);

				return;
			}

			// At this point we know that we have changes that are uniquely identifiable: e.g. one inserted list item,
			// one toggled list item, multiple removed list items, or multiple list items whose text has been updated.
			undoManager.RemoveAllActions(this);
			Delegate.WillChangeListLayout (this, true);

			// Make the changes based on the unique change kind.
			switch (listItemsBatchChangeKind) {
				case BatchChangeKind.Removed:
					ListPresenterUtilities.RemoveListItemsFromListItemsWithListPresenter (this, list.Items, newRemovedListItems);
					break;

				case BatchChangeKind.Inserted:
					UnsafeInsertListItem (newInsertedListItems [0]);
					break;

				case BatchChangeKind.Toggled:
						// We want to toggle the *old* list item, not the one that's in newList.
					int indexOfToggledListItemInOldListItems = oldList.Items.IndexOf (newToggledListItems [0]);
					ListItem listItemToToggle = oldList.Items [indexOfToggledListItemInOldListItems];

					UnsafeToggleListItem (listItemToToggle);
					break;

				case BatchChangeKind.UpdatedText:
					ListPresenterUtilities.UpdateListItemsWithListItemsForListPresenter (this, list.Items, newListItemsWithUpdatedText);
					break;

				default:
					throw new NotImplementedException ();
			}

			ListPresenterUtilities.UpdateListColorForListPresenterIfDifferent (this, list, newList.Color, ListColorUpdateAction.DontSendDelegateChangeLayoutCalls);
			Delegate.DidChangeListLayout (this, true);
		}

		public IListPresenterDelegate Delegate {
			get {
				throw new NotImplementedException ();
			}
		}

		public ListColor Color {
			get {
				return list.Color;
			}
			set {
				ListColor oldColor = Color;

				bool different = ListPresenterUtilities.UpdateListColorForListPresenterIfDifferent (this, list, Color, ListColorUpdateAction.SendDelegateChangeLayoutCallsForNonInitialLayout);

				// Register the undo color operation with the old color if the list's color was changed.
				if (!different)
					return;

				((IListPresenter)undoManager.PrepareWithInvocationTarget (this)).Color = oldColor;
				undoManager.SetActionname ("Change Color");
			}
		}

		public List ArchiveableList {
			get {
				return list;
			}
		}

		public int Count {
			get {
				return list.Count;
			}
		}

		public bool IsEmpty {
			get {
				return list.Count == 0;
			}
		}

		public List<ListItem> PresentedListItems {
			get {
				return list.Items;
			}
		}

		#endregion

		int IndexOfFirstCompletedItem()
		{
			for (int i = 0; i < PresentedListItems.Count; i++) {
				if (PresentedListItems [i].IsComplete)
					return i;
			}
			return -1;
		}

		void InsertListItem(ListItem listItem)
		{
			Delegate.WillChangeListLayout (this, false);

			UnsafeInsertListItem (listItem);

			Delegate.DidChangeListLayout (this, false);

			// Undo
			((AllListItemsPresenter)undoManager.PrepareWithInvocationTarget(this)).RemoveListItem(listItem);

			// TODO: https://trello.com/c/V0IXnqGU
			undoManager.SetActionname ("Remove");
		}

		void InsertListItems(IEnumerable<ListItem> listItems)
		{
			if (!listItems.Any ())
				return;

			Delegate.WillChangeListLayout (this, false);

			foreach (var listItem in listItems)
				UnsafeInsertListItem (listItem);

			Delegate.DidChangeListLayout (this, false);

			// Undo
			((AllListItemsPresenter)undoManager.PrepareWithInvocationTarget(this)).RemoveListItems(listItems);

			// TODO: https://trello.com/c/V0IXnqGU
			undoManager.SetActionname ("Remove");
		}

		void RemoveListItem (ListItem listItem)
		{
			int listItemIndex = PresentedListItems.IndexOf (listItem);

			if (listItemIndex == -1)
				throw new InvalidProgramException ("To remove a list item, it must already be in the list.");

			Delegate.WillChangeListLayout (this, false);

			list.Items.RemoveAt (listItemIndex);

			Delegate.DidRemoveListItem (this, listItem, listItemIndex);

			Delegate.DidChangeListLayout (this, false);

			// Undo
			((AllListItemsPresenter)undoManager.PrepareWithInvocationTarget (this))
				.InsertListItemsForUndo (new ListItem[]{ listItem }, new int[]{ listItemIndex });

			// TODO: https://trello.com/c/V0IXnqGU
			undoManager.SetActionname ("Remove");
		}

		void RemoveListItems (IEnumerable<ListItem> listItemsToRemove)
		{
			if (!listItemsToRemove.Any ())
				return;

			// We're going to store the indexes of the list items that will be removed in an array.
			// We do that so that when we insert the same list items back in for undo, we don't need
			// to worry about insertion order (since it will just be the opposite of insertion order).

			var removedIndexes = new List<int> ();

			var map = new Dictionary<ListItem, int> ();
			for (int i = 0; i < PresentedListItems.Count; i++)
				map [PresentedListItems [i]] = i;

			foreach (ListItem listItem in listItemsToRemove) {
				int listItemIndex;
				if (!map.TryGetValue (listItem, out listItemIndex))
					throw new InvalidProgramException ("List items to remove must already be in the list.");

				list.Items.RemoveAt (listItemIndex);
				Delegate.DidRemoveListItem (this, listItem, listItemIndex);
				removedIndexes.Add (listItemIndex);
			}

			Delegate.DidChangeListLayout (this, false);

			// Undo
			var reverseListItemsToRemove = listItemsToRemove.Reverse ().ToArray ();
			var reverseRemovedIndexes = removedIndexes.Reverse ().ToArray ();
			((AllListItemsPresenter)undoManager.PrepareWithInvocationTarget (this)).InsertListItemsForUndo (reverseListItemsToRemove, reverseRemovedIndexes);

			// TODO: https://trello.com/c/V0IXnqGU
			undoManager.SetActionname ("Remove");
		}

		List<ListItem> ReorderedListItemsFromListItems(IList<ListItem> listItems)
		{
			throw new NotImplementedException ();
//			NSPredicate *incompleteListItemsPredicate = [NSPredicate predicateWithFormat:@"isComplete = NO"];
//			NSPredicate *completeListItemsPredicate = [NSPredicate predicateWithFormat:@"isComplete = YES"];
//
//			NSArray *incompleteListItems = [listItems filteredArrayUsingPredicate:incompleteListItemsPredicate];
//			NSArray *completeListItems = [listItems filteredArrayUsingPredicate:completeListItemsPredicate];
//
//			return [incompleteListItems arrayByAddingObjectsFromArray:completeListItems];
		}


		void UnsafeInsertListItem(ListItem listItem)
		{
			throw new NotImplementedException ();
//			NSAssert(![self.presentedListItems containsObject:listItem], @"A list item was requested to be added that is already in the list.");
//
//			NSInteger indexToInsertListItem = listItem.isComplete ? self.count : 0;
//
//			[[self.list mutableArrayValueForKey:@"items"] insertObject:listItem atIndex:indexToInsertListItem];
//
//			[self.delegate listPresenter:self didInsertListItem:listItem atIndex:indexToInsertListItem];
		}

		int UnsafeToggleListItem(ListItem listItem)
		{
			throw new NotImplementedException ();
//			NSAssert([self.presentedListItems containsObject:listItem], @"A list item can only be toggled if it already exists in the list.");
//
//			// Move the list item.
//			NSInteger targetIndex = listItem.isComplete ? 0 : self.count - 1;
//			NSInteger fromIndex = [self unsafeMoveListItem:listItem toIndex:targetIndex];
//
//			// Update the list item's state.
//			listItem.complete = !listItem.isComplete;
//			[self.delegate listPresenter:self didUpdateListItem:listItem atIndex:targetIndex];
//
//			return fromIndex;
		}

		#region Undo Helper Methods

		void InsertListItemsForUndo (ListItem[] arr, int[] arr2)
		{
			throw new NotImplementedException ();
		}

		#endregion

	}
}

