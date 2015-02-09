using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;

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
	public class IncompleteListItemsPresenter : IListPresenter
	{
		public IListPresenterDelegate Delegate {
			get {
				throw new NotImplementedException ();
			}
		}

		// The internal storage for the list that we're presenting. By default, it's an empty list.
		List list;

		// Flag to see whether or not the first SetList call should trigger a batch reload.
		bool isInitialList;

		/// <summary>
		/// A cached array of the list items that should be presented. When the presenter initially has its underlying <c>List</c>
		/// set, the <c>presentedListItems</c> is set to all of the incomplete list items. As list items are toggled, <c>presentedListItems</c>
		/// may not only contain incomplete list items.
		/// </summary>
		public List<ListItem> PresentedListItems { get; private set; }

		public ListColor Color {
			get {
				return list.Color;
			}
			set {
				ListPresenterUtilities.UpdateListColorForListPresenterIfDifferent (this, list, value, ListColorUpdateAction.SendDelegateChangeLayoutCallsForNonInitialLayout);
			}
		}

		public List ArchiveableList {
			get {
				return list;
			}
		}

		public int Count {
			get {
				return PresentedListItems.Count;
			}
		}

		public bool IsEmpty {
			get {
				return PresentedListItems.Count == 0;
			}
		}

		public IncompleteListItemsPresenter ()
		{
			list = new List();
			isInitialList = true;
			PresentedListItems = new List<ListItem> ();
		}

		public void SetList (List newList)
		{
			if (isInitialList) {
				isInitialList = false;
				list = newList;

				PresentedListItems = newList.Items.Where (item => !item.IsComplete).ToList ();
				Delegate.DidRefreshCompleteLayout (this);
				return;
			}

			// First find all the differences between the lists that we want to reflect in the presentation
			// of the list: removed list items that were incomplete, inserted list items that are incomplete, presented list items
			// that are toggled, and presented list items whose text has changed. Note that although we'll gradually
			// update presentedListItems to reflect the changes we find, we also want to save the latest state of
			// the list (i.e. the `newList` parameter) as the underlying storage of the list. Since we'll be presenting
			// the same list either way, it's better not to change the underlying list representation unless we need
			// to. Keep in mind, however, that all of the list items in presentedListItems should also be in `list.items`.
			// In short, once we modify `presentedListItems` with all of the changes, we need to also update `list.items`
			// to contain all of the list items that were unchanged (this can be done by replacing the new list item
			// representation by the old representation of the list item). Once that happens, all of the presentation
			// logic carries on as normal.

			List oldList = newList;

			IList<ListItem> newRemovedPresentedListItems = ListPresenterAlgorithms.FindRemovedListItemsFromInitialListItemsToChangedListItems(PresentedListItems, newList.Items);
			IList<ListItem> newInsertedIncompleteListItems = ListPresenterAlgorithms.FindInsertedListItemsFromInitialListItemsToChangedListItems (PresentedListItems, newList.Items, listItem => {
				return !listItem.IsComplete;
			});
			var newPresentedToggledListItems = ListPresenterAlgorithms.FindToggledListItemsFromInitialListItemsToChangedListItems(PresentedListItems, newList.Items);
			var newPresentedListItemsWithUpdatedText = ListPresenterAlgorithms.FindListItemsWithUpdatedTextFromInitialListItemsToChangedListItems(PresentedListItems, newList.Items);

			var listItemsBatchChangeKind = ListPresenterAlgorithms.ListItemsBatchChangeKindForChanges(newRemovedPresentedListItems, newInsertedIncompleteListItems, newPresentedToggledListItems, newPresentedListItemsWithUpdatedText);

			// If no changes occured we'll ignore the update.
			if (listItemsBatchChangeKind == BatchChangeKind.None && oldList.Color == newList.Color)
				return;

			// Start performing changes to the presentation of the list.
			Delegate.WillChangeListLayout(this, true);

			// Remove the list items from the presented list items that were removed somewhere else.
			if (newRemovedPresentedListItems.Count > 0)
				ListPresenterUtilities.RemoveListItemsFromListItemsWithListPresenter(this, PresentedListItems, newRemovedPresentedListItems);

			// Insert the incomplete list items into the presented list items that were inserted elsewhere.
			if (newInsertedIncompleteListItems.Count > 0)
				ListPresenterUtilities.InsertListItemsIntoListItemsWithListPresenter(this, PresentedListItems, newInsertedIncompleteListItems);

			// For all of the list items whose content has changed elsewhere, we need to update the list items in place.
			// Since the `IncompleteListItemsPresenter` keeps toggled list items in place, we only need to perform one
			// update for list items that have a different completion state and text. We'll batch both of these changes
			// into a single update.

			if (newPresentedToggledListItems.Count > 0 || newPresentedListItemsWithUpdatedText.Count > 0) {
				// Find the unique list of list items that are updated.
				var hs = new HashSet<ListItem> (newPresentedToggledListItems);
				hs.UnionWith (newPresentedListItemsWithUpdatedText);
				ListPresenterUtilities.UpdateListItemsWithListItemsForListPresenter(this, PresentedListItems, hs);
			}

			// At this point the presented list items have been updated. As mentioned before, to ensure that we're
			// consistent about how we persist the updated list, we'll just use new the new list as the underlying
			// model. To do that we'll need to update the new list's unchanged list items with the list items that
			// are stored in the visual list items. i.e. We need to make sure that any references to list items in 
			// `presentedListItems` are reflected in the new list's items.

			list = newList;

			// Obtain the presented list items that were unchanged. We need to update the new list to reference the old list items.
			var unchangedPresentedListItems = PresentedListItems.Where (item => {
				return !newRemovedPresentedListItems.Contains (item) &&
				!newInsertedIncompleteListItems.Contains (item) &&
				!newPresentedToggledListItems.Contains (item) &&
				!newPresentedListItemsWithUpdatedText.Contains (item);
			}).ToArray ();

			ListPresenterAlgorithms.ReplaceAnyEqualUnchangedNewListItemsWithPreviousUnchangedListItems (list.Items, unchangedPresentedListItems);

			// Even though the old list's color will change if there's a difference between the old list's color and
			// the new list's color, the delegate only cares about this change in reference to what it already knows.
			// Because the delegate hasn't seen a color change yet, the update (if it happens) is ok.

			ListPresenterUtilities.UpdateListColorForListPresenterIfDifferent (this, oldList, newList.Color, ListColorUpdateAction.DontSendDelegateChangeLayoutCalls);
			Delegate.DidChangeListLayout (this, true);
		}

		/// <summary>
		/// Toggles listItem within the list. This method keeps the list item in the same place, but it toggles the
		/// completion state of the list item. Toggling a list item will call the delegate's
		/// DidUpdateListItem method.
		/// </summary>
		/// <param name="listItem">The list item to toggle</param>
		public void ToggleListItem(ListItem listItem)
		{
			if (!PresentedListItems.Contains (listItem))
				throw new InvalidProgramException ("The list item must already be in the presented list items.");

			Delegate.WillChangeListLayout (this, false);

			listItem.IsComplete = !listItem.IsComplete;

			int currentIndex = PresentedListItems.IndexOf (listItem);
			Delegate.DidUpdateListItem (this, listItem, currentIndex);
			Delegate.DidChangeListLayout (this, false);
		}

		/// <summary>
		/// Sets all of the presented list item's completion states to completionState. This method does not move the
		/// list items around whatsoever. Changing the completion state on all of the list items will call the
		/// delegate's DidUpdateListItem method for each list item that has been updated.
 		/// </summary>
		/// <param name="completionState">The value that all presented list item instances should have as their IsComplete property.</param>
		public void UpdatePresentedListItems(bool completionState)
		{
			var presentedListItemsNotMatchingCompletionState = PresentedListItems.Where(item => item.IsComplete != completionState).ToArray();

			// If there are no list items that match the completion state, it's a no op.
			if (presentedListItemsNotMatchingCompletionState.Length == 0)
				return;

			Delegate.WillChangeListLayout (this, false);

			foreach (ListItem listItem in presentedListItemsNotMatchingCompletionState) {
				listItem.IsComplete = !listItem.IsComplete;
				int indexOfListItem = PresentedListItems.IndexOf (listItem);
				Delegate.DidUpdateListItem (this, listItem, indexOfListItem);
			}

			Delegate.DidChangeListLayout (this, false);
		}
	}
}

