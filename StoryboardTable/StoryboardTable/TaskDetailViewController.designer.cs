// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace StoryboardTable
{
	[Register ("TaskDetailViewController")]
	partial class TaskDetailViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton DeleteButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView detail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch DoneSwitch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField NotesText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton SaveButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField TitleText { get; set; }

		[Action ("DeleteButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void DeleteButtonTouchUpInside (UIButton sender);

		[Action ("SaveButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void SaveButtonTouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (DeleteButton != null) {
				DeleteButton.Dispose ();
				DeleteButton = null;
			}
			if (detail != null) {
				detail.Dispose ();
				detail = null;
			}
			if (DoneSwitch != null) {
				DoneSwitch.Dispose ();
				DoneSwitch = null;
			}
			if (NotesText != null) {
				NotesText.Dispose ();
				NotesText = null;
			}
			if (SaveButton != null) {
				SaveButton.Dispose ();
				SaveButton = null;
			}
			if (TitleText != null) {
				TitleText.Dispose ();
				TitleText = null;
			}
		}
	}
}
