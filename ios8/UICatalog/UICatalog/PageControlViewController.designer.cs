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
	[Register ("PageControlViewController")]
	partial class PageControlViewController
	{
		[Outlet]
		UIKit.UIView colorView { get; set; }

		[Outlet]
		UIKit.UIPageControl pageControl { get; set; }

		[Action ("PageControllerValueChanged:")]
		partial void PageControllerValueChanged (Foundation.NSObject sender);

		[Action ("PageControlValueChanged:")]
		partial void PageControlValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (colorView != null) {
				colorView.Dispose ();
				colorView = null;
			}

			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}
		}
	}
}
