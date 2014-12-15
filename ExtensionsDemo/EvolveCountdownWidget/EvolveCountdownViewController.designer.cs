// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace EvolveCountdownWidget
{
	[Register ("EvolveCountdownViewController")]
	partial class EvolveCountdownViewController
	{
		[Outlet]
		UIKit.UIButton WidgetButton { get; set; }

		[Outlet]
		UIKit.UIImageView WidgetImage { get; set; }

		[Outlet]
		UIKit.UILabel WidgetTitle { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (WidgetImage != null) {
				WidgetImage.Dispose ();
				WidgetImage = null;
			}

			if (WidgetTitle != null) {
				WidgetTitle.Dispose ();
				WidgetTitle = null;
			}

			if (WidgetButton != null) {
				WidgetButton.Dispose ();
				WidgetButton = null;
			}
		}
	}
}
