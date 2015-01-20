using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("LabelDetailController")]
	partial class LabelDetailController
	{
		[Outlet]
		WKInterfaceLabel customFontLabel { get; set; }

		[Outlet]
		WKInterfaceLabel coloredLabel { get; set; }

		[Outlet]
		WKInterfaceTimer timer { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (customFontLabel != null) {
				customFontLabel.Dispose ();
				customFontLabel = null;
			}

			if (coloredLabel != null) {
				coloredLabel.Dispose ();
				coloredLabel = null;
			}

			if (timer != null) {
				timer.Dispose ();
				timer = null;
			}
		}
	}
}

