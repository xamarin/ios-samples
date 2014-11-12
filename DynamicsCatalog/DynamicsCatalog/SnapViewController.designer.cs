// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace DynamicsCatalog
{
	[Register ("SnapViewController")]
	partial class SnapViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIView square { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (square != null) {
				square.Dispose ();
				square = null;
			}
		}
	}
}
