// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace QRchestra
{
	[Register ("SendViewController")]
	partial class SendViewController
	{
		[Outlet]
		UIKit.UIImageView keyImageView1 { get; set; }

		[Outlet]
		UIKit.UIImageView keyImageView2 { get; set; }

		[Outlet]
		UIKit.UIImageView keyImageView3 { get; set; }

		[Outlet]
		UIKit.UIImageView keyImageView4 { get; set; }

		[Outlet]
		UIKit.UITapGestureRecognizer tapGestureRecognizer1 { get; set; }

		[Outlet]
		UIKit.UITapGestureRecognizer tapGestureRecognizer2 { get; set; }

		[Outlet]
		UIKit.UITapGestureRecognizer tapGestureRecognizer3 { get; set; }

		[Outlet]
		UIKit.UITapGestureRecognizer tapGestureRecognizer4 { get; set; }

		[Action ("done:")]
		partial void done (Foundation.NSObject sender);

		[Action ("handleTap:")]
		partial void handleTap (UITapGestureRecognizer sender);

		void ReleaseDesignerOutlets ()
		{
			if (keyImageView1 != null) {
				keyImageView1.Dispose ();
				keyImageView1 = null;
			}

			if (keyImageView2 != null) {
				keyImageView2.Dispose ();
				keyImageView2 = null;
			}

			if (keyImageView3 != null) {
				keyImageView3.Dispose ();
				keyImageView3 = null;
			}

			if (keyImageView4 != null) {
				keyImageView4.Dispose ();
				keyImageView4 = null;
			}

			if (tapGestureRecognizer1 != null) {
				tapGestureRecognizer1.Dispose ();
				tapGestureRecognizer1 = null;
			}

			if (tapGestureRecognizer2 != null) {
				tapGestureRecognizer2.Dispose ();
				tapGestureRecognizer2 = null;
			}

			if (tapGestureRecognizer3 != null) {
				tapGestureRecognizer3.Dispose ();
				tapGestureRecognizer3 = null;
			}

			if (tapGestureRecognizer4 != null) {
				tapGestureRecognizer4.Dispose ();
				tapGestureRecognizer4 = null;
			}
		}
	}
}
