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
	[Register ("SecondViewController")]
	partial class SecondViewController
	{
		[Outlet]
		UIKit.UILabel ButtonLabel { get; set; }

		[Outlet]
		tvRemote.SiriRemoteView RemoteView { get; set; }
		
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
