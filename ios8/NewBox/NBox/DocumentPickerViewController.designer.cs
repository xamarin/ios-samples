// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace NBox
{
	[Register ("DocumentPickerViewController")]
	partial class DocumentPickerViewController
	{
		[Outlet]
		UILabel MovedImportedList { get; set; }

		[Export("MoveExportBtn")]
		public UIButton MoveExportBtn { get; set; }

		[Action ("OnExportMoveClicked:")]
		partial void OnExportMoveClicked (Foundation.NSObject sender);

		[Action ("openDocument:")]
		partial void OpenDocument (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (MovedImportedList != null) {
				MovedImportedList.Dispose ();
				MovedImportedList = null;
			}
		}
	}
}
