// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CustomFonts
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet]
		CustomFonts.CTLabel capitalsLabel { get; set; }

		[Outlet]
		CustomFonts.CTLabel lowercaseLabel { get; set; }

		[Outlet]
		CustomFonts.CTLabel digitsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (capitalsLabel != null) {
				capitalsLabel.Dispose ();
				capitalsLabel = null;
			}

			if (lowercaseLabel != null) {
				lowercaseLabel.Dispose ();
				lowercaseLabel = null;
			}

			if (digitsLabel != null) {
				digitsLabel.Dispose ();
				digitsLabel = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}
		}
	}
}
