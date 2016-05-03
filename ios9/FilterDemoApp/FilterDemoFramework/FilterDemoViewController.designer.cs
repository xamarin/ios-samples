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

namespace FilterDemoFramework
{
	[Register ("FilterDemoViewController")]
	partial class FilterDemoViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		FilterView filterView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel frequencyLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel resonanceLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (filterView != null) {
				filterView.Dispose ();
				filterView = null;
			}
			if (frequencyLabel != null) {
				frequencyLabel.Dispose ();
				frequencyLabel = null;
			}
			if (resonanceLabel != null) {
				resonanceLabel.Dispose ();
				resonanceLabel = null;
			}
		}
	}
}
