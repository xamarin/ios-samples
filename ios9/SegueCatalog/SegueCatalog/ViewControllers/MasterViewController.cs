using System;

using Foundation;
using UIKit;

namespace SegueCatalog
{
	public partial class MasterViewController : UITableViewController
	{
		public MasterViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Action("unwindInMaster:")]
		void UnwindInMaster(UIStoryboardSegue segue)
		{
			/*
			Empty. Exists solely so that "unwind in master" segues can
			find this instance as a destination.
			*/
		}
	}
}
