using System;

using Foundation;
using UIKit;

namespace ColorFeed {
	public partial class FeedEntryTableViewCell : UITableViewCell {
		Post post;
		public Post Post {
			get => post;
			set {
				post = value;
				UpdateCell ();
			}
		}

		public FeedEntryTableViewCell (IntPtr handle) : base (handle)
		{
		}

		void UpdateCell () => colorView.Post = Post;
	}
}
