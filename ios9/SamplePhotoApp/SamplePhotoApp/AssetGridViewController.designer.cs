// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SamplePhotoApp
{
	[Register ("AssetGridViewController")]
	partial class AssetGridViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem AddButton { get; set; }

		[Action ("AddButtonClickHandler:")]
		partial void AddButtonClickHandler (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddButton != null) {
				AddButton.Dispose ();
				AddButton = null;
			}
		}
	}
}
