using System;

namespace ListerKit
{
	/// <summary>
	/// This interface defines the contract between list presenters and how their lists are presented / archived
	///
	/// The IListPresenter interface defines the building blocks required for an object to be used as a list
	/// presenter. List presenters are meant to be used where an List object is displayed; in essence, a list
	/// presenter "fronts" an List object. With iOS / OS X apps, iOS / OS X widgets, and WatchKit extensions,
	/// we can classify these interaction models into list presenters. All of the logic can then be abstracted away
	/// so that the interaction is testable, reusable, and scalable. By defining the core requirements of a list
	/// presenter through the IListPresenter, consumers of IListPresenter instances can share a common
	/// interaction interface to a list.
	///
	/// Types that implements IListPresenter will have other methods to manipulate a list. For example, a
	/// presenter can allow for inserting list items into the list, it can allow moving a list item from one index
	/// to another, etc. All of these updates require that the IListPresenter notify its consumers
	/// of these changes through the events. Each of these methods
	/// should be surrounded by ListPresenterWillChangeListLayout and ListPresenterDidChangeListLayout
	/// invocations.
	///
	/// The underlying implementation of the IListPresenter may use an List object to store certain properties
	/// as a convenience, but there's no need to do that directly. You query an instance of an IListPresenter
	/// instance for its archiveableList representation; that is, a representation of the currently presented list
	/// that can be archiveable. This may happen, for example, when a document needs to save the currently presented
	/// list in an archiveable form. Note that list presenters should be used on the main thread / queue only.
	/// </summary>
	public interface IListPresenter
	{
		IListPresenterDelegate Delegate { get; }

		/// <summary>
		/// The color of the presented list. If the new color is different from the old color, the consuber should be
		/// notified through the ColorChanged event.
		/// </summary>
		ListColor Color { get; set; }

		/// <summary>
		/// An archiveable presentation of the list that that presenter is presenting. This commonly returns the underlying
		/// list being manipulated. However, this can be computed based on the current state of the presenter (color, list
		/// items, etc.). If a presenter has changes that are not yet applied to the list, the list returned here should
		/// have those changes applied.
		/// </summary>
		List ArchiveableList { get; }

		/// <summary>
		/// A convenience property that should return the equivalent of GetPresentedListItems().Length.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// A convenience property that should return whether or not there are any presented list items.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Resets the presented list to a new list. This can be called, for example, when a new list is unarchived and
		/// needs to be presented. Calls to this method should wrap the entire sequence of changes in a single
		/// ListPresenterWillChangeListLayout and ListPresenterDidChangeListLayout invocation.
		/// In more complicated implementations of this method, you can find the intersection / difference
		/// between the new list's presented list items and the old list's presented list items. You can then call into the
		/// remove / update / move events to inform the consumer of the re-organization. Consumers should
		/// receive updates if the text of a ListItem instance has changed. Consumers should also receive a
		/// callback if the new color is different from the old list's color.
		/// </summary>
		/// <param name="list">The new list that the IListPresener instance should present.</param>
		void SetList(List list);

		/// <summary>
		/// The presented list items that should be displayed in order. Implementations of the IListPresenter interface can
		/// decide not to show all of the list items within a list.
		/// </summary>
		/// <returns>The copy of presented list items.</returns>
		List[] GetPresentedListItems();
	}
}