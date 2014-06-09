// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace AVCustomEdit
{
	[Register ("TransitionTypeController")]
	partial class TransitionTypeController
	{
		[Outlet]
		MonoTouch.UIKit.UITableViewCell crossDissolveCell { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableViewCell diagonalWipeCell { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Action ("TransitionSelected:")]
		partial void TransitionSelected (MonoTouch.Foundation.NSObject sender);
		
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
