using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("TableRowController")]
	partial class TableRowController
	{
		[Outlet]
		public WKInterfaceLabel RowLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (RowLabel != null) {
				RowLabel.Dispose ();
				RowLabel = null;
			}
		}
	}
}

