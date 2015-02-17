using System;
using Foundation;

namespace ListerKit
{
	// TODO: add comments
	public interface IListsControllerDelegate
	{
		void WillChangeContent(ListsController listsController);

		void DidInsertListInfo(ListsController listsController, ListInfo listInfo, int index);

		void DidRemoveListInfo(ListsController listsController, ListInfo listInfo, int index);

		void DidUpdateListInfo(ListsController listsController, ListInfo listInfo, int index);

		void DidChangeContent(ListsController listsController);

		void DidFailCreatingListInfo(ListsController listsController, ListInfo listInfo, NSError error);

		void DidFailRemovingListInfo(ListsController listsController, ListInfo listInfo, NSError error);
	}
}

