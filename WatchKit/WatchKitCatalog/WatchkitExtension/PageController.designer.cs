using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("PageController")]
	partial class PageController
	{
		[Outlet]
		WKInterfaceLabel pageLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (pageLabel != null) {
				pageLabel.Dispose ();
				pageLabel = null;
			}
		}
	}
}

