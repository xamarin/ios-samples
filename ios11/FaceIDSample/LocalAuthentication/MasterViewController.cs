using System;
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace StoryboardTable
{
	public partial class MasterViewController : UITableViewController
	{
       	public MasterViewController (IntPtr handle) : base (handle)
		{
			Title = "Secret Data";
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// bind every time, to reflect deletion in the Detail view
			TableView.Source = new SecretTableSource();
		}
	}
}