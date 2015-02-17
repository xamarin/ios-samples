using System;
using System.Collections.Generic;
using Foundation;

namespace ListerKit
{
	// TODO: add comments
	public interface IListCoordinatorDelegate
	{
		void DidUpdateContentsWithInsertedURLs(IEnumerable<NSUrl> insertedURLs, IEnumerable<NSUrl> removedURLs, IEnumerable<NSUrl> updatedURLs);

		void DidFailRemovingListAtURL(NSUrl url, NSError error);

		void DidFailCreatingListAtURL(NSUrl url, NSError error);
	}
}

