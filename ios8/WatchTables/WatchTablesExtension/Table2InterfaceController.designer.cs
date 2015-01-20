// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WatchTablesExtension
{
	[Register ("Table2InterfaceController")]
	partial class Table2InterfaceController
	{
		[Outlet]
		WatchKit.WKInterfaceLabel table2Label { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (table2Label != null) {
				table2Label.Dispose ();
				table2Label = null;
			}
		}
	}
}
