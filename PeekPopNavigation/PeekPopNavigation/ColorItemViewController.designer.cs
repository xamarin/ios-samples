// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PeekPopNavigation
{
	[Register ("ColorItemViewController")]
	partial class ColorItemViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem starButton { get; set; }

		[Action ("Delete:")]
		partial void Delete (Foundation.NSObject sender);

		[Action ("TriggerStar:")]
		partial void TriggerStar (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (starButton != null) {
				starButton.Dispose ();
				starButton = null;
			}
		}
	}
}
