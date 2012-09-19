// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace PokerNightVoting
{
	[Register ("PokerNightVotingViewController")]
	partial class PokerNightVotingViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem calendarsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem addEventButton { get; set; }

		[Action ("showCalendarChooser:")]
		partial void showCalendarChooser (MonoTouch.UIKit.UIBarButtonItem sender);

		[Action ("addTime:")]
		partial void addTime (MonoTouch.UIKit.UIBarButtonItem sender);

		void ReleaseDesignerOutlets ()
		{
			if (calendarsButton != null) {
				calendarsButton.Dispose ();
				calendarsButton = null;
			}

			if (addEventButton != null) {
				addEventButton.Dispose ();
				addEventButton = null;
			}
		}
	}
}
