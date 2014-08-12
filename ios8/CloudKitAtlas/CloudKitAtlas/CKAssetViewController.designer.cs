// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace CloudKitAtlas
{
	[Register ("CKAssetViewController")]
	partial class CKAssetViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView assetPreview { get; set; }

		[Action ("DownloadPhoto:")]
		partial void DownloadPhoto (MonoTouch.Foundation.NSObject sender);

		[Action ("TakePhoto:")]
		partial void TakePhoto (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (assetPreview != null) {
				assetPreview.Dispose ();
				assetPreview = null;
			}
		}
	}
}
