// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AVCustomEdit
{
	[Register ("TransitionTypeController")]
	partial class TransitionTypeController
	{
		[Outlet]
		UIKit.UITableViewCell crossDissolveCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell diagonalWipeCell { get; set; }

		[Outlet]
		UIKit.UITableView tableView { get; set; }

		[Action ("TransitionSelected:")]
		partial void TransitionSelected (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (crossDissolveCell != null) {
				crossDissolveCell.Dispose ();
				crossDissolveCell = null;
			}

			if (diagonalWipeCell != null) {
				diagonalWipeCell.Dispose ();
				diagonalWipeCell = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}
		}
	}
}
