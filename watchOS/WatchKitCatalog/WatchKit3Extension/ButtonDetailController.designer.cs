using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("ButtonDetailController")]
	partial class ButtonDetailController
	{
		[Outlet]
		WKInterfaceButton defaultButton { get; set; }

		[Outlet]
		WKInterfaceButton hiddenButton { get; set; }

		[Outlet]
		WKInterfaceButton placeholderButton { get; set; }

		[Outlet]
		WKInterfaceButton alphaButton { get; set; }

		[Action ("hideAndShow:")]
		partial void HideAndShow (NSObject obj);

		[Action ("changeAlpha:")]
		partial void ChangeAlpha (NSObject obj);

		void ReleaseDesignerOutlets ()
		{
			if (defaultButton != null) {
				defaultButton.Dispose ();
				defaultButton = null;
			}

			if (hiddenButton != null) {
				hiddenButton.Dispose ();
				hiddenButton = null;
			}

			if (placeholderButton != null) {
				placeholderButton.Dispose ();
				placeholderButton = null;
			}

			if (alphaButton != null) {
				alphaButton.Dispose ();
				alphaButton = null;
			}
		}
	}
}

