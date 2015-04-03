// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace VideoTimeLine
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem playButton { get; set; }

		[Action ("chooseVideoTapped:")]
		partial void ChooseVideoTapped (UIKit.UIBarButtonItem sender);

		[Action ("playButtonTapped:")]
		partial void PlayButtonTapped (UIKit.UIBarButtonItem sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (playButton != null) {
				playButton.Dispose ();
				playButton = null;
			}
		}
	}
}
