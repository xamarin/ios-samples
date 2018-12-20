// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("AlertControllerViewController")]
	partial class AlertControllerViewController
	{
		[Outlet]
		UIKit.UITableViewCell okayActionSheetCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell otherActionSheetCell { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (okayActionSheetCell != null) {
				okayActionSheetCell.Dispose ();
				okayActionSheetCell = null;
			}

			if (otherActionSheetCell != null) {
				otherActionSheetCell.Dispose ();
				otherActionSheetCell = null;
			}
		}
	}
}
