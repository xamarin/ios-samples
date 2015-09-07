using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("GlanceController")]
	partial class GlanceController
	{
		[Outlet]
		WKInterfaceImage glanceImage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (glanceImage != null) {
				glanceImage.Dispose ();
				glanceImage = null;
			}
		}
	}
}

