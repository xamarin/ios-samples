// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WatchConnectivity
{
	[Register ("CommandTableCell")]
	partial class CommandTableCell
	{
		[Outlet]
		public UIKit.UILabel label { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (label != null) {
				label.Dispose ();
				label = null;
			}
		}
	}
}
