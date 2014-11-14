// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace Quotes
{
	[Register ("PageViewController")]
	partial class PageViewController
	{
		[Outlet]
		Quotes.PageView pageView { get; set; }

		[Action ("ParagraphSelected:")]
		partial void ParagraphSelected (UIKit.UILongPressGestureRecognizer sender);

		[Action ("DrawingModeToggled:")]
		partial void DrawingModeToggled (UIKit.UISwipeGestureRecognizer sender);

		[Action ("MenuDismissed:")]
		partial void MenuDismissed (UIKit.UITapGestureRecognizer sender);

		[Action ("LineHeightChanged:")]
		partial void LineHeightChanged (UIKit.UISlider sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (pageView != null) {
				pageView.Dispose ();
				pageView = null;
			}
		}
	}
}
