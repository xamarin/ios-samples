// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace tvRemote
{
	[Register ("FirstViewController")]
	partial class FirstViewController
	{
		[Outlet]
		UIKit.UILabel ButtonLabel { get; set; }

		[Outlet]
		tvRemote.SiriRemoteView RemoteView { get; set; }

		[Action ("DownPressed:")]
		partial void DownPressed (Foundation.NSObject sender);

		[Action ("LeftPressed:")]
		partial void LeftPressed (Foundation.NSObject sender);

		[Action ("MenuPressed:")]
		partial void MenuPressed (Foundation.NSObject sender);

		[Action ("PlayPausePressed:")]
		partial void PlayPausePressed (Foundation.NSObject sender);

		[Action ("RightPressed:")]
		partial void RightPressed (Foundation.NSObject sender);

		[Action ("TouchSurfaceClicked:")]
		partial void TouchSurfaceClicked (Foundation.NSObject sender);

		[Action ("UpPressed:")]
		partial void UpPressed (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (RemoteView != null) {
				RemoteView.Dispose ();
				RemoteView = null;
			}

			if (ButtonLabel != null) {
				ButtonLabel.Dispose ();
				ButtonLabel = null;
			}
		}
	}
}
