// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace StreamingAudio
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel statusLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton streamAndPlayButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton streamSaveAndPlayButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField urlTextbox { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (streamAndPlayButton != null) {
				streamAndPlayButton.Dispose ();
				streamAndPlayButton = null;
			}

			if (streamSaveAndPlayButton != null) {
				streamSaveAndPlayButton.Dispose ();
				streamSaveAndPlayButton = null;
			}

			if (urlTextbox != null) {
				urlTextbox.Dispose ();
				urlTextbox = null;
			}

			if (statusLabel != null) {
				statusLabel.Dispose ();
				statusLabel = null;
			}
		}
	}
}
