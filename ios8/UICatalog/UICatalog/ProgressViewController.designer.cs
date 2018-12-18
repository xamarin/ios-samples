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
	[Register ("ProgressViewController")]
	partial class ProgressViewController
	{
		[Outlet]
		UIKit.UIProgressView barProgressView { get; set; }

		[Outlet]
		UIKit.UIProgressView defaultProgressView { get; set; }

		[Outlet]
		UIKit.UIProgressView tintedProgressView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (defaultProgressView != null) {
				defaultProgressView.Dispose ();
				defaultProgressView = null;
			}

			if (barProgressView != null) {
				barProgressView.Dispose ();
				barProgressView = null;
			}

			if (tintedProgressView != null) {
				tintedProgressView.Dispose ();
				tintedProgressView = null;
			}
		}
	}
}
