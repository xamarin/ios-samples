using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("TableDetailController")]
	partial class TableDetailController
	{
		[Outlet]
		WKInterfaceTable interfaceTable { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (interfaceTable != null) {
				interfaceTable.Dispose ();
				interfaceTable = null;
			}
		}
	}
}

