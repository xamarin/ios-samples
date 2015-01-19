using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace Common
{
	[Register("List")]
	public class List : NSObject, ICloneable, INSCoding
	{
		const string ListEncodingItemsKey = "items";
		const string ListEncodingColorKey = "color";

		List<ListItem> items;
		public ListColor Color { get; set; }

		public bool IsEmpty {
			get {
				return items.Count == 0;
			}
		}

		public int Count {
			get {
				return items.Count;
			}
		}

		public ListItem this[int index] {
			get {
				return items [index];
			}
		}

		public List ()
		{
			items = new List<ListItem> ();
		}

		public List(IEnumerable<ListItem> items, ListColor color)
			: this()
		{
			this.items.AddRange (items);
			Color = color;
		}

		[Export("initWithCoder:")]
		public List(NSCoder coder)
			: this()
		{
			NSArray array = (NSArray)coder.DecodeObject (ListEncodingItemsKey);
			for (nuint i = 0; i < array.Count; i++)
				items.Add (array.GetItem<ListItem> (i));

			Color = (ListColor)coder.DecodeInt (ListEncodingColorKey);
		}

		[Export ("encodeWithCoder:")]
		public void EncodeTo (NSCoder coder)
		{
			NSArray array = NSArray.FromNSObjects (items.ToArray());
			coder.Encode (array, ListEncodingItemsKey);
			coder.Encode((int)Color, ListEncodingColorKey);
		}

		public int IndexOfItem(ListItem item)
		{
			return items.IndexOf (item);
		}

		/// Use this function to ensure that all inserted items are complete.
		/// All inserted items must be incomplete when inserted.
		public bool CanInsertIncompleteItems(IEnumerable<ListItem> incompleteItems, int index)
		{
			bool anyCompleteItem = incompleteItems.Any (item => item.IsComplete);

			if (anyCompleteItem)
				return false;

			return index <= IndexOfFirstCompletedItem ();
		}

		/// Items will be inserted according to their completion state, maintaining their initial ordering.
		/// e.g. if items are [complete(0), incomplete(1), incomplete(2), completed(3)], they will be inserted
		/// into to sections of the items array. [incomplete(1), incomplete(2)] will be inserted at index 0 of the
		/// list. [complete(0), completed(3)] will be inserted at the index of the list.
		public NSIndexSet InsertItems(IEnumerable<ListItem> items)
		{
			int initialCount = Count;

			int incompleteItemsCount = 0;
			int completedItemsCount = 0;

			foreach (ListItem item in items) {
				if (item.IsComplete) {
					completedItemsCount++;
					this.items.Add (item);
				} else {
					incompleteItemsCount++;
					this.items.Insert (0, item);
				}
			}

			NSMutableIndexSet insertedIndexes = new NSMutableIndexSet ();
			insertedIndexes.AddIndexesInRange (new NSRange (0, incompleteItemsCount));
			insertedIndexes.AddIndexesInRange (new NSRange (incompleteItemsCount + initialCount, completedItemsCount));

			return insertedIndexes;
		}

		public void InsertItem(ListItem item, int index)
		{
			items.Insert (index, item);
		}

		public int InsertItem(ListItem item)
		{
			if (item.IsComplete) {
				items.Add (item);
				return items.Count - 1;
			} else {
				items.Insert (0, item);
				return 0;
			}
		}

		public bool CanMoveItem(ListItem item, int index, bool inclusive)
		{
			int fromIndex = items.IndexOf (item);

			if (fromIndex == -1)
				return false;

			if (item.IsComplete)
				return index <= Count && index >= IndexOfFirstCompletedItem ();
			else if (inclusive)
				return index >= 0 && index <= IndexOfFirstCompletedItem ();
			else
				return index >= 0 && index < IndexOfFirstCompletedItem ();
		}

		public ListOperationInfo MoveItem(ListItem item, int toIndex)
		{
			int fromIndex = items.IndexOf (item);

			if (fromIndex == -1)
				throw new InvalidProgramException ("Moving an item that isn't in the list is undefined.");

			items.RemoveAt (fromIndex);

			int normalizedToIndex = toIndex;

			if (fromIndex < toIndex)
				normalizedToIndex--;

			items.Insert (normalizedToIndex, item);

			var moveInfo = new ListOperationInfo {
				FromIndex = fromIndex,
				ToIndex = normalizedToIndex
			};

			return moveInfo;
		}

		public void RemoveItems(IEnumerable<ListItem> items)
		{
			foreach (var item in items)
				this.items.Remove (item);
		}

		// Toggles an item's completion state and moves the item to the appropriate index. The normalized from/to indexes are returned in the ListOperationInfo struct.
		public ListOperationInfo ToggleItem(ListItem item, int preferredDestinationIndex)
		{
			int fromIndex = items.IndexOf (item);

			if (fromIndex == -1)
				throw new InvalidProgramException ("Toggling an item that isn't in the list is undefined.");

			items.RemoveAt (fromIndex);

			item.IsComplete = !item.IsComplete;

			int toIndex = preferredDestinationIndex;

			if (toIndex == -1)
				toIndex = item.IsComplete ? Count : IndexOfFirstCompletedItem();

			items.Insert (toIndex, item);

			var toggleInfo = new ListOperationInfo {
				FromIndex = fromIndex,
				ToIndex = toIndex
			};

			return toggleInfo;
		}

		// Set all of the items to be a specific completion state.
		public void UpdateAllItemsToCompletionState(bool completeStatus)
		{
			foreach (ListItem item in items)
				item.IsComplete = completeStatus;
		}

		public int IndexOfFirstCompletedItem()
		{
			int index = items.FindIndex (item => item.IsComplete);
			return index == -1 ? items.Count : index;
		}

		public ListItem[] CopyAllItems()
		{
			return items.ToArray ();
		}

		#region ICloneable implementation

		public object Clone ()
		{
			return new List (items, Color);
		}

		#endregion
	}
}

