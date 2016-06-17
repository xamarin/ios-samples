// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WatchkitExtension
{
	[Register ("LabelDetailController")]
	partial class LabelDetailController
	{
		[Outlet]
		WatchKit.WKInterfaceLabel coloredLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceTimer timer { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel ultralightLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (coloredLabel != null) {
				coloredLabel.Dispose ();
				coloredLabel = null;
			}

			if (ultralightLabel != null) {
				ultralightLabel.Dispose ();
				ultralightLabel = null;
			}

			if (timer != null) {
				timer.Dispose ();
				timer = null;
			}
		}
	}
}
