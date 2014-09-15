//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using UIKit;

namespace NBox
{
	[Register ("DocumentPickerViewController")]
	partial class DocumentPickerViewController
	{
		[Export("MoveExportBtn")]
		public UIButton MoveExportBtn { get; set; }

		[Action ("openDocument:")]
		partial void OpenDocument (Foundation.NSObject sender);

		[Action ("OnExportMoveClicked:")]
		partial void OnExportMoveClicked(NSObject sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
