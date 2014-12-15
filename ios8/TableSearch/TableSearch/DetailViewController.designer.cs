// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TableSearch
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet("price")]
		UIKit.UILabel Price { get; set; }

		[Outlet("year")]
		UIKit.UILabel Year { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Year != null) {
				Year.Dispose ();
				Year = null;
			}

			if (Price != null) {
				Price.Dispose ();
				Price = null;
			}
		}
	}
}
