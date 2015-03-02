using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("DeviceDetailController")]
	partial class DeviceDetailController
	{
		[Outlet]
		WKInterfaceLabel boundsLabel { get; set; }
		
		[Outlet]
		WKInterfaceLabel scaleLabel { get; set; }
		
		[Outlet]
		WKInterfaceLabel localeLabel { get; set; }

		[Outlet]
		WKInterfaceLabel preferredContentSizeLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (boundsLabel != null) {
				boundsLabel.Dispose ();
				boundsLabel = null;
			}

			if (scaleLabel != null) {
				scaleLabel.Dispose ();
				scaleLabel = null;
			}

			if (localeLabel != null) {
				localeLabel.Dispose ();
				localeLabel = null;
			}

			if (preferredContentSizeLabel != null) {
				preferredContentSizeLabel.Dispose ();
				preferredContentSizeLabel = null;
			}
		}
	}
}

