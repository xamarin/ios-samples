// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MessagesExtension
{
	[Register ("CompletedIceCreamViewController")]
	partial class CompletedIceCreamViewController
	{
		[Outlet]
		Messages.MSStickerView StickerView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (StickerView != null) {
				StickerView.Dispose ();
				StickerView = null;
			}
		}
	}
}
