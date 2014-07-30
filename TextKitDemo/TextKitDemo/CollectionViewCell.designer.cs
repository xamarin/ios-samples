// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TextKitDemo
{
	[Register ("CollectionViewCell")]
	partial class CollectionViewCell
	{
		[Outlet]
		UIKit.UIView containerView { get; set; }

		[Outlet]
		UIKit.UILabel labelView { get; set; }

		[Outlet]
		UIKit.UITextView textView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (containerView != null) {
				containerView.Dispose ();
				containerView = null;
			}

			if (labelView != null) {
				labelView.Dispose ();
				labelView = null;
			}

			if (textView != null) {
				textView.Dispose ();
				textView = null;
			}
		}
	}
}
