using System;

namespace ListerKit
{
	/// <summary>
	/// The IListPresenterDelegate interface is used to receive events from an IListPresenting implementation
	/// about updates to the presenter's layout. This happens, for example, if an ListItem object is
	/// inserted into the list or removed from the list. For any change that occurs to the list, a delegate message
	/// can be called, but you may decide not to take any action if the method doesn’t apply to your use case. For
	/// an implementation of IListPresenterDelegate, see the AllListItemsPresenter or IncompleteListItemsPresenter types.
	/// </summary>
	public interface IListPresenterDelegate
	{
		/// <summary>
		/// An IListPresenting implementation invokes this method on its delegate when a large change to the underlying
		/// list changed, but the presenter couldn't resolve the granular changes. A full layout change includes
		///	changing anything on the underlying list: list item toggling, text updates, color changes, etc. This is
		///	invoked, for example, when the list is initially loaded, because there could be many changes that happened
		///	relative to an empty list--the delegate should just reload everything immediately. This method is not
		///	wrapped in WillChangeListLayout and DidChangeListLayout method invocations.
		/// </summary>
		/// <param name="presenter">The list presenter whose full layout has changed</param>
		void DidRefreshCompleteLayout(IListPresenter presenter);

		/// <summary>
		/// An IListPresenter invokes this method on its delegate before a set of layout changes
		/// occur. This could involve list item insertions, removals, updates, toggles, etc. This can also include
		/// changes to the color of the IListPresenter instance. If isInitialLayout is <c>true</c>, it means that
		/// the new list is being presented for the first time--for example, if SetList is called on the IListPresenter
		/// instance, the delegate will receive a WillChangeListLayout call where isInitialLayout parameter is <c>true</c>.
		/// </summary>
		/// <param name="listPresenter">The list presenter whose presentation will change.</param>
		/// <param name="isInitialLayout">Whether or not the presenter is presenting the most recent list for the first time.</param>
		void WillChangeListLayout(IListPresenter listPresenter, bool isInitialLayout);

		/// <summary>
		/// A <c>ILListPresenting</c> invokes this method on its delegate when an item was inserted into the list. This
		/// method is called only if the invocation is wrapped in a call to <c>WillChangeListLayout</c> and 
		/// <c>DidChangeListLayout</c>
		/// </summary>
		/// <param name="listPresenter">The list presenter whose presentation has changed.</param>
		/// <param name="listItem">The list item that has been inserted.</param>
		/// <param name="atIndex">The index that <c>listItem</c> was inserted into.</param>
		void DidInsertListItem(IListPresenter listPresenter, ListItem listItem, int atIndex);

		/// <summary>
		/// An <c>IListPresenter</c> invokes this method on its delegate when an item was removed from the list. This
		/// method is called only if the invocation is wrapped in a call to <c>WillChangeListLayout</c> and
		/// <c>DidChangeListLayout</c>.
		/// </summary>
		/// <param name="listPresenter">The list presenter whose presentation has changed.</param>
		/// <param name="listItem">The list item that has been removed.</param>
		/// <param name="atIndex">The index that <c>listItem</c> was removed from.</param>
		void DidRemoveListItem(IListPresenter listPresenter, ListItem listItem, int atIndex);

		/// <summary>
		/// An <c>IListPresenter<c> invokes this method on its delegate when an item is updated in place. This could
		/// happen, for example, if the text of an <c>ListItem</c> instance changes. This method is called only if the
		/// invocation is wrapped in a call to <c>WillChangeListLayout</c> and <c>DidChangeListLayout</c>.
		/// </summary>
		/// <param name="listPresenter">The list presenter whose presentation has changed.</param>
		/// <param name="listItem">The list item that has been updated.</param>
		/// <param name="atIndex">The index that <c>listItem</c> was updated at in place.</param>
		void DidUpdateListItem(IListPresenter listPresenter, ListItem listItem, int atIndex);

		/// <summary>
		/// An <c>ListPresenting</c> invokes this method on its delegate when an item moved <c>fromIndex<c> to <c>toIndex/<c>.
		/// This could happen, for example, if the list presenter toggles an <c>AAPLListItem</c> instance and it needs to
		/// be moved from one index to another. This method is called only if the invocation is wrapped in a call to
		/// <c>WillChangeListLayout</c> and <c>DidChangeListLayout</c>.
		/// </summary>
		/// <param name="listPresenter">The list presenter whose presentation has changed.</param>
		/// <param name="listItem">The list item that has been moved.</param>
		/// <param name="fromIndex">The original index that <c>listItem</c> was located at before the move.</param>
		/// <param name="toIndex">The index that <c>listItem</c> was moved to.</param>
		void DidMoveListItem(IListPresenter listPresenter, ListItem listItem, int fromIndex, int toIndex);

		/// <summary>
		/// An <c>IListPresenter<c> invokes this method on its delegate when the color of the <c>ListPresenter</c>
		/// instance's changes. This method is called only if the invocation is wrapped in a call to
		/// <c>WillChangeListLayout<c> and <c>DidChangeListLayout</c>
		/// </summary>
		/// <param name="listPresenter">The list presenter whose presentation has changed.</param>
		/// <param name="color">The new color of the presented list.</param>
		void DidUpdateListColorWithColor(IListPresenter listPresenter, ListColor color);

		/// <summary>
		/// An <c>IListPresenting</c> invokes this method on its delegate after a set of layout changes occur. See
		/// <c>WillChangeListLayout</c> for examples of when this would be called.
		/// </summary>
		/// <param name="listPresenter">The list presenter whose presentation has changed.</param>
		/// <param name="isInitialLayout">Whether or not the presenter is presenting the most recent list for the first time.</param>
		void DidChangeListLayout(IListPresenter listPresenter, bool isInitialLayout);
	}
}