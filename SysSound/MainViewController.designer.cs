// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SysSound
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		UIKit.UIButton playAlertButton { get; set; }

		[Outlet]
		UIKit.UIButton playSystemButton { get; set; }

		[Outlet]
		UIKit.UIButton VibrateButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (playAlertButton != null) {
				playAlertButton.Dispose ();
				playAlertButton = null;
			}

			if (playSystemButton != null) {
				playSystemButton.Dispose ();
				playSystemButton = null;
			}

			if (VibrateButton != null) {
				VibrateButton.Dispose ();
				VibrateButton = null;
			}
		}
	}
}
