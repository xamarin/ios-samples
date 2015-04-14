// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Chat
{
	[Register ("ChatViewController")]
	partial class ChatViewController
	{
		[Outlet]
		UIKit.NSLayoutConstraint BottomConstraint { get; set; }

		[Outlet]
		UIKit.UIToolbar Chat { get; set; }

		[Outlet]
		UIKit.UIButton SendButton { get; set; }

		[Outlet]
		UIKit.UITableView TableView { get; set; }

		[Outlet]
		UIKit.UITextView TextView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomConstraint != null) {
				BottomConstraint.Dispose ();
				BottomConstraint = null;
			}

			if (SendButton != null) {
				SendButton.Dispose ();
				SendButton = null;
			}

			if (TableView != null) {
				TableView.Dispose ();
				TableView = null;
			}

			if (TextView != null) {
				TextView.Dispose ();
				TextView = null;
			}

			if (Chat != null) {
				Chat.Dispose ();
				Chat = null;
			}
		}
	}
}
