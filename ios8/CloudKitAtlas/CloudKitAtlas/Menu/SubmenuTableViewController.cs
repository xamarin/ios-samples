using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class SubmenuTableViewController : UITableViewController
	{
		public string GroupTitle { get; set; }
		public CodeSample [] CodeSamples { get; set; }

		public SubmenuTableViewController (IntPtr handle) : base (handle)
		{
		}
	}
}
