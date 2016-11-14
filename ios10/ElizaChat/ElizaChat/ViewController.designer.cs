// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ElizaChat
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UITextView ChatHistory { get; set; }

		[Outlet]
		UIKit.UITextField ChatInput { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ContainerBottomConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ContainerTopConstraint { get; set; }

		[Outlet]
		UIKit.UIView InterfaceContainer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ChatHistory != null) {
				ChatHistory.Dispose ();
				ChatHistory = null;
			}

			if (ChatInput != null) {
				ChatInput.Dispose ();
				ChatInput = null;
			}

			if (ContainerTopConstraint != null) {
				ContainerTopConstraint.Dispose ();
				ContainerTopConstraint = null;
			}

			if (InterfaceContainer != null) {
				InterfaceContainer.Dispose ();
				InterfaceContainer = null;
			}

			if (ContainerBottomConstraint != null) {
				ContainerBottomConstraint.Dispose ();
				ContainerBottomConstraint = null;
			}
		}
	}
}
