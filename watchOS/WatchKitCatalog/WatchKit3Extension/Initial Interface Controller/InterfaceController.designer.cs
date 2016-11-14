using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("InterfaceController")]
	partial class InterfaceController
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