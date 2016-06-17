// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace tvTable
{
	[Register ("AttractionTableCell")]
	partial class AttractionTableCell
	{
		[Outlet]
		UIKit.UIImageView Favorite { get; set; }

		[Outlet]
		UIKit.UILabel Title { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Title != null) {
				Title.Dispose ();
				Title = null;
			}

			if (Favorite != null) {
				Favorite.Dispose ();
				Favorite = null;
			}
		}
	}
}
