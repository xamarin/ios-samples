// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PrivacyPrompts
{
	[Register("PrivacyDetailViewController")]
	partial class PrivacyDetailViewController
	{
		[Outlet]
		UIKit.UILabel accessStatus { get; set; }

		[Outlet]
		UIKit.UIButton requestBtn { get; set; }

		[Outlet]
		UIKit.UILabel titleLbl { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (accessStatus != null) {
				accessStatus.Dispose ();
				accessStatus = null;
			}

			if (requestBtn != null) {
				requestBtn.Dispose ();
				requestBtn = null;
			}

			if (titleLbl != null) {
				titleLbl.Dispose ();
				titleLbl = null;
			}
		}
	}
}
