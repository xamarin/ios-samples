using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("ElementRowController")]
	partial class ElementRowController
	{
		[Outlet]
		public WKInterfaceLabel ElementLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ElementLabel != null) {
				ElementLabel.Dispose ();
				ElementLabel = null;
			}
		}
	}
}

