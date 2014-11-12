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
	[Register ("ItemPropertiesViewController")]
	partial class ItemPropertiesViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIView square1 { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIView square2 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (square1 != null) {
				square1.Dispose ();
				square1 = null;
			}

			if (square2 != null) {
				square2.Dispose ();
				square2 = null;
			}
		}
	}
}
