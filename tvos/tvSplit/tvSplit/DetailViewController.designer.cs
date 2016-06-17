// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace tvSplit
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet]
		UIKit.UIImageView BackgroundImage { get; set; }

		[Outlet]
		UIKit.UIButton ButtonA { get; set; }

		[Outlet]
		UIKit.UIButton ButtonB { get; set; }

		[Outlet]
		UIKit.UILabel PageTitle { get; set; }

		[Action ("ChooseA:")]
		partial void ChooseA (Foundation.NSObject sender);

		[Action ("ChooseB:")]
		partial void ChooseB (Foundation.NSObject sender);

		[Action ("PlayPausePressed:")]
		partial void PlayPausePressed (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BackgroundImage != null) {
				BackgroundImage.Dispose ();
				BackgroundImage = null;
			}

			if (ButtonA != null) {
				ButtonA.Dispose ();
				ButtonA = null;
			}

			if (ButtonB != null) {
				ButtonB.Dispose ();
				ButtonB = null;
			}

			if (PageTitle != null) {
				PageTitle.Dispose ();
				PageTitle = null;
			}
		}
	}
}
