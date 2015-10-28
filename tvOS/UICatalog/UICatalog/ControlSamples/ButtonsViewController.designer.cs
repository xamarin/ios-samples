// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("ButtonsViewController")]
	partial class ButtonsViewController
	{
		[Outlet]
		UIKit.UIButton AttributedTextButton { get; set; }

		[Action ("ButtonClicked:")]
		partial void ButtonClicked (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AttributedTextButton != null) {
				AttributedTextButton.Dispose ();
				AttributedTextButton = null;
			}
		}
	}
}
