using System;
using System.Collections.Generic;
using UIKit;

namespace MediaNotes
{
	public interface YYCommentNotifying
	{
		void AssociatedCommentDidChange (string comment);
		string AssociatedComment ();
		List<UIImage> ItemsForSharing ();
	}
}