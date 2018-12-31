// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
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
		[Outlet]
		UIKit.UILabel priceLabel { get; set; }

		[Outlet]
		UIKit.UILabel yearsLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (priceLabel != null) {
				priceLabel.Dispose ();
				priceLabel = null;
			}

			if (yearsLabel != null) {
				yearsLabel.Dispose ();
				yearsLabel = null;
			}
		}
	}
}
